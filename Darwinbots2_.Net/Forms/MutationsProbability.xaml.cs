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
public partial class MutationsProbability : Window {
  private static MutationsProbability _instance;
  public static MutationsProbability instance { set { _instance = null; } get { return _instance ?? (_instance = new MutationsProbability()); }}  public static void Load() { if (_instance == null) { dynamic A = MutationsProbability.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public MutationsProbability() { InitializeComponent(); }


// Option Explicit //False
//Botsareus 6/12/2012 form's icon change
//Botsareus 12/10/2013 moved the order mean and stddev is stored to prevent a bug, implemented new mutation algos.
private byte Mode = 0;


private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { int c = 0, u = 0 ;  Form_QueryUnload(out c, ref u); e.Cancel = c != 0;  }//Botsareus 12/11/2013
private void Form_QueryUnload(ref int Cancel_UNUSED, ref int UnloadMode_UNUSED) {
  if (!optionsform.CurrSpec == 50) {
//generate mrates file for robot
    string outpath = "";

    string path = "";

    path = TmpOpts.Specie(optionsform.CurrSpec).path + "\\" + TmpOpts.Specie(optionsform.CurrSpec).Name;
    outpath = TmpOpts.Specie(optionsform.CurrSpec).path + "\\" + extractexactname(ref TmpOpts.Specie(optionsform.CurrSpec).Name) + ".mrate";
    outpath = Replace(outpath, "&#", MDIForm1.instance.MainDir);
    path = Replace(path, "&#", MDIForm1.instance.MainDir); //Botsareus 3/16/2014 Bug fix
//Botsareus 12/28/2013 Search robots folder only if path not found
    if (dir(path) == "") {
      outpath = MDIForm1.instance.MainDir + "\\Robots\\" + extractexactname(ref TmpOpts.Specie(optionsform.CurrSpec).Name) + ".mrate";
    }
    Save_mrates(TmpOpts.Specie(optionsform.CurrSpec).Mutables, outpath);
  }
}

private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
  TypeOption(1).value = true * true;
  TypeOption(2).value = true * true;
  TypeOption(3).value = true * true;
  TypeOption(4).value = true * true;
  TypeOption(5).value = true * true;
  TypeOption(6).value = true * true;
  TypeOption(7).value = true * true;
  TypeOption(8).value = true * true;
  TypeOption(9).value = true * true;
  TypeOption(10).value = true * true;
  TypeOption(0).value = true * true;
//Botsareus 12/11/2013 Make new mutations optional
  TypeOption(4).Visibility = sunbelt;
  TypeOption(8).Visibility = sunbelt;
  TypeOption(9).Visibility = sunbelt;
  TypeOption(10).Visibility = sunbelt;

//no real distinction between minor deletion and major deletion
  if (Delta2) {
    TypeOption(1).Caption = "\"Minor\" Deletion";
    TypeOption(5).Caption = "\"Major\" Deletion";
    TypeOption(7).setVisible(false);
  }


//  If (TmpOpts.Specie(optionsform.CurrSpec).Mutables.Mutations = True) Then
//    EnableAllCheck.value = 1
//  End If
}

private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
private void Command1_Click(ref int Index) {
  if (Index == 1) {
    Unload(this);
  } else {
    SetDefaultMutationRates(TmpOpts.Specie(optionsform.CurrSpec).Mutables);
    dynamic _WithVar_1273;
    _WithVar_1273 = TmpOpts.Specie(optionsform.CurrSpec).Mutables;

      decimal Pnone = 0;
      decimal Psome = 0;


      Pnone = Anti_Prob(_WithVar_1273.mutarray(1)) * Anti_Prob(_WithVar_1273.mutarray(2)) * Anti_Prob(_WithVar_1273.mutarray(3)) * Anti_Prob(_WithVar_1273.mutarray(5)) * Anti_Prob(_WithVar_1273.mutarray(6)) * Anti_Prob(_WithVar_1273.mutarray(7)) * Anti_Prob(_WithVar_1273.mutarray(8)) * Anti_Prob(_WithVar_1273.mutarray(9)) * Anti_Prob(_WithVar_1273.mutarray(10));
      Psome = 1 - Pnone;


      Probs(1).text = CStr(CLng(1 / Psome));
  }
}

/*
'Private Sub EnableAllCheck_Click()
'  If (EnableAllCheck.value = 0) Then
'    EnableAllCheck.Caption = "All Disabled"
'  Else
'    EnableAllCheck.Caption = "All Enabled"
'  End If

'  TmpOpts.Specie(optionsform.CurrSpec).Mutables.Mutations = (EnableAllCheck.value * True)
'End Sub
*/
private void Lower_LostFocus(object sender, RoutedEventArgs e) { Lower_LostFocus(); }
private void Lower_LostFocus(ref int Index) {
  Mean(Index).text = (val(Lower(Index).text) + val(Upper(Index).text)) / 2;
  RevalueStdDev(Index);
}

private void Upper_LostFocus(object sender, RoutedEventArgs e) { Upper_LostFocus(); }
private void Upper_LostFocus(ref int Index) {
  Mean(Index).text = (val(Lower(Index).text) + val(Upper(Index).text)) / 2;
  RevalueStdDev(Index);
}

private void Mean_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { Mean_Change(); }
private void Mean_Change(ref int Index) {
  TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode) = val(Mean(Index).text);
  if (val(Mean(Index).text) != val(Lower(Index).text) / 2 + val(Upper(Index).text) / 2) {
    decimal temp = 0;

    temp = (-val(Lower(Index).text) + val(Upper(Index).text)) / 2;
    Lower(Index).text = val(Mean(Index).text) - temp;
    Upper(Index).text = val(Mean(Index).text) + temp;

    Upper_LostFocus(Index);
    Lower_LostFocus(Index);
  }
}

private void StdDev_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { StdDev_Change(); }
private void StdDev_Change(ref int Index) {
//new upper and lower = mean +- 2 * stddev.text
  Lower(Index).text = val(Mean(Index).text) - val(StdDev(Index).text) * 2;
  Upper(Index).text = val(Mean(Index).text) + val(StdDev(Index).text) * 2;
  TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode) = val(StdDev(Index).text);
}

