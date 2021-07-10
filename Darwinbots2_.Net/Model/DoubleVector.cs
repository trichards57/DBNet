using System;

namespace DarwinBots.Model
{
    public record DoubleVector
    {
        private readonly double _x;
        private readonly double _y;

        public double X { get => _x; init => _x = Math.Clamp(value, -32000, 32000); }
        public double Y { get => _y; init => _y = Math.Clamp(value, -32000, 32000); }

        public DoubleVector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public DoubleVector() { }

        public static DoubleVector operator -(DoubleVector v1, DoubleVector v2)
        {
            return new()
            {
                X = v1.X - v2.X,
                Y = v1.Y - v2.Y
            };
        }

        public static DoubleVector operator *(DoubleVector v1, double k)
        {
            return new()
            {
                X = v1.X * k,
                Y = v1.Y * k
            };
        }

        public static DoubleVector operator /(DoubleVector v1, double k)
        {
            return new()
            {
                X = v1.X / k,
                Y = v1.Y / k
            };
        }

        public static DoubleVector operator +(DoubleVector v1, DoubleVector v2)
        {
            return new()
            {
                X = v1.X + v2.X,
                Y = v1.Y + v2.Y
            };
        }

        public IntVector ToIntVector()
        {
            return new()
            {
                X = (int)X,
                Y = (int)Y
            };
        }

        public static DoubleVector Floor(DoubleVector v1)
        {
            return new()
            {
                X = Math.Floor(v1.X),
                Y = Math.Floor(v1.Y)
            };
        }

        public static DoubleVector Max(DoubleVector v1, double val)
        {
            return new()
            {
                X = Math.Max(v1.X, val),
                Y = Math.Max(v1.Y, val)
            };
        }

        public static DoubleVector Min(DoubleVector v1, double val)
        {
            return new()
            {
                X = Math.Min(v1.X, val),
                Y = Math.Min(v1.Y, val)
            };
        }

        public static DoubleVector Clamp(DoubleVector v1, double min, double max)
        {
            return new()
            {
                X = Math.Clamp(v1.X, min, max),
                Y = Math.Clamp(v1.Y, min, max)
            };
        }

        public static DoubleVector Zero { get; } = new DoubleVector(0, 0);

        public static DoubleVector Clamp(DoubleVector v1, DoubleVector min, DoubleVector max)
        {
            return new()
            {
                X = Math.Clamp(v1.X, min.X, max.X),
                Y = Math.Clamp(v1.Y, min.Y, max.Y)
            };
        }

        public double Magnitude()
        {
            var minVal = Math.Min(Math.Abs(X), Math.Abs(Y));
            var maxVal = Math.Max(Math.Abs(X), Math.Abs(Y));

            return maxVal < 0.00001 ? 0 : maxVal * Math.Sqrt(Math.Pow(1 + minVal / maxVal, 2));
        }

        public double MagnitudeSquare()
        {
            return X * X + Y * Y;
        }

        public DoubleVector Unit()
        {
            return this * (1.0 / Magnitude());
        }

        public DoubleVector InvertX()
        {
            return new(-X, Y);
        }

        public DoubleVector InvertY()
        {
            return new(X, -Y);
        }
    }
}
