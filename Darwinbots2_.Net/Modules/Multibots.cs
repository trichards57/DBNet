using DarwinBots.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DarwinBots.Modules
{
    internal static class Multibots
    {
        public static void FreezeOrganism(robot rob)
        {
            foreach (var c in ListCells(rob))
                c.highlight = true;
        }

        public static async Task KillOrganism(robot rob)
        {
            foreach (var c in ListCells(rob))
                await Robots.KillRobot(c);
        }

        public static void ReSpawn(robot rob, double X, double Y)
        {
            robot robmin = null;

            var cellList = ListCells(rob).ToList();
            var min = 999999999999.0;

            foreach (var cell in cellList)
            {
                var mag = Math.Pow(cell.pos.X - X, 2) + Math.Pow(cell.pos.Y - Y, 2);

                if (!(mag <= min)) continue;

                min = mag;
                robmin = cell;
            }

            var distance = new DoubleVector(X, Y) - robmin.pos;

            var radiidif = rob.radius - robmin.radius;

            distance += DoubleVector.Sign(distance) * (radiidif - 1);

            foreach (var cell in cellList)
            {
                cell.pos += distance;
                cell.opos = cell.pos;
                Globals.BucketManager.UpdateBotBucket(cell);
            }
        }

        private static IEnumerable<robot> ListCells(robot rob, HashSet<robot> checkedBots = null)
        {
            checkedBots ??= new HashSet<robot>();

            if (checkedBots.Contains(rob))
                return checkedBots;

            checkedBots.Add(rob);

            foreach (var tie in rob.Ties)
                ListCells(tie.OtherBot, checkedBots);

            return checkedBots;
        }
    }
}
