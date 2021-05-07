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


static class Evo {
// Option Explicit
// * * * * * * * * * * * * * * * * * * *
// All special evolution modes are here
// * * * * * * * * * * * * * * * * * * *


private static void exportdata() {
//save main data
  VBOpenFile(1, MDIForm1.MainDir + "\\evolution\\data.gset");;
  Write(#1, LFOR); //LFOR init
  Write(#1, LFORdir); //dir True means decrease diff
  Write(#1, LFORcorr); //corr

  Write(#1, hidePredCycl); //hidePredCycl

  Write(#1, curr_dna_size); //curr_dna_size
  Write(#1, target_dna_size); //target_dna_size

  Write(#1, Init_hidePredCycl);

  Write(#1, y_Stgwins);
  VBCloseFile(1);();
//save restart mode
  if (x_restartmode == 5) {
    x_restartmode = 4;
  }
  VBOpenFile(1, App.path + "\\restartmode.gset");;
  Write(#1, x_restartmode);
  Write(#1, x_filenumber);
  VBCloseFile(1);();
//Restart
  VBOpenFile(1, App.path + "\\Safemode.gset");;
  Write(#1, false);
  VBCloseFile(1);();
  VBOpenFile(1, App.path + "\\autosaved.gset");;
  Write(#1, false);
  VBCloseFile(1);();
  Call(restarter());
}

private static int n10(decimal a) {//range correction
  int n10 = 0;
  int b = 0;

  if (a <= 1) {
    b = 10 ^ (1 - (Log(a * 2)/Log(10)));
  } else {
    b = 1;
  }
  n10 = b;
  return n10;
}

private static void Increase_Difficulty() {
  if (LFORdir) {
    LFORdir = false;
    LFORcorr = LFORcorr / 2;
  }
//Botsare us 7/01/2014 a little mod here, more sane floor on lfor
  decimal tmpLFOR = 0;

  tmpLFOR = LFOR;
  LFOR = LFOR - LFORcorr / n10(LFOR);
  if (LFOR < 1 / n10(tmpLFOR)) {
    LFOR = 1 / n10(tmpLFOR);
  }
  if (LFOR < 0.01m) {
    LFOR = 0.01m;
  }

  hidePredCycl = Init_hidePredCycl + 300 * rndy() - 150;

  if (hidePredCycl < 150) {
    hidePredCycl = 150;
  }
  if (hidePredCycl > 15000) {
    hidePredCycl = 15000;
  }
//In really rear cases start emping up difficulty using other means
  if (LFOR == 0.01m) {
    hidePredCycl = 150;
    Init_hidePredCycl = 150;
  }
}

private static void Next_Stage() {
//Reset F1 test
  y_Stgwins = 0;

//Configure settings

  LFORdir = !LFORdir;
  LFORcorr = 5;
  Init_hidePredCycl = hidePredCycl;

  if (y_normsize) { //This stuff should only happen if y_normalize is enabled

    int gotdnalen = 0;


    if (LoadDNA(ref MDIForm1.instance.MainDir + "\\evolution\\Test.txt", ref 0)) {
      gotdnalen = DnaLen(ref rob(0).dna);
    }

    decimal sizechangerate = 0;

    sizechangerate = (5000 - target_dna_size) / 4750;
    if (sizechangerate < 0) {
      sizechangerate = 0;
    }

    if (gotdnalen < target_dna_size) {
      curr_dna_size = gotdnalen + 5; //current dna size is 5 more then old size
      if ((gotdnalen >= (target_dna_size - 15)) && (gotdnalen <= (target_dna_size + sizechangerate * 75))) {
        target_dna_size = target_dna_size + (sizechangerate * 250) + 10;
      }
    } else {
      curr_dna_size = target_dna_size;
      if ((gotdnalen >= (target_dna_size - 15)) && (gotdnalen <= (target_dna_size + sizechangerate * 75))) {
        target_dna_size = target_dna_size + (sizechangerate * 250) + 10;
      }
    }

  }

//Configure robots

//next stage
  x_filenumber = x_filenumber + 1;

  if (y_eco_im > 0) {
    byte ecocount = 0;

    for(ecocount=1; ecocount<15; ecocount++) {
      FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stagerob" + ecocount + "\\stage" + x_filenumber + ".txt");
      FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\stages\\stagerob" + ecocount + "\\stage" + x_filenumber + ".mrate");
    }
  } else {
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt");
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".mrate");
  }

//kill main dir robots
  if (y_eco_im > 0) {
    for(ecocount=1; ecocount<15; ecocount++) {
      RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount);
      RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount);
    }
  } else {
    File.Delete(MDIForm1.MainDir + "\\evolution\\Base.txt");;
    File.Delete(MDIForm1.MainDir + "\\evolution\\Mutate.txt");;
    if (dir(MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate") != "") {
      File.Delete(MDIForm1.MainDir + "\\evolution\\Mutate.mrate");;
    }
  }

//copy robots
  if (y_eco_im > 0) {
    for(ecocount=1; ecocount<15; ecocount++) {
      MkDir(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount);
      MkDir(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount);
      FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt");
      FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
      FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");
    }
  } else {
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt");
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");
  }

//kill test robot
  if (y_eco_im > 0) {
    for(ecocount=1; ecocount<15; ecocount++) {
      RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount);
    }
  } else {
    File.Delete(MDIForm1.MainDir + "\\evolution\\Test.txt");;
    File.Delete(MDIForm1.MainDir + "\\evolution\\Test.mrate");;
  }
}

private static void Decrease_Difficulty() {
//Botsareus 12/11/2015 renormalize the mutation rates
  renormalize_mutations();


  if (!LFORdir) {
    LFORdir = true;
    LFORcorr = LFORcorr / 2;
  }
  LFOR = LFOR + LFORcorr / n10(LFOR);
  if (LFOR > 150) {
    LFOR = 150;
  }

  hidePredCycl = Init_hidePredCycl + 300 * rndy() - 150;

  if (hidePredCycl < 150) {
    hidePredCycl = 150;
  }
  if (hidePredCycl > 15000) {
    hidePredCycl = 15000;
  }

//Botsareus 8/17/2016 Revert one stage, should not apply to eco evo
  if (LFORcorr < 0.00000005m && y_eco_im == 0& x_filenumber > 0) {
    logevo("Reverting one stage.");
    revert();
  }
}

private static void revert() {
//Kill a stage
  File.Delete(MDIForm1.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt");;
  File.Delete(MDIForm1.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".mrate");;
//Update file number
  x_filenumber = x_filenumber - 1;
//Move files
  FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt", MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
  FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt", MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt");
  FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".mrate", MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");
//Reset data
  LFORcorr = 5;
  LFOR = (LFOR + 10) / 2; //normalize LFOR toward 10
  int fdnalen = 0;

  if (LoadDNA(ref MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt", ref 0)) {
    fdnalen = DnaLen(ref rob(0).dna);
  }
  curr_dna_size = fdnalen + 5;
}

private static void renormalize_mutations() {
  decimal val = 0;

  val = 5 / LFORcorr;
  val = val * 90;

  byte ecocount = 0;

  mutationprobs norm = null;

  int a = 0;

  mutationprobs filem = null;


  byte i = 0;

  decimal tot = 0;

  decimal rez = 0;//the mult 3 of the average value


  int length = 0;


  if (y_eco_im == 0) {
//load mutations

    // TODO (not supported):     On Error GoTo nofile

    filem = Load_mrates(ref MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");

//calculate normalized rate

    tot = 0;
    i = 0;
    for(a=0; a<10; a++) {
      if (filem.mutarray(a) > 0) {
        tot = tot + filem.mutarray(a);
        i = i + 1;
      }
      Next(a);
      rez = tot / i * 3;

      if (LoadDNA(ref MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt", ref 0)) {
        length = DnaLen(ref rob(0).dna);
      }

      if (rez > IIf(NormMut, length * CLng(valMaxNormMut), 2000000000)) {
        rez = IIf(NormMut, length * CLng(valMaxNormMut), 2000000000);
      }

//norm holds normalized mutation rates

      mutationprobs _WithVar_norm;
      _WithVar_norm = norm;

        for(a=0; a<10; a++) {
          _WithVar_norm.mutarray(a) = rez;
          _WithVar_norm.Mean(a) = 1;
          _WithVar_norm.StdDev(a) = 0;
          Next(a);

          SetDefaultLengths(norm);

          norm = _WithVar_norm;

//renormalize mutations

        filem.CopyErrorWhatToChange = (filem.CopyErrorWhatToChange * (val - 1) + norm.CopyErrorWhatToChange) / val;
        filem.PointWhatToChange = (filem.PointWhatToChange * (val - 1) + norm.PointWhatToChange) / val;

        for(a=0; a<10; a++) {
          if (filem.mutarray(a) > 0) {
            filem.mutarray(a) = (filem.mutarray(a) * (val - 1) + norm.mutarray(a)) / val;
          }
          filem.Mean(a) = (filem.Mean(a) * (val - 1) + norm.Mean(a)) / val;
          filem.StdDev(a) = (filem.StdDev(a) * (val - 1) + norm.StdDev(a)) / val;
          Next(a);

//save mutations

          Save_mrates(filem, MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");

nofile:

        } else {
          for(ecocount=1; ecocount<15; ecocount++) {
//load mutations

            // TODO (not supported):         On Error GoTo nextrob

            filem = Load_mrates(ref MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");

//calculate normalized rate

            tot = 0;
            i = 0;
            for(a=0; a<10; a++) {
              if (filem.mutarray(a) > 0) {
                tot = tot + filem.mutarray(a);
                i = i + 1;
              }
              Next(a);
              rez = tot / i * 3;

              if (LoadDNA(ref MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt", ref 0)) {
                length = DnaLen(ref rob(0).dna);
              }

              if (rez > IIf(NormMut, length * CLng(valMaxNormMut), 2000000000)) {
                rez = IIf(NormMut, length * CLng(valMaxNormMut), 2000000000);
              }


//norm holds normalized mutation rates

              mutationprobs _WithVar_norm;
              _WithVar_norm = norm;

                for(a=0; a<10; a++) {
                  _WithVar_norm.mutarray(a) = rez;
                  _WithVar_norm.Mean(a) = 1;
                  _WithVar_norm.StdDev(a) = 0;
                  Next(a);

                  SetDefaultLengths(norm);

                  norm = _WithVar_norm;

//renormalize mutations

                filem.CopyErrorWhatToChange = (filem.CopyErrorWhatToChange * (val - 1) + norm.CopyErrorWhatToChange) / val;
                filem.PointWhatToChange = (filem.PointWhatToChange * (val - 1) + norm.PointWhatToChange) / val;

                for(a=0; a<10; a++) {
                  if (filem.mutarray(a) > 0) {
                    filem.mutarray(a) = (filem.mutarray(a) * (val - 1) + norm.mutarray(a)) / val;
                  }
                  filem.Mean(a) = (filem.Mean(a) * (val - 1) + norm.Mean(a)) / val;
                  filem.StdDev(a) = (filem.StdDev(a) * (val - 1) + norm.StdDev(a)) / val;
                  Next(a);

//save mutations

                  Save_mrates(filem, MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");

nextrob:

                }

              }

            }

private static void scale_mutations() {
  decimal val = 0;

  decimal holdrate = 0;

  val = 5 / LFORcorr;
  val = val * 5;

  byte ecocount = 0;

  int a = 0;

  mutationprobs filem = null;


  byte i = 0;

  decimal tot = 0;

  decimal rez = 0;


  if (y_eco_im == 0) {
//load mutations

    // TODO (not supported):     On Error GoTo nofile

    filem = Load_mrates(ref MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");

    tot = 0;
    i = 0;
    for(a=0; a<10; a++) {
      if (filem.mutarray(a) > 0) {
        tot = tot + filem.mutarray(a);
        i = i + 1;
      }
      Next(a);
      rez = tot / i;

      for(a=0; a<10; a++) {
        if (filem.mutarray(a) > 0) {
//The lower the value, the faster it reaches 1
          holdrate = filem.mutarray(a);

          if (holdrate >= (Log(6) / Log(4) * rez)) {
            holdrate = (Log(6) / Log(4) * rez) - 1;
          }
          holdrate = holdrate / 6 * 4 ^ (holdrate / rez);
          if (holdrate < 1) {
            holdrate = 1;
          }

          filem.mutarray(a) = (filem.mutarray(a) * (val - 1) + holdrate) / val;
        }
        Next(a);

//save mutations

        Save_mrates(filem, MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");

nofile:

      } else {
        for(ecocount=1; ecocount<15; ecocount++) {
//load mutations

          // TODO (not supported):         On Error GoTo nextrob

          filem = Load_mrates(ref MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");

          tot = 0;
          i = 0;
          for(a=0; a<10; a++) {
            if (filem.mutarray(a) > 0) {
              tot = tot + filem.mutarray(a);
              i = i + 1;
            }
            Next(a);
            rez = tot / i;

            for(a=0; a<10; a++) {
              if (filem.mutarray(a) > 0) {
//The lower the value, the faster it reaches 1
                holdrate = filem.mutarray(a);

                if (holdrate >= (Log(6) / Log(4) * rez)) {
                  holdrate = (Log(6) / Log(4) * rez) - 1;
                }
                holdrate = holdrate / 6 * 4 ^ (holdrate / rez);
                if (holdrate < 1) {
                  holdrate = 1;
                }

                filem.mutarray(a) = (filem.mutarray(a) * (val - 1) + holdrate) / val;
              }
              Next(a);

//save mutations

              Save_mrates(filem, MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");

nextrob:

            }

          }
        }

public static void UpdateWonEvo(int bestrob) { //passing best robot
  if (rob(bestrob).Mutations > 0& (totnvegsDisplayed >= 15 || y_eco_im == 0)) {
    logevo("Evolving robot changed, testing robot.");
//F1 mode init
    if (y_eco_im == 0) {
      salvarob(bestrob, MDIForm1.instance.MainDir + "\\evolution\\Test.txt");
    } else {
//The Eco Calc

//Step1 disable simulation execution
      DisplayActivations = false;
      Form1.Active = false;
      Form1.SecTimer.Enabled = false;

//Step2 calculate cumelative genetic distance
      Form1.GraphLab.Visible = true;

      List<decimal> maxgdi = new List<decimal> {}; // TODO - Specified Minimum Array Boundary Not Supported:           Dim maxgdi() As Single

      List<decimal> maxgdi_4709_tmp = new List<decimal>();
for (int redim_iter_3789=0;i<0;redim_iter_3789++) {maxgdi.Add(0);}

      int t = 0;

      int t2 = 0;


      for(t=1; t<MaxRobs; t++) {
        if (rob(t).exist && !rob(t).Veg && !rob(t).FName == "Corpse" && !(rob(t).FName == "Base.txt" && hidepred)) {
//calculate cumelative genetic distance
          for(t2=1; t2<MaxRobs; t2++) {
            if (t != t2) {
              if (rob(t2).exist && !rob(t2).Corpse && rob(t2).FName == rob(t).FName) { // Must exist, and be of same species
                maxgdi[t] = maxgdi[t] + DoGeneticDistance(ref t, ref t2);
              }
            }
            Next(t2);
            Form1.GraphLab.Caption = "Calculating eco result: " + Int(t / MaxRobs * 100) + "%";
            DoEvents();
          }
        }

        Form1.GraphLab.Visible = false;

//step3 calculate robots

        byte ecocount = 0;


        for(ecocount=1; ecocount<15; ecocount++) {
          decimal sPopulation = 0;

          decimal sEnergy = 0;

          sEnergy = (IIf(intFindBestV2 > 100, 100, intFindBestV2)) / 100;
          sPopulation = (IIf(intFindBestV2 < 100, 100, 200 - intFindBestV2)) / 100;

          decimal s = 0;

          decimal Mx = 0;

          int fit = 0;


          Mx = 0;
          fit = 0;
          for(t=1; t<MaxRobs; t++) {
            if (rob(t).exist && !rob(t).Veg && !rob(t).FName == "Corpse" && !(rob(t).FName == "Base.txt" && hidepred)) {
              Form1.TotalOffspring = 1;
              s = Form1.score(t, 1, 10, 0) + rob(t).nrg + rob(t).body * 10; //Botsareus 5/22/2013 Advanced fit test
              if (s < 0) {
                s = 0; //Botsareus 9/23/2016 Bug fix
              }
              s = (Form1.TotalOffspring ^ sPopulation) * (s ^ sEnergy);
              s = s * maxgdi[t];
              if (s >= Mx) {
                Mx = s;
                fit = t;
              }

            }
            Next(t);

//save and kill the robot
            if (dir(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount, vbDirectory) == "") {
              MkDir(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount);
            }
            salvarob(fit, MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.txt");
            rob(fit).exist = false;

          }

        }
        x_restartmode = 6;
      } else {
        logevo("Evolving robot never changed, increasing difficulty.");
//Increase mutation rates
        scale_mutations();
//Robot never mutated so we need to tighten up the difficulty
        Increase_Difficulty();
      }
      exportdata();
    }

public static void UpdateLostEvo() {
  logevo("Evolving robot lost, decreasing difficulty.");
  Decrease_Difficulty(); //Robot simply lost, se we need to loosen up the difficulty
  exportdata();
}

public static void UpdateWonF1() {
//figure out next opponent
  int currenttest = 0;

  y_Stgwins = y_Stgwins + 1;
  currenttest = x_filenumber - y_Stgwins * (x_filenumber ^ (1 / 3));
  if (currenttest < 0 || x_filenumber == 0 || y_eco_im > 0) { //check for x_filenumber is zero here to prevent endless loop
    logevo("Evolving robot won all tests, setting up stage " + (x_filenumber + 1));
    Next_Stage(); //Robot won, go to next stage
    x_restartmode = 4;
  } else {
//copy a robot for current test
    logevo("Robot is currently under test against stage " + currenttest);
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + currenttest + ".txt", MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
  }
  exportdata();
}

public static void UpdateLostF1() {
  logevo("Evolving robot lost the test, increasing difficulty.");

  if (y_eco_im > 0) {
    byte ecocount = 0;

    for(ecocount=1; ecocount<15; ecocount++) {
      RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount);
    }
  } else {
    File.Delete(MDIForm1.MainDir + "\\evolution\\Test.txt");;
    File.Delete(MDIForm1.MainDir + "\\evolution\\Test.mrate");;
//reset base robot
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt", MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
    y_Stgwins = 0;

  }
  x_restartmode = 4;
  Increase_Difficulty(); //Robot lost, might as well have never mutated
  exportdata();
}

public static void logevo(string s, ref int Index) {
  VBOpenFile(1, MDIForm1.MainDir + "\\evolution\\log" + IIf(Index > -1, Index, "") + ".txt");;
  VBWriteFile(1, s + " " + Date$ + " " + Time$);;
  VBCloseFile(1);();
}

/*
' * * * * * * * * * * * * * * * * * * *
' Zerobot - Botsareus 4/14/2014
' * * * * * * * * * * * * * * * * * * *
*/
private static void ZBreadyforTest(int bestrob) {
  salvarob(bestrob, MDIForm1.instance.MainDir + "\\evolution\\Test.txt");
//the robot did evolve, so lets update
  x_filenumber = x_filenumber + 1;
  FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt");
  FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".mrate");

  int ecocount = 0;

  int lowestindex = 0;

  int dbn = 0;


//what is our lowest index?
  lowestindex = x_filenumber - 7;
  if (lowestindex < 0) {
    lowestindex = 0;
  }

  logevo("Progress.");
  for(ecocount=1; ecocount<8; ecocount++) {
//calculate index and copy robots
    dbn = lowestindex + (ecocount - 1) % (x_filenumber + 1);
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + dbn + ".txt", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
    if (dir(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + dbn + ".mrate") != "") {
      FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + dbn + ".mrate", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");
    }
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + dbn + ".txt", MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt");
  }

  x_restartmode = 9;
  SimOpts.TotRunCycle = 8001; //make sure we skip the message
//restart now
  VBOpenFile(1, App.path + "\\restartmode.gset");;
  Write(#1, x_restartmode);
  Write(#1, x_filenumber);
  VBCloseFile(1);();
//Restart
  DisplayActivations = false;
  Form1.Active = false;
  Form1.SecTimer.Enabled = false;
  VBOpenFile(1, App.path + "\\Safemode.gset");;
  Write(#1, false);
  VBCloseFile(1);();
  VBOpenFile(1, App.path + "\\autosaved.gset");;
  Write(#1, false);
  VBCloseFile(1);();
  Call(restarter());
}

public static void ZBpassedtest() {
  MsgBox("Zerobot evolution complete.", vbInformation, "Zerobot evo");
  DisplayActivations = false;
  Form1.Active = false;
  Form1.SecTimer.Enabled = false;
}

public static void ZBfailedtest() {
  logevo("Zerobot failed the test, evolving further.");
  File.Delete(MDIForm1.MainDir + "\\evolution\\Test.txt");;
  File.Delete(MDIForm1.MainDir + "\\evolution\\Test.mrate");;
  x_restartmode = 7;
//restart now
  VBOpenFile(1, App.path + "\\restartmode.gset");;
  Write(#1, x_restartmode);
  Write(#1, x_filenumber);
  VBCloseFile(1);();
//Botsareus 10/9/2016 If runs out of stages restart everything
  if (x_filenumber > 500) {
//erase the folder
    RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution");

//make folder again
    RecursiveMkDir(MDIForm1.instance.MainDir + "\\evolution");
    RecursiveMkDir(MDIForm1.instance.MainDir + "\\evolution\\stages");

//populate folder init
    byte ecocount = 0;

    for(ecocount=1; ecocount<8; ecocount++) {
//generate folders for multi
      MkDir(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount);
      MkDir(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount);
//generate the zb file (multi)
      VBOpenFile(1, MDIForm1.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt");;
      int zerocount = 0;

      for(zerocount=1; zerocount<y_zblen; zerocount++) {
        Write(#1, 0);
      }
      VBCloseFile(1);();
      FileCopy(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
    }

//Botsareus 10/22/2015 the stages are singuler
    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\baserob1\\Base.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage0.txt");

//restart
    VBOpenFile(1, App.path + "\\restartmode.gset");;
    Write(#1, 7);
    Write(#1, 0);
    VBCloseFile(1);();

  }
//Restart
  DisplayActivations = false;
  Form1.Active = false;
  Form1.SecTimer.Enabled = false;
  VBOpenFile(1, App.path + "\\Safemode.gset");;
  Write(#1, false);
  VBCloseFile(1);();
  VBOpenFile(1, App.path + "\\autosaved.gset");;
  Write(#1, false);
  VBCloseFile(1);();
  Call(restarter());
}

public static void calculateZB(int robid, decimal Mx, int bestrob) {
  if (rob(bestrob).LastMut > 0) {
    Static(oldid(As(Long)));
    Static(oldMx(As(Double)));

    int MratesMax = 0;//used to correct out of range mutations

    MratesMax = IIf(NormMut, CLng(rob(bestrob).DnaLen) * CLng(valMaxNormMut), 2000000000);

    bool goodtest = false;//no duplicate message


    if (oldid != robid && oldid != 0) {
      logevo("'GoodTest' reason: oldid(" + oldid + ") comp. id(" + robid + ")", x_filenumber);
      dynamic _WithVar_1835;//robot is doing well, why not?
      _WithVar_1835 = rob(bestrob);
        _WithVar_1835.Mutables.mutarray(PointUP) = _WithVar_1835.Mutables.mutarray(PointUP) * 1.15m;
        if (_WithVar_1835.Mutables.mutarray(PointUP) > MratesMax) {
          _WithVar_1835.Mutables.mutarray(PointUP) = MratesMax;
        }
        _WithVar_1835.Mutables.mutarray(P2UP) = _WithVar_1835.Mutables.mutarray(P2UP) * 1.15m;
        if (_WithVar_1835.Mutables.mutarray(P2UP) > MratesMax) {
          _WithVar_1835.Mutables.mutarray(P2UP) = MratesMax;
        }
      goodtest = true;
    }


    if (oldid == robid && Mx > oldMx) {
      dynamic _WithVar_7704;//robot is doing well, why not?
      _WithVar_7704 = rob(bestrob);
        _WithVar_7704.Mutables.mutarray(PointUP) = _WithVar_7704.Mutables.mutarray(PointUP) * 1.75m;
        if (_WithVar_7704.Mutables.mutarray(PointUP) > MratesMax) {
          _WithVar_7704.Mutables.mutarray(PointUP) = MratesMax;
        }
        _WithVar_7704.Mutables.mutarray(P2UP) = _WithVar_7704.Mutables.mutarray(P2UP) * 1.75m;
        if (_WithVar_7704.Mutables.mutarray(P2UP) > MratesMax) {
          _WithVar_7704.Mutables.mutarray(P2UP) = MratesMax;
        }
      ZBreadyforTest(bestrob);
    } else {
      if (!goodtest) {
        logevo("'Reset' reason: oldid(" + oldid + ") comp. id(" + robid + ") Mx(" + Mx + ") comp. oldMx(" + oldMx + ")", x_filenumber);
      }
    }

    oldMx = Mx;
    oldid = robid;
  } else { //if robot did not mutate
    logevo("'Reset' reason: No mutations", x_filenumber);
  }
}

/*
'Cleaner but still uses global vars:
*/
public static decimal calc_exact_handycap() {
  decimal calc_exact_handycap = 0;
  calc_exact_handycap = energydifXP - energydifXP2;
  return calc_exact_handycap;
}

public static decimal calc_handycap() {
  decimal calc_handycap = 0;
  if (SimOpts.TotRunCycle < (CLng(hidePredCycl) * CLng(8))) {
    calc_handycap = calc_exact_handycap() * SimOpts.TotRunCycle / (CLng(hidePredCycl) * CLng(8));
  } else {
    calc_handycap = calc_exact_handycap();
  }
  return calc_handycap;
}
}
