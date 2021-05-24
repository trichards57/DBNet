using System.Collections.Generic;
using static varspecie;

internal static class SimOptModule
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

    //Set this high casue it does not use the Cost's control array
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

    //Botsareus 8/24/2013 The new chloroplast cost
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
    public static SimOptions SimOpts = null;

    // Used to count species in other sims for IM mode
    // Max number of species that can be in this sim
    public static List<Species> Species = new List<Species>(new Species[(MAXSPECIES + 1)]);  // TODO: Confirm Array Size By Token

    public static SimOptions TmpOpts = null;

    // definition of the SimOpts structure
    //BE VERY CAREFUL changing the TYPE of a variable
    //the read/write functions for saving sim settings/bots/simulations are
    //very particular about variable TYPE
    //Botsareus 3/15/2013 Had to resize this so it works better with nonnative species elimination
    //toroidal is updnconnected = dxsxconected = true
    // Indicates whether all mutations should be disabled
    //UserSeedToggle As Boolean 'Botsareus 5/3/2013 Replaced by safemode
    //used to tell the simulation that robots don't get to keep velocity they've acquired.
    //obsolete
    //above replaced by:
    //first 20 costs 0..19 are for DNA types (that's right, an extra 9 slots for future types)
    //then it goes:
    //move cost = 20
    //turn cost = 21
    //tie cost = 22
    //shot cost = 23
    //cost per bp per cycle = 24
    //cost per bp per copy = 25
    //Lots of room for future costs
    //the toggle
    //the variable
    //FlowType As Byte Botsareus 2/6/2013 never implemented
    //swimming constant will hopefully soon be either obsolete or revamped.
    // EricL 4/1/2006 Added this
    // EricL 4/1/2006 Added this
    // EricL 4/29/2006
    // EricL 5/7/2006 Used for initializing the field properties UI
    //EricL 5/7/2006 Used for iniitializing the costs radio button UI
    // EricL 6/8/2006 Used to indicate shots should not decay
    //Botsareus 9/28/2013 Do not decay waste shots
    //EricL 6/7/2006 Indicates if we are using the option of setting the sun at an nrg threshold
    //EricL 6/7/2006 Indicates if we are using the option to have the sun rise on a nrg threshold
    // Indicates whether dynamic cost adjustment is enabled
    //Shapes Stuff
    // Flag indicates whether to populate bot eye values for shapes
    //Egrid Stuff
    // Highest Maximum number assigned so far in the sim
    // Unique ID for this sim
    //Botsareus 4/18/2016 Put (simple) recording back
    public class SimOptions
    {
        public bool allowHorizontalShapeDrift = false;
        public bool allowVerticalShapeDrift = false;
        public int BadWastelevel = 0;
        public bool BlockedVegs = false;
        public int chartingInterval = 0;
        public decimal CoefficientElasticity = 0;
        public decimal CoefficientKinetic = 0;
        public decimal CoefficientStatic = 0;
        public bool CorpseEnabled = false;
        public double CostExecCond = 0;
        public int CostRadioSetting = 0;
        public double[] Costs = new decimal[70];
        public int CycleLength = 0;
        public decimal CycSec = 0;
        public bool DayNight = false;
        public int DayNightCycleCounter = 0;
        public bool Daytime = false;
        public bool DeadRobotSnp = false;
        public decimal Decay = 0;
        public int Decaydelay = 0;
        public int DecayType = 0;
        public decimal Density = 0;
        public decimal Diffuse = 0;
        public bool DisableFixing = false;
        public bool DisableMutations = false;
        public bool DisableTies = false;
        public bool DisableTypArepro = false;
        public bool Dxsxconnected = false;
        public bool DynamicCosts = false;
        public bool EGridEnabled = false;
        public int EGridWidth = 0;
        public bool EnableAutoSpeciation = false;
        public bool EnergyExType = false;
        public int EnergyFix = 0;
        public decimal EnergyProp = 0;
        public bool F1 = false;
        public int FieldHeight = 0;
        public int FieldSize = 0;
        public int FieldWidth = 0;
        public bool FixedBotRadii = false;
        public int FluidSolidCustom = 0;
        public double Gradient = 0;
        public bool KillDistVegs = false;
        public bool League = false;
        public int LightIntensity = 0;
        public bool makeAllShapesBlack = false;
        public bool makeAllShapesTransparent = false;
        public int MaxAbsNum = 0;
        public int MaxEnergy = 0;
        public int MaxPopulation = 0;
        public double MaxVelocity = 0;
        public int MinVegs = 0;
        public double MutCurrMult = 0;
        public int MutCycMax = 0;
        public int MutCycMin = 0;
        public bool MutOscill = false;
        public bool MutOscillSine = false;
        public bool NoShotDecay = false;
        public bool NoWShotDecay = false;
        public double oldCostX = 0;
        public decimal PhysBrown = 0;
        public decimal PhysMoving = 0;
        public decimal PhysSwim = 0;
        public bool PlanetEaters = false;
        public decimal PlanetEatersG = 0;
        public bool Pondmode = false;
        public int PopLimMethod = 0;
        public int RepopAmount = 0;
        public int RepopCooldown = 0;
        public bool Restart = false;
        public int shapeDriftRate = 0;
        public bool shapesAbsorbShots = false;
        public bool shapesAreSeeThrough = false;
        public bool shapesAreVisable = false;
        public int SimGUID = 0;
        public string SimName = "";
        public bool SnpExcludeVegs = false;
        public int SpeciationForkInterval = 0;
        public int SpeciationGenerationalDistance = 0;
        public int SpeciationGeneticDistance = 0;
        public int SpeciationMinimumPopulation = 0;
        public List<Species> Specie = new List<Species>();
        public int SpeciesNum = 0;
        public bool SunDown = false;
        public int SunDownThreshold = 0;
        public bool SunOnRnd = false;
        public int SunThresholdMode = 0;
        public bool SunUp = false;
        public int SunUpThreshold = 0;
        public int Tides = 0;
        public int TidesOf = 0;
        public bool Toroidal = false;
        public int TotBorn = 0;
        public int TotRunCycle = 0;
        public int TotRunTime = 0;
        public bool Updnconnected = false;
        public int UserSeedNumber = 0;
        public double VegFeedingToBody = 0;
        public decimal Viscosity = 0;
        public decimal Ygravity = 0;
        public bool ZeroMomentum = false;
        public decimal Zgravity = 0;
    }
}
