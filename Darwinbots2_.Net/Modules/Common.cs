using DarwinBots.Support;
using System;

namespace DarwinBots.Modules
{
    internal static class Common
    {
        public static double Gauss(double stdDev, double mean = 0)
        {
            mean = Math.Clamp(mean, -32000, 32000);

            if (stdDev > 32000 || (stdDev != 0 && stdDev < 0.0000001))
                stdDev = 1;

            return Math.Clamp(GaussianDistribution() * stdDev + mean, -32000, 32000);
        }

        private static double GaussianDistribution()
        {
            var u1 = 1.0 - ThreadSafeRandom.Local.NextDouble();
            var u2 = 1.0 - ThreadSafeRandom.Local.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        }
    }
}
