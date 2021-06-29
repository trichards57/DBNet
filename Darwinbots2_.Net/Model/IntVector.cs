using System;

namespace DarwinBots.Model
{
    public class IntVector
    {
        private int _x;
        private int _y;

        public IntVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntVector()
        {
        }

        public int X { get => _x; set => _x = Math.Clamp(value, -32000, 32000); }
        public int Y { get => _y; set => _y = Math.Clamp(value, -32000, 32000); }

        public static IntVector operator -(IntVector v1, IntVector v2)
        {
            return new()
            {
                X = v1.X - v2.X,
                Y = v1.Y - v2.Y
            };
        }

        public static IntVector operator *(IntVector v1, int k)
        {
            return new()
            {
                X = v1.X * k,
                Y = v1.Y * k
            };
        }

        public static IntVector operator +(IntVector v1, IntVector v2)
        {
            return new()
            {
                X = v1.X + v2.X,
                Y = v1.Y + v2.Y
            };
        }
    }
}