private void RevalueStdDev(ref int Index) {
  StdDev(Index).text = (val(Upper(Index).text) - val(Lower(Index).text)) / 4;
}

private void Slider1_LostFocus(object sender, RoutedEventArgs e) { Slider1_LostFocus(); }
private void Slider1_LostFocus() {
  switch(Mode) {
    case 0:  //point
      TmpOpts.Specie(optionsform.CurrSpec).Mutables.PointWhatToChange = Slider1.value;
      break;
    case CopyErrorUP:
      TmpOpts.Specie(optionsform.CurrSpec).Mutables.CopyErrorWhatToChange = Slider1.value;
break;
}
}

private void MutTypeEnabled_Click(object sender, RoutedEventArgs e) { MutTypeEnabled_Click(); }
private void MutTypeEnabled_Click() {
  int sign = 0;


  sign = Sgn(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));

  if (MutTypeEnabled.value != (sign + 1) / 2) {
    sign = -sign;
    TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode) = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode)) * sign;
  }

  MutTypeEnabled.Caption = IIf(sign == 1, "Enabled", "Disabled");

}

private void Probs_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { Probs_Change(); }
private void Probs_Change(ref int Index) {
  if (Index == 0) {
    if (TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode) == 0) {
      TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode) = 1;
    }

    TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode) = val(Probs(Index)) * Sgn(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));

//update summations...

    dynamic _WithVar_5778;
    _WithVar_5778 = TmpOpts.Specie(optionsform.CurrSpec).Mutables;

      decimal Pnone = 0;
      decimal Psome = 0;


      Pnone = Anti_Prob(_WithVar_5778.mutarray(1)) * Anti_Prob(_WithVar_5778.mutarray(2)) * Anti_Prob(_WithVar_5778.mutarray(3)) * Anti_Prob(_WithVar_5778.mutarray(5)) * Anti_Prob(_WithVar_5778.mutarray(6)) * Anti_Prob(_WithVar_5778.mutarray(7)) * Anti_Prob(_WithVar_5778.mutarray(8)) * Anti_Prob(_WithVar_5778.mutarray(9)) * Anti_Prob(_WithVar_5778.mutarray(10));
      Psome = 1 - Pnone;

      if (Psome == 0) {
        Probs(1).text = "Inf";
      } else {
        Probs(1).text = CStr(CLng(1 / Psome));
      }
  }
}

