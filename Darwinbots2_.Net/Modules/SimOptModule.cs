using VB6 = Microsoft.VisualBasic.Compatibility.VB6;
using System.Runtime.InteropServices;
using static VBExtension;
using static VBConstants;
using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Windows.Controls;
using static System.DateTime;
using static System.Math;
using static Microsoft.VisualBasic.Globals;
using static Microsoft.VisualBasic.Collection;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.ErrObject;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Financial;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using System.Collections.Generic;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ColorConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.DrawStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.FillStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.GlobalModule;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.Printer;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterCollection;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterObjectConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ScaleModeConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.SystemColorConstants;
using ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DBNet.Forms;
using static stringops;
using static varspecie;
using static stayontop;
using static localizzazione;
using static SimOptModule;
using static Common;
using static Flex;
using static Robots;
using static Ties;
using static Shots_Module;
using static Globals;
using static Physics;
using static F1Mode;
using static DNAExecution;
using static Vegs;
using static Senses;
using static Multibots;
using static HDRoutines;
using static Scripts;
using static Database;
using static Buckets_Module;
using static NeoMutations;
using static Master;
using static DNAManipulations;
using static DNATokenizing;
using static Bitwise;
using static Obstacles;
using static Teleport;
using static IntOpts;
using static stuffcolors;
using static Evo;
using static DBNet.Forms.MDIForm1;
using static DBNet.Forms.datirob;
using static DBNet.Forms.InfoForm;
using static DBNet.Forms.ColorForm;
using static DBNet.Forms.parentele;
using static DBNet.Forms.Consoleform;
using static DBNet.Forms.frmAbout;
using static DBNet.Forms.optionsform;
using static DBNet.Forms.NetEvent;
using static DBNet.Forms.grafico;
using static DBNet.Forms.ActivForm;
using static DBNet.Forms.Form1;
using static DBNet.Forms.Contest_Form;
using static DBNet.Forms.DNA_Help;
using static DBNet.Forms.MutationsProbability;
using static DBNet.Forms.PhysicsOptions;
using static DBNet.Forms.CostsForm;
using static DBNet.Forms.EnergyForm;
using static DBNet.Forms.ObstacleForm;
using static DBNet.Forms.TeleportForm;
using static DBNet.Forms.frmGset;
using static DBNet.Forms.frmMonitorSet;
using static DBNet.Forms.frmPBMode;
using static DBNet.Forms.frmRestriOps;
using static DBNet.Forms.frmEYE;
using static DBNet.Forms.frmFirstTimeInfo;


