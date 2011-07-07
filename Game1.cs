using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGameTest {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //fonty
        SpriteFont PlayerLabelFont;
        SpriteFont CurrentFighterLabelFont;

        //textury
        Texture2D background;//obraz t³a
        Texture2D foreground;//obraz terenu
        Texture2D fighterTexture;//tekstura postaci
        Texture2D lifebarTexture;

        //opcje i zmienne zwi¹zane z widokiem i jego zmianami
        int screenWidth = 1024;
        int screenHeight = 780;
        int backgroundWidth = 1400;
        int backgroundHeight = 900;
        int rightOffset = 0;
        int topOffset = 0;
        int speed = 2;
        int centerX;
        int centerY;
        bool mousePressed = false;
        bool blockBorders = true;

        //teren
        Terrain playground;

        //silnik gry:
        PlayerManager manager = new PlayerManager();


        List<Keys> pressedKeys = new List<Keys>();//flaga do trzymania info o aktualnie naciœniêtych klawiszach. S³u¿y do tego, ¿eby nie powtarzaæ funkcji, które maj¹ odpalaæ siê tylko raz, po naciœniêciu przycisku.

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = screenWidth;
            this.graphics.PreferredBackBufferHeight = screenHeight;
            this.graphics.IsFullScreen = false;
            this.IsMouseVisible = true;

            playground = new Terrain() { screenHeight = this.backgroundHeight, screenWidth = this.backgroundWidth };

            //Dodaje graczy tutaj tymczasowo:
            manager.AddPlayer(new Player() { Name = "Player 1", PlayerColor = Color.Aqua });
            manager.AddPlayer(new Player() { Name = "Player 2", PlayerColor = Color.Red });
            manager.AddPlayer(new Player() { Name = "Player 3", PlayerColor = Color.Yellow });
            manager.AddPlayer(new Player() { Name = "Player 4", PlayerColor = Color.Green });
            manager.screenWidth = backgroundWidth;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            this.rightOffset = (backgroundWidth - screenWidth) / 2;
            this.topOffset = (backgroundHeight - screenHeight) / 2;
            this.centerX = screenWidth / 2;
            this.centerY = screenHeight / 2;

            this.playground.rightOffset = this.rightOffset;
            this.playground.topOffset = this.topOffset;

            MoveMouse(centerX, centerY);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("background");
            foreground = Content.Load<Texture2D>("foreground");
            fighterTexture = Content.Load<Texture2D>("fighter");
            lifebarTexture = Content.Load<Texture2D>("lifebar");

            //fonty
            PlayerLabelFont = Content.Load<SpriteFont>("CurrentPlayerTurnLabel");
            CurrentFighterLabelFont = Content.Load<SpriteFont>("CurrentFighterLabel");

            playground.texture = this.foreground;
            playground.spriteBatch = this.spriteBatch;
            playground.Initialize();
            this.playground.texture = foreground;

            //silnik
            manager.Terrain = playground;
            manager.FighterTexture = fighterTexture;
            manager.LifeBarTexture = lifebarTexture;
            manager.FighterNameFont = CurrentFighterLabelFont;
            manager.StartGame();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            ViewHandle(keyState, mouse);

            //players handlers
            if (KeyPressed(Keys.Enter)) {
                manager.EndTurn();
            }
            manager.UpdateHandler();
            manager.UpdatePhysics();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (!manager.GameOver) {
                // TODO: Add your drawing code here
                spriteBatch.Begin();

                //t³o
                Rectangle screenRectangle = new Rectangle(-rightOffset, -topOffset, backgroundWidth, backgroundHeight);
                spriteBatch.Draw(background, screenRectangle, Color.White);

                //plansza
                this.playground.rightOffset = this.rightOffset;
                this.playground.topOffset = this.topOffset;
                this.playground.Render();

                //playerzy
                DrawCurrentPlayerTitle();
                DrawCurrentPlayerPoints();
                this.manager.Terrain.rightOffset = this.rightOffset;
                this.manager.Terrain.topOffset = this.topOffset;
                this.manager.DrawFighters(spriteBatch);
            }
            else {
                DrawSummaryPane();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws Current Player title in top right part of screen
        /// </summary>
        protected void DrawCurrentPlayerTitle() {
            spriteBatch.DrawString(PlayerLabelFont, manager.CurrentPlayer.Name, new Vector2(20, 10), manager.CurrentPlayer.PlayerColor);
        }
        /// <summary>
        /// Draws Current Player points in top right part of screen
        /// </summary>
        protected void DrawCurrentPlayerPoints() {
            string points = manager.CurrentPlayer.Points.ToString();
            int stringLength = (int)PlayerLabelFont.MeasureString(points).X;
            spriteBatch.DrawString(PlayerLabelFont, points, new Vector2(screenWidth - 20 - stringLength, 10), Color.Black);
        }

        protected void DrawSummaryPane() {
            string text = "Wygra³ " + manager.Winner.Name;
            Vector2 dim = PlayerLabelFont.MeasureString(text);
            Vector2 position = new Vector2(screenWidth/2 - dim.X/2,screenHeight/2 - dim.Y/2);

            spriteBatch.DrawString(PlayerLabelFont, text, position, Color.White);
        }

        /// <summary>
        /// This is made to handle view moves.
        /// </summary>
        /// <param name="keyState">Current Keybord state</param>
        /// <param name="mouseState">Current Mouse state</param>
        protected void ViewHandle(KeyboardState keyState, MouseState mouseState) {
            //sterowanie widokiem za pomoc¹ strza³ek
            /*if (keyState.IsKeyDown(Keys.Left) && rightOffset - speed >= 0) {
                rightOffset -= speed;
            }
            if (keyState.IsKeyDown(Keys.Right) && rightOffset + speed <= (backgroundWidth - screenWidth)) {
                rightOffset += speed;
            }
            if (keyState.IsKeyDown(Keys.Up) && topOffset - speed >= 0) {
                topOffset -= speed;
            }
            if (keyState.IsKeyDown(Keys.Down) && topOffset + speed <= (backgroundHeight - screenHeight)) {
                topOffset += speed;
            }*/

            //update mouse position

            if (mouseState.LeftButton == ButtonState.Pressed) {
                if (!mousePressed) {
                    this.centerX = mouseState.X;
                    this.centerY = mouseState.Y;
                    mousePressed = true;
                }
                int mouseOffsetX = this.centerX - mouseState.X;
                int mouseOffsetY = this.centerY - mouseState.Y;

                rightOffset -= mouseOffsetX * speed / 10;
                topOffset -= mouseOffsetY * speed / 10;

                if (blockBorders) {
                    //blokada pozioma
                    if (rightOffset < 0) {
                        rightOffset = 0;
                    }
                    if (rightOffset > (backgroundWidth - screenWidth)) {
                        rightOffset = (backgroundWidth - screenWidth);
                    }
                    //blokada pionowa
                    if (topOffset < 0) {
                        topOffset = 0;
                    }
                    if (topOffset > (backgroundHeight - screenHeight)) {
                        topOffset = (backgroundHeight - screenHeight);
                    }
                }
            }
            else {
                if (mousePressed) {
                    this.centerX = screenWidth / 2;
                    this.centerY = screenHeight / 2;
                    mousePressed = false;
                }
            }

            if (!mousePressed) {
                CenterView();
            }

        }

        protected void MoveMouse(int x, int y) {
            Mouse.SetPosition(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool KeyPressed(Keys key) {
            if (!Keyboard.GetState().IsKeyDown(key)) {
                if (this.pressedKeys.Contains(key)) {
                    this.pressedKeys.Remove(key);
                }
                return false;
            }
            else {
                if (this.pressedKeys.Contains(key)) {
                    return false;
                }
                else {
                    this.pressedKeys.Add(key);
                    return true;
                }
            }
        }
        //do poprawki
        /// <summary>
        /// Centers view at current fighter
        /// </summary>
        protected void CenterView() {
            Fighter f = manager.CurrentPlayer.CurrentFighter;
            Vector2 fighterPos = new Vector2(f.Position.X + f.Width / 2, f.Position.Y + f.Height / 2);
            int left = (int)fighterPos.X - rightOffset;
            int right = rightOffset + screenWidth - (int)fighterPos.X;
            int top = (int)fighterPos.Y - topOffset;
            int bottom = topOffset + screenHeight - (int)fighterPos.Y;
            int diffX = left - right;
            int diffY = top-bottom;
            int tolerance = 0;


            //oœ x
            if (Math.Abs(diffX) > tolerance) {

                rightOffset += diffX / 2;

                if (rightOffset + screenWidth > backgroundWidth) {
                    rightOffset = backgroundWidth - screenWidth;
                }
                if (rightOffset < 0) {
                    rightOffset = 0;
                }
            }
            //oœ y
            if (Math.Abs(diffY) > tolerance) {
                topOffset += diffY / 2;
                if (topOffset + screenHeight > backgroundHeight) {
                    topOffset = backgroundHeight - screenHeight;
                }
                if (topOffset < 0) {
                    rightOffset = 0;
                }
            }
        }
    }
}
