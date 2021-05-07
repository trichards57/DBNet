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


static class stuffcolors {
public class R_G_B {
 public int r = 0;
 public int g = 0;
 public int b = 0;
}
public class H_S_L {
 public float h = 0;
 public float s = 0;
 public float l = 0;
}
public static int chartcolor = 0;
public static int backgcolor = 0;


public static R_G_B hsltorgb(ref H_S_L hslin) {
  R_G_B hsltorgb = null;
  hsltorgb = huetorgb(ref hslin.h);
  R_G_B c = null;

  c.r = 127.5m;
  c.g = 127.5m;
  c.b = 127.5m;
  hsltorgb = mixrgb(ref hsltorgb, ref c, ref 1 - (hslin.s / 240));
  if (hslin.l < 120) {
    c.r = 0;
    c.g = 0;
    c.b = 0;
    hsltorgb = mixrgb(ref c, ref hsltorgb, ref hslin.l / 120);
  } else {
    c.r = 255;
    c.g = 255;
    c.b = 255;
    hsltorgb = mixrgb(ref hsltorgb, ref c, ref (hslin.l - 120) / 120);
  }
  return hsltorgb;
}

public static R_G_B huetorgb(ref dynamic h) {
  R_G_B huetorgb = null;
  decimal Delta = 0;

  Delta = Int(h) % 40;
  h = h - Delta;
  Delta = 255 / 40 * Delta;
  if (h < 240) {
    huetorgb.r = 255;
    huetorgb.g = 0;
    huetorgb.b = 255 - Delta;
  }
  if (h < 200) {
    huetorgb.r = Delta;
    huetorgb.g = 0;
    huetorgb.b = 255;
  }
  if (h < 160) {
    huetorgb.r = 0;
    huetorgb.g = 255 - Delta;
    huetorgb.b = 255;
  }
  if (h < 120) {
    huetorgb.r = 0;
    huetorgb.g = 255;
    huetorgb.b = Delta;
  }
  if (h < 80) {
    huetorgb.r = 255 - Delta;
    huetorgb.g = 255;
    huetorgb.b = 0;
  }
  if (h < 40) {
    huetorgb.r = 255;
    huetorgb.g = Delta;
    huetorgb.b = 0;
  }
  return huetorgb;
}

public static R_G_B mixrgb(ref R_G_B c1, ref R_G_B c2, ref dynamic factor) {
  R_G_B mixrgb = null;
  mixrgb.r = c1.r * (1 - factor) + c2.r * factor;
  mixrgb.g = c1.g * (1 - factor) + c2.g * factor;
  mixrgb.b = c1.b * (1 - factor) + c2.b * factor;
  return mixrgb;
}
}
