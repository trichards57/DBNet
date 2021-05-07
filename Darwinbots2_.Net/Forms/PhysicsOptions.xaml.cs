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
public partial class PhysicsOptions : Window {
  private static PhysicsOptions _instance;
  public static PhysicsOptions instance { set { _instance = null; } get { return _instance ?? (_instance = new PhysicsOptions()); }}  public static void Load() { if (_instance == null) { dynamic A = PhysicsOptions.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public PhysicsOptions() { InitializeComponent(); }


// Option Explicit //s Number for an average bot moving at 1 twip/cycle:
//Botsareus 6/12/2012 form's icon change


private void Command_Click(object sender, RoutedEventArgs e) { Command_Click(); }
private void Command_Click(ref int Index) {
  switch(Index) {
    case 0:  //help on units
      Text1.text = "Darwinbots uses the following units:" + vbCrLf + vbCrLf + "Distance : Twip.  A small bot has a diameter of 120 twips." + vbCrLf + vbCrLf + "Time : Cycle. The smallest measure of time possible is a single cycle" + vbCrLf + vbCrLf + "Mass : Mass.  Yes, the unit is called a Mass.  A small bot has a mass of roughly 1 Mass." + vbCrLf + vbCrLf + "Force :                            .  Mass twip per cycle per cycle.  A Bang of one cycle has a force of one Darwin." + vbCrLf + vbCrLf + "Energy : NRG.  1 NRG is the basic unit block of what a bot reads back as " + "*.nrg.  Each Bang costs 1 NRG under ideal settings." + vbCrLf + vbCrLf + "Impulse : Bang.  A Bang is a measure of the change in momentum of " + "an object over a given time period, as well as a measure of flow " + "of energy.  A robot under ideal conditions doing 1 .up store is " + "spending 1 NRG and producing 1 Bang.";
      break;
    case 1:
      Unload(this);
      break;//general help
    case 3:
      Text1.text = "The medium in which the bots live has some remarkable " + "properties that can become difficult to invision all at once.  " + "Imagine their world is shaped like a piece of paper.  It's very " + "long and very wide, but not very deep.  For friction, 'down' is " + "considered to be in the direction of depth, that is, into your screen.  " + "Since bots cannot move in this direction, Z axis Gravity only effects " + "the force of friction.  Y axis gravity is imagining that 'down' for " + "the bots and 'down' for you, the user, are the same.  " + vbCrLf + vbCrLf + "For the purposes " + "of friction, the bots are considered to be sliding along on a solid " + "surface.  For purposes of drag (fluid dynamics) and bouyancy, it's " + "supposed that the bots are living in an infinite body of liquid which " + "can flow around them in all 3 dimensions." + vbCrLf + vbCrLf + "If the above paragraphs just confused the heck out of you, don't worry.  " + "That's why there's presets on the main options page.  If you still feel " + "up to playing with the sliders, I feel confidant you'll soon understand " + "the effects with a little practice.";
break;
}
}

private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
private void Command1_Click() {
  Text1.text = "Reynolds number is a unitless representation of the ratio of " + "turbulent forces to laminar forces.  Low Reynolds numbers mean a Laminar " + "flow, while high numbers mean a turbulent flow.  The program will only " + "use this number if flow type is dynamic.  Otherwise it's for your " + "information only.  A reynolds number of less than 3E+5 is characteristic " + "of Laminar flows.";
}

private void FluidText_GotFocus(object sender, RoutedEventArgs e) { FluidText_GotFocus(); }
private void FluidText_GotFocus(ref int Index) {
  switch(Index) {
    case 0:  //viscosity
      Text1.text = "Viscosity currently only effects bots under laminar " + "flow types.  Resistive force under laminar flow = " + "6 * pi * viscosity * velocity * radius of bot (normal bot has a radius of " + "60).  Experimentation has yielded that the " + "approximate magnitude of an appropriate viscosity is between 1E-5 and " + "1E-3 (or between 1 and 100 in the text box, since it's multiplied by E-5).";

      break;//density
    case 1:
      Text1.text = "Density controls Added Mass, and Terbulent Flow.  " + "A small bot has a density of about 1E-6" + ".  The higher the density, the stronger added mass, " + "and terbulent drag are.  Terbulent drag is pi/4 * radius of bot ^ 2 " + "(remember that an average bot radius is 60) * velocity ^2 * density.";
break;
}
}

private void FluidText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { FluidText_Change(); }
private void FluidText_Change(ref int Index) {
  decimal UnitReynolds = 0;

  decimal temp = 0;


  switch(Index) {
    case 0:
      TmpOpts.Viscosity = val(FluidText(Index).text) * 10 ^ -5;
      break;
    case 1:
      TmpOpts.Density = val(FluidText(Index).text) * 10 ^ -7;
break;
}
//update Reynolds number
  if (TmpOpts.Viscosity == 0) {
    temp = 10 ^ -7; //to prevent divide by zero
  } else {
    temp = TmpOpts.Viscosity;
  }
  UnitReynolds = (TmpOpts.Density * 2 * RobSize / 2 * 1) / temp;
  if (UnitReynolds >= 10000) {
    FluidText(2).text = Format(UnitReynolds, "Scientific");
  } else {
    FluidText(2).text = UnitReynolds;
  }
}

private void FluidText_LostFocus(object sender, RoutedEventArgs e) { FluidText_LostFocus(); }
private void FluidText_LostFocus(ref int Index) {
  FluidText_Change((Index));

  FluidText(0).text = TmpOpts.Viscosity * 10 ^ 5;
  FluidText(1).text = TmpOpts.Density * 10 ^ 7;
}

private void Friction_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { Friction_Change(); }
private void Friction_Change(ref int Index) {
  switch(Index) {
    case 0:
      TmpOpts.Zgravity = Friction(Index).value / 10;
      FrictionText(Index).Caption = TmpOpts.Zgravity;
      break;
    case 1:
      TmpOpts.CoefficientKinetic = Friction(Index).value / 100;
      FrictionText(Index).Caption = TmpOpts.CoefficientKinetic;
      break;
    case 2:
      TmpOpts.CoefficientStatic = Friction(Index).value / 100;
      FrictionText(Index).Caption = TmpOpts.CoefficientStatic;
break;
}
}

private void Friction_Scroll(ref int Index) {
  Friction_Change((Index));
}

private void Friction_GotFocus(object sender, RoutedEventArgs e) { Friction_GotFocus(); }
private void Friction_GotFocus(ref int Index) {
  switch(Index) {
    case 0:  //Z axis gravity
      Text1.text = "Z axis gravity only effects the force of gravity.  " + "The larger Z axis gravity, the stronger the effect of friction will be.";
      break;//kinetic
    case 1:
      Text1.text = "Coefficient of Kinetic Friction.  Generally lower than " + "coefficient of static friction.  This multiplied by weight caused by " + "Z axis gravity from a moving object gives a resistive force to motion " + "that's independant of velocity.";
      break;//static
    case 2:
      Text1.text = "Coefficient of Static Friction.  Generally higher than " + "coefficient of kinetic friction.  This multiplied by weight caused by " + "Z axis gravity give a resistive force that " + "must be overcome for motion to be achieved.";
break;
}
}

private void MiscSlider_Scroll(ref int Index) {
  MiscSlider_Change((Index));
}

private void MiscSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MiscSlider_Change(); }
private void MiscSlider_Change(ref int Index) {
  switch(Index) {
    case 0:  //bang efficiency
      TmpOpts.PhysMoving = MiscSlider(Index).value / 100;
      MiscText(Index).Caption = CStr(MiscSlider(Index).value) + "%";
      break;//Y axis gravity
    case 1:
      TmpOpts.Ygravity = MiscSlider(Index).value / 10;
      MiscText(Index).Caption = CStr(MiscSlider(Index).value);
      break;
    case 2:
      TmpOpts.PhysBrown = MiscSlider(Index).value / 10;
      MiscText(Index).Caption = CStr(MiscSlider(Index).value);
break;
}
}

private void MiscSlider_GotFocus(object sender, RoutedEventArgs e) { MiscSlider_GotFocus(); }
private void MiscSlider_GotFocus(ref int Index) {
  switch(Index) {
    case 0:  //bang efficiency
      Text1.text = "Bang efficiency measures how efficient the " + ".up, .dn, .dx, and .sx commands are.  100% is the ideal " + "case, meaning that there is 100% conversion of energy " + "spent to work done." + vbCrLf + vbCrLf + "This is " + "technically impossible because " + "of the second law of thermodynamics.  A range of 60%-80% " + "corresponds to biological systems, while 10-30% corresponds " + "to most artifical systems such as machines.";
      break;//Y axis Gravity
    case 1:
      Text1.text = "Measured in twips per cycle per cycle.  " + "Points downwards, as if your computer is the side of a fishbowl.  " + "Used in bouyancy and weight calculations.";
      break;//Brownian Motion
    case 2:
      Text1.text = "Random perturbations in the medium.  " + "High Brownian motion is representative of very tiny organisms," + "such as viruses and bacteria.  The effect is to give " + "the bot up to the set amount of Darwins in a random " + "direction each cycle.";
break;
}
}

private void PlanetEatersText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { PlanetEatersText_Change(); }
private void PlanetEatersText_Change() {
  TmpOpts.PlanetEatersG = val(PlanetEatersText.text) * 10 ^ 3;
}

private void Toggles_Click(object sender, RoutedEventArgs e) { Toggles_Click(); }
private void Toggles_Click(ref int Index) {
  switch(Index) {
    case 0:
      Text1.text = "Zero momentum simply prevents a bot from keeping any " + "velocity from cycle to cycle.  Effects are similar to Laminar flow, " + "but there are subtle differences.";
      TmpOpts.ZeroMomentum = Toggles(Index).value * true;
      break;
    case 1:
      Text1.text = "Planet Eaters gives all bots an attractive force towards " + "all other bots.  The force is equal to G * m1 * m2 / distance between " + "bots ^ 2.  A value of 14.4E+3 gives 1 Darwin attractive force to touching " + "bots.  A G of 864E+3 gives a force of 60 Darwins to touching bots, and a " + "force of 1.7 to bots 5 bot lengths away from each other.";
      TmpOpts.PlanetEaters = Toggles(Index).value * true;
      PlanetEatersText.IsEnabled = TmpOpts.PlanetEaters;
break;
}
}

private void Form_Activate() {
//Update all displays
  dynamic _WithVar_2434;
  _WithVar_2434 = TmpOpts;

//toggles
    if (_WithVar_2434.ZeroMomentum) {
      Toggles(0).value = 1;
    }
    if (_WithVar_2434.PlanetEaters) {
      Toggles(1).value = 1;
    }
    PlanetEatersText.text = _WithVar_2434.PlanetEatersG / 10 ^ 3;

    if (TmpOpts.Tides) {
      _WithVar_2434.Ygravity = SimOpts.Ygravity;
      _WithVar_2434.PhysBrown = SimOpts.PhysBrown;
    }

    MiscSlider(0).value = _WithVar_2434.PhysMoving * 100;
    MiscSlider(1).value = _WithVar_2434.Ygravity * 10;
    MiscSlider(2).value = _WithVar_2434.PhysBrown * 10; //EricL 3/21/2006 Changed from 100 to 10 to fix bug where slider set to 10x to high


    Friction(0).value = _WithVar_2434.Zgravity * 10;
    Friction(1).value = _WithVar_2434.CoefficientKinetic * 100;
    Friction(2).value = _WithVar_2434.CoefficientStatic * 100;

    FluidText(0).text = _WithVar_2434.Viscosity * 10 ^ 5;
    FluidText(1).text = _WithVar_2434.Density * 10 ^ 7;

}


}
}
