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


static class Multibots {
// Option Explicit
// M U L T I C E L L U L A R   R O U T I N E S
// moves the organism of which robot n is part to the position x,y


public static void ReSpawn(ref int n, ref decimal X, ref decimal Y) {
  List<int> clist = new List<int> (new int[51]);//changed from 20 to 50

  decimal Min = 0;
  int nmin = 0;

  int t = 0;
  decimal dx = 0;
  decimal dy = 0;

  decimal radiidif = 0;

  clist[0] = n;
  ListCells(clist[]);
  Min() = 999999999999;
  t = 0;
  While(clist[t] > 0);
  if (((rob(clist(t)).pos.X - X) ^ 2 + (rob(clist(t)).pos.Y - Y) ^ 2) <= Min()) {
    Min() = (rob(clist(t)).pos.X - X) ^ 2 + (rob(clist(t)).pos.Y - Y) ^ 2;
    nmin = clist[t];
  }
  t = t + 1;
  if (t > 50) {
goto getout;
  }
  Wend();
  dx = X - rob(nmin).pos.X;
  dy = Y - rob(nmin).pos.Y;

//Botsareus 7/15/2016 Bug fix: corrects by radii difference between the two robots
  radiidif = rob[n].radius - rob(nmin).radius;

  dx = dx - 1 * Sgn(dx) + Sgn(dx) * radiidif;
  dy = dy - 1 * Sgn(dy) + Sgn(dy) * radiidif;

  t = 0;
  While(clist[t] > 0);
  rob(clist(t)).pos.X = rob(clist(t)).pos.X + dx;
  rob(clist(t)).pos.Y = rob(clist(t)).pos.Y + dy;
//Botsareus 7/6/2016 Make sure to resolve actvel
  rob(clist(t)).opos.X = rob(clist(t)).pos.X;
  rob(clist(t)).opos.Y = rob(clist(t)).pos.Y;
//Bot is already part of a bucket...
//rob(clist(t)).BucketPos.x = -2
//rob(clist(t)).BucketPos.Y = -2
  UpdateBotBucket(clist[t]);
  t = t + 1;
  Wend();
getout:
}

/*
' kill organism
*/
public static void KillOrganism(ref int n) {
  List<int> clist = new List<int> (new int[51]);//changed from 20 to 50
  List<int> t = new List<int> (new int[51]);

  bool temp = false;

  clist[0] = n;
  ListCells(clist[]);
  temp = MDIForm1.instance.nopoff;
  MDIForm1.instance.nopoff = true;
  While(clist[t] > 0);
  KillRobot(clist[t]);
  t = t + 1;
  Wend();
  MDIForm1.instance.nopoff = temp;
}

/*
' selects the whole organism
*/
public static void FreezeOrganism(ref int n) {
  List<int> clist = new List<int> (new int[51]);//changed from 20 to 50
  List<int> t = new List<int> (new int[51]);

  clist[0] = n;
  ListCells(clist[]);
  While(clist[t] > 0);
  rob(clist(t)).highlight = true;
  t = t + 1;
  Wend();
}

/*
' lists all the cells of an organism, starting from any one
' in position lst(0). Leaves the result in array lst()
*/
public static void ListCells(ref dynamic lst(_UNUSED) {
  int k = 0;

  int j = 0;

  int w = 0;

  bool pres = false;

  int n = 0;

  w = 0;
  n = lst(w);

  While(n > 0);
  dynamic _WithVar_2072;
  _WithVar_2072 = rob[n];
    if (!rob[n].Multibot) {
goto skipties; // If the bot isn't a multibot, then ignore connected cells
    }
    k = 1;
    While(_WithVar_2072.Ties(k).pnt > 0);
    pres = false;
    j = 0;
    While(lst(j) > 0);
    if (lst(j) == _WithVar_2072.Ties(k).pnt) {
      pres = true;
    }
    j = j + 1;
    if (j == 50) {
      lst(j) = 0;
    }
    Wend();
    if (!pres) {
      lst(j) = _WithVar_2072.Ties(k).pnt;
    }
    k = k + 1;
    Wend();
skipties:
  w = w + 1;
  if (w > 50) {
    w = 50; //don't know what effect this will have. Should stop overflows
    lst(w) = 0; //EricL - added June 2006 to prevent overflows
goto ;
  }
  n = lst(w);
  Wend();
getout:
}
}
