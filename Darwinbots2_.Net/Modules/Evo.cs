using System;
using System.IO;
using System.Threading.Tasks;
using static DarwinBots.Modules.Globals;
using static DarwinBots.Modules.Master;
using static DarwinBots.Modules.SimOpt;

namespace DarwinBots.Modules
{
    internal static class Evo
    {
        public static double CalculateExactHandycap()
        {
            return energydifXP - energydifXP2;
        }

        public static double CalculateHandycap()
        {
            return SimOpts.TotRunCycle < hidePredCycl * 8
                ? CalculateExactHandycap() * SimOpts.TotRunCycle / (hidePredCycl * 8)
                : CalculateExactHandycap();
        }

        public static async Task LogEvolution(string s, int idx = -1)
        {
            await File.AppendAllTextAsync($@"evolution\log{(idx > -1 ? idx : "")}.txt", $"{s} {DateTime.Now}\n");
        }
    }
}
