using System;

namespace DarwinBots.Model
{
    public record IntVector
    {
        private readonly int _x;
        private readonly int _y;

        public int X { get => _x; init => _x = Math.Clamp(value, -32000, 32000); }
        public int Y { get => _y; init => _y = Math.Clamp(value, -32000, 32000); }

        public IntVector() { }

        public IntVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator DoubleVector(IntVector vector)
        {
            return new(vector.X, vector.Y);
        }

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
