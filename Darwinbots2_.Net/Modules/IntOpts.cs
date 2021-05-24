using System;
using static Common;
using static SimOptModule;
using static varspecie;

internal static class IntOpts
{
    public const int MAXINTERNETSIMS = 100;

    //This stuff is needed so graphing works
    public const int MAXINTERNETSPECIES = 500;

    public static bool Active = false;

    // Option Explicit
    //Persistant Settings
    public static string IName = "";

    public static string InboundPath = "";
    public static bool InternetMode = false;
    public static Species[] InternetSpecies = new Species[(MAXINTERNETSPECIES + 1)];
    public static string[] namesOfInternetBots = new string[(MAXINTERNETSPECIES + 1)];

    // TODO: Confirm Array Size By Token// Used for graphing the number of species in the inter connected internet sim
    public static int numInternetSpecies = 0;

    public static string OutboundPath = "";

    //This is the window handle to DarwinbotsIM
    public static int pid = 0;

    public static string ServIP = "";
    public static string ServPort = "";
    public static bool StartInInternetMode = false;
    // TODO: Confirm Array Size By Token
    // gives an internet organism his absurd name

    public static string AttribuisciNome()
    {
        var p = "dt" + DateTime.Today.ToString("yymmdd");
        p = p + "cn" + "00";
        p = p + "mf" + ((int)(SimOpts.PhysMoving * 100));
        p = p + "bm" + ((int)(SimOpts.PhysBrown * 100));
        p = p + "sf" + ((int)(SimOpts.PhysSwim * 100));
        p = p + "ac" + ((int)(SimOpts.CostExecCond * 100));
        p = p + "sc" + ((int)(SimOpts.Costs[COSTSTORE] * 100));
        p = p + "ce" + ((int)(SimOpts.Costs[SHOTCOST] * 100));
        if (SimOpts.EnergyExType)
        {
            p = p + "et" + ((int)(SimOpts.EnergyProp * 100));
            p = p + "tt1";
        }
        else
        {
            p = p + "et" + (SimOpts.EnergyFix * 100);
            p = p + "tt2";
        }
        p = p + "rc" + Random(0, 99999);
        p = p + ".dbo";
        return p;
    }
}
