using System;
using System.Windows;

namespace DarwinBots.Model
{
    public struct DoubleVector
    {
        private readonly double _x;
        private readonly double _y;

        public DoubleVector(double x, double y)
        {
            _x = Math.Clamp(x, -32000, 32000);
            _y = Math.Clamp(y, -32000, 32000);
        }

        public static DoubleVector Zero { get; } = new(0, 0);

        public double X { get => _x; init => _x = Math.Clamp(value, -32000, 32000); }

        public double Y { get => _y; init => _y = Math.Clamp(value, -32000, 32000); }

        public static DoubleVector Clamp(DoubleVector v1, double min, double max)
        {
            return new()
            {
                X = Math.Clamp(v1.X, min, max),
                Y = Math.Clamp(v1.Y, min, max)
            };
        }

        public static DoubleVector Clamp(DoubleVector v1, DoubleVector min, DoubleVector max)
        {
            return new()
            {
                X = Math.Clamp(v1.X, min.X, max.X),
                Y = Math.Clamp(v1.Y, min.Y, max.Y)
            };
        }

        public static double Cross(DoubleVector v1, DoubleVector v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public static double Dot(DoubleVector v1, DoubleVector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static DoubleVector Floor(DoubleVector v1)
        {
            return new()
            {
                X = Math.Floor(v1.X),
                Y = Math.Floor(v1.Y)
            };
        }

        public static implicit operator Point(DoubleVector vector)
        {
            return new Point((int)Math.Round(vector.X), (int)Math.Round(vector.Y));
        }

        public static implicit operator Size(DoubleVector vector)
        {
            return new Size((int)Math.Round(vector.X), (int)Math.Round(vector.Y));
        }

        public static DoubleVector Max(DoubleVector x, DoubleVector y)
        {
            return new()
            {
                X = Math.Max(x.X, y.X),
                Y = Math.Max(x.Y, y.Y)
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

        public static DoubleVector Min(DoubleVector x, DoubleVector y)
        {
            return new()
            {
                X = Math.Min(x.X, y.X),
                Y = Math.Min(x.Y, y.Y)
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

        public static DoubleVector operator -(DoubleVector v1, DoubleVector v2)
        {
            return new()
            {
                X = v1.X - v2.X,
                Y = v1.Y - v2.Y
            };
        }

        public static bool operator !=(DoubleVector left, DoubleVector right)
        {
            return !(left == right);
        }

        public static DoubleVector operator *(DoubleVector v1, double k)
        {
            return new()
            {
                X = v1.X * k,
                Y = v1.Y * k
            };
        }

        public static DoubleVector operator *(DoubleVector v1, DoubleVector v2)
        {
            return new()
            {
                X = v1.X * v2.X,
                Y = v1.Y * v2.Y
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

        public static bool operator ==(DoubleVector left, DoubleVector right)
        {
            return left.Equals(right);
        }

        public static DoubleVector Sign(DoubleVector v)
        {
            return new()
            {
                X = Math.Sign(v.X),
                Y = Math.Sign(v.Y)
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is IntVector vector)
                return vector.X == X && vector.Y == Y;

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public DoubleVector InvertX()
        {
            return new(-X, Y);
        }

        public DoubleVector InvertY()
        {
            return new(X, -Y);
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

        public IntVector ToIntVector()
        {
            return new()
            {
                X = (int)X,
                Y = (int)Y
            };
        }

        public DoubleVector Unit()
        {
            return this * (1.0 / Magnitude());
        }
    }
}
