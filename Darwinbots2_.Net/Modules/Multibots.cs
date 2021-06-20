using DarwinBots.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DarwinBots.Modules.BucketManager;

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
            //var temp = MDIForm1.instance.nopoff;
            //MDIForm1.instance.nopoff = true;

            //foreach (var c in ListCells(rob))
            //    await KillRobot(c);

            //MDIForm1.instance.nopoff = temp;
        }

        public static IEnumerable<robot> ListCells(robot rob, HashSet<robot> checkedBots = null)
        {
            if (checkedBots == null)
                checkedBots = new HashSet<robot>();

            if (checkedBots.Contains(rob))
                return checkedBots;

            checkedBots.Add(rob);

            foreach (var tie in rob.Ties)
                ListCells(tie.OtherBot, checkedBots);

            return checkedBots;
        }

        public static void ReSpawn(robot rob, double X, double Y)
        {
            robot robmin = null;

            var clist = ListCells(rob);
            var Min = 999999999999.0;

            foreach (var cell in clist)
            {
                var mag = Math.Pow(cell.pos.X - X, 2) + Math.Pow(cell.pos.Y - Y, 2);
                if (mag <= Min)
                {
                    Min = mag;
                    robmin = cell;
                }
            }

            var dx = X - robmin.pos.X;
            var dy = Y - robmin.pos.Y;

            //Botsareus 7/15/2016 Bug fix: corrects by radii difference between the two robots
            var radiidif = rob.radius - robmin.radius;

            dx = dx - 1 * Math.Sign(dx) + Math.Sign(dx) * radiidif;
            dy = dy - 1 * Math.Sign(dy) + Math.Sign(dy) * radiidif;

            foreach (var cell in clist)
            {
                cell.pos.X += dx;
                cell.pos.Y += dy;
                cell.opos.X = cell.pos.X;
                cell.opos.Y = cell.pos.Y;
                UpdateBotBucket(cell);
            }
        }
    }
}