private decimal Anti_Prob(decimal a) {
  decimal Anti_Prob = 0;
  if (a <= 0) {
    Anti_Prob = 1;
  } else {
    Anti_Prob = 1 - 1 / a;
  }
  return Anti_Prob;
}

private void TypeOption_Click(object sender, RoutedEventArgs e) { TypeOption_Click(); }
private void TypeOption_Click(ref int Index) {
  int value = 0;

  int sign = 0;


  Mode = Index;
  value = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));
  sign = Sgn(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));
  Probs(0).text = value;
  MutTypeEnabled.value = (sign + 1) / 2;
  MutTypeEnabled.Caption = IIf(sign == 1, "Enabled", "Disabled");

  switch(Index) {
    case 0:  //point mutation
      SetupPoint();
      break;//minor deletion
    case 1:
      SetupMinorDeletion();
      break;//reversal
    case 2:
      SetupReversal();
      break;//insertion
    case 3:
      SetupInsertion();
      break;//amplification
    case 4:
      SetupAmplify();
      break;//major deletion
    case 5:
      SetupMajorDeletion();
      break;//copy error
    case 6:
      SetupCopyError();
      break;//change in mutation rates
    case 7:
      SetupDelta();
      break;//movement of a segment
    case 8:
      SetupIntraCT();
      break;
    case 9:
      SetupP2();
      break;
    case 10:
      SetupCE2();
break;
}
}

private void SetupPoint() {
  CustGauss(0).setVisible(true);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(true);
  Slider1Text(0).setVisible(true);
  Slider1Text(1).setVisible(true);
  Sliders6.setVisible(false);

  Text2.text = "A small scale mutation that causes a small series of commands " + "to change.  It may occur at any time in a " + "bots life.  Represents environmental mutations such as UV light or " + "an error in DNA maintenance.  Length should be kept relatively small " + "to mirror real life (~1 bp).  Unlike other mutations, point mutation " + "chances are given as 1 in X per bp per kilocycles, so they occur quite " + "independantly of reproduction rate.  To find the liklihood " + "of at least one mutation over any length of time: 1/(1 - (1-1/X)^(how many cycles)) " + "= Y, as in 1 chance in Y per that many cycles.  Finding the probable number " + "of mutations in that range is more difficult.  (Lookup Negative Binomial Distribution).";

//now set appropriate values for the distributions
  Slider1Text(0).Content = "Change Type";
  Slider1Text(1).Content = "Change Value";
  Slider1.value = TmpOpts.Specie(optionsform.CurrSpec).Mutables.PointWhatToChange;

  CustGauss(0).Caption = "Length";

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(PointUP);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(PointUP);

  Label3(0).Content = "1 chance in  XXXXXXX 000 per bp per cycle";
  Probs(0).text = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));
}

private void SetupP2() {
  CustGauss(0).setVisible(false);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Sliders6.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);

  Text2.text = "Note: The length of this mutation is always 1, but the rate is multiplied by the Gaussen Length of Point Mutation." + vbCrLf + "Similar to point mutations, but always changes to an existing sysvar, *sysvar, or special values if followed by .shoot store or .focuseye store." + "The algorithm is also designed to introduce more stores." + " Should allow for evolving a zero-bot the same as a random-bot.";

  Label3(0).Content = "1 chance in  XXXXXXX 000 per bp per cycle";
  Probs(0).text = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));

}

private void SetupCE2() {
  CustGauss(0).setVisible(false);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Sliders6.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);

  Text2.text = "Similar to copy error, but always changes to an existing sysvar, *sysvar, or special values if followed by .shoot store or .focuseye store.";
  Label3(0).Content = "1 chance in  XXXXXXX per bp per copy";
  Probs(0).text = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));

}

