using DarwinBots.Model;
using System.Collections.Generic;

namespace DarwinBots.Modules
{
    internal static class Globals
    {
        public static BucketManager BucketManager { get; set; }
        public static bool Delta2 { get; set; }
        public static int DeltaDevChance { get; set; }
        public static double DeltaDevExp { get; set; }
        public static double DeltaDevLn { get; set; }
        public static int DeltaMainChance { get; set; }
        public static double DeltaMainExp { get; set; }
        public static double DeltaMainLn { get; set; }
        public static int DeltaPm { get; set; }
        public static int DeltaWtc { get; set; }
        public static bool EpiReset { get; set; }
        public static double EpiResetEmp { get; set; }
        public static int EpiResetOp { get; set; }
        public static bool NormMut { get; set; }
        public static ObstaclesManager ObstacleManager { get; internal set; }
        public static bool ReproFix { get; set; }
        public static bool SimAlreadyRunning { get; set; }
        public static int StartChlr { get; set; }
        public static bool SunBelt { get; set; }
        public static int TotalChlr { get; set; }
        public static int TotalNotVegs { get; set; }
        public static int TotalNotVegsDisplayed { get; set; }
        public static bool UseEpiGene { get; set; }
        public static bool UseSafeMode { get; set; }
        public static int ValMaxNormMut { get; set; }
        public static int ValNormMut { get; set; }
        public static List<Obstacle> xObstacle { get; } = new();
        public static bool y_normsize { get; set; }
    }
}
