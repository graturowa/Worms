using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGameTest {
    class Terrain {
        public Texture2D texture;
        private Texture2D output;
        public int screenWidth;
        public int screenHeight;
        public int[] terrainContour;
        private Random randomizer = new Random();
        public SpriteBatch spriteBatch;
        public int rightOffset;
        public int topOffset;

        #region construct
        public Terrain() { }

        public Terrain(SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
        }
        #endregion

        public void Initialize() {
            GenerateTerrainContour();
            CreateForeground();
        }

        public void Render() {
            Rectangle screenRectangle = new Rectangle(-rightOffset, -topOffset, screenWidth, screenHeight);
            spriteBatch.Draw(output, screenRectangle, Color.White);
        }

        private void GenerateTerrainContour() {
            terrainContour = new int[screenWidth];

            double rand1 = randomizer.NextDouble() + 1;
            double rand2 = randomizer.NextDouble() + 2;
            double rand3 = randomizer.NextDouble() + 3;

            float offset = screenHeight / (float)1.5;
            float peakheight = 100;
            float flatness = 100;

            for (int x = 0; x < screenWidth; x++) {
                double height = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakheight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakheight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                terrainContour[x] = (int)height;
            }
        }

        private void CreateForeground() {
            Color[,] groundColors = TextureTo2DArray(texture);
            Color[] foregroundColors = new Color[screenWidth * screenHeight];

            for (int x = 0; x < screenWidth; x++) {
                for (int y = 0; y < screenHeight; y++) {
                    if (y > terrainContour[x]) {
                        foregroundColors[x + y * screenWidth] = Color.BurlyWood;
                        foregroundColors[x + y * screenWidth] = groundColors[x % texture.Width, y % texture.Height];
                    }
                    else {
                        foregroundColors[x + y * screenWidth] = Color.Transparent;
                    }
                }
            }

            output = new Texture2D(spriteBatch.GraphicsDevice, this.screenWidth, this.screenHeight, true, SurfaceFormat.Color);
            output.SetData(foregroundColors);
        }

        private Color[,] TextureTo2DArray(Texture2D texture) {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        public bool Intersects(Vector2 position) {
            return position.Y >= this.terrainContour[Convert.ToInt32(position.X+rightOffset)];
        }

        public bool Intersects(Rectangle obj) {
            return this.Intersects(new Vector2((float)(obj.Right + obj.Width / 2), obj.Bottom));
        }
    }
}
