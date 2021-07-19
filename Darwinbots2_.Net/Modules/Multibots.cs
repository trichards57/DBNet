using DarwinBots.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarwinBots.Modules
{
    internal static class Multibots
    {
        public static void ReSpawn(robot rob, double x, double y)
        {
            var cellList = ListCells(rob).ToList();
            var min = 999999999999.0;
            var robmin = cellList.First();

            foreach (var cell in cellList)
            {
                var mag = Math.Pow(cell.pos.X - x, 2) + Math.Pow(cell.pos.Y - y, 2);

                if (!(mag <= min)) continue;

                min = mag;
                robmin = cell;
            }

            var distance = new DoubleVector(x, y) - robmin.pos;

            var radiiDiff = rob.Radius - robmin.Radius;

            distance += DoubleVector.Sign(distance) * (radiiDiff - 1);

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
