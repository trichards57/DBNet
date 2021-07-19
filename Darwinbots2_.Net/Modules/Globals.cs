using DarwinBots.Model;
using System.Collections.Generic;

namespace DarwinBots.Modules
{
    internal static class Globals
    {
        public static BucketManager BucketManager { get; set; }
        public static ObstaclesManager ObstacleManager { get; set; }
        public static RobotsManager RobotsManager { get; set; }
        public static ShotsManager ShotsManager { get; set; }
        public static int StartChlr => 32000;
        public static int TotalChlr { get; set; }
        public static int TotalNotVegs { get; set; }
        public static int TotalNotVegsDisplayed { get; set; }
        public static List<Obstacle> xObstacle { get; } = new();
    }
}
