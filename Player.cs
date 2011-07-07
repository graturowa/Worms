using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGameTest {
    class Player {
        #region properties
        public Queue<Fighter> Fighters = new Queue<Fighter>();
        public string Name = "Player";
        public Color PlayerColor = Color.Blue;
        public int FighterCount {
            get {
                return Fighters.Count;
            }
        }
        public Fighter CurrentFighter {
            get {
                return Fighters.Peek();
            }
        }
        public int Points = 0;
        #endregion

        #region construct
        public Player() {

        }

        public Player(int fighterCount) {
            for (int i = 1; i <= fighterCount; i++) {
                Fighters.Enqueue(new Fighter() { Name = "Fighter" + i.ToString() });
            }
        }
        #endregion

        #region methods
        public void SetFightersQuantity(uint quantity) {
            if (FighterCount == 0){
                for (int i = 1; i <= quantity; i++) {
                    Fighters.Enqueue(new Fighter() { Name = "Fighter" + i.ToString() });
                }
            }
            else if (FighterCount < quantity) {
                for (int i = FighterCount; i <= quantity; i++) {
                    Fighters.Enqueue(new Fighter() { Name = "Fighter" + i.ToString() });
                }
            }
            else {
                for (int i = FighterCount - (int)quantity; i >=0; i--) {
                    Fighters.Dequeue();
                }
            }
        }

        public void EndTurn() {
            Fighter current = this.Fighters.Dequeue();
            current.CurrentlyActive = false;
            if (current.Life > 0) {
                this.Fighters.Enqueue(current);
            }
        }

        public void StartTurn() {
            this.Fighters.Peek().CurrentlyActive = true;
        }

        #endregion
    }
}
