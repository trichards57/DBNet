using System;
using System.IO;
using System.Threading.Tasks;
using static Globals;
using static Master;
using static SimOpt;

internal static class Evo
{
    private static int oldid;

    private static double oldMx;

    public static double CalculateExactHandycap()
    {
        return energydifXP - energydifXP2;
    }

    public static double CalculateHandycap()
    {
        return SimOpts.TotRunCycle < (hidePredCycl * 8)
            ? CalculateExactHandycap() * SimOpts.TotRunCycle / (hidePredCycl * 8)
            : CalculateExactHandycap();
    }

    public static async Task LogEvolution(string s, int idx = -1)
    {
        await File.AppendAllTextAsync($@"evolution\log{(idx > -1 ? idx : "")}.txt", $"{s} {DateTime.Now}\n");
    }
}