static class SimOptModule
{
    public const int NUMCOST = 0;
    public const int DOTNUMCOST = 1;
    public const int BCCMDCOST = 2;
    public const int ADCMDCOST = 3;
    public const int BTCMDCOST = 4;
    public const int CONDCOST = 5;
    public const int LOGICCOST = 6;
    public const int COSTSTORE = 7;
    public const int CHLRCOST = 8; //Botsareus 8/24/2013 The new chloroplast cost
    public const int FLOWCOST = 9;
    public const int MOVECOST = 20;
    public const int TURNCOST = 21;
    public const int TIECOST = 22;
    public const int SHOTCOST = 23;
    public const int DNACYCCOST = 24;
    public const int DNACOPYCOST = 25;
    public const int VENOMCOST = 26;
    public const int POISONCOST = 27;
    public const int SLIMECOST = 28;
    public const int SHELLCOST = 29;
    public const int BODYUPKEEP = 30;
    public const int AGECOST = 31;
    public const int AGECOSTSTART = 32;
    public const int AGECOSTLINEARFRACTION = 33;
    public const int AGECOSTMAKELOG = 51; //Set this high casue it does not use the Cost's control array
    public const int BOTNOCOSTLEVEL = 52;
    public const int DYNAMICCOSTTARGET = 53;
    public const int COSTMULTIPLIER = 54;
    public const int DYNAMICCOSTSENSITIVITY = 55;
    public const int USEDYNAMICCOSTS = 56;
    public const int DYNAMICCOSTTARGETUPPERRANGE = 57;
    public const int DYNAMICCOSTTARGETLOWERRANGE = 58;
    public const int COSTXREINSTATEMENTLEVEL = 59;
    public const int AGECOSTMAKELINEAR = 60;
    public const int DYNAMICCOSTINCLUDEPLANTS = 61;
    public const int ALLOWNEGATIVECOSTX = 62;
    public const int TEMPSUNSUSPEND = 0;
    public const int ADVANCESUN = 2;
    public const int PERMSUNSUSPEND = 1;
    public const int MAXSPECIES = 500; // Used to count species in other sims for IM mode
    public const int MAXNATIVESPECIES = 76; // Max number of species that can be in this sim
    public static List<datispecie> Species = new List<datispecie>(new datispecie[(MAXSPECIES + 1)]);  // TODO: Confirm Array Size By Token
    public const int MAXNUMEYES = 8;
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
        public string SimName = "";
        public int TotRunCycle = 0;
        public decimal CycSec = 0;
        public int TotRunTime = 0;
        public int TotBorn = 0;
        public int SpeciesNum = 0;
        public List<datispecie> Specie = new List<datispecie>();
        public int FieldSize = 0;
        public int FieldWidth = 0;
        public int FieldHeight = 0;
        public int MaxPopulation = 0;
        public int MinVegs = 0;
        public bool KillDistVegs = false;
        public bool BlockedVegs = false;
        public bool DisableTies = false;
        public bool DisableTypArepro = false;
        public bool DisableFixing = false;
        public int PopLimMethod = 0;
        public bool Toroidal = false;
        public bool Updnconnected = false;
        public bool Dxsxconnected = false;
        public double MutCurrMult = 0;
        public bool MutOscill = false;
        public bool MutOscillSine = false;
        public int MutCycMax = 0;
        public int MutCycMin = 0;
        public bool DisableMutations = false;
        public bool F1 = false;
        public bool League = false;
        public bool Restart = false;
        public int UserSeedNumber = 0;
        public int MaxEnergy = 0;
        public bool ZeroMomentum = false;
        public double CostExecCond = 0;
        public double[] Costs = new decimal[70];
        public bool Pondmode = false;
        public int LightIntensity = 0;
        public double Gradient = 0;
        public bool DayNight = false;
        public bool Daytime = false;
        public int CycleLength = 0;
        public bool CorpseEnabled = false;
        public decimal Decay = 0;
        public int Decaydelay = 0;
        public int DecayType = 0;
        public decimal EnergyProp = 0;
        public int EnergyFix = 0;
        public bool EnergyExType = false;
        public decimal CoefficientStatic = 0;
        public decimal CoefficientKinetic = 0;
        public decimal Zgravity = 0;
        public decimal Ygravity = 0;
        public decimal Density = 0;
        public decimal Viscosity = 0;
        public decimal PhysBrown = 0;
        public decimal PhysMoving = 0;
        public decimal PhysSwim = 0;
        public bool PlanetEaters = false;
        public decimal PlanetEatersG = 0;
        public int RepopCooldown = 0;
        public int RepopAmount = 0;
        public decimal Diffuse = 0;
        public decimal VegFeedingToBody = 0;
        public double MaxVelocity = 0;
        public int BadWastelevel = 0;
        public int chartingInterval = 0;
        public decimal CoefficientElasticity = 0;
        public int FluidSolidCustom = 0;
        public int CostRadioSetting = 0;
        public bool NoShotDecay = false;
        public bool NoWShotDecay = false;
        public bool SunUp = false;
        public int SunUpThreshold = 0;
        public bool SunDown = false;
        public int SunDownThreshold = 0;
        public bool DynamicCosts = false;
        public bool FixedBotRadii = false;
        public int DayNightCycleCounter = 0;
        public int SunThresholdMode = 0;
        public bool shapesAreVisable = false;
        public bool allowVerticalShapeDrift = false;
        public bool allowHorizontalShapeDrift = false;
        public bool shapesAreSeeThrough = false;
        public bool shapesAbsorbShots = false;
        public int shapeDriftRate = 0;
        public bool makeAllShapesTransparent = false;
        public bool makeAllShapesBlack = false;
        public bool EGridEnabled = false;
        public int EGridWidth = 0;
        public double oldCostX = 0;
        public int MaxAbsNum = 0;
        public int SimGUID = 0;
        public bool EnableAutoSpeciation = false;
        public int SpeciationGeneticDistance = 0;
        public int SpeciationGenerationalDistance = 0;
        public int SpeciationMinimumPopulation = 0;
        public int SpeciationForkInterval = 0;
        public bool SunOnRnd = false;
        public int Tides = 0;
        public int TidesOf = 0;
        public bool DeadRobotSnp = false;
        public bool SnpExcludeVegs = false;
    }
    public static SimOptions SimOpts = null;
    public static SimOptions TmpOpts = null;
}
