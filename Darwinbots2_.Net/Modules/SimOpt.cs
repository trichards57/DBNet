using DarwinBots.Model;
using System.Collections.Generic;

namespace DarwinBots.Modules
{
    internal static class SimOpt
    {
        public const int MaxNativeSpecies = 76;

        public static SimOptions SimOpts { get; set; }
        public static List<Species> Species { get; } = new();
    }
}
