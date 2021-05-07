using DBNet.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static Globals;
using static Master;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.VBMath;
using static System.Math;
using static VBExtension;

internal static class Common
{
    public static int cprndy = 0;
    public static string filemem = "";
    public static List<double> rndylist = new List<double> { };
    public static int timerthis = 0;
    private static bool called = false;
    private static double gset;
    private static int iset;
    private static int y;

    public static double Cross(vector V1, vector V2)
    {
        return V1.x * V2.y - V1.y * V2.x;
    }

    public static double Dot(vector V1, vector V2)
    {
        return V1.x * V2.x + V1.y * V2.y;
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

    [DllImport("user32.dll")] public static extern int GetInputState();

    [DllImport("kernel32.dll")] public static extern int GetTickCount();

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
        var Random = (int)Int((hi - low + 1) * rndy() + low);
        if (hi < low && hi == 0)
            Random = 0;

        return Random;
    }

    public static void restarter()
    {
        if (called)
            return;

        called = true;

        traycleanup();
        Process.Start(App.path + "\\Restarter.exe " + App.path + "\\" + App.EXEName);
    }

    public static double rndy()
    {
        double rndy = 0;
        if (UseIntRnd)
        {
            if (y == 0)
            {
                Randomize(rndylist[cprndy]);
                cprndy++;
                if (cprndy == 3900)
                    savenow = true; //Time for an autosave

                if (cprndy > 3999)
                {
                    cprndy = 0;
                    File.Delete(App.path + "\\" + filemem); ; //kill current file and grab the next
                    MDIForm1.instance.grabfile();
                }
                y = (int)(Rnd() * 1500) + 1;
            }

            rndy = Rnd();

            y--;
        }
        else
            rndy = Rnd();

        return rndy;
    }

    public static int TargetDNASize(int size)
    {
        var Max = 250;
        for (var i = 1; i < size; i++)
        {
            if (i > Max)
            {
                var overload = (5000 - Max) / 4750;
                if (overload < 0)
                {
                    overload = 0;
                }
                Max = Max + 10 + overload * 250;
            }
        }
        return Max;
    }

    public static vector VectorAdd(vector V1, vector V2)
    {
        return new vector
        {
            x = V1.x + V2.x,
            y = V1.y + V2.y
        };
    }

    public static double VectorInvMagnitude(vector V1)
    {
        var mag = VectorMagnitude(V1);
        return mag == 0 ? -1 : 1 / mag;
    }

    public static double VectorMagnitude(vector V1)
    {
        // This might seem overly complicated compared to sqr(X^2 + Y^2),
        // But it gives better numerical behavior
        var minVal = Min(Abs(V1.x), Abs(V1.y));
        var maxVal = Max(Abs(V1.x), Abs(V1.y));

        return maxVal < 0.00001 ? 0 : maxVal * Sqrt(Pow(1 + (minVal / maxVal), 2));
    }

    public static double VectorMagnitudeSquare(vector V1)
    {
        if (Abs(V1.x) > 32000)
            V1.x = Sign(V1.x) * 32000;

        if (Abs(V1.y) > 32000)
            V1.y = Sign(V1.y) * 32000;

        return V1.x * V1.x + V1.y * V1.y;
    }

    public static vector VectorMax(vector x, vector y)
    {
        return new vector
        {
            x = Max(x.x, y.x),
            y = Max(x.y, y.y)
        };
    }

    public static vector VectorMin(vector x, vector y)
    {
        return new vector
        {
            x = Min(x.x, y.x),
            y = Min(x.y, y.y)
        };
    }

    public static vector VectorScalar(vector V1, double k)
    {
        if (Abs(k) > 32000)
            k = Sign(k) * 32000;

        if (Abs(V1.x) > 32000)
            V1.x = Sign(V1.x) * 32000;

        if (Abs(V1.y) > 32000)
            V1.y = Sign(V1.y) * 32000;

        return new vector
        {
            x = V1.x * k,
            y = V1.y * k
        };
    }

    public static vector VectorSet(double x, double y)
    {
        return new vector
        {
            x = x,
            y = y
        };
    }

    public static vector VectorSub(vector V1, vector V2)
    {
        return new vector
        {
            x = V1.x - V2.x,
            y = V1.y - V2.y
        };
    }

    public static vector VectorUnit(vector V1)
    {
        var mag = VectorInvMagnitude(V1);

        return new vector
        {
            x = V1.x * mag,
            y = V1.y * mag
        };
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

    public class vector
    {
        public double x = 0;
        public double y = 0;
    }
}
