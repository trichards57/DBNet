using System;

namespace Iersera.Model
{
    public class vector
    {
        private double x = 0;
        private double y = 0;

        public vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public vector()
        {
        }

        public double X { get => x; set => x = Math.Min(Math.Max(value, -32000), 32000); }
        public double Y { get => y; set => y = Math.Min(Math.Max(value, -32000), 32000); }

        public static vector operator -(vector v1, vector v2)
        {
            return new vector
            {
                X = v1.X - v2.X,
                Y = v1.Y - v2.Y
            };
        }

        public static vector operator *(vector v1, double k)
        {
            return new vector
            {
                X = v1.X * k,
                Y = v1.Y * k
            };
        }

        public static vector operator +(vector v1, vector v2)
        {
            return new vector
            {
                X = v1.X + v2.X,
                Y = v1.Y + v2.Y
            };
        }

        public double Magnitude()
        {
            var minVal = Math.Min(Math.Abs(X), Math.Abs(Y));
            var maxVal = Math.Max(Math.Abs(X), Math.Abs(Y));

            return maxVal < 0.00001 ? 0 : maxVal * Math.Sqrt(Math.Pow(1 + (minVal / maxVal), 2));
        }

        public double MagnitudeSquare()
        {
            return X * X + Y * Y;
        }

        public vector Unit()
        {
            return this * (1.0 / Magnitude());
        }
    }
}
