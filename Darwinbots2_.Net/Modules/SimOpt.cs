using DarwinBots.Model;
using System.Collections.Generic;

namespace DarwinBots.Modules
{
    internal static class SimOpt
    {
        public const int ADCMDCOST = 3;
        public const int ADVANCESUN = 2;
        public const int AGECOST = 31;
        public const int AGECOSTLINEARFRACTION = 33;
        public const int AGECOSTMAKELINEAR = 60;
        public const int AGECOSTMAKELOG = 51;
        public const int AGECOSTSTART = 32;
        public const int ALLOWNEGATIVECOSTX = 62;
        public const int BCCMDCOST = 2;
        public const int BODYUPKEEP = 30;
        public const int BOTNOCOSTLEVEL = 52;
        public const int BTCMDCOST = 4;
        public const int CHLRCOST = 8;
        public const int CONDCOST = 5;
        public const int COSTMULTIPLIER = 54;
        public const int COSTSTORE = 7;
        public const int COSTXREINSTATEMENTLEVEL = 59;
        public const int DNACOPYCOST = 25;
        public const int DNACYCCOST = 24;
        public const int DOTNUMCOST = 1;
        public const int DYNAMICCOSTINCLUDEPLANTS = 61;
        public const int DYNAMICCOSTSENSITIVITY = 55;
        public const int DYNAMICCOSTTARGET = 53;
        public const int DYNAMICCOSTTARGETLOWERRANGE = 58;
        public const int DYNAMICCOSTTARGETUPPERRANGE = 57;
        public const int FLOWCOST = 9;
        public const int LOGICCOST = 6;
        public const int MAXNATIVESPECIES = 76;
        public const int MAXNUMEYES = 8;
        public const int MAXSPECIES = 500;
        public const int MOVECOST = 20;
        public const int NUMCOST = 0;
        public const int PERMSUNSUSPEND = 1;
        public const int POISONCOST = 27;
        public const int SHELLCOST = 29;
        public const int SHOTCOST = 23;
        public const int SLIMECOST = 28;
        public const int TEMPSUNSUSPEND = 0;
        public const int TIECOST = 22;
        public const int TURNCOST = 21;
        public const int USEDYNAMICCOSTS = 56;
        public const int VENOMCOST = 26;

        public static SimOptions SimOpts { get; set; }
        public static List<Species> Species { get; set; } = new();
        public static SimOptions TmpOpts { get; set; }
    }
}
