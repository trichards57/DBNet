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


static class Vegs {
// Option Explicit
//  V E G E T A B L E S   M A N A G E M E N T
public static int totvegs = 0;// total vegs in sim
public static int totvegsDisplayed = 0;// Value to display so as to not get a half-updated value
public static int cooldown = 0;
public static List<int> TotalSimEnergy = new List<int> (new int[101]);// Any array of the total amount of sim energy over the past 100 cycles.
public static int CurrentEnergyCycle = 0;// Index into he above array for calculating this cycle's sim energy.
public static int TotalSimEnergyDisplayed = 0;
public static decimal LightAval = 0;//Botsareus 8/14/2013 amount of avaialble light
//Botsareus 8/16/2014 Variable Sun
public static double SunPosition = 0;
public static decimal SunRange = 0;
public static double SunChange = 0;//0 1 2 position + 10 20 range
// adds vegetables in random positions


public static void VegsRepopulate() {
  int r = 0;

  int Rx = 0;

  int Ry = 0;

  int t = 0;

  cooldown = cooldown + 1;
  if (cooldown >= SimOpts.RepopCooldown) {
    for(t=1; t<SimOpts.RepopAmount; t++) {
//If Form1.Active Then 'Botsareus 3/20/2013 Bug fix to load vegs when cycle button pressed
      aggiungirob(-1, Random(ref 60, ref SimOpts.FieldWidth - 60), Random(ref 60, ref SimOpts.FieldHeight - 60));
      totvegs = totvegs + 1;
//End If
      Next(t);
      cooldown = cooldown - SimOpts.RepopCooldown;
    }
  }

/*
' gives vegs their energy meal
*/
public static void feedvegs(ref int totnrg) { //Panda 8/23/2013 Removed totv as it is no longer needed

//Sun position calculation
  if (SimOpts.SunOnRnd) {
    byte Sposition = 0;

    byte Srange = 0;

//0 1 2 position + 10 20 range (calculated as one byte, being aware of memory at this pont)
    Sposition = SunChange % 10;
    Srange = SunChange/10;

    if (Int(Rndy() * 2000) == 0) {
      Srange = IIf(Srange == 0, 1, 0);
    }
    if (Int(Rndy() * 2000) == 0) {
      Sposition = Int(Rndy() * 3);
    }

    if (Srange == 1) {
      SunRange = SunRange + 0.0005m;
    }
    if (Srange == 0) {
      SunRange = SunRange - 0.0005m;
    }
    if (SunRange >= 1) {
      Srange = 0;
    }
    if (SunRange <= 0) {
      Srange = 1;
    }

    if (Sposition == 0) {
      SunPosition = SunPosition - 0.0005m;
    }
    if (Sposition == 2) {
      SunPosition = SunPosition + 0.0005m;
    }
    if (SunPosition >= 1) {
      Sposition = 0;
    }
    if (SunPosition <= 0) {
      Sposition = 2;
    }
//0 1 2 position + 10 20 range
    SunChange = Sposition + Srange * 10;
  }

  int t = 0;

  decimal tok = 0;

  int depth = 0;

  bool FeedThisCycle = false;

  bool OverrideDayNight = false;


  decimal ScreenArea = 0;

  decimal TotalRobotArea = 0;

  decimal AreaCorrection = 0;

  decimal ChloroplastCorrection = 0;

  decimal AddEnergyRate = 0;

  decimal SubtractEnergyRate = 0;

  decimal acttok = 0;


  FeedThisCycle = SimOpts.Daytime; //Default is to feed if it is daytime, not feed if night
  OverrideDayNight = false;

  if (TotalSimEnergyDisplayed < SimOpts.SunUpThreshold && SimOpts.SunUp) {
//Sim Energy has fallen below the threshold.  Let the sun shine!
    switch(SimOpts.SunThresholdMode) {
      case TEMPSUNSUSPEND:
// We only suspend the sun cycles for this cycle.  We want to feed this cycle, but not
// advance the sun or disable day/night cycles
        FeedThisCycle = true;
        OverrideDayNight = true;
        break;
      case ADVANCESUN:
//Speed up time until Dawn.  No need to override the day night cycles as we want them to take over.
//Note that the real dawn won't actually start until the nrg climbs above the threshold since
//we will keep coming in here and zeroing the counter, but that's probably okay.
        SimOpts.DayNightCycleCounter = 0;
        SimOpts.Daytime = true;
        FeedThisCycle = true;
        break;
      case PERMSUNSUSPEND:
//We don't care about cycles.  We are just bouncing back and forth between the thresholds.
//We want to feed this cycle.
//We also want to turn on the sun.  The test below should avoid trying to execute day/night cycles.
        FeedThisCycle = true;
        SimOpts.Daytime = true;
break;
}
  } else if (TotalSimEnergyDisplayed > SimOpts.SunDownThreshold && SimOpts.SunDown) {
    switch(SimOpts.SunThresholdMode) {
      case TEMPSUNSUSPEND:
// We only suspend the sun cycles for this cycle.  We do not want to feed this cycle, nor do we
// advance the sun or disable day/night cycles
        FeedThisCycle = false;
        OverrideDayNight = true;
        break;
      case ADVANCESUN:
//Speed up time until Dusk.  No need to override the day night cycles as we want them to take over.
//Note that the real night time won't actually start until the nrg falls below the threshold since
//we will keep coming in here and zeroing the counter, but that's probably okay.
        SimOpts.DayNightCycleCounter = 0;
        SimOpts.Daytime = false;
        FeedThisCycle = false;
        break;
      case PERMSUNSUSPEND:
//We don't care about cycles.  We are just bouncing back and forth between the thresholds.
//We do not want to feed this cycle.
//We also want to turn off the sun.  The test below should avoid trying to execute day/night cycles
        FeedThisCycle = false;
        SimOpts.Daytime = false;
break;
}
  }

//In this mode, we ignore sun cycles and just bounce between thresholds.  I don't really want to add another
//feature enable checkbox, so we will just test to make sure the user is using both thresholds.  If not, we
//don't override the cycles even if one of the thresholds is set.
  if (SimOpts.SunThresholdMode == PERMSUNSUSPEND && SimOpts.SunDown && SimOpts.SunUp) {
    OverrideDayNight = true;
  }

  if (SimOpts.DayNight && !OverrideDayNight) {
//Well, we are neither above nor below the thresholds or we arn't using thresholds so lets see if it's time to rise and shine
    SimOpts.DayNightCycleCounter = SimOpts.DayNightCycleCounter + 1;
    if (SimOpts.DayNightCycleCounter > SimOpts.CycleLength) {
      SimOpts.Daytime = !SimOpts.Daytime;
      SimOpts.DayNightCycleCounter = 0;
    }
    if (SimOpts.Daytime) {
      FeedThisCycle = true;
    } else {
      FeedThisCycle = false;
    }
  }

  if (FeedThisCycle) {
//    MDIForm1.daypic.Visible = True
//   MDIForm1.nightpic.Visible = False
    MDIForm1.instance.SunButton.value = 0;
  } else {
//   MDIForm1.daypic.Visible = False
//    MDIForm1.nightpic.Visible = True
    MDIForm1.instance.SunButton.value = 1;
  }

//Botsareus 8/16/2014 All robots are set to think there is no sun, sun is calculated later
  for(t=1; t<MaxRobs; t++) {
    if (rob(t).nrg > 0& rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred)) {
      rob(t).mem(218) = 0;
    }
  }

  if (!FeedThisCycle) {
goto getout;
  }

  ScreenArea = CDbl(SimOptModule.SimOpts.FieldWidth) * CDbl(SimOptModule.SimOpts.FieldHeight); //Botsareus 12/28/2013 Formula simplified, people are getting resonable frame rates with 3ghz cpus

//Botsareus 12/28/2013 Subtract Obstacles
  for(t=1; t<numObstacles; t++) {
    if (Obstacles.Obstacles(t).exist) {
      ScreenArea = ScreenArea - Obstacles.Obstacles(t).Width * Obstacles.Obstacles(t).Height;
    }
  }

  for(t=1; t<MaxRobs; t++) { //Panda 8/14/2013 Figure out total robot area
    if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred)) { //Botsareus 8/14/2013 We have to make sure the robot is alive first
      TotalRobotArea = TotalRobotArea + rob(t).radius ^ 2 * PI;
    }
    Next(t);

