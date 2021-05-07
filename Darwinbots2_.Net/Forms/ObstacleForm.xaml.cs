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
public partial class ObstacleForm : Window {
  private static ObstacleForm _instance;
  public static ObstacleForm instance { set { _instance = null; } get { return _instance ?? (_instance = new ObstacleForm()); }}  public static void Load() { if (_instance == null) { dynamic A = ObstacleForm.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public ObstacleForm() { InitializeComponent(); }


// Copyright (c) 2006 Eric Lockard
// eric@sulaadventures.com
// All rights reserved.
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that:
//(1) source code distributions retain the above copyright notice and this
//    paragraph in its entirety,
//(2) distributions including binary code include the above copyright notice and
//    this paragraph in its entirety in the documentation or other materials
//    provided with the distribution, and
//(3) Without the agreement of the author redistribution of this product is only allowed
//    in non commercial terms and non profit distributions.
//THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR IMPLIED
//WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
// Option Explicit
//Botsareus 6/12/2012 form's icon change


private void ShapesAbsorbShotsCheck_Click(object sender, RoutedEventArgs e) { ShapesAbsorbShotsCheck_Click(); }
private void ShapesAbsorbShotsCheck_Click() {
  SimOpts.shapesAbsorbShots = ShapesAbsorbShotsCheck.value;
}

private void ShapesSeeThroughCheck_Click(object sender, RoutedEventArgs e) { ShapesSeeThroughCheck_Click(); }
private void ShapesSeeThroughCheck_Click() {
  SimOpts.shapesAreSeeThrough = ShapesSeeThroughCheck.value;
  if (SimOpts.shapesAreSeeThrough) {
    ShapesVisableCheck.IsEnabled = false;
    SimOpts.shapesAreVisable = false;
  } else {
    ShapesVisableCheck.IsEnabled = true;
    SimOpts.shapesAreVisable = ShapesVisableCheck.value;
  }
}

private void ShapesVisableCheck_Click(object sender, RoutedEventArgs e) { ShapesVisableCheck_Click(); }
private void ShapesVisableCheck_Click() {
  SimOpts.shapesAreVisable = ShapesVisableCheck.value;
  if (SimOpts.shapesAreVisable) {
    ShapesSeeThroughCheck.IsEnabled = false;
    SimOpts.shapesAreSeeThrough = false;
  } else {
    ShapesSeeThroughCheck.IsEnabled = true;
    SimOpts.shapesAreSeeThrough = ShapesSeeThroughCheck.value;
  }
}

private void VerticalDriftCheck_Click(object sender, RoutedEventArgs e) { VerticalDriftCheck_Click(); }
private void VerticalDriftCheck_Click() {
  SimOpts.allowVerticalShapeDrift = VerticalDriftCheck.value;
  if (!SimOpts.allowVerticalShapeDrift) {
    Obstacles.StopAllVerticalObstacleMovement();
  }
}

private void BlackColorOption_Click(object sender, RoutedEventArgs e) { BlackColorOption_Click(); }
private void BlackColorOption_Click() {
  SimOpts.makeAllShapesBlack = true;
  ChangeAllObstacleColor(vbBlack);
  RandomColorOption.value = false;
}

private void DriftRateSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { DriftRateSlider_Change(); }
private void DriftRateSlider_Change() {
  SimOpts.shapeDriftRate = DriftRateSlider.value;
  if (leftCompactor > 0) {
    Obstacles.Obstacles(leftCompactor).vel.x = (SimOpts.shapeDriftRate * 0.1m) * Sgn(Obstacles.Obstacles(leftCompactor).vel.x);
  }
  if (rightCompactor > 0) {
    Obstacles.Obstacles(rightCompactor).vel.x = (SimOpts.shapeDriftRate * 0.1m) * Sgn(Obstacles.Obstacles(rightCompactor).vel.x);
  }
}

public void InitShapesDialog() {
  TransparentOption.value = SimOpts.makeAllShapesTransparent;
  OpaqueOption.value = !SimOpts.makeAllShapesTransparent;
  RandomColorOption.value = !SimOpts.makeAllShapesBlack;
  BlackColorOption.value = SimOpts.makeAllShapesBlack;
  WidthSlider.value = CInt(defaultWidth * 1000);
  HeightSlider.value = CInt(defaultHeight * 1000);
  DriftRateSlider.value = SimOpts.shapeDriftRate;
  if (SimOpts.allowHorizontalShapeDrift) {
    HorizontalDriftCheck.value = 1;
  } else {
    HorizontalDriftCheck.value = 0;
  }
  if (SimOpts.allowVerticalShapeDrift) {
    VerticalDriftCheck.value = 1;
  } else {
    VerticalDriftCheck.value = 0;
  }
  MazeWidthSlider.value = mazeCorridorWidth;
  WallThicknessSlider.value = mazeWallThickness;
  if (SimOpts.shapesAreSeeThrough) {
    ShapesSeeThroughCheck.value = 1;
  } else {
    ShapesSeeThroughCheck.value = 0;
  }
  if (SimOpts.shapesAbsorbShots) {
    ShapesAbsorbShotsCheck.value = 1;
  } else {
    ShapesAbsorbShotsCheck.value = 0;
  }
  if (SimOpts.shapesAreVisable) {
    ShapesVisableCheck.value = 1;
  } else {
    ShapesVisableCheck.value = 0;
  }
}

private void CopyToTmpOpts() { //Botsareus 1/5/2014 Make sure the shape settings are saved
  TmpOpts.makeAllShapesTransparent = SimOpts.makeAllShapesTransparent;
  TmpOpts.makeAllShapesBlack = SimOpts.makeAllShapesBlack;
  TmpOpts.shapeDriftRate = SimOpts.shapeDriftRate;
  TmpOpts.allowHorizontalShapeDrift = SimOpts.allowHorizontalShapeDrift;
  TmpOpts.allowVerticalShapeDrift = SimOpts.allowVerticalShapeDrift;
  TmpOpts.shapesAreSeeThrough = SimOpts.shapesAreSeeThrough;
  TmpOpts.shapesAbsorbShots = SimOpts.shapesAbsorbShots;
  TmpOpts.shapesAreVisable = SimOpts.shapesAreVisable;
}

private void HeightSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { HeightSlider_Change(); }
private void HeightSlider_Change() {
  defaultHeight = HeightSlider.value * 0.001m;
}

private void HorizontalDriftCheck_Click(object sender, RoutedEventArgs e) { HorizontalDriftCheck_Click(); }
private void HorizontalDriftCheck_Click() {
  SimOpts.allowHorizontalShapeDrift = HorizontalDriftCheck.value;
  if (!SimOpts.allowHorizontalShapeDrift) {
    Obstacles.StopAllHorizontalObstacleMovement();
  }
}

private void MakeShape_Click(object sender, RoutedEventArgs e) { MakeShape_Click(); }
private void MakeShape_Click() {
  decimal randomX = 0;

  decimal randomy = 0;


  randomX = Random(ref 0, ref SimOpts.FieldWidth) - SimOpts.FieldWidth * (defaultWidth / 2);
  randomy = Random(ref 0, ref SimOpts.FieldHeight) - SimOpts.FieldHeight * (defaultHeight / 2);
  NewObstacle(randomX, randomy, SimOpts.FieldWidth * defaultWidth, SimOpts.FieldHeight * defaultHeight);
}

private void MazeWidthSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MazeWidthSlider_Change(); }
private void MazeWidthSlider_Change() {
  mazeCorridorWidth = MazeWidthSlider.value;
}

private void OK_Click(object sender, RoutedEventArgs e) { OK_Click(); }
private void OK_Click() {
  CopyToTmpOpts();
  this
}

private void OpaqueOption_Click(object sender, RoutedEventArgs e) { OpaqueOption_Click(); }
private void OpaqueOption_Click() {
  SimOpts.makeAllShapesTransparent = false;
  TransparentOption.value = false;
}

private void RandomColorOption_Click(object sender, RoutedEventArgs e) { RandomColorOption_Click(); }
private void RandomColorOption_Click() {
  BlackColorOption.value = false;
  ChangeAllObstacleColor(-1);
  SimOpts.makeAllShapesBlack = false;
}

private void TransparentOption_Click(object sender, RoutedEventArgs e) { TransparentOption_Click(); }
private void TransparentOption_Click() {
  SimOpts.makeAllShapesTransparent = true;
  OpaqueOption.value = false;
}

private void WallThicknessSlider_Click(object sender, RoutedEventArgs e) { WallThicknessSlider_Click(); }
private void WallThicknessSlider_Click() {
  mazeWallThickness = WallThicknessSlider.value;
}

private void WidthSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { WidthSlider_Change(); }
private void WidthSlider_Change() {
  defaultWidth = WidthSlider.value * 0.001m;
}


}
}
