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


static class localizzazione {
public static string SBcycsec = "";
public static string SBtotobj = "";
public static string SBtotrob = "";
public static string SBtotveg = "";
public static string SBborn = "";
public static string SBtotcyc = "";
public static string SBtottim = "";
public static string MBexit = "";
public static string MBsure = "";
public static string MBnotloaded = "";
public static string MBnotsaved = "";
public static string MBwarning = "";
public static string MBcannotfindV = "";
public static string MBcannotfindI = "";
public static string MBrobotsdead = "";
public static string MBnovalidrob = "";
public static string MBSaveDNA = "";
public static string MBDNANotSaved = "";
public static string MBSaveSim = "";
public static string MBLoadSim = "";
public static string WSmutratesfor = "";
public static string WSmutrates = "";
public static string WSchoosedna = "";
public static string WSproperties = "";
public static string WSnone = "";
public static string WScannotfind = "";
public static string OTtoggledisplay = "";


static void globstrings() {
/*
On Error Resume Next
  SBcycsec = LoadResString(14001)
  SBtotobj = LoadResString(14002)
  SBtotrob = LoadResString(14003)
  SBtotveg = LoadResString(14004)
  SBborn = LoadResString(14005)
  SBtotcyc = LoadResString(14006)
  SBtottim = LoadResString(14007)
  MBsure = LoadResString(20001)
  MBnotloaded = LoadResString(20002)
  MBnotsaved = LoadResString(20003)
  MBwarning = LoadResString(20004)
  MBcannotfindV = LoadResString(20005)
  MBcannotfindI = LoadResString(20006)
  MBrobotsdead = LoadResString(20007)
  MBnovalidrob = LoadResString(20014)
  WSmutratesfor = LoadResString(20008)
  WSmutrates = LoadResString(20009)
  WSchoosedna = LoadResString(20010)
  WSproperties = LoadResString(20011)
  WSnone = LoadResString(20012)
  WScannotfind = LoadResString(20013)
  MBSaveDNA = LoadResString(20016)
  MBDNANotSaved = LoadResString(20015)
  MBSaveSim = LoadResString(20017)
  MBLoadSim = LoadResString(20018)
  OTtoggledisplay = LoadResString(30000)
End Sub
*/
static void strings(ref Window frm_UNUSED) {
}