    if (ScreenArea < 1) {
      ScreenArea = 1;
    }

    LightAval = TotalRobotArea / ScreenArea; //Panda 8/14/2013 Figure out AreaInverted a.k.a. available light
    if (LightAval > 1) {
      LightAval = 1; //Botsareus make sure LighAval never goes negative
    }

    AreaCorrection = (1 - LightAval) ^ 2 * 4;

//Botsareus 8/16/2014 Sun calculation
    int sunstart = 0;

    int sunstop = 0;

    int sunstart2 = 0;//wrap logic

    int sunstop2 = 0;


//Botsareus 8/16/2014 calculate the sun
    sunstart = (SunPosition - (0.25m + (SunRange ^ 3) * 0.75m) / 2) * SimOpts.FieldWidth;
    sunstop = (SunPosition + (0.25m + (SunRange ^ 3) * 0.75m) / 2) * SimOpts.FieldWidth;

    sunstop2 = sunstop;
    sunstart2 = sunstart; //Do not delete, bug fix!

    if (sunstart < 0) {
      sunstart2 = SimOpts.FieldWidth + sunstart;
      sunstop2 = SimOpts.FieldWidth;
    }
    if (sunstop > SimOpts.FieldWidth) {
      sunstop2 = sunstop - SimOpts.FieldWidth;
      sunstart2 = 0;
    }