private void SetupCopyError() {
  CustGauss(0).setVisible(true);
  CustGauss(0).Caption = "Length";
  CustGauss(1).setVisible(false);
  Slider1.setVisible(true);
  Slider1Text(0).setVisible(true);
  Slider1Text(1).setVisible(true);
  Sliders6.setVisible(false);

  Slider1Text(0).Content = "Change Type";
  Slider1Text(1).Content = "Change Value";
  Slider1.value = TmpOpts.Specie(optionsform.CurrSpec).Mutables.CopyErrorWhatToChange;

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode);

  Text2.text = "Similar to point mutations, but these occur during DNA replication " + "for reproduction or viruses.  A small series (usually 1 bp) is changed in either parent " + "or child.";

  Label3(0).Content = "1 chance in  XXXXXXX per bp per copy";
  Probs(0).text = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));

  CustGauss(0).Caption = "Length";
}

private void SetupAmplify() {
  CustGauss(0).setVisible(true);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);
  Sliders6.setVisible(false);

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode);

  Text2.text = "A series of bp are replicated and inserted in another place in the " + "genome.";

  Label3(0).Content = "1 chance in  XXXXXXX per bp per copy";
  Probs(0).text = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));

  CustGauss(0).Caption = "Length";
}

private void SetupMajorDeletion() {
  CustGauss(0).setVisible(true);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);
  Sliders6.setVisible(false);

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode);

  Label3(0).Content = "1 chance in  XXXXXXX per bp per copy";
  Probs(0).text = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));

  Text2.text = "A relatively long series of bp are deleted from the genome.  " + "This can be quite disasterous, so set probabilities wisely.";

  CustGauss(0).Caption = "Length";
}

private void SetupMinorDeletion() {
  CustGauss(0).setVisible(true);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);
  Sliders6.setVisible(false);

  Text2.text = "A small series of bp are deleted from the genome.";

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode);

  Label3(0).Content = "1 chance in  XXXXXXX per bp per copy";
  Probs(0).text = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));

  CustGauss(0).Caption = "Length";
}

private void SetupInsertion() {
  CustGauss(0).setVisible(true);
  CustGauss(0).Caption = "Length";
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);
//Sliders6.Visible = True
  Sliders6.setVisible(false);

  Text2.text = "A run of random bp are inserted into the genome.  " + "The size of this run should be fairly small.";

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode);

  CustGauss(0).Caption = "Length";

  Label3(0).Content = "1 chance in  XXXXXXX per bp per copy";
  Probs(0).text = Abs(TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.mutarray(Mode));
}

private void SetupReversal() {
  CustGauss(0).setVisible(true);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);
  Sliders6.setVisible(false);

  CustGauss(0).Caption = "Length of Reversal";
  Label3(0).Content = "1 chance in  XXXXXXX per bp per copy";

  Text2.text = "A series of bp are reversed in the genome.  " + "For example, '2 3 > or' becomes 'or > 3 2'.  Length of " + "reversal should be >= 2.";

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode);
}

private void SetupDelta() {
  CustGauss(0).setVisible(true);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);
  Sliders6.setVisible(false);

  CustGauss(0).Caption = "Standard Deviation";

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode);

  Text2.text = "The mutation rates of a bot are allowed to change " + "slowly over time.  This change in mutation rates can include " + "Delta Mutations as well.  Theoretically, it may be possible for " + "a bot to figure out its own optimal mutation rate.";

  Label3(0).Content = "1 chance in  XXXXXXX 00 per cycle";
}

private void SetupIntraCT() {
  CustGauss(0).setVisible(true);
  CustGauss(1).setVisible(false);
  Slider1.setVisible(false);
  Slider1Text(0).setVisible(false);
  Slider1Text(1).setVisible(false);
  Sliders6.setVisible(false);

  Text2.text = "Tranlocation moves a segment of DNA from one location " + "to another in the genome.";

  CustGauss(0).Caption = "Length";

  StdDev(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.StdDev(Mode);
  Mean(0).text = TmpOpts.Specie(OptionsForm.instance.CurrSpec).Mutables.Mean(Mode);

  Label3(0).Content = "1 chance in  XXXXXXX per bp per copy";
}


}
}
