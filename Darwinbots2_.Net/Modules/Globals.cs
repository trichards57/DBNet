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


static class Globals {
//A temporary module for everything without a home.
// Option Explicit
//Botsareus 7/2/2014 PlayerBot settings
public class keydata {
 public byte key = 0;
 public int memloc = 0;
 public int value = 0;
 public bool Active = false;
 public bool Invert = false;
}
public static vector Mouse_loc = null;
public static List<keydata> PB_keys = new List<keydata> {}; // TODO - Specified Minimum Array Boundary Not Supported: Public PB_keys() As keydata
//G L O B A L  S E T T I N G S Botsareus 3/15/2013
public static bool screenratiofix = false;
public static int bodyfix = 0;
public static bool reprofix = false;
public static bool chseedstartnew = false;
public static bool chseedloadsim = false;
public static bool UseSafeMode = false;
public static bool UseEpiGene = false;
public static bool UseIntRnd = false;
public static bool GraphUp = false;
public static bool HideDB = false;
public static int intFindBestV2 = 0;
public static bool UseOldColor = false;
public static bool startnovid = false;
//Botsareus 11/29/2013 The mutations tab
public static bool epireset = false;
public static decimal epiresetemp = 0;
public static int epiresetOP = 0;
public static bool sunbelt = false;
//Botsareus 12/16/2013 Delta2 on mutations tab
public static bool Delta2 = false;
public static decimal DeltaMainExp = 0;
public static decimal DeltaMainLn = 0;
public static decimal DeltaDevExp = 0;
public static decimal DeltaDevLn = 0;
public static int DeltaPM = 0;
public static byte DeltaWTC = 0;
public static byte DeltaMainChance = 0;
public static byte DeltaDevChance = 0;
//Botsareus 12/16/2013 Normalize DNA length
public static bool NormMut = false;
public static int valNormMut = 0;
public static int valMaxNormMut = 0;
//some global settings change within simulation
public static bool loadboylabldisp = false;
public static bool loadstartnovid = false;
public static int tmpseed = 0;//used only by "load simulation"
public static bool simalreadyrunning = false;
public static bool autosaved = false;
//Botsareus 1/5/2014 Copy of Obstacle array
public static List<Obstacle> xObstacle = new List<Obstacle> {}; // TODO - Specified Minimum Array Boundary Not Supported: Public xObstacle() As Obstacle
//Variables below prefixed x_ are used for league and evolution, y_ are used only for evolution
//Variables prefixed _res_ are used for restriction overwrites
public static byte x_res_kill_chlr = 0;
public static bool x_res_kill_mb = false;
public static byte x_res_other = 0;
public static byte y_res_kill_chlr = 0;
public static bool y_res_kill_mb = false;
public static bool y_res_kill_dq = false;
public static byte y_res_other = 0;
public static bool x_res_kill_mb_veg = false;
public static byte x_res_other_veg = 0;
public static bool y_res_kill_mb_veg = false;
public static bool y_res_kill_dq_veg = false;
public static byte y_res_other_veg = 0;
//Botsareus 1/31/2014 Restart modes
public static byte x_restartmode = 0;
public static int x_filenumber = 0;
public static string leagueSourceDir = "";
public static bool UseStepladder = false;
public static byte x_fudge = 0;
public static bool FudgeEyes = false;
public static bool FudgeAll = false;
public static byte Disqualify = 0;
public static int StartChlr = 0;//Botsareus 2/12/2014 Start repopulating robots with chloroplasts
public static int ModeChangeCycles = 0;//Botsareus 2/14/2014 Used to calculate time difference and mode change for survival
public static string y_robdir = "";
public static bool y_graphs = false;
public static bool y_normsize = false;
public static int y_hidePredCycl = 0;
public static decimal y_LFOR = 0;
public static int y_Stgwins = 0;
public static int y_zblen = 0;
public static byte y_eco_im = 0;
//actual evolution globals
public static int curr_dna_size = 0;
public static int hidePredCycl = 0;
public static int Init_hidePredCycl = 0;
public static int hidePredOffset = 0;
public static decimal LFOR = 0;
public static bool LFORdir = false;//direction
public static decimal LFORcorr = 0;//correction
public static bool hidepred = false;
public static int target_dna_size = 0;
// var structure, to store the correspondance name<->value
public class var_ {
 public string Name = "";
 public int value = 0;
}
//Constants for the graphs, which are used all over the place unfortunately -Botsareus 8/3/2012 reimplemented
public const int POPULATION_GRAPH = 1;
public const int MUTATIONS_GRAPH = 2;
public const int AVGAGE_GRAPH = 3;
public const int OFFSPRING_GRAPH = 4;
public const int ENERGY_GRAPH = 5;
public const int DNALENGTH_GRAPH = 6;
public const int DNACOND_GRAPH = 7;
public const int MUT_DNALENGTH_GRAPH = 8;
public const int ENERGY_SPECIES_GRAPH = 9;
public const int DYNAMICCOSTS_GRAPH = 10;
public const int SPECIESDIVERSITY_GRAPH = 11;
public const int AVGCHLR_GRAPH = 12; //Botsareus 8/31/2013 Average chloroplasts graph
public const int GENETIC_DIST_GRAPH = 13;
public const int GENERATION_DIST_GRAPH = 14;
public const int GENETIC_SIMPLE_GRAPH = 15;
//Botsareus 5/24/2013 Customizable graphs
public const int CUSTOM_1_GRAPH = 16;
public const int CUSTOM_2_GRAPH = 17;
public const int CUSTOM_3_GRAPH = 18;
public static string strGraphQuery1 = "";
public static string strGraphQuery2 = "";
public static string strGraphQuery3 = "";
//Botsareus 5/31/2013 Special graph info
public static string strSimStart = "";
public const dynamic NUMGRAPHS = 18; //Botsareus 5/25/2013 Two more graphs, moved to globals
public static List<int> graphfilecounter = new List<int> (new int[(NUMGRAPHS + 1)]);  // TODO: Confirm Array Size By Token
public static List<int> graphleft = new List<int> (new int[(NUMGRAPHS + 1)]);  // TODO: Confirm Array Size By Token
public static List<int> graphtop = new List<int> (new int[(NUMGRAPHS + 1)]);  // TODO: Confirm Array Size By Token
public static List<bool> graphvisible = new List<bool> (new bool[(NUMGRAPHS + 1)]);  // TODO: Confirm Array Size By Token
public static List<bool> graphsave = new List<bool> (new bool[(NUMGRAPHS + 1)]);  // TODO: Confirm Array Size By Token
public static int TotalEnergy = 0;// total energy in the sim
public static int totnvegs = 0;// total non vegs in sim
public static int totnvegsDisplayed = 0;// Toggle for display purposes, so the display doesn't catch half calculated value
public static int totwalls = 0;// total walls count
public static int totcorpse = 0;// Total corpses
public static int TotalChlr = 0;//Panda 8/24/2013 total number of chlroroplasts
public static bool NoDeaths = false;//Attempt to stop robots dying during the first cycle of a loaded sim
//later used in conjunction with a routine to give robs a bit of energy back after loading up.
public static int maxfieldsize = 0;
public static bool ismutating = false;//Botsareus 2/2/2013 Tells the parseor to ignore debugint and debugbool while the robot is mutating
//Botsareus 6/11/2013 For music
[DllImport("winmm.dll", EntryPoint = "mciSendStringA")] public static extern dynamic mciSendString(dynamic _);
[DllImport("user32.dll", EntryPoint = "CallWindowProcA")] public static extern dynamic CallWindowProc(ref dynamic _);
[DllImport("user32.dll", EntryPoint = "SetWindowLongA")] public static extern dynamic SetWindowLong(ref dynamic _);
[DllImport("user32.dll")] private static extern dynamic RegisterWindowMessage(ref dynamic _);
//Windows API calls for GetWinHandle
//Stolen from MSDN somewhere
private const dynamic GW_HWNDNEXT = 2;
[DllImport("user32.dll")] private static extern int GetParent(int hwnd);
[DllImport("user32.dll")] private static extern dynamic GetWindow(int hwnd, ref dynamic _);
[DllImport("user32.dll", EntryPoint = "FindWindowA")] private static extern dynamic FindWindow(ref dynamic _);
public const dynamic GWL_WNDPROC = -4;
[DllImport("user32.dll")] public static extern dynamic GetWindowThreadProcessId(ref dynamic _);
//Stuff for close window
[DllImport("kernel32.dll")] private static extern dynamic WaitForSingleObject(ref dynamic _);
[DllImport("user32.dll")] private static extern dynamic PostMessage(ref dynamic _);
[DllImport("user32.dll")] private static extern dynamic IsWindow(ref dynamic _);
[DllImport("kernel32.dll")] private static extern dynamic OpenProcess(ref dynamic _);
//For args to the IM client
//Botsareus 10/21/2015 No longer a required feature
//Public Declare Function GetCurrentProcessId Lib "kernel32" () As Long
public const dynamic WM_CLOSE = 0x10;
public const dynamic INFINITE = 0xFFFFFFFF;
public const dynamic SYNCHRONIZE = 0x100000;
private static int MSWHEEL_ROLLMSG = 0;


public static void Hook() {
  MSWHEEL_ROLLMSG = RegisterWindowMessage("MSWHEEL_ROLLMSG");
  lpPrevWndProc = SetWindowLong(gHW, GWL_WNDPROC, AddressOf(WindowProc()));
}

public static void UnHook() {
  int lngReturnValue = 0;


  lngReturnValue = SetWindowLong(gHW, GWL_WNDPROC, lpPrevWndProc);
}

static int WindowProc(int hw, int uMsg, int wParam, int lParam) {
  int WindowProc = 0;

  switch(uMsg) {
    case MSWHEEL_ROLLMSG:
//Form1.MouseWheelZoom 'Botsareus 4/1/2014 Never worked
      break;
    default:
      WindowProc = CallWindowProc(lpPrevWndProc, hw, uMsg, wParam, lParam);
break;
}
  return WindowProc;
}
private static int ProcIDFromWnd(int hwnd) {
  int ProcIDFromWnd = 0;
  int idProc = 0;


// Get PID for this HWnd
  GetWindowThreadProcessId(hwnd, idProc);

// Return PID
  ProcIDFromWnd = idProc;
  return ProcIDFromWnd;
}
public static int GetWinHandle(ref int pid) {
  int GetWinHandle = 0;
  int tempHwnd = 0;


// Grab the first window handle that Windows finds:
  tempHwnd = FindWindow(vbNullString, vbNullString);

// Loop until you find a match or there are no more window handles:
  while(!(tempHwnd == 0)) {
// Check if no parent for this window
    if (GetParent(tempHwnd) == 0) {
// Check for PID match
      if (pid == ProcIDFromWnd[tempHwnd]) {
// Return found handle
        GetWinHandle = tempHwnd;
// Exit search loop
        break;
      }
    }

// Get the next window handle
    tempHwnd = GetWindow(tempHwnd, GW_HWNDNEXT);
  }
  return GetWinHandle;
}
public static dynamic CloseWindow(ref int pid) {
  dynamic CloseWindow = null;
  int lngReturnValue = 0;

  int lngResult = 0;

  int hThread = 0;

  int hProcess = 0;

  int hWindow = 0;


  hWindow = GetWinHandle[pid];
  hThread = GetWindowThreadProcessId(hWindow, pid);
  hProcess = OpenProcess(SYNCHRONIZE, 0&, pid);
  lngReturnValue = PostMessage(hWindow, WM_CLOSE, 0&, 0&);
  lngResult = WaitForSingleObject(hProcess, INFINITE);

  return CloseWindow;
}
// Not sure where to put this function, so it's going here
// makes poff. that is, creates that explosion effect with
// some fake shots...
public static void makepoff(ref int n) {
  int an = 0;

  int vs = 0;

  int vx = 0;

  int vy = 0;

  int x = 0;

  int y = 0;

  byte t = 0;

  for(t=1; t<20; t++) {
    an = (640 / 20) * t;
    vs = Random(ref RobSize / 40, ref RobSize / 30);
    vx = rob[n].vel.x + absx(ref an / 100, vs, 0, 0, 0);
    vy = rob[n].vel.y + absy(ref an / 100, vs, 0, 0, 0);
    dynamic _WithVar_7828;
    _WithVar_7828 = rob[n];
      x = Random(ref _WithVar_7828.pos.x - _WithVar_7828.radius, ref _WithVar_7828.pos.x + _WithVar_7828.radius);
      y = Random(ref _WithVar_7828.pos.y - _WithVar_7828.radius, ref _WithVar_7828.pos.y + _WithVar_7828.radius);
    if (Random(ref 1, ref 2) == 1) {
      createshot(x, y, vx, vy, -100, 0, 0, RobSize * 2, rob[n].color);
    } else {
      createshot(x, y, vx, vy, -100, 0, 0, RobSize * 2, DBrite(rob[n].color));
    }
    Next(t);
  }

private static bool checkvegstatus(int r) {
  bool checkvegstatus = false;
  int t = 0;


  string FName = "";

  List<string> splitname = new List<string> {}; // TODO - Specified Minimum Array Boundary Not Supported: Dim splitname() As String//just incase original species is dead

  string robname = "";


  FName = extractname(ref SimOpts.Specie(r).Name);

  checkvegstatus = false;

  if (SimOpts.Specie(r).Veg == true && SimOpts.Specie(r).Native) {
//see if any active robots have chloroplasts
    for(t=1; t<MaxRobs; t++) {
      dynamic _WithVar_5538;
      _WithVar_5538 = rob(t);
        if (_WithVar_5538.exist && _WithVar_5538.chloroplasts > 0) {
//remove old nick name
          splitname = Split(_WithVar_5538.FName, ")");
//if it is a nick name only
          if (Left(splitname[0], 1) == "(" && IsNumeric(Right(splitname[0], Len(splitname[0]) - 1))) {
            robname = splitname[1];
          } else {
            robname = _WithVar_5538.FName;
          }

          if (SimOpts.Specie(r).Name == robname) {
            checkvegstatus = true;
            return checkvegstatus;


          }

        }
    }

//If there is no robots at all with chlr then repop everything

    checkvegstatus = true;

    for(t=1; t<MaxRobs; t++) {
      dynamic _WithVar_7204;
      _WithVar_7204 = rob(t);
        if (_WithVar_7204.exist && _WithVar_7204.Veg && _WithVar_7204.age > 0) { //Botsareus 11/4/2015 age test makes sure all robots spawn
          checkvegstatus = false;
          return checkvegstatus;

        }
    }

  }
  return checkvegstatus;
}

/*
' not sure where to put this function, so it's going here
' adds robots on the fly loading the script of specie(r)
' if r=-1 loads a vegetable (used for repopulation)
*/
public static void aggiungirob(int r, decimal x, decimal y) { //Botsareus 5/22/2014 Bugfix by adding byval
  int a = 0;

  int i = 0;

  bool anyvegy = false;


  if (r == -1) {
//run one loop to check vegy status
    for(i=0; i<SimOpts.SpeciesNum - 1; i++) {
      if (checkvegstatus(i)) {
        anyvegy = true;
        break;
      }
    }
    if (!anyvegy) {
return;

    }

    do {
      r = Random(ref 0, ref SimOpts.SpeciesNum - 1); // start randomly in the list of species
    } while(!(checkvegstatus(r));

    x = fRnd(SimOpts.Specie(r).Poslf * (SimOpts.FieldWidth - 60), SimOpts.Specie(r).Posrg * (SimOpts.FieldWidth - 60));
    y = fRnd(SimOpts.Specie(r).Postp * (SimOpts.FieldHeight - 60), SimOpts.Specie(r).Posdn * (SimOpts.FieldHeight - 60));
  }

  if (SimOpts.Specie(r).Name != "" && SimOpts.Specie(r).path != "Invalid Path") {
    a = RobScriptLoad(ref respath(ref SimOpts.Specie(r).path) + "\\" + SimOpts.Specie(r).Name);
    if (a < 0) {
      SimOpts.Specie(r).Native = false;
goto ;
    }

//Check to see if we were able to load the bot.  If we can't, the path may be wrong, the sim may have
//come from another machine with a different install path.  Set the species path to an empty string to
//prevent endless looping of error dialogs.
    if (!rob(a).exist) {
      SimOpts.Specie(r).path = "Invalid Path";
goto ;
    }

    rob(a).Veg = SimOpts.Specie(r).Veg;
    if (rob(a).Veg) {
      rob(a).chloroplasts = StartChlr; //Botsareus 2/12/2014 Start a robot with chloroplasts
    }
//NewMove loaded via robscriptload
    rob(a).Fixed = SimOpts.Specie(r).Fixed;
    rob(a).CantSee = SimOpts.Specie(r).CantSee;
    rob(a).DisableDNA = SimOpts.Specie(r).DisableDNA;
    rob(a).DisableMovementSysvars = SimOpts.Specie(r).DisableMovementSysvars;
    rob(a).CantReproduce = SimOpts.Specie(r).CantReproduce;
    rob(a).VirusImmune = SimOpts.Specie(r).VirusImmune;
    rob(a).Corpse = false;
    rob(a).Dead = false;
    rob(a).body = 1000;
//  EnergyAddedPerCycle = EnergyAddedPerCycle + 10000
    rob(a).radius = FindRadius(a);
    rob(a).Mutations = 0;
    rob(a).OldMutations = 0; //Botsareus 10/8/2015
    rob(a).LastMut = 0;
    rob(a).generation = 0;
    rob(a).SonNumber = 0;
    rob(a).parent = 0;
//    rob(a).mem(468) = 32000 Botsareus 10/5/2015 why set memory right before an erase call?
//    rob(a).mem(AimSys) = Random(1, 1256) / 200
//    rob(a).mem(SetAim) = rob(a).aim * 200
//    rob(a).mem(480) = 32000
//    rob(a).mem(481) = 32000
//    rob(a).mem(482) = 32000
//    rob(a).mem(483) = 32000
//    rob(a).aim = Rnd(PI)
    Erase(rob(a).mem);
//If rob(a).Veg Then rob(a).Feed = 8
    if (rob(a).Fixed) {
      rob(a).mem(216) = 1;
    }
    rob(a).pos.x = x;
    rob(a).pos.y = y;


    rob(a).aim = rndy() * PI * 2; //Botsareus 5/30/2012 Added code to rotate the robot on placment
    rob(a).mem(SetAim) = rob(a).aim * 200;

//Bot is already in a bucket due to the prepare routine
// rob(a).BucketPos.x = -2
// rob(a).BucketPos.Y = -2
    UpdateBotBucket(a);
    rob(a).nrg = SimOpts.Specie(r).Stnrg;
// EnergyAddedPerCycle = EnergyAddedPerCycle + rob(a).nrg
    rob(a).Mutables = SimOpts.Specie(r).Mutables;

    rob(a).Vtimer = 0;
    rob(a).virusshot = 0;
    rob(a).genenum = CountGenes(ref rob(a).dna);


    rob(a).DnaLen = DnaLen(ref rob(a).dna());
    rob(a).GenMut = rob(a).DnaLen / GeneticSensitivity; //Botsareus 4/9/2013 automatically apply genetic to inserted robots


    rob(a).mem(DnaLenSys) = rob(a).DnaLen;
    rob(a).mem(GenesSys) = rob(a).genenum;

//Botsareus 10/8/2015 New kill restrictions
    rob(a).multibot_time = IIf(SimOpts.Specie(r).kill_mb, 210, 0);
    rob(a).dq = IIf(SimOpts.Specie(r).dq_kill, 1, 0);
    rob(a).NoChlr = SimOpts.Specie(r).NoChlr; //Botsareus 11/1/2015 Bug fix


    for(i=0; i<7; i++) { //Botsareus 5/20/2012 fix for skin engine
      rob(a).Skin(i) = SimOpts.Specie(r).Skin(i);
      Next(i);

      rob(a).color = SimOpts.Specie(r).color;
      makeoccurrlist(a);
    }
getout:
  }
}
