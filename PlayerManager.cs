using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGameTest {
    class PlayerManager {
        #region properties
        public uint TurnCount = 1;
        public Queue<Player> PlayerQueue = new Queue<Player>();
        public Player CurrentPlayer {
            get {
                return PlayerQueue.Peek();
            }
        }
        public bool GameStarted = false;
        public uint DefaultFighterQuantity = 2;
        public Texture2D FighterTexture;
        public Texture2D LifeBarTexture;
        public SpriteFont FighterNameFont;
        public int screenWidth = 0;
        public Terrain Terrain;
        public bool GameOver = false;
        public Player Winner = null;

        private Random Randomizer = new Random();
        #endregion

        #region Methods
        public void EndTurn() {
            if (GameStarted) {
                Player current = this.PlayerQueue.Dequeue();
                current.EndTurn();
                this.PlayerQueue.Enqueue(current);
                this.TurnCount++;
                this.PlayerQueue.Peek().StartTurn();
            }
            //sprawdzanie, czy gra została zakończona
            List<Player> playersAlive = new List<Player>();
            foreach (Player p in PlayerQueue) {
                foreach (Fighter f in p.Fighters) {
                    if (f.Life > 0) {
                        playersAlive.Add(p);
                        break;
                    }
                }
            }

            if (playersAlive.Count <= 1) {
                GameOver = true;
                Winner = playersAlive.FirstOrDefault();//t ojest do zmiany!
            }
        }

        public void AddPlayer(Player newPlayer) {
            if (!this.GameStarted) {
                if (newPlayer.FighterCount > 0) {
                    this.PlayerQueue.Enqueue(newPlayer);
                }
                else {
                    newPlayer.SetFightersQuantity(this.DefaultFighterQuantity);
                    this.PlayerQueue.Enqueue(newPlayer);
                }
            }
            else {
                throw new Exception("Game already started");
            }
        }

        public void StartGame() {
            this.GameStarted = true;
            SetUpFightersPositions();
            this.PlayerQueue.Peek().Fighters.Peek().CurrentlyActive = true;
        }

        public void DrawFighters(SpriteBatch spriteBatch) {
            foreach (Player p in this.PlayerQueue) {
                foreach (Fighter f in p.Fighters) {
                    //if (!this.Terrain.Intersects(f.FighterArea)) {
                    //    f.Position = Gravity.FreeFall(f.Position);
                    //}
                    //f.Position = new Vector2(f.Position.X - this.Terrain.rightOffset, f.Position.Y - this.Terrain.topOffset);

                    
                    Rectangle lifebar = f.GetLifeBarRect();
                    lifebar.X -= this.Terrain.rightOffset;
                    lifebar.Y -= this.Terrain.topOffset;

                    if (f.CurrentlyActive) {
                        spriteBatch.DrawString(FighterNameFont, f.Name, new Vector2(lifebar.X - 20, lifebar.Y - 20), p.PlayerColor);
                    }

                    

                    spriteBatch.Draw(LifeBarTexture, lifebar, Color.Green);
                    spriteBatch.Draw(FighterTexture, new Vector2(f.Position.X - this.Terrain.rightOffset, f.Position.Y - this.Terrain.topOffset)  /*new Vector2(f.Position.X - this.Terrain.rightOffset, f.Position.Y - this.Terrain.topOffset)*/, Color.White);
                    //spriteBatch.Draw(FighterTexture, new Vector2(f.Position.X - this.Terrain.rightOffset, f.Position.Y - this.Terrain.topOffset), Color.White);
                }
            }
        }

        /// <summary>
        /// Update handler designed to split update methods, such as gravity free fall, from draw methods
        /// </summary>
        public void UpdatePhysics() {
            foreach (Player p in this.PlayerQueue) {
                foreach (Fighter f in p.Fighters) {
                    if (!f.Intersects(ref this.Terrain)) {
                        f.Position = Gravity.FreeFall(f.Position);
                    }
                }
            }
        }

        /// <summary>
        /// This is used to handle all events in "Update" method, such as keys pressed, etc.
        /// </summary>
        public void UpdateHandler() {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Left)) {
                this.PlayerQueue.Peek().Fighters.Peek().MoveLeft(ref this.Terrain);
            }
            if (keyState.IsKeyDown(Keys.Right)) {
                this.PlayerQueue.Peek().Fighters.Peek().MoveRight(ref this.Terrain);
            }
        }
        
        
        private void SetUpFightersPositions() {
            foreach (Player p in this.PlayerQueue) {
                foreach (Fighter f in p.Fighters) {
                    f.Position = new Vector2(Randomizer.Next(this.screenWidth-f.Width), 0);
                }
            }
        }


        #endregion
    }
}
