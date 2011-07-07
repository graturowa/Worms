using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGameTest {
    static class Gravity {
        public static int speed = 5;

        /// <summary>
        /// This simulates free fall from some height
        /// </summary>
        /// <param name="beginingPosition">Position from which object is falling</param>
        /// <returns>Position in next tick</returns>
        public static Vector2 FreeFall(Vector2 beginingPosition) {
            Vector2 result = beginingPosition;

            result.Y += speed;

            return result;
        }
    }
}
