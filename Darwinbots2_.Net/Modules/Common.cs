using Iersera.Model;
using Iersera.Support;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

internal static class Common
{
    public static string filemem = "";
    private static bool called = false;

    public static double Cross(vector V1, vector V2)
    {
        return V1.X * V2.Y - V1.Y * V2.X;
    }

    public static double Dot(vector V1, vector V2)
    {
        return V1.X * V2.X + V1.Y * V2.Y;
    }

    [Obsolete("Use rand.Next instead")]
    public static int fRnd(int low, int up)
    {
        return ThreadSafeRandom.Local.Next(low, up);
    }

    public static double Gauss(double stdDev, double mean = 0)
    {
        mean = Math.Clamp(mean, -32000, 32000);

        if (stdDev > 32000 || (stdDev != 0 && stdDev < 0.0000001))
            stdDev = 1;

        return Math.Clamp(GaussianDistribution() * stdDev + mean, -32000, 32000);
    }

    [Obsolete("Use rand.NextDouble instead")]
    public static double ThreadSafeRandom.Local.NextDouble()
    {
        return ThreadSafeRandom.Local.NextDouble();
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

    public static void Restarter()
    {
        if (called)
            return;

        called = true;

        var appPath = Assembly.GetEntryAssembly().Location;

        Process.Start(Path.GetDirectoryName(appPath) + "\\Restarter.exe", appPath);
    }

    [Obsolete("Use + operator instead")]
    public static vector VectorAdd(vector V1, vector V2)
    {
        return V1 + V2;
    }

    [Obsolete("Use .Magnitude instead")]
    public static double VectorInvMagnitude(vector V1)
    {
        var mag = V1.Magnitude();
        return mag == 0 ? -1 : 1 / mag;
    }

    [Obsolete("Use .Magnitude instead")]
    public static double VectorMagnitude(vector V1)
    {
        return V1.Magnitude();
    }

    [Obsolete("Use .MagnitudeSquare instead")]
    public static double VectorMagnitudeSquare(vector V1)
    {
        return V1.MagnitudeSquare();
    }

    public static vector VectorMax(vector x, vector y)
    {
        return new vector
        {
            X = Math.Max(x.X, y.X),
            Y = Math.Max(x.Y, y.Y)
        };
    }

    public static vector VectorMin(vector x, vector y)
    {
        return new vector
        {
            X = Math.Min(x.X, y.X),
            Y = Math.Min(x.Y, y.Y)
        };
    }

    [Obsolete("Use * instead")]
    public static vector VectorScalar(vector V1, double k)
    {
        return V1 * k;
    }

    [Obsolete("Use new vector(x,y) instead")]
    public static vector VectorSet(double x, double y)
    {
        return new vector
        {
            X = x,
            Y = y
        };
    }

    [Obsolete("Use - instead")]
    public static vector VectorSub(vector V1, vector V2)
    {
        return V1 - V2;
    }

    [Obsolete("Use .Unit instead")]
    public static vector VectorUnit(vector V1)
    {
        return V1.Unit();
    }

    private static double GaussianDistribution()
    {
        var u1 = 1.0 - ThreadSafeRandom.Local.NextDouble();
        var u2 = 1.0 - ThreadSafeRandom.Local.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
    }
}
