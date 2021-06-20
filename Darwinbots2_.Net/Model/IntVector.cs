using System;

namespace DarwinBots.Model
{
    public class IntVector
    {
        private int x = 0;
        private int y = 0;

        public IntVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntVector()
        {
        }

        public int X { get => x; set => x = Math.Min(Math.Max(value, -32000), 32000); }
        public int Y { get => y; set => y = Math.Min(Math.Max(value, -32000), 32000); }

        public static IntVector operator -(IntVector v1, IntVector v2)
        {
            return new IntVector
            {
                X = v1.X - v2.X,
                Y = v1.Y - v2.Y
            };
        }

        public static IntVector operator *(IntVector v1, int k)
        {
            return new IntVector
            {
                X = v1.X * k,
                Y = v1.Y * k
            };
        }

        public static IntVector operator +(IntVector v1, IntVector v2)
        {
            return new IntVector
            {
                X = v1.X + v2.X,
                Y = v1.Y + v2.Y
            };
        }
    }
}
