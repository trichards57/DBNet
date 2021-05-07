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


static class IntOpts {
// Option Explicit
//Persistant Settings
public static string IName = "";
public static string InboundPath = "";
public static string OutboundPath = "";
public static string ServIP = "";
public static string ServPort = "";
//This is the window handle to DarwinbotsIM
public static int pid = 0;
public static bool Active = false;
public static bool InternetMode = false;
public static bool StartInInternetMode = false;
//This stuff is needed so graphing works
public const dynamic MAXINTERNETSPECIES = 500;
public const dynamic MAXINTERNETSIMS = 100;
public static List<datispecie> InternetSpecies = new List<datispecie> (new datispecie[(MAXINTERNETSPECIES + 1)]);  // TODO: Confirm Array Size By Token// Used for graphing the number of species in the inter connected internet sim
public static int numInternetSpecies = 0;
public static List<string> namesOfInternetBots = new List<string> (new string[(MAXINTERNETSPECIES + 1)]);  // TODO: Confirm Array Size By Token
// gives an internet organism his absurd name


public static string AttribuisciNome(ref int n_UNUSED) {
  string AttribuisciNome = "";
  string p = "";

  p = "dt" + CStr(Format(DateTime.Today;, "yymmdd"));
  p = p + "cn" + "00"; //CStr(n)
  p = p + "mf" + CStr(Int(SimOpts.PhysMoving * 100));
  p = p + "bm" + CStr(Int(SimOpts.PhysBrown * 100));
  p = p + "sf" + CStr(Int(SimOpts.PhysSwim * 100));
  p = p + "ac" + CStr(Int(SimOpts.CostExecCond * 100));
  p = p + "sc" + CStr(Int(SimOpts.Costs(COSTSTORE) * 100));
  p = p + "ce" + CStr(Int(SimOpts.Costs(SHOTCOST) * 100));
  if (SimOpts.EnergyExType) {
    p = p + "et" + CStr(Int(SimOpts.EnergyProp * 100));
    p = p + "tt1";
  } else {
    p = p + "et" + CStr(Int(SimOpts.EnergyFix * 100));
    p = p + "tt2";
  }
  p = p + "rc" + CStr(Random(ref 0, ref 99999));
  p = p + ".dbo";
  AttribuisciNome = p;
  return AttribuisciNome;
}
}
