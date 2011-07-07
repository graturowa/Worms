using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGameTest {
    class Fighter {
        private static int _lifeMaximum = 200;
        private static int _lifeBarMaxWidth = 30;
        private static int _lifeBarHeight = 5;
        private static int _width = 28;
        private static int _height = 30;

        public string Name = "Players Fighter";
        public int Life;
        public int LifeMax {
            get {
                return _lifeMaximum;
            }
        }
        public Vector2 Position;
        public Rectangle FighterArea {
            get {
                return new Rectangle(Convert.ToInt32(this.Position.X), Convert.ToInt32(this.Position.Y), _width, _height);
            }
        }
        public bool CurrentlyActive = false;
        public int speed = 1;
        public int Width {
            get {
                return _width;
            }
        }
        public int Height {
            get {
                return _height;
            }
        }

        public Fighter() {
            this.Life = _lifeMaximum;
        }

        /// <summary>
        /// Gets life bar Rectangle
        /// </summary>
        /// <returns></returns>
        public Rectangle GetLifeBarRect() { 
            int width = (Life / _lifeMaximum) * _lifeBarMaxWidth;
            int x = Convert.ToInt32(this.Position.X) - 1;
            int y = Convert.ToInt32(this.Position.Y) - 6;

            return new Rectangle(x,y,width,_lifeBarHeight);
        }

        /// <summary>
        /// Used to move fighters all around the world ;)
        /// </summary>
        /// <param name="world">Terrain</param>
        public void MoveLeft(ref Terrain world) {
            if (this.Position.X - speed > 0) {
                this.Position.X -= speed;
            }
            this.MoveY(ref world);
        }

        /// <summary>
        /// Used to move fighters all around the world ;)
        /// </summary>
        /// <param name="world">Terrain</param>
        public void MoveRight(ref Terrain world) {
            if (Position.X + speed + this.Width < world.terrainContour.Length) {
                this.Position.X += speed;
            }
            this.MoveY(ref world);
        }

        public void MoveY(ref Terrain world) {
            int midX = (int)this.Position.X + (_width / 2);
            if (!this.Intersects(ref world)) {
                this.Position.Y = world.terrainContour[midX] - _height;
                //if (midX > 0 && midX < world.terrainContour.Count()) {
                //    int yBorder = world.terrainContour[midX];

                //    if (yBorder > this.FighterArea.Bottom) {
                //        Gravity.FreeFall(this.Position);
                //    }
                //}
            }
            else {
                this.Position.Y = world.terrainContour[midX] - _height;
            }
        }

        public bool Intersects(ref Terrain world) {
            int midX = (int) this.Position.X + (_width/2);
            if (world.terrainContour[midX] <= this.Position.Y +_height ) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
