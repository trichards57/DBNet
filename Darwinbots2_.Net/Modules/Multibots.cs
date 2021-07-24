using DarwinBots.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarwinBots.Modules
{
    internal static class Multibots
    {
        public static void ReSpawn(IBucketManager bucketManager, Robot rob, double x, double y)
        {
            var cellList = ListCells(rob).ToList();
            var min = 999999999999.0;
            var robmin = cellList.First();

            foreach (var cell in cellList)
            {
                var mag = Math.Pow(cell.Position.X - x, 2) + Math.Pow(cell.Position.Y - y, 2);

                if (!(mag <= min)) continue;

                min = mag;
                robmin = cell;
            }

            var distance = new DoubleVector(x, y) - robmin.Position;

            var radiiDiff = rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) - robmin.GetRadius(SimOpt.SimOpts.FixedBotRadii);

            distance += DoubleVector.Sign(distance) * (radiiDiff - 1);

            foreach (var cell in cellList)
            {
                cell.Position += distance;
                cell.OldPosition = cell.Position;
                bucketManager.UpdateBotBucket(cell);
            }
        }

        private static IEnumerable<Robot> ListCells(Robot rob, HashSet<Robot> checkedBots = null)
        {
            checkedBots ??= new HashSet<Robot>();

            if (checkedBots.Contains(rob))
                return checkedBots;

            checkedBots.Add(rob);

            foreach (var tie in rob.Ties)
                ListCells(tie.OtherBot, checkedBots);

            return checkedBots;
        }
    }
}
