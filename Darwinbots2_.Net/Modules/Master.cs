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


static class Master {
// Option Explicit
public static int DynamicCountdown = 0;// Used to countdown the cycles until we modify the dynamic costs
public static bool CostsWereZeroed = false;// Flag used to indicate to the reinstatement threshodl that the costs were zeroed
public static List<int> PopulationLast10Cycles = new List<int> (new int[11]);
[DllImport("user32.dll")] public static extern int GetAsyncKeyState(int vKey); //Botsareus 12/11/2012 Pause Break Key to Pause code
public static decimal energydif = 0;//Total last hide pred
public static decimal energydifX = 0;//Avg last hide pred
public static decimal energydifXP = 0;//The actual handycap
public static decimal energydif2 = 0;//Total last hide pred
public static decimal energydifX2 = 0;//Avg last hide pred
public static decimal energydifXP2 = 0;//The actual handycap
public static bool stopflag = false;
public static bool savenow = false;
public static bool stagnent = false;


public static void UpdateSim() {
//core evo
  decimal avrnrgStart = 0;

  decimal avrnrgEnd = 0;


  decimal AmountOff = 0;

  decimal UpperRange = 0;

  decimal LowerRange = 0;

  decimal CorrectionAmount = 0;

  int CurrentPopulation = 0;

  int AllChlr = 0;//Panda 8/13/2013 The new way to figure out total number vegys

  int i = 0;

  int t = 0;


  int Base_count = 0;

  int Mutate_count = 0;


//Botsareus 12/11/2012 Pause Break Key to Pause code
  if (GetAsyncKeyState(vbKeyF12)) {
    DisplayActivations = false;
    Form1.Active = false;
    Form1.SecTimer.Enabled = false;
    MDIForm1.instance.unpause.IsEnabled = true;
  }

  ModeChangeCycles = ModeChangeCycles + 1;
  SimOpts.TotRunCycle = SimOpts.TotRunCycle + 1;

//Botsareus 3/22/2014 Main hidepred logic (hide pred means hide base robot a.k.a. Predator)
  bool usehidepred = false;

  usehidepred = x_restartmode == 4 || x_restartmode == 5; //Botsareus expend to evo mode

//Dim avgsize As Long
  int k = 0;//robots moved last attempt

  int k2 = 0;//robots moved total

  decimal ingdist = 0;

  vector pozdif = null;

  vector newpoz = null;

  vector posdif = null;


  if (usehidepred) {
//Count species for end of evo
    Base_count = 0;
    Mutate_count = 0;
    for(t=1; t<MaxRobs; t++) {
      if (rob(t).exist) {
        if (rob(t).FName == "Base.txt") {
          Base_count = Base_count + 1;
        }
        if (rob(t).FName == "Mutate.txt") {
          Mutate_count = Mutate_count + 1;
        }
      }
      Next(t);
      if (Base_count > Mutate_count) {
        stagnent = false; //Botsareus 10/20/2015 Base went above mutate, reset stagnent flag
      }
//See if end of evo
      if (Mutate_count == 0) {
        DisplayActivations = false;
        Form1.Active = false;
        Form1.SecTimer.Enabled = false;
        stopflag = true; //Botsareus 9/2/2014 A bug fix from Spork22
        UpdateLostEvo();
      }
      if (Base_count == 0& !stopflag) {
        DisplayActivations = false;
        Form1.Active = false;
        Form1.SecTimer.Enabled = false;
        UpdateWonEvo(Form1.fittest);
      }
//Botsareus 10/19/2015 Prevents simulation from running too long
      if (SimOpts.TotRunCycle == 1000000) {
        stagnent = true; //Set the stagnent flag now and see what happens
      }
//    If SimOpts.TotRunCycle = 3000000 And stagnent Then 'Botsareus 1/9/2016 Rule no longer required as I am no longer evolving plants
//        DisplayActivations = False
//        Form1.Active = False
//        Form1.SecTimer.Enabled = False
//        UpdateWonEvo Form1.fittest
//    End If

Mode:
      if (ModeChangeCycles > (hidePredCycl / 1.2m + hidePredOffset)) {
//Botsareus 11/5/2015 If lfor max lower limit wait for mutate pop to match base pop
        if (LFOR == 150& Mutate_count < Base_count && hidepred) {
          ModeChangeCycles = ModeChangeCycles - 100;
goto ;
        }
//calculate new energy handycap
        energydif2 = energydif2 + energydif / ModeChangeCycles; //inverse handycap
        if (hidepred) {
          decimal holdXP = 0;

          holdXP = (energydifX - (energydif / ModeChangeCycles)) / LFOR;
          if (holdXP < energydifXP) {
            energydifXP = holdXP;
          } else {
            energydifXP = (energydifXP * 9 + holdXP) / 10;
          }

//inverse handycap
          energydifXP2 = (energydifX2 - energydif2) / LFOR;
          if (energydifXP2 > 0) {
            energydifXP2 = 0;
          }
          if ((energydifXP - energydifXP2) > 0.1m) {
            energydifXP2 = energydifXP - 0.1m;
          }
          energydifX2 = energydif2;
          energydif2 = 0;
        }
        energydifX = energydif / ModeChangeCycles;
        energydif = 0;
//Botsareus 6/12/2016 An attempt to get rid of 'chasers' without using any reposition code:
        if (hidepred) {
//Erase offensive shots
          for(t=1; t<maxshotarray; t++) {
            dynamic _WithVar_8466;
            _WithVar_8466 = Shots(t);
              if (_WithVar_8466.shottype == -1 || _WithVar_8466.shottype == -6) {
                _WithVar_8466.exist = false;
                _WithVar_8466.flash = false;
              }
            Next(t);

//Reposition robots the safe way
            k2 = 0;
            do {
              k = 0;
              for(t=1; t<MaxRobs; t++) {
                if (rob(t).exist && rob(t).FName == "Base.txt") {
                  for(i=1; i<MaxRobs; i++) {
                    if (rob[i].exist && rob[i].FName == "Mutate.txt") {
//calculate ingagment distance
                      if (rob(t).body > rob[i].body) {
                        if (rob(t).body > 10) {
                          ingdist = Log(rob(t).body) * 60 + 41;
                        } else {
                          ingdist = 40;
                        }
                      } else {
                        if (rob[i].body > 10) {
                          ingdist = Log(rob[i].body) * 60 + 41;
                        } else {
                          ingdist = 40;
                        }
                      }
                      ingdist = rob(t).radius + rob[i].radius + ingdist + 40; //both radii plus shot dist plus offset 1 shot travel dist

                      posdif = VectorSub(ref rob(t).pos, ref rob[i].pos);
                      if (VectorMagnitude(ref posdif) < ingdist) {
//if the distance between the robots is less then ingagment distance
                        dynamic _WithVar_5785;
                        _WithVar_5785 = rob[i];
                          ingdist = ingdist - VectorMagnitude(ref posdif); //ingdist becomes offset dist
                          newpoz = VectorSub(ref _WithVar_5785.pos, ref VectorScalar(ref VectorUnit(ref posdif), ref ingdist)); //offset the multibot by ingagment distance

                          pozdif.X = newpoz.X - _WithVar_5785.pos.X;
                          pozdif.Y = newpoz.Y - _WithVar_5785.pos.Y;
                          if (_WithVar_5785.numties > 0) {
                            List<int> clist = new List<int> (new int[51]);
                            List<int> tk = new List<int> (new int[51]);

                            clist[0] = i;
                            ListCells(clist[]);
//move multibot
                            tk = 1;
                            While(clist[tk] > 0);
//Botsareus 7/15/2016 Only own species
                            if (rob(clist(tk)).FName == "Mutate.txt") {
                              rob(clist(tk)).pos.X = rob(clist(tk)).pos.X + pozdif.X;
                              rob(clist(tk)).pos.Y = rob(clist(tk)).pos.Y + pozdif.Y;
                            }
                            tk = tk + 1;
                            Wend();
                          }
                          _WithVar_5785.pos.X = _WithVar_5785.pos.X + pozdif.X;
                          _WithVar_5785.pos.Y = _WithVar_5785.pos.Y + pozdif.Y;
                        k = k + 1;
                        k2 = k2 + 1;
                      }
                    }
                  }
                }
                Next(t);
              } while(!(k == 0 || k2 > (3200 + Mutate_count * 0.9m)); //Scales as mutate_count scales

            }
//change hide pred
            hidepred = !hidepred;
            hidePredOffset = hidePredCycl / 3 * rndy();
            ModeChangeCycles = 0;
          }
        }

//provides the mutation rates oscillation Botsareus 8/3/2013 moved to UpdateSim)
        int fullrange = 0;

        if (SimOpts.MutOscill) { //Botsareus 10/8/2015 Yet another redo, sine wave optional
          dynamic _WithVar_7834;
          _WithVar_7834 = SimOpts;

            if ((_WithVar_7834.MutCycMax + _WithVar_7834.MutCycMin) > 0) {
              if (_WithVar_7834.MutOscillSine) {
                fullrange = _WithVar_7834.TotRunCycle % (_WithVar_7834.MutCycMax + _WithVar_7834.MutCycMin);
                if (fullrange < _WithVar_7834.MutCycMax) {
                  _WithVar_7834.MutCurrMult = 20 ^ Sin(fullrange / _WithVar_7834.MutCycMax * PI);
                } else {
                  _WithVar_7834.MutCurrMult = 20 ^ (Sin((fullrange - _WithVar_7834.MutCycMax) / _WithVar_7834.MutCycMin * PI,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -);
                }

              } else {
                fullrange = _WithVar_7834.TotRunCycle % (_WithVar_7834.MutCycMax + _WithVar_7834.MutCycMin);
                if (fullrange < _WithVar_7834.MutCycMax) {
                  _WithVar_7834.MutCurrMult = 16;
                } else {
                  _WithVar_7834.MutCurrMult = 1 / 16;
                }

              }

            }

        }


        TotalSimEnergyDisplayed = TotalSimEnergy(CurrentEnergyCycle);
        CurrentEnergyCycle = SimOpts.TotRunCycle % 100;
        TotalSimEnergy(CurrentEnergyCycle) = 0;

        CurrentPopulation = totnvegsDisplayed;
        if (SimOpts.Costs(DYNAMICCOSTINCLUDEPLANTS) != 0) {
          CurrentPopulation = CurrentPopulation + totvegsDisplayed; //Include Plants in target population
        }

//If (SimOpts.TotRunCycle + 200) Mod 2000 = 0 Then MsgBox "sup" & SimOpts.TotRunCycle 'debug only

        if (SimOpts.TotRunCycle % 10 == 0) {
          for(i=10; i<2 Step -1; i++) {
            PopulationLast10Cycles(i) = PopulationLast10Cycles(i - 1);
            Next(i);
            PopulationLast10Cycles(1) = CurrentPopulation;
          }

          if (SimOpts.Costs(USEDYNAMICCOSTS)) {
            AmountOff = CurrentPopulation - SimOpts.Costs(DYNAMICCOSTTARGET);

//If we are more than X% off of our target population either way AND the population isn't moving in the
//the direction we want or hasn't moved at all in the past 10 cycles then adjust the cost multiplier
            UpperRange = TmpOpts.Costs(DYNAMICCOSTTARGETUPPERRANGE) * 0.01m * SimOpts.Costs(DYNAMICCOSTTARGET);
            LowerRange = TmpOpts.Costs(DYNAMICCOSTTARGETLOWERRANGE) * 0.01m * SimOpts.Costs(DYNAMICCOSTTARGET);
            if ((CurrentPopulation == PopulationLast10Cycles(10))) {
              DynamicCountdown = DynamicCountdown - 1;
              if (DynamicCountdown < -10) {
                DynamicCountdown = -10;
              }
            } else {
              DynamicCountdown = 10;
            }

            if ((AmountOff > UpperRange && (PopulationLast10Cycles(10) < CurrentPopulation || DynamicCountdown <= 0)) Or(AmountOff < -LowerRange && (PopulationLast10Cycles(10) > CurrentPopulation || DynamicCountdown <= 0))) {
              if (AmountOff > UpperRange) {
                CorrectionAmount = AmountOff - UpperRange;
              } else {
                CorrectionAmount = Abs(AmountOff) - LowerRange;
              }

//Adjust the multiplier. The idea is to rachet this over time as bots evolve to be more effecient.
//We don't muck with it if the bots are within X% of the target.  If they are outside the target, then
//we adjust only if the populatiuon isn't heading towards the range and then we do it my an amount that is a function
//of how far out of the range we are (not how far from the target itself) and the sensitivity set in the sim
              SimOpts.Costs(COSTMULTIPLIER) = SimOpts.Costs(COSTMULTIPLIER) + (0.0000001m * CorrectionAmount * Sgn(AmountOff) * SimOpts.Costs(DYNAMICCOSTSENSITIVITY));

//Don't let the costs go negative if the user doesn't want them to
              if ((SimOpts.Costs(ALLOWNEGATIVECOSTX) != 1)) {
                if (SimOpts.Costs(COSTMULTIPLIER) < 0) {
                  SimOpts.Costs(COSTMULTIPLIER) = 0;
                }
              }
              DynamicCountdown = 10; // Reset the countdown timer
            }
// Else
//   SimOpts.Costs(COSTMULTIPLIER) = 1
          }

          if ((CurrentPopulation < SimOpts.Costs(BOTNOCOSTLEVEL)) && (SimOpts.Costs(COSTMULTIPLIER) != 0)) {
            CostsWereZeroed = true;
            SimOpts.oldCostX = SimOpts.Costs(COSTMULTIPLIER);
            SimOpts.Costs(COSTMULTIPLIER) = 0; // The population has fallen below the threshold to 0 all costs
          } else if ((CurrentPopulation > SimOpts.Costs(COSTXREINSTATEMENTLEVEL)) && CostsWereZeroed) {
            CostsWereZeroed = false; // Set the flag so we don't do this again unless they get zeored again
            SimOpts.Costs(COSTMULTIPLIER) = SimOpts.oldCostX;
          }

//Store new energy handycap
          for(t=1; t<MaxRobs; t++) {
            if (rob(t).exist) {
              if (rob(t).FName == "Mutate.txt" && hidepred) {
                if (rob(t).LastMut > 0) { //4/5/2016 Handycap freshly mutated robots more than other robots
                  rob(t).nrg = rob(t).nrg + calc_handycap();
                } else {
                  rob(t).nrg = rob(t).nrg + calc_handycap() / 2;
                }
              }
            }
            Next(t);

            if (usehidepred) {
//Calculate average energy before sim update
              avrnrgStart = 0;
              i = 0;
              for(t=1; t<MaxRobs; t++) {
                if (rob(t).FName == "Mutate.txt" && rob(t).exist) {
                  if (rob(t).LastMut > 0) { //4/17/2014 New rule from Botsareus, only handycap fresh robots
                    i = i + 1;
                    avrnrgStart = avrnrgStart + rob(t).nrg;
                  }
                }
                Next(t);
                if (i > 0) {
                  avrnrgStart = avrnrgStart / i;
                }
              }

              ExecRobs();
              if (datirob.Visible && datirob.ShowMemoryEarlyCycle) {
                dynamic _WithVar_5883;
                _WithVar_5883 = rob(robfocus);
                  datirob.infoupdate(robfocus, _WithVar_5883.nrg, _WithVar_5883.parent, _WithVar_5883.Mutations, _WithVar_5883.age, _WithVar_5883.SonNumber, 1, _WithVar_5883.FName, _WithVar_5883.genenum, _WithVar_5883.LastMut, _WithVar_5883.generation, _WithVar_5883.DnaLen, _WithVar_5883.LastOwner, _WithVar_5883.Waste, _WithVar_5883.body, _WithVar_5883.mass, _WithVar_5883.venom, _WithVar_5883.shell, _WithVar_5883.Slime, _WithVar_5883.chloroplasts);
              }

//updateshots can write to bot sense, so we need to clear bot senses before updating shots
              for(t=1; t<MaxRobs; t++) {
                if (rob(t).exist) {
                  if ((rob(t).DisableDNA == false)) {
                    EraseSenses(t);
                  }
                }
                Next(t);

//it is time for some overwrites by playerbot mode
                if (MDIForm1.instance.pbOn.Checked) {
                  for(t=1; t<MaxRobs; t++) {
                    dynamic _WithVar_5374;
                    _WithVar_5374 = rob(t);
                      if (_WithVar_5374.exist) {
                        if (t == robfocus || _WithVar_5374.highlight) {
                          if (!(Mouse_loc.X == 0& Mouse_loc.Y == 0)) {
                            _WithVar_5374.mem(SetAim) = angnorm(angle(_WithVar_5374.pos.X, _WithVar_5374.pos.Y, Mouse_loc.X, Mouse_loc.Y)) * 200;
                          }
                          for(i=1; i<UBound(PB_keys); i++) {
                            if (PB_keys(i).Active != PB_keys(i).Invert) {
                              _WithVar_5374.mem(PB_keys(i).memloc) = PB_keys(i).value;
                            }
                          }
                        }
                      }
                    Next(t);
                  }

                  updateshots();

//Botsareus 6/22/2016 to figure out actual velocity of the bot incase there is a collision event
                  for(t=1; t<MaxRobs; t++) {
                    if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred)) {
                      rob(t).opos = rob(t).pos;
                    }
                  }

                  UpdateBots();

//to figure out actual velocity of the bot incase there is a collision event
                  for(t=1; t<MaxRobs; t++) {
                    if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred)) {
//Only if the robots position was already configured
                      if (!(rob(t).opos.X == 0& rob(t).opos.Y == 0)) {
                        rob(t).actvel = VectorSub(ref rob(t).pos, ref rob(t).opos);
                      }
                    }
                  }

                  if (numObstacles > 0) {
                    MoveObstacles();
                  }
                  if (numTeleporters > 0) {
                    UpdateTeleporters();
                  }

                  for(t=1; t<MaxRobs; t++) { //Panda 8/14/2013 to figure out how much vegys to repopulate across all robots
                    if (rob(t).exist && !(rob(t).FName == "Base.txt"& hidepred)) { //Botsareus 8/14/2013 We have to make sure the robot is alive first
                      AllChlr = AllChlr + rob(t).chloroplasts;
                    }
                    Next(t);

                    TotalChlr = AllChlr / 16000; //Panda 8/23/2013 Calculate total unit chloroplasts

                    if (TotalChlr < CLng(SimOpts.MinVegs)) { //Panda 8/23/2013 Only repopulate vegs when total chlroplasts below value
                      if (totvegsDisplayed != -1) {
                        VegsRepopulate(); //Will be -1 first cycle after loading a sim.  Prevents spikes.
                      }
                    }

                    feedvegs(SimOpts.MaxEnergy);

                    if (usehidepred) {
//Calculate average energy after sim update
                      avrnrgEnd = 0;
                      i = 0;
                      for(t=1; t<MaxRobs; t++) {
                        if (rob(t).FName == "Mutate.txt" && rob(t).exist) {
                          if (rob(t).LastMut > 0) { //4/17/2014 New rule from Botsareus, only handycap fresh robots
                            i = i + 1;
                            avrnrgEnd = avrnrgEnd + rob(t).nrg;
                          }
                        }
                        Next(t);
                        if (i > 0) {
                          avrnrgEnd = avrnrgEnd / i;
                          energydif = energydif - avrnrgStart + avrnrgEnd;
                        }
                      }

//okay, time to store some values for RGB monitor
                      if (MDIForm1.instance.MonitorOn.DefaultProperty) {
                        for(t=1; t<MaxRobs; t++) {
                          if (rob(t).exist) {
                            dynamic _WithVar_5581;
                            _WithVar_5581 = frmMonitorSet.instance;
                              rob(t).monitor_r = rob(t).mem(_WithVar_5581.Monitor_mem_r);
                              rob(t).monitor_g = rob(t).mem(_WithVar_5581.Monitor_mem_g);
                              rob(t).monitor_b = rob(t).mem(_WithVar_5581.Monitor_mem_b);
                          }
                          Next(t);
                        }

//Kill some robots to prevent out of memory
                        int totlen = 0;

                        totlen = 0;
                        for(t=1; t<MaxRobs; t++) {
                          if (rob(t).exist) {
                            totlen = totlen + rob(t).DnaLen;
//        On Error GoTo b:     'Botsareus 10/5/2015 Replaced with something better
//        For i = 0 To UBound(rob(t).delgenes) 'Botsareus 9/16/2014 More overflow prevention stuff
//         totlen = totlen + UBound(rob(t).delgenes(i).dna)
//        Next
//b:
                          }
                          Next(t);
                          if (totlen > 4000000) {
                            decimal calcminenergy = 0;

                            int selectrobot = 0;

                            int maxdel = 0;


                            maxdel = 1500 * (CLng(TotalRobotsDisplayed) * 425 / totlen);

                            for(i=0; i<maxdel; i++) {
                              calcminenergy = 320000; //only erase robots with lowest energy
                              for(t=1; t<MaxRobs; t++) {
                                if (rob(t).exist) {
                                  if ((rob(t).nrg + rob(t).body * 10) < calcminenergy) {
                                    calcminenergy = (rob(t).nrg + rob(t).body * 10);
                                    selectrobot = t;
                                  }
                                }
                                Next(t);
                                Call(KillRobot(ref selectrobot));
                                Next(i);
                              }
                              if (totlen > 3000000) {
                                for(t=1; t<MaxRobs; t++) {
                                  rob(t).LastMutDetail = "";
                                  Next(t);
                                }

//Botsareus 5/6/2013 The safemode system
                                if (UseSafeMode) { //special modes does not apply, may need to expended to other restart modes
                                  if (IIf(UseIntRnd, savenow, SimOpts.TotRunCycle % 2000 == 0& SimOpts.TotRunCycle > 0)) { //Botsareus 10/19/2015 Safe mode uses different logic under use internet as randomizer
                                    if (x_restartmode == 0 || x_restartmode == 4 || x_restartmode == 5 || x_restartmode == 7 || x_restartmode == 8) { //Botsareus 10/5/2015 restartmodes 1, 2, 3, 6 and 9 are test only and do not need autosaves
                                      SaveSimulation(MDIForm1.instance.MainDir + "\\saves\\lastautosave.sim");
//Botsareus 5/13/2013 delete local copy
                                      if (dir(MDIForm1.instance.MainDir + "\\saves\\localcopy.sim") != "") {
                                        File.Delete((MDIForm1.MainDir + "\\saves\\localcopy.sim"));;
                                      }
                                      VBOpenFile(1, App.path + "\\autosaved.gset");;
                                      Write(#1, true);
                                      VBCloseFile(1);();
                                      savenow = false;
                                    }
                                  }
                                }

//R E S T A R T  N E X T
//Botsareus 1/31/2014 seeding
                                if (x_restartmode == 1) {
                                  if (SimOpts.TotRunCycle == 2000) {
                                    FileCopy(MDIForm1.instance.MainDir + "\\league\\Test.txt", NamefileRecursive(MDIForm1.instance.MainDir + "\\league\\seeded\\" + totnvegsDisplayed + ".txt"));
                                    VBOpenFile(1, App.path + "\\restartmode.gset");;
                                    Write(#1, x_restartmode);
                                    Write(#1, x_filenumber);
                                    VBCloseFile(1);();
                                    VBOpenFile(1, App.path + "\\Safemode.gset");;
                                    Write(#1, false);
                                    VBCloseFile(1);();
                                    Call(restarter());
                                  }
                                }

//Z E R O B O T
//evo mode
                                if (x_restartmode == 7 || x_restartmode == 8) {
                                  if (SimOpts.TotRunCycle % 50 == 0& SimOpts.TotRunCycle > 0) {
                                    Form1.fittest();
                                  }
                                  Mutate_count = 0;
//Botsareus 10/192015 count robots to see if time to restart zb evo
                                  for(t=1; t<MaxRobs; t++) {
                                    if (rob(t).exist) {
                                      if (rob(t).FName == "Mutate.txt") {
                                        Mutate_count = Mutate_count + 1;
                                      }
                                    }
                                    Next(t);
                                    if (Mutate_count == 0) {
//Restart
                                      logevo("A restart is needed.");

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
                                  }

                                  Static(totnrgnvegs(As(Double)));
                                  decimal cmptotnrgnvegs = 0;


//test mode
                                  if (x_restartmode == 9) {
                                    if (SimOpts.TotRunCycle == 1) { //record starting energy
                                      for(t=1; t<MaxRobs; t++) {
                                        if (rob(t).exist) {
                                          if (rob(t).FName == "Test.txt") {
                                            totnrgnvegs = totnrgnvegs + rob(t).nrg + rob(t).body * 10;
                                          }
                                        }
                                        Next(t);
                                      }
                                      if (SimOpts.TotRunCycle == 8000) { //ending energy must be more
                                        for(t=1; t<MaxRobs; t++) {
                                          if (rob(t).exist) {
                                            if (rob(t).FName == "Test.txt") {
                                              cmptotnrgnvegs = cmptotnrgnvegs + rob(t).nrg + rob(t).body * 10;
                                            }
                                          }
                                          Next(t);
                                          if (totnvegsDisplayed > 10& cmptotnrgnvegs > totnrgnvegs * 2) { //did population and energy x2?
                                            ZBpassedtest();
                                          } else {
                                            ZBfailedtest();
                                          }
                                        }
                                      }
                                    }
}