    for(t=1; t<MaxRobs; t++) {
      dynamic _WithVar_7750;
      _WithVar_7750 = rob(t);
        if (_WithVar_7750.nrg > 0& _WithVar_7750.exist && !(_WithVar_7750.FName == "Base.txt" && hidepred)) {
//Botsareus 8/16/2014 Allow robots to share chloroplasts again
          if (_WithVar_7750.chloroplasts > 0) {
            if (_WithVar_7750.Chlr_Share_Delay > 0) {
              _WithVar_7750.Chlr_Share_Delay = _WithVar_7750.Chlr_Share_Delay - 1;
            }

            acttok = 0;

            if ((_WithVar_7750.pos.x < sunstart2 || _WithVar_7750.pos.x > sunstop2) && (_WithVar_7750.pos.x < sunstart || _WithVar_7750.pos.x > sunstop)) {
goto nextrob;
            }

            if (SimOpts.Pondmode) {
              depth = (_WithVar_7750.pos.y / 2000) + 1;
              if (depth < 1) {
                depth = 1;
              }
              tok = (SimOpts.LightIntensity / depth ^ SimOpts.Gradient); //Botsareus 3/26/2013 No longer add one, robots get fed more accuratly
            } else {
              tok = totnrg;
            }

            if (tok < 0) {
              tok = 0;
            }

            tok = tok / 3.5m; //Botsareus 2/25/2014 A little mod for PhinotPi

//Panda 8/14/2013 New chloroplast codez
            ChloroplastCorrection = _WithVar_7750.chloroplasts / 16000;
            AddEnergyRate = (AreaCorrection * ChloroplastCorrection) * 1.25m;
            SubtractEnergyRate = (_WithVar_7750.chloroplasts / 32000) ^ 2;

            acttok = (AddEnergyRate - SubtractEnergyRate) * tok;
          }
          _WithVar_7750.mem(218) = 1; //Botsareus 8/16/2014 Now it is time view the sun

nextrob:

          if (_WithVar_7750.chloroplasts > 0) {
            acttok = acttok - CSng(_WithVar_7750.age) * CSng(_WithVar_7750.chloroplasts) / 1000000000; //Botsareus 10/6/2015 Robots should start losing body at around 32000 cycles

            if (TmpOpts.Tides > 0) {
              acttok = acttok * (1 - BouyancyScaling); //Botsareus 10/6/2015 Cancer effect corrected for
            }

            _WithVar_7750.nrg = _WithVar_7750.nrg + acttok * (1 - SimOpts.VegFeedingToBody);
            _WithVar_7750.body = _WithVar_7750.body + (acttok * SimOpts.VegFeedingToBody) / 10;

            if (_WithVar_7750.nrg > 32000) {
              _WithVar_7750.nrg = 32000;
            }
            if (_WithVar_7750.body > 32000) {
              _WithVar_7750.body = 32000;
            }
            _WithVar_7750.radius = FindRadius(t);
          }

        }
      Next(t);
getout:
    }

public static void feedveg2(ref int t) { //gives veg an additional meal based on waste 'Botsareus 8/25/2013 Fix for all robots based on chloroplasts
//Botsareus 9/21/2013 completely redesigned to be liner and spread body vs energy
  decimal Energy = 0;

  decimal body = 0;


  dynamic _WithVar_4648;
  _WithVar_4648 = rob(t);
    Energy = _WithVar_4648.chloroplasts / 64000 * (1 - SimOpts.VegFeedingToBody);
    body = (_WithVar_4648.chloroplasts / 64000 * SimOpts.VegFeedingToBody) / 10;

    if (Int(Rndy() * 2) == 0) {
//energy first

      if (_WithVar_4648.Waste > 0) {
        if (_WithVar_4648.nrg + Energy < 32000) {
          _WithVar_4648.nrg = _WithVar_4648.nrg + Energy;
          _WithVar_4648.Waste = _WithVar_4648.Waste - _WithVar_4648.chloroplasts / 32000 * (1 - SimOpts.VegFeedingToBody);
        }
        if (_WithVar_4648.Waste < 0) {
          _WithVar_4648.Waste = 0;
        }
      }

      if (_WithVar_4648.Waste > 0) {
        if (_WithVar_4648.body + body < 32000) {
          _WithVar_4648.body = _WithVar_4648.body + body;
          _WithVar_4648.Waste = _WithVar_4648.Waste - _WithVar_4648.chloroplasts / 32000 * SimOpts.VegFeedingToBody;
        }
        if (_WithVar_4648.Waste < 0) {
          _WithVar_4648.Waste = 0;
        }
      }

    } else {
//body first

      if (_WithVar_4648.Waste > 0) {
        if (_WithVar_4648.body + body < 32000) {
          _WithVar_4648.body = _WithVar_4648.body + body;
          _WithVar_4648.Waste = _WithVar_4648.Waste - _WithVar_4648.chloroplasts / 32000 * SimOpts.VegFeedingToBody;
        }
        if (_WithVar_4648.Waste < 0) {
          _WithVar_4648.Waste = 0;
        }
      }

      if (_WithVar_4648.Waste > 0) {
        if (_WithVar_4648.nrg + Energy < 32000) {
          _WithVar_4648.nrg = _WithVar_4648.nrg + Energy;
          _WithVar_4648.Waste = _WithVar_4648.Waste - _WithVar_4648.chloroplasts / 32000 * (1 - SimOpts.VegFeedingToBody);
        }
        if (_WithVar_4648.Waste < 0) {
          _WithVar_4648.Waste = 0;
        }
      }

    }

}
}
