using DBNet.Forms;
using Iersera.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using static Globals;
using static Microsoft.VisualBasic.Conversion;
using static System.Math;
using static VBExtension;

internal static class Common
{
    public static int cprndy = 0;
    public static string filemem = "";
    public static int timerthis = 0;
    private static bool called = false;
    private static double gset;
    private static int iset;
    private static Random rand;

    public static double Cross(vector V1, vector V2)
    {
        return V1.X * V2.Y - V1.Y * V2.X;
    }

    public static double Dot(vector V1, vector V2)
    {
        return V1.X * V2.X + V1.Y * V2.Y;
    }

    public static int fRnd(int low, int up)
    {
        return CInt(rndy() * (up - low + 1) + low);
    }

    public static double Gauss(double StdDev, double Mean = 0)
    {
        double Gauss;

        //gasdev returns a gauss value with unit variance centered at 0

        //Protection against crazy values
        if (Mean < -32000)
            Mean = -32000;

        if (Mean > 32000)
            Mean = 32000;

        //Or is it Gauss = gasdev * stddev * stddev + mean
        if ((Abs(StdDev) < 0.0000001 && StdDev != 0) || Abs(StdDev) > 32000)
            Gauss = Mean + gasdev();
        else
            Gauss = gasdev() * StdDev + Mean;

        if (Gauss > 32000)
            Gauss = 32000;

        if (Gauss < -32000)
            Gauss = -32000;

        return Gauss;
    }

    public static int nextlowestmultof2(int value)
    {
        var a = 1;
        do
        {
            a *= 2;
        } while (!(a > value));

        return a / 2;
    }

    public static int Random(int low, int hi)
    {
        if (hi < low && hi == 0)
            return 0;

        return (int)Int((hi - low + 1) * rndy() + low);
    }

    public static void restarter()
    {
        if (called)
            return;

        called = true;

        traycleanup();

        var appPath = Assembly.GetEntryAssembly().Location;

        Process.Start(Path.GetDirectoryName(appPath) + "\\Restarter.exe", appPath);
    }

    public static double rndy()
    {
        return rand.NextDouble();
    }

    public static int TargetDNASize(int size)
    {
        var max = 250;
        for (var i = 1; i < size; i++)
        {
            if (i > max)
            {
                var overload = (5000 - max) / 4750;
                if (overload < 0)
                {
                    overload = 0;
                }
                max += 10 + overload * 250;
            }
        }
        return max;
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
            X = Max(x.X, y.X),
            Y = Max(x.Y, y.Y)
        };
    }

    public static vector VectorMin(vector x, vector y)
    {
        return new vector
        {
            X = Min(x.X, y.X),
            Y = Min(x.Y, y.Y)
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

    private static double gasdev()
    {
        double gasdev;

        if (iset == 0)
        {
            double V1;
            double V2;
            double rsq;
            do
            {
                V1 = 2 * rndy() - 1;
                V2 = 2 * rndy() - 1;
                rsq = V1 * V1 + V2 * V2;
            } while (rsq < 1 && rsq != 0);

            var fac = Sqrt(-2 * Log(rsq) / rsq);
            gset = V1 * fac;
            iset = 1;
            gasdev = V2 * fac;
        }
        else
        {
            iset = 0;
            gasdev = gset;
        }

        return gasdev;
    }

    private static void traycleanup()
    {
        if (HideDB)
        {
            MDIForm1.instance.Show();
            Form1.instance.t.Remove();
            MDIForm1.instance.stealthmode = false;
        }
    }
}
