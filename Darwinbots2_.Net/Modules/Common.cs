using DarwinBots.Model;
using DarwinBots.Support;
using System;

namespace DarwinBots.Modules
{
    internal static class Common
    {
        public static string filemem { get; set; } = string.Empty;

        public static double Cross(DoubleVector v1, DoubleVector v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public static double Dot(DoubleVector v1, DoubleVector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static double Gauss(double stdDev, double mean = 0)
        {
            mean = Math.Clamp(mean, -32000, 32000);

            if (stdDev > 32000 || (stdDev != 0 && stdDev < 0.0000001))
                stdDev = 1;

            return Math.Clamp(GaussianDistribution() * stdDev + mean, -32000, 32000);
        }

        public static int NextLowestMultOfTwo(int value)
        {
            var a = 1;
            do
            {
                a *= 2;
            } while (!(a > value));

            return a / 2;
        }

        [Obsolete("Use rand.Next instead")]
        public static int Random(int low, int hi)
        {
            return ThreadSafeRandom.Local.Next(low, hi);
        }

        [Obsolete("Use .Magnitude instead")]
        public static double VectorMagnitude(DoubleVector v1)
        {
            return v1.Magnitude();
        }

        public static DoubleVector VectorMax(DoubleVector x, DoubleVector y)
        {
            return new()
            {
                X = Math.Max(x.X, y.X),
                Y = Math.Max(x.Y, y.Y)
            };
        }

        public static DoubleVector VectorMin(DoubleVector x, DoubleVector y)
        {
            return new()
            {
                X = Math.Min(x.X, y.X),
                Y = Math.Min(x.Y, y.Y)
            };
        }

        [Obsolete("Use new DoubleVector(x,y) instead")]
        public static DoubleVector VectorSet(double x, double y)
        {
            return new()
            {
                X = x,
                Y = y
            };
        }

        [Obsolete("Use - instead")]
        public static DoubleVector VectorSub(DoubleVector v1, DoubleVector v2)
        {
            return v1 - v2;
        }

        private static double GaussianDistribution()
        {
            var u1 = 1.0 - ThreadSafeRandom.Local.NextDouble();
            var u2 = 1.0 - ThreadSafeRandom.Local.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        }
    }
}
