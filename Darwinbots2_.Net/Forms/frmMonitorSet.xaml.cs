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
public partial class frmMonitorSet : Window {
  private static frmMonitorSet _instance;
  public static frmMonitorSet instance { set { _instance = null; } get { return _instance ?? (_instance = new frmMonitorSet()); }}  public static void Load() { if (_instance == null) { dynamic A = frmMonitorSet.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public frmMonitorSet() { InitializeComponent(); }


// Option Explicit //Windows Default
public int Monitor_mem_r = 0;
public int Monitor_mem_g = 0;
public int Monitor_mem_b = 0;
public int Monitor_floor_r = 0;
public int Monitor_floor_g = 0;
public int Monitor_floor_b = 0;
public int Monitor_ceil_r = 0;
public int Monitor_ceil_g = 0;
public int Monitor_ceil_b = 0;
public bool overwrite = false;
private bool okclick = false;


private void btnCancel_Click(object sender, RoutedEventArgs e) { btnCancel_Click(); }
private void btnCancel_Click() {
  Unload(this);
}

private void btnOK_Click(object sender, RoutedEventArgs e) { btnOK_Click(); }
private void btnOK_Click() {
  Monitor_mem_r = txtMem(0).Text;
  Monitor_mem_g = txtMem(1).Text;
  Monitor_mem_b = txtMem(2).Text;

  Monitor_floor_r = txtFloor(0).Text;
  Monitor_floor_g = txtFloor(1).Text;
  Monitor_floor_b = txtFloor(2).Text;

  Monitor_ceil_r = txtCeil(0).Text;
  Monitor_ceil_g = txtCeil(1).Text;
  Monitor_ceil_b = txtCeil(2).Text;

  okclick = true;
  Unload(this);
}

private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }//load a preset
private void Command1_Click() {
  int holdint = 0;


  CommonDialog1.FileName = "";
  CommonDialog1.InitDir = MDIForm1.instance.MainDir;
  CommonDialog1.Filter = "Monitor preset file(*.mtrp)|*.mtrp";
  CommonDialog1.ShowOpen();
  if (CommonDialog1.FileName != "") {
    VBOpenFile(80, CommonDialog1.FileName);;
    Get(80);
    txtMem(0).text = holdint;
    Get(80);
    txtMem(1).text = holdint;
    Get(80);
    txtMem(2).text = holdint;

    Get(80);
    txtFloor(0).text = holdint;
    Get(80);
    txtFloor(1).text = holdint;
    Get(80);
    txtFloor(2).text = holdint;

    Get(80);
    txtCeil(0).text = holdint;
    Get(80);
    txtCeil(1).text = holdint;
    Get(80);
    txtCeil(2).text = holdint;
    VBCloseFile(80);();
  }
}

private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }//save a preset
private void Command2_Click() {
  CommonDialog1.FileName = "";
  CommonDialog1.InitDir = MDIForm1.instance.MainDir;
  CommonDialog1.Filter = "Monitor preset file(*.mtrp)|*.mtrp";
  CommonDialog1.ShowSave();
  if (CommonDialog1.FileName != "") {
    VBOpenFile(80, CommonDialog1.FileName);;
    Put(80);
    Put(80);
    Put(80);

    Put(80);
    Put(80);
    Put(80);

    Put(80);
    Put(80);
    Put(80);
    VBCloseFile(80);();
  }
}

private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
  if (overwrite) {
    txtMem(0).Text = Monitor_mem_r;
    txtMem(1).Text = Monitor_mem_g;
    txtMem(2).Text = Monitor_mem_b;

    txtFloor(0).Text = Monitor_floor_r;
    txtFloor(1).Text = Monitor_floor_g;
    txtFloor(2).Text = Monitor_floor_b;

    txtCeil(0).Text = Monitor_ceil_r;
    txtCeil(1).Text = Monitor_ceil_g;
    txtCeil(2).Text = Monitor_ceil_b;
  } else {
    btnOK.Left = btnCancel.Left;
    btnCancel.setVisible(false);
  }
  overwrite = true;
}

private void Form_Unload(ref int Cancel_UNUSED) {
  if (!okclick) {
    overwrite = btnCancel.Visibility;
  }
}

private void txtCeil_LostFocus(object sender, RoutedEventArgs e) { txtCeil_LostFocus(); }
private void txtCeil_LostFocus(ref int Index) {
  decimal v = 0;

  v = val(txtCeil(Index));
  if (v < -32000) {
    v = -32000;
  }
  if (v > 32000) {
    v = 32000;
  }
  if (v < val(txtFloor(Index)) + 1) {
    v = val(txtFloor(Index)) + 1;
  }
  v = CInt(v);
  txtCeil(Index) = v;
}

private void txtFloor_LostFocus(object sender, RoutedEventArgs e) { txtFloor_LostFocus(); }
private void txtFloor_LostFocus(ref int Index) {
  decimal v = 0;

  v = val(txtFloor(Index));
  if (v < -32000) {
    v = -32000;
  }
  if (v > 32000) {
    v = 32000;
  }
  if (v > val(txtCeil(Index)) - 1) {
    v = val(txtCeil(Index)) - 1;
  }
  v = CInt(v);
  txtFloor(Index) = v;
}

private void txtMem_LostFocus(object sender, RoutedEventArgs e) { txtMem_LostFocus(); }
private void txtMem_LostFocus(ref int Index) {
  if (!IsNumeric(txtMem(Index))) {
    txtMem(Index) = SysvarTok(ref "." + txtMem(Index));

  }

  int v = 0;

  v = val(txtMem(Index));
  if (v < 1) {
    v = 1;
  }
  if (v > 999) {
    v = 999;
  }
  txtMem(Index) = v;
}


}
}
