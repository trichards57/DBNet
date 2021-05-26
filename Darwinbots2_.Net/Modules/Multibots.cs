using DBNet.Forms;
using static BucketManager;
using static Robots;
using static System.Math;

internal static class Multibots
{
    // Option Explicit
    // M U L T I C E L L U L A R   R O U T I N E S
    // moves the organism of which robot n is part to the position x,y

    public static void FreezeOrganism(int n)
    {
        var clist = new int[51];//changed from 20 to 50
        var t = 0;

        clist[0] = n;
        ListCells(clist);
        while (clist[t] > 0)
        {
            rob[clist[t]].highlight = true;
            t++;
        }
    }

    public static void KillOrganism(robot rob)
    {
        var clist = new int[51];//changed from 20 to 50
        var t = 0;

        clist[0] = n;
        ListCells(clist);
        var temp = MDIForm1.instance.nopoff;
        MDIForm1.instance.nopoff = true;
        while (clist[t] > 0)
        {
            KillRobot(clist[t]);
            t++;
        }
        MDIForm1.instance.nopoff = temp;
    }

    public static void ListCells(int[] lst)
    {
        var w = 0;
        var n = lst[w];

        while (n > 0)
        {
            dynamic _WithVar_2072;
            _WithVar_2072 = rob[n];
            if (!rob[n].Multibot)
            {
                goto skipties; // If the bot isn't a multibot, then ignore connected cells
            }
            var k = 1;
            while (_WithVar_2072.Ties(k).pnt > 0)
            {
                var pres = false;
                var j = 0;
                while (lst[j] > 0)
                {
                    if (lst[j] == _WithVar_2072.Ties(k).pnt)
                    {
                        pres = true;
                    }
                    j++;
                    if (j == 50)
                    {
                        lst[j] = 0;
                    }
                }
                if (!pres)
                {
                    lst[j] = _WithVar_2072.Ties(k).pnt;
                }
                k++;
            }
        skipties:
            w++;
            if (w > 50)
            {
                w = 50; //don't know what effect this will have. Should stop overflows
                lst[w] = 0; //EricL - added June 2006 to prevent overflows
                return;
            }
            n = lst[w];
        }
    }

    public static void ReSpawn(robot rob, double X, double Y)
    {
        var clist = new int[51];//changed from 20 to 50

        var nmin = 0;

        clist[0] = n;
        ListCells(clist);
        var Min = 999999999999.0;
        var t = 0;
        while (clist[t] > 0)
        {
            if (((rob[clist[t]].pos.X - X) ^ 2 + (rob[clist[t]].pos.Y - Y) ^ 2) <= Min)
            {
                Min = (rob[clist[t]].pos.X - X) ^ 2 + (rob[clist[t]].pos.Y - Y) ^ 2;
                nmin = clist[t];
            }
            t++;
            if (t > 50)
            {
                return;
            }
        }
        var dx = X - rob[nmin].pos.X;
        var dy = Y - rob[nmin].pos.Y;

        //Botsareus 7/15/2016 Bug fix: corrects by radii difference between the two robots
        var radiidif = rob[n].radius - rob[nmin].radius;

        dx = dx - 1 * Sign(dx) + Sign(dx) * radiidif;
        dy = dy - 1 * Sign(dy) + Sign(dy) * radiidif;

        t = 0;
        while (clist[t] > 0)
        {
            rob[clist[t]].pos.X = rob[clist[t]].pos.X + dx;
            rob[clist[t]].pos.Y = rob[clist[t]].pos.Y + dy;
            //Botsareus 7/6/2016 Make sure to resolve actvel
            rob[clist[t]].opos.X = rob[clist[t]].pos.X;
            rob[clist[t]].opos.Y = rob[clist[t]].pos.Y;
            //Bot is already part of a bucket...
            UpdateBotBucket(clist[t]);
            t++;
        }
    }

    /*
    ' kill organism
    */
    /*
    ' selects the whole organism
    */
    /*
    ' lists all the cells of an organism, starting from any one
    ' in position lst(0). Leaves the result in array lst()
    */
}
