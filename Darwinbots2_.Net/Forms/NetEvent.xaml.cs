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


namespace DBNet.Forms
{
public partial class NetEvent : Window {
  private static NetEvent _instance;
  public static NetEvent instance { set { _instance = null; } get { return _instance ?? (_instance = new NetEvent()); }}  public static void Load() { if (_instance == null) { dynamic A = NetEvent.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public NetEvent() { InitializeComponent(); }


private decimal Mx = 0;//False
private decimal My = 0;


public void stayontop() {
  SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE + SWP_NOSIZE);
}

private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
  SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE + SWP_NOSIZE);
}

void Appear(ref string txt) {
  this
  this
  NetLab.Content = txt;
  Netlab2.Content = txt;
  if (OptionsForm.instance.Visible == false) {
    Timer1.Interval = 5000;
    Timer1.IsEnabled = true;
    this
  }
}

private void Form_MouseDown(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x, ref decimal y) {
  Mx = x;
  My = y;
}

private void Form_MouseMove(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x, ref decimal y) {
  if (Mx > 0 || My > 0) {
    dx = x - Mx;
    dy = y - My;
    this
  }
}

private void Form_MouseUp(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x_UNUSED, ref decimal y_UNUSED) {
  Mx = 0;
  My = 0;
}

private void Label1_Click(object sender, RoutedEventArgs e) { Label1_Click(); }
private void Label1_Click() {
  Timer1.IsEnabled = false;
  this
}

private void NetLab_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { NetLab_Change(); }
private void NetLab_Change() {
  this
}

private void NetLab_MouseDown(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x, ref decimal y) {
  Mx = x;
  My = y;
}

private void NetLab_MouseMove(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x, ref decimal y) {
  if (Mx > 0 || My > 0) {
    dx = x - Mx;
    dy = y - My;
    this
    if (this < 0) {
      this
    }
  }
}

private void NetLab_MouseUp(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x_UNUSED, ref decimal y_UNUSED) {
  Mx = 0;
  My = 0;
}

private void NetLab2_MouseDown(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x, ref decimal y) {
  Mx = x;
  My = y;
}

private void NetLab2_MouseMove(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x, ref decimal y) {
  if (Mx > 0 || My > 0) {
    dx = x - Mx;
    dy = y - My;
    this
    if (this < 0) {
      this
    }
  }
}

private void NetLab2_MouseUp(ref int button_UNUSED, ref int Shift_UNUSED, ref decimal x_UNUSED, ref decimal y_UNUSED) {
  Mx = 0;
  My = 0;
}

private void Timer1_Timer() {
  Timer1.IsEnabled = false;
  this
}


}
}
