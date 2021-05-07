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


static class Shots_Module {
// Option Explicit
// shot structure definition
// exists?
// position vector
// old position vector
// velocity vector
// who shot it?
// rob age
// energy carrier
// shot range (the maximum .nrg ever was)
// power of shot for negative shots (or amt of shot, etc.), value to write for > 0
// colour
// carried location/value couple
// does shot come from veg?
// Which species fired the shot
// Memory location for custom poison and venom
// Value to insert into custom venom location
// Somewhere to store genetic code for a virus or sperm
// length of DNA  stored on this shot
// which gene to copy in host bot
// for virus shots (and maybe future types) this shot is stored
// inside the bot until it's ready to be launched
// For showing shot impacts
public class shot {
 public bool exist = false;
 public vector pos = null;
 public vector opos = null;
 public vector velocity = null;
 public int parent = 0;
 public int age = 0;
 public decimal nrg = 0;
 public decimal Range = 0;
 public int value = 0;
 public int color = 0;
 public int shottype = 0;
 public bool fromveg = false;
 public string FromSpecie = "";
 public int memloc = 0;
 public int Memval = 0;
 public block[] dna = new block[];
 public int DnaLen = 0;
 public int genenum = 0;
 public bool stored = false;
 public bool flash = false;
}
public static List<shot> Shots = new List<shot> {}; // TODO - Specified Minimum Array Boundary Not Supported: Public Shots() As shot// array of shots
public static int shotpointer = 0;// index into the Shots array used to find new slots for new shots
//Public maxshots As Integer
public static int numshots = 0;//Counter for tracking number of shots in the sim
public static int ShotsThisCycle = 0;// Shots this cycle.  Only updated at end of UpdateShots()
public static int maxshotarray = 0;
public const int shotdecay = 40; //increase to have shots lose power slower
public const int ShellEffectiveness = 20; //how strong each unit of shell is
public const decimal SlimeEffectiveness = 1 / 20; //how strong each unit of slime is against viruses 'Botsareus 10/5/2015 Virus more effective
public const int VenumEffectivenessVSShell = 25; //Botsareus 3/15/2013 Multiply strength of venum agenst shell
public const dynamic MinBotRadius = 0.2m; //A total hack.  Used to bypass checking the rest of the bots if the collision occurred during this
//intial fraction of the cycle.  We assume that no bot is small enough to possibly have been hit earlier
//in the cycle.  We risk not detecting collisions with tiny bots in the case where the shot hits it early
//in the cycle, but the perf benefit of skipping the rest of the bots is significant.
public static double MaxBotShotSeperation = 0;
public static List<int> FlashColor = new List<int> (new int[11]);// array of colors to use for flashing bots when they get shot
//   S H O T S   M A N A G E M E N T
// calculates the half brightness of a colour
// for a vaguely shiny effect in particles burst


private static int HBrite(int col) {
  int HBrite = 0;
  int b = 0;
  int g = 0;
  int r = 0;

  b = Int(col / 65536);
  col = col - (b * 65536);
  g = Int(col / 256);
  r = col - (g * 256);
  b = b / 2;
  g = g / 2;
  r = r / 2;
  HBrite = RGB(r, g, b);
  return HBrite;
}

/*
' same, but doubles
*/
public static int DBrite(int col) {
  int DBrite = 0;
  int b = 0;
  int g = 0;
  int r = 0;

  b = Int(col / 65536);
  col = col - (b * 65536);
  g = Int(col / 256);
  r = col - (g * 256);
  b = b + (255 - b) / 2;
  g = g + (255 - g) / 2;
  r = r + (255 - r) / 2;
  DBrite = RGB(r, g, b);
  return DBrite;
}

/*
' creates a shot shooted by robot n, with couple location/value
' returns the shot num of the shot
*/
public static int newshot(ref int n, int shottype, decimal val, ref decimal rngmultiplier, ref bool offset) {
  int newshot = 0;
  int a = 0;

  decimal ran = 0;

  vector angle = null;

  decimal ShAngle = 0;

  int X = 0;


//If IsArrayBounded(Shots) = False Then
//  ReDim Shots(300)
//  maxshotarray = 300
//End If

  a = FirstSlot;
  if (a > maxshotarray) {
    shotpointer = maxshotarray; // we know the array is full.  Set the pointer to the end so it will point to the free space
    maxshotarray = CLng(maxshotarray * 1.1m); // Increase the array by 10%
    List<> Shots_8293_tmp = new List<>();
for (int redim_iter_2846=0;i<0;redim_iter_2846++) {Shots.Add(redim_iter_2846<Shots.Count ? Shots(redim_iter_2846) : null);}
  }

  if (val > 32000) {
    val = 32000; // EricL March 16, 2006 This line moved here from below to catch val before assignment
  }
  Shots(a).exist = true;
  Shots(a).age = 0;
  Shots(a).parent = n;
  Shots(a).FromSpecie = rob[n].FName; //Which species fired the shot
  Shots(a).fromveg = rob[n].Veg; //does shot come from a veg or not?
  Shots(a).color = rob[n].color;
  Shots(a).value = Int(val);

  if ((shottype > 0) || (shottype == -100)) {
    Shots(a).shottype = shottype;
  } else {
    Shots(a).shottype = ((Abs(shottype) % 8,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +); // EricL 6/2006 essentially Mod 8 so as to increse probabiltiy that mutations do something interesting
    if (Shots(a).shottype == 0) {
      Shots(a).shottype = -8; // want multiples of -8 to be -8
    }
  }
  if (shottype == -2) {
    Shots(a).color = vbWhite;
  }
  Shots(a).memloc = rob[n].mem(835); //location for venom to target
//If Shots(a).memloc < 1 Then Shots(a).memloc = ((Shots(a).memloc - 1) Mod 1000) + 1 'Botsareus 10/6/2015 Normalized on reseaving side
//If Shots(a).memloc > 1000 Then Shots(a).memloc = ((Shots(a).memloc - 1) Mod 1000) + 1
  Shots(a).Memval = rob[n].mem(836); //value to insert into venom target location

//If val > 32000 Then val = 32000 'EricL March 16, 2006 This line commented out since moved to above
  ran = Random(ref -2, ref 2) / 20;

  if (rob[n].mem(backshot) == 0) {
    ShAngle = rob[n].aim; //forward shots
  } else {
    ShAngle = angnorm(rob[n].aim - PI); //backward shots
    rob[n].mem(backshot) = 0;
  }

  if (rob[n].mem(aimshoot) != 0) { //0 is the same as .shoot without any aiming
    rob[n].mem(aimshoot) = rob[n].mem(aimshoot) % 1256;

    ShAngle = (rob[n].aim - rob[n].mem(aimshoot) / 200);
    rob[n].mem(aimshoot) = 0;
  }

  ShAngle = ShAngle + Random(ref -20, ref 20) / 200;

  angle() = VectorSet(Cos(ShAngle), -Sin(ShAngle));
  Shots(a).pos = VectorAdd(ref rob[n].pos, ref VectorScalar(ref angle(), ref rob[n].radius));

//Botsareus 6/23/2016 Takes care of shot position bug - so it matches the painted robot position
  if (offset) {
    Shots(a).pos = VectorSub(ref Shots(a).pos, ref rob[n].vel);
    Shots(a).pos = VectorAdd(ref Shots(a).pos, ref rob[n].actvel);
  }


  Shots(a).velocity = VectorAdd(ref rob[n].actvel, ref VectorScalar(ref angle(), ref 40));

  Shots(a).opos = VectorSub(ref Shots(a).pos, ref Shots(a).velocity);

  if (rob[n].vbody > 10) {
    Shots(a).nrg = Log(Abs(rob[n].vbody)) * 60 * rngmultiplier;
    int temp = 0;

    temp = (Shots(a).nrg + 40 + 1)/40; //divides and rounds up
    Shots(a).Range = temp;
    Shots(a).nrg = temp * 40;
  } else {
    Shots(a).Range = rngmultiplier;
    Shots(a).nrg = 40 * rngmultiplier;
  }

//return the new shot
  newshot = a;

  if (shottype == -7) {
    Shots(a).color = vbCyan;
    Shots(a).genenum = val;
    Shots(a).stored = true;
    if (!copygene(ref a, Shots(a).genenum)) {
      Shots(a).exist = false;
      Shots(a).stored = false;
//Shots(a).flash = True
      newshot = -1;
    }
//Botsareus 3/14/2014 Disqualify
    if ((SimOpts.F1 || x_restartmode == 1) && (Disqualify == 1 || Disqualify == 2)) {
      dreason(rob[n].FName, rob[n].tag, "using a virus");
    }
    if (!SimOpts.F1 && rob[n].dq == 1 && (Disqualify == 1 || Disqualify == 2)) {
      rob[n].Dead = true; //safe kill robot
    }
  } else {
    Shots(a).stored = false;
  }

  if (shottype == -2) {
    Shots(a).nrg = val;
  }

// sperm shot
  if (shottype == -8) {
//ReDim Shots(a).DNA(rob[n].dnalen)
    Shots(a).dna = rob[n].dna;
    Shots(a).DnaLen = rob[n].DnaLen;
  }

  return newshot;
}

/*
' creates a generic particle with arbitrary x & y, vx & vy, etc
*/
public static void createshot(int X, int Y, int vx, int vy, ref int loc, ref int par, ref decimal val, ref decimal Range, ref int col) {
  int a = 0;


//If IsArrayBounded(Shots) = False Then
//  ReDim Shots(300)
//  maxshotarray = 300
//End If

  a = FirstSlot;
  if (a > maxshotarray) {
    shotpointer = maxshotarray; // we know the array is full.  Set the pointer to the end so it will point to the free space
    maxshotarray = CLng(maxshotarray * 1.1m); // Increase the array by 10%
    List<> Shots_8264_tmp = new List<>();
for (int redim_iter_4525=0;i<0;redim_iter_4525++) {Shots.Add(redim_iter_4525<Shots.Count ? Shots(redim_iter_4525) : null);}
  }
  Shots(a).parent = par;
  Shots(a).FromSpecie = rob(par).FName;
  Shots(a).fromveg = rob(par).Veg;

  Shots(a).pos.X = X; //+ vx
  Shots(a).pos.Y = Y; //+ vy
  Shots(a).velocity.X = vx;
  Shots(a).velocity.Y = vy;
  Shots(a).opos = VectorSub(ref Shots(a).pos, ref Shots(a).velocity);

  Shots(a).age = 0;
  Shots(a).color = col;
  Shots(a).exist = true;
  Shots(a).stored = false;
  Shots(a).DnaLen = 0;


  int temp = 0;

  temp = (Range + 40 + 1)/40; //divides and rounds up ie: range / (Robsize/3)

  Shots(a).nrg = Range + 40 + 1;
  if (val > 32000) {
    val = 32000; // Overflow protection
  }
  if (loc == -2) {
    Shots(a).nrg = val;
  }
  Shots(a).Range = temp;
  Shots(a).value = CInt(val);
  if (loc > 0 || loc == -100) {
    Shots(a).shottype = loc;
  } else {
    Shots(a).shottype = ((Abs(loc) % 8,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +); // EricL 6/2006 essentially Mod 8 so as to increse probabiltiy that mutations do something interesting
    if (Shots(a).shottype == 0) {
      Shots(a).shottype = -8; // want multiples of -8 to be -8
    }
  }
//  If rob(par).mem(834) <= 0 Then
//    Shots(a).memloc = 0
//  Else
//    Shots(a).memloc = rob(par).mem(834) Mod 1000
//    If Shots(a).memloc = 0 Then Shots(a).memloc = 1000
//  End If
  Shots(a).memloc = rob(par).mem(834); //Botsareus 10/6/2015 Normalized on reseaving side

  if (Shots(a).shottype == -5) {
    Shots(a).Memval = rob(Shots(a).parent).mem(839);
  }

}

/*
' searches some place to insert the new shot in the
' shots array.
*/
private static int FirstSlot() {
  int FirstSlot = 0;
  int counter = 0;


  counter = 1;

  While(Shots(shotpointer).exist);
  counter = counter + 1;
  shotpointer = shotpointer + 1;
  if (shotpointer > maxshotarray) {
    shotpointer = 1;
  }
  if (counter > maxshotarray) {
goto exitloop;
  }
  Wend();
exitloop:

  if (counter > maxshotarray) {
//maxshots = counter
//Ran off the end of the array.  Return the array size + 1 to indicate it needs needs to be redimed.
    FirstSlot = counter;
  } else {
    FirstSlot = shotpointer;
  }
  return FirstSlot;
}

/*
' calculates next shots position
*/
public static void updateshots() {
//moves shot then checks for collision
  int a = 0;

  int t = 0;

  int h = 0;

  int dx = 0;

  int sx = 0;

  int rp = 0;

  int jj = 0;

  decimal ti = 0;

  int X = 0;

  int Y = 0;

  decimal tempnum = 0;


// shotpointer = 1

  numshots = 0;
  for(t=1; t<maxshotarray; t++) {
//This is one of the most CPU intensive routines.  We need to make the UI responsive.
    if (t % 250 == 0) {
      DoEvents();
    }
    if (t <= maxshotarray) { //Botsareus 4/5/2016 Bug fix

      if (Shots(t).flash) {
        Shots(t).exist = false;
        Shots(t).flash = false;
        Shots(t).DnaLen = 0;
      }
      if (Shots(t).exist) {
        numshots = numshots + 1; // Counts the number of existing shots each cycle for display purposes

//Add the energy in the shot to the total sim energy if it is an energy shot
        if (Shots(t).shottype == -2) {
          TotalSimEnergy(CurrentEnergyCycle) = TotalSimEnergy(CurrentEnergyCycle) + Shots(t).nrg;
        }

        if ((Shots(t).shottype == -100) || (Shots(t).stored == true)) {
          h = 0; // It's purely an ornimental shot like a poff or it's a virus shot that hasn't been fired yet
        } else {
          h = NewShotCollision(t); // go off and check for collisions with bots.
        }

//babies born into a stream of shots from its parent shouldn't die
//from those shots.  I can't imagine this temporary imunity can be
//exploited, so it should be safe
        if (h > 0& !(Shots(t).parent == rob(h).parent && rob(h).age <= 1)) {
//EricL 4/19/2006 Divide by zero protection for cases where the shot range is zero
          if (Shots(t).Range == 0) {
            tempnum = Shots(t).age + 1; // / (.range + 1)
          } else {
            tempnum = Shots(t).age / Shots(t).Range;
          }

//this below is horribly complicated:  allow me to explain:
//nrg dissipates in a non-linear fashion.  Very little nrg disappears until you
//get near the last 10% of the journey or so.
//Don't dissipate nrg if nrg shots last forever.
          if (!SimOpts.NoShotDecay || Shots(t).shottype != -2) {
            if (!(Shots(t).shottype == -4 && SimOpts.NoWShotDecay)) { //Botsareus 9/29/2013 Do not decay waste shots
              Shots(t).nrg = Shots(t).nrg * (Atn(tempnum * shotdecay - shotdecay)) / Atn(-shotdecay);
            }
          }


          if (Shots(t).shottype > 0) {
//Botsareus 10/6/2015 Minor bug fixing and redundent code removal
            Shots(t).shottype = (Shots(t).shottype - 1) % 1000 + 1; // EricL 6/2006 Mod 1000 so as to increse probabiltiy that mutations do something interesting

            if (Shots(t).shottype != DelgeneSys) {
              if ((Shots(t).nrg / 2 > rob(h).poison) || (rob(h).poison == 0)) {
                rob(h).mem(Shots(t).shottype) = Shots(t).value;
              } else {
                createshot(Shots(t).pos.X, Shots(t).pos.Y, -Shots(t).velocity.X, -Shots(t).velocity.Y, -5, h, Shots(t).nrg / 2, Shots(t).Range * 40, vbYellow);
                rob(h).poison = rob(h).poison - (Shots(t).nrg / 2) * 0.9m;
                rob(h).Waste = rob(h).Waste + (Shots(t).nrg / 2) * 0.1m;
                if (rob(h).poison < 0) {
                  rob(h).poison = 0;
                }
                rob(h).mem(poison) = rob(h).poison;
              }

            }
          } else {
// Shots(t).shottype = -(Abs(Shots(t).shottype) Mod 8)  ' EricL 6/2006 essentially Mod 8 so as to increse probabiltiy that mutations do something interesting
// If Shots(t).shottype = 0 Then Shots(t).shottype = -8 ' want multiples of -8 to be -8
            switch(Shots(t).shottype) {
//Problem with this: returning nrg shots appear where the shot would have been instead of where
//it hit the bot - EricL 5/20/2006 - Not anymore as of 2.42.5!
              case -1:
                releasenrg(h, t);
                break;
              case -2:
                takenrg(h, t);
                break;
              case -3:
                takeven(h, t);
                break;
              case -4:
                takewaste(h, t);
                break;
              case -5:
                takepoison(h, t);
                break;
              case -6:
                releasebod(h, t);
                break;
              case -7:
                addgene(h, t);
                break;// bot hit by a sperm shot for sexual reproduction
              case -8:
                takesperm(h, t);
break;
}
          }
          taste(h, Shots(t).opos.X, Shots(t).opos.Y, Shots(t).shottype);
          Shots(t).flash = true;

        }
        if (numObstacles > 0) {
          DoShotObstacleCollisions(t);
        }
        Shots(t).opos = Shots(t).pos;
        Shots(t).pos = VectorAdd(ref Shots(t).pos, ref Shots(t).velocity); //Euler integration

//Age shots unless we are not decaying them.  At some point, we may want to see how old shots are, so
//this may need to be changed at some point but for now, it lets shots never die by never growing old.
//Always age Poff shots
        if ((SimOpts.NoShotDecay && Shots(t).shottype == -2) || (Shots(t).stored)) {
        } else {
          if (Shots(t).shottype == -4 && SimOpts.NoWShotDecay) {
          } else {
            Shots(t).age = Shots(t).age + 1;
          }
        }

        if (Shots(t).age > Shots(t).Range && !Shots(t).flash) { //Botsareus 9/12/2016 Bug fix
          Shots(t).exist = false; // Kill shots once they reach maturity
          Shots(t).DnaLen = 0;
        }

      }

    }
    Next(t);

// Here we test for sparsity of the shots array.  If the number of shots is less than 70% of the array size, then we
// compact the array and reset maxshotarray
    if ((numshots < (maxshotarray * 0.7m)) && (maxshotarray > 100)) {
      CompactShots();
      if (numshots < 90) {
        maxshotarray = CLng(100);
      } else {
        maxshotarray = CLng(numshots * 1.2m);
      }
      shotpointer = numshots; // set the shot pointer at the beginning of the free space in the newly shrunk array
      List<> Shots_1619_tmp = new List<>();
for (int redim_iter_9590=0;i<0;redim_iter_9590++) {Shots.Add(redim_iter_9590<Shots.Count ? Shots(redim_iter_9590) : null);}
    }
    ShotsThisCycle = numshots;
  }

public static void CompactShots() {
  int i = 0;

  int j = 0;

  int X = 0;


  j = 1;
  for(i=1; i<maxshotarray; i++) {
    if (Shots(i).exist) {
      if (Shots(i).stored) {
        if (rob(Shots(i).parent).exist && !(rob(Shots(i).parent).FName == "Base.txt" && hidepred)) {
          rob(Shots(i).parent).virusshot = j;
        } else {
          Shots(i).exist = false;
          Shots(i).stored = false;
          Shots(i).DnaLen = 0;
        }
      }
      if (i != j) {
        if ((Shots(j).shottype == -8 || Shots(j).shottype == -7) && Shots(i).DnaLen > 0) {
          List<> Shots_6408_tmp = new List<>();
for (int redim_iter_6087=0;i<0;redim_iter_6087++) {Shots.Add(null);}
        }
        Shots(j) = Shots(i);
        Shots(i).exist = false;
        Shots(i).stored = false;
        Shots(i).DnaLen = 0;
//ReDim Shots(i).DNA(1) ' 1 so as to not hit the bounded routine exception everytime
      }
      j = j + 1;
    }
    Next(i);
  }

public static void Decay(ref int n) { //corpse decaying as waste shot, energy shot or no shot
  int SH = 0;

  decimal va = 0;

  rob[n].DecayTimer = rob[n].DecayTimer + 1;
  if (rob[n].DecayTimer >= SimOpts.Decaydelay) {
    rob[n].DecayTimer = 0;

    rob[n].aim = rndy() * 2 * PI;
    rob[n].aimvector = VectorSet(Cos(rob[n].aim), Sin(rob[n].aim));

    if (rob[n].body > SimOpts.Decay / 10) {
      va = SimOpts.Decay;
    } else if (rob[n].body > 0) {
      va = rob[n].body;
    } else {
      va = 0;
    }

    if (SimOpts.DecayType == 2 && va != 0) {
      SH = -4;
      newshot(n, SH, va, 1);
    }

    if (SimOpts.DecayType == 3 && va != 0) {
      SH = -2;
      newshot(n, SH, va, 1);
    }


    rob[n].body = rob[n].body - SimOpts.Decay / 10;
    rob[n].radius = FindRadius(n);
  }
}

public static void defacate(ref int n) { //only used to get rid of massive amounts of waste
  int SH = 0;

  decimal va = 0;

  SH = -4;
  va = 200;

  if (va > rob[n].Waste) {
    va = rob[n].Waste;
  }
  if (rob[n].Waste > 32000) {
    rob[n].Waste = 31500;
    va = 500;
  }

  rob[n].Waste = rob[n].Waste - va;
  rob[n].nrg = rob[n].nrg - (SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER)) / (IIf(rob[n].numties < 0, 0, rob[n].numties) + 1);
  newshot(n, SH, va, 1, true);
  rob[n].Pwaste = rob[n].Pwaste + va / 1000;
}

/*
' robot n, hit by shot t, releases energy
*/
public static void releasenrg(int n, int t) {
//n=robot number
//t=shot number
  vector vel = null;


  int vs = 0;

  decimal vr = 0;

  decimal power = 0;

  decimal Range = 0;

  decimal scalingfactor = 0;

  decimal Newangle = 0;

  vector startingPos = null;

  vector incoming = null;

  decimal offcenter = 0;

  vector shotNow = null;

  decimal X = 0;

  decimal Y = 0;

  decimal angle = 0;

  vector relVel = null;

  decimal EnergyLost = 0;

  int a = 0;


  a = FirstSlot;

  if (rob[n].nrg <= 0.5m) {
// rob[n].Dead = True ' Don't kill them here so they can be corpses.  Still might have body.
goto ;
  }

  vel = VectorSub(ref rob[n].actvel, ref Shots(t).velocity); //negative relative velocity of shot hitting bot 'Botsareus 6/22/2016 Now based on robots actual velocity
//the shot to the hit bot
  vel = VectorAdd(ref vel, ref VectorScalar(ref rob[n].actvel, ref 0.5m)); //then add in half the velocity of hit robot

  if (SimOpts.EnergyExType) {
    if (Shots(t).Range == 0) { // Divide by zero protection
      power = 0;
    } else {
      power = CSng(Shots(t).value) * Shots(t).nrg / CSng((Shots(t).Range * (RobSize / 3))) * SimOpts.EnergyProp;
    }
    if (Shots(t).nrg < 0) {
goto getout;
    }
  } else {
    power = SimOpts.EnergyFix;
  }

//If power > rob[n].nrg + rob[n].poison And rob[n].nrg > 0 Then
//  power = rob[n].nrg + rob[n].poison
//End If

  if (rob[n].Corpse) {
    power = power * 0.5m; //half power against corpses.  Most of your shot is wasted 'The only way I can see this happening is if something tie injected energy into corpse
  }

  Range = Shots(t).Range * 2; //returned energy shots have twice the range as the shot that it came from (but half the velocity)

  if (rob[n].poison > power) { //create poison shot
    createshot(Shots(t).pos.X, Shots(t).pos.Y, vel.X, vel.Y, -5, n, power, Range * (RobSize / 3), vbYellow);
//  rob[n].Waste = rob[n].Waste + (power * 0.1)
    rob[n].poison = rob[n].poison - (power * 0.9m);
    if (rob[n].poison < 0) {
      rob[n].poison = 0;
    }
    rob[n].mem(poison) = rob[n].poison;
  } else { // create energy shot

    EnergyLost = power * 0.9m;
    if (EnergyLost > rob[n].nrg) {
//   EnergyLostPerCycle = EnergyLostPerCycle - rob[n].nrg
      power = rob[n].nrg;
      rob[n].nrg = 0;
    } else {
      rob[n].nrg = rob[n].nrg - EnergyLost; //some of shot comes from nrg
//  EnergyLostPerCycle = EnergyLostPerCycle - EnergyLost
    }

    EnergyLost = power * 0.01m;
    if (EnergyLost > rob[n].body) {
//   EnergyLostPerCycle = EnergyLostPerCycle - (rob[n].body * 10)
      rob[n].body = 0;
    } else {
      rob[n].body = rob[n].body - EnergyLost; //some of shot comes from body
// EnergyLostPerCycle = EnergyLostPerCycle - EnergyLost * 10
    }

// pass local vars to createshot so that no Shots array elements are on the stack in case the Shots array gets redimmed
    X = Shots(t).pos.X;
    Y = Shots(t).pos.Y;

    createshot(X, Y, vel.X, vel.Y, -2, n, power, Range * (RobSize / 3), vbWhite);
    rob[n].radius = FindRadius(n);
  }

  if (rob[n].body <= 0.5m || rob[n].nrg <= 0.5m) {
    rob[n].Dead = true;
    rob(Shots(t).parent).Kills = rob(Shots(t).parent).Kills + 1;
    rob(Shots(t).parent).mem(220) = rob(Shots(t).parent).Kills;
  }
getout:
}

private static void releasebod(int n, int t) { //a robot is shot by a -6 shot and releases energy directly from body points
//much more effective against a corpse
  vector vel = null;

  decimal Range = 0;

  decimal power = 0;

  decimal shell = 0;

  decimal EnergyLost = 0;


//If rob[n].body <= 0 Or rob[n].wall Then goto getout
  if (rob[n].body <= 0) {
goto getout;
  }


  vel = VectorSub(ref rob[n].actvel, ref Shots(t).velocity); //negative relative velocity of shot hitting bot 'Botsareus 6/22/2016 Now based on robots actual velocity
//the shot to the hit bot
  vel = VectorAdd(ref vel, ref VectorScalar(ref rob[n].actvel, ref 0.5m)); //then add in half the velocity of hit robot
// vel = VectorScalar(VectorSub(rob[n].vel, Shots(t).velocity), 0.5) 'half the relative velocity of
//the shot to the hit bot
//vel = VectorAdd(vel, rob[n].vel) 'then add in the velocity of hit robot

  if (SimOpts.EnergyExType) {
    if (Shots(t).Range == 0) { // Divide by zero protection
      power = 0;
    } else {
      power = CSng(Shots(t).value) * Shots(t).nrg / CSng((Shots(t).Range * (RobSize / 3))) * SimOpts.EnergyProp;
    }
  } else {
    power = SimOpts.EnergyFix;
  }

  if (power > 32000) {
    power = 32000;
  }

  shell = rob[n].shell * CSng(ShellEffectiveness);

  if (power > ((rob[n].body * 10) / 0.8m + shell)) {
    power = (rob[n].body * 10) / 0.8m + shell;
  }

  if (power < shell) {
    rob[n].shell = rob[n].shell - power / ShellEffectiveness;
    if (rob[n].shell < 0) {
      rob[n].shell = 0;
    }
    rob[n].mem(823) = rob[n].shell;
goto ;
  } else {
    rob[n].shell = rob[n].shell - power / ShellEffectiveness;
    if (rob[n].shell < 0) {
      rob[n].shell = 0;
    }
    rob[n].mem(823) = rob[n].shell;
    power = power - shell;
  }

  if (power <= 0) {
goto getout;
  }

  Range = Shots(t).Range * 2; //new range formula based on range of incoming shot

// create energy shot
  if (rob[n].Corpse == true) {
    power = power * 4; //So effective against corpses it makes me siiiiiinnnnnggg
    if (power > rob[n].body * 10) {
      power = rob[n].body * 10;
    }
    rob[n].body = rob[n].body - power / 10; //all energy comes from body
//  EnergyLostPerCycle = EnergyLostPerCycle - power
    rob[n].radius = FindRadius(n);
  } else {
    decimal leftover = 0;


    leftover = 0;
    EnergyLost = power * 0.2m;
    if (EnergyLost > rob[n].nrg) {
//   EnergyLostPerCycle = EnergyLostPerCycle - rob[n].nrg
      leftover = EnergyLost - rob[n].nrg;
      rob[n].nrg = 0;
    } else {
      rob[n].nrg = rob[n].nrg - EnergyLost; //some of shot comes from nrg
//   EnergyLostPerCycle = EnergyLostPerCycle - EnergyLost
    }

    EnergyLost = power * 0.08m;
    if (EnergyLost > rob[n].body) {
//   EnergyLostPerCycle = EnergyLostPerCycle - (rob[n].body * 10)
      leftover = leftover + EnergyLost - rob[n].body * 10;
      rob[n].body = 0;
    } else {
      rob[n].body = rob[n].body - EnergyLost; //some of shot comes from body
//   EnergyLostPerCycle = EnergyLostPerCycle - EnergyLost * 10
    }

    dynamic _WithVar_4298;
    _WithVar_4298 = rob[n];
      if (leftover > 0) {
        if (_WithVar_4298.nrg > 0& _WithVar_4298.nrg > leftover) {
          _WithVar_4298.nrg = _WithVar_4298.nrg - leftover;
//  EnergyLostPerCycle = EnergyLostPerCycle - leftover
          leftover = 0;
        } else if (_WithVar_4298.nrg > 0& _WithVar_4298.nrg < leftover) {
          leftover = leftover - _WithVar_4298.nrg;
//  EnergyLostPerCycle = EnergyLostPerCycle - rob[n].nrg
          _WithVar_4298.nrg = 0;
        }

        if (_WithVar_4298.body > 0& _WithVar_4298.body * 10 > leftover) {
          _WithVar_4298.body = _WithVar_4298.body - leftover * 0.1m;
//   EnergyLostPerCycle = EnergyLostPerCycle - leftover
          leftover = 0;
        } else if (rob[n].body > 0& rob[n].body * 10 < leftover) {
//   EnergyLostPerCycle = EnergyLostPerCycle - (rob[n].body * 10)
          _WithVar_4298.body = 0;
        }
      }
    rob[n].radius = FindRadius(n);
  }

  if (rob[n].body <= 0.5m || rob[n].nrg <= 0.5m) {
    rob[n].Dead = true;
    rob(Shots(t).parent).Kills = rob(Shots(t).parent).Kills + 1;
    rob(Shots(t).parent).mem(220) = rob(Shots(t).parent).Kills;
  }

  createshot(Shots(t).pos.X, Shots(t).pos.Y, vel.X, vel.Y, -2, n, power, Range * (RobSize / 3), vbWhite);
getout:
}

/*
' robot n takes the energy carried by shot t
*/
private static void takenrg(int n, int t) {
  decimal partial = 0;

  decimal overflow = 0;


//If rob[n].Corpse Or rob[n].wall Then goto getout
  if (rob[n].Corpse) {
goto getout;
  }

  if (Shots(t).Range < 0.00001m) {
    partial = 0;
  } else {
// If SimOpts.NoShotDecay Then
    partial = Shots(t).nrg;
// Else
//   partial = CSng(Shots(t).nrg / CSng(Shots(t).Range * (RobSize / 3)) * Shots(t).value)
// End If
  }

  if ((rob[n].nrg + partial * 0.95m) > 32000) {
    overflow = rob[n].nrg + (partial * 0.95m) - 32000;
    rob[n].nrg = 32000;
  } else {
    rob[n].nrg = rob[n].nrg + partial * 0.95m; //95% of energy goes to nrg
  }

  if ((rob[n].body + partial * 0.004m) + (overflow * 0.1m) > 32000) {
    rob[n].body = 32000;
  } else {
    rob[n].body = rob[n].body + (partial * 0.004m) + (overflow * 0.1m); //4% goes to body
  }

  rob[n].Waste = rob[n].Waste + partial * 0.01m; //1% goes to waste

//Shots(t).Exist = False
  rob[n].radius = FindRadius(n);
getout:
}

/*
'  robot takes a venomous shot and becomes seriously messed up
*/
private static void takeven(int n, int t) {
  decimal power = 0;

  decimal temp = 0;


//If rob[n].Corpse Or rob[n].wall Then goto getout
  if (rob[n].Corpse) {
goto getout;
  }

  power = CSng(Shots(t).nrg / CSng((Shots(t).Range * (RobSize / 3))) * Shots(t).value);

  if (power < 1) {
goto getout;
  }

  if (Shots(t).FromSpecie == rob[n].FName) { //Robot is immune to venom from his own species
    rob[n].venom = rob[n].venom + power; //Robot absorbs venom fired by conspec

//EricL 4/10/2006 This line prevents an overflow when power is too large
    if (((rob[n].venom) > 32000)) {
      rob[n].venom = 32000;
    }

    rob[n].mem(825) = rob[n].venom;
  } else {
    power = power * VenumEffectivenessVSShell; //Botsareus 3/6/2013 max power for venum is capped at 100 so I multiply to get an average
    if (power < rob[n].shell * ShellEffectiveness) {
      rob[n].shell = rob[n].shell - power / ShellEffectiveness;
      rob[n].mem(823) = rob[n].shell;
goto ; //Botsareus 3/6/2013 Exit sub if enough shell
    } else {
      temp = power;
      power = power - rob[n].shell * ShellEffectiveness;
      rob[n].shell = rob[n].shell - temp / ShellEffectiveness;
      if (rob[n].shell < 0) {
        rob[n].shell = 0;
      }
      rob[n].mem(823) = rob[n].shell;
    }
    power = power / VenumEffectivenessVSShell; //Botsareus 3/6/2013 after shell conversion devide

    if (power < 1) {
goto getout;
    }

    rob[n].Paralyzed = true;

//EricL - Following lines added March 15, 2006 to avoid Paracount being overflowed.
    if (((rob[n].Paracount + power) > 32000)) {
      rob[n].Paracount = 32000;
    } else {
      rob[n].Paracount = rob[n].Paracount + power;
    }

    if (Shots(t).memloc > 0) { //Botsareus 10/6/2015 Minor bug fixing
      rob[n].Vloc = (Shots(t).memloc - 1) % 1000 + 1;
      if (rob[n].Vloc == 340) {
        rob[n].Vloc = 0; //protection from delgene attacks Botsareus 10/7/2015 Moved here after mod
      }
    } else {
      do {
        rob[n].Vloc = Random(ref 1, ref 1000);
      } while(!(rob[n].Vloc != 340);
    }

    rob[n].Vval = Shots(t).Memval;
  }
//Shots(t).Exist = False
getout:
}

/*
'  Robot n takes shot t and adds its value to his waste reservoir
*/
private static void takewaste(int n, int t) {
  decimal power = 0;


//  If rob[n].wall Then goto getout

  power = Shots(t).nrg / (Shots(t).Range * (RobSize / 3)) * Shots(t).value;
  if (power < 0) {
goto getout;
  }
  rob[n].Waste = rob[n].Waste + power;
// Shots(t).Exist = False
getout:
}

/*
' Robot receives poison shot and becomes disorientated
*/
private static void takepoison(int n, int t) {
  decimal power = 0;


//If rob[n].Corpse Or rob[n].wall Then goto getout
  if (rob[n].Corpse) {
goto getout;
  }

  power = CSng(Shots(t).nrg / CSng((Shots(t).Range * (RobSize / 3))) * Shots(t).value);

  if (power < 1) {
goto getout;
  }

  if (Shots(t).FromSpecie == rob[n].FName) { //Robot is immune to poison from his own species
    rob[n].poison = rob[n].poison + power; //Robot absorbs poison fired by conspecs
    if (rob[n].poison > 32000) {
      rob[n].poison = 32000;
    }
    rob[n].mem(827) = rob[n].poison;
  } else {
    rob[n].Poisoned = true;
    rob[n].Poisoncount = rob[n].Poisoncount + power / 1.5m; //Botsareus 6/24/3016 Div by 1.5 to make poison shots more proportenal to venom shots
    if (rob[n].Poisoncount > 32000) {
      rob[n].Poisoncount = 32000;
    }
    if (Shots(t).memloc > 0) { //Botsareus 10/6/2015 Minor bug fixing
      rob[n].Ploc = (Shots(t).memloc - 1) % 1000 + 1;
      if (rob[n].Ploc == 340) {
        rob[n].Ploc = 0; //protection from delgene attacks Botsareus 10/7/2015 Moved here after mod
      }
    } else {
      do {
        rob[n].Ploc = Random(ref 1, ref 1000);
      } while(!(rob[n].Ploc != 340);
    }
    rob[n].Pval = Shots(t).Memval;
  }
//  Shots(t).Exist = False
getout:
}

/*
'Robot is hit by sperm shot and becomes fertilized for potential sexual reproduction
*/
private static void takesperm(int n, int t) {
  if (rob[n].fertilized < -10) {
return;//block sex repro when necessary

  }

  int X = 0;


  if (Shots(t).DnaLen == 0) {
goto getout;
  }
  rob[n].fertilized = 10; // bots stay fertilized for 10 cycles currently
  rob[n].mem(SYSFERTILIZED) = 10;
  List<> rob_349_tmp = new List<>();// copy the male's DNA to the female's bot structure
for (int redim_iter_5502=0;i<0;redim_iter_5502++) {rob.Add(null);}
  rob[n].spermDNA = Shots(t).dna;
  rob[n].spermDNAlen = Shots(t).DnaLen;
getout:
}

/*
'' checks the collisions between robots and shots
'Private Function ShotColl(n As Integer) As Integer
' ' Dim nd As node
''  Dim vel As vector

' ' Dim dist As Single

'  With Shots(n)

'  If SimOpts.Updnconnected = True Then
'    If .pos.y > SimOpts.FieldHeight Then
'      .pos.y = .pos.y - SimOpts.FieldHeight
'    ElseIf .pos.y < 0 Then
'      .pos.y = .pos.y + SimOpts.FieldHeight
'    End If
'  Else
'    If .pos.y > SimOpts.FieldHeight Or .pos.y < 0 Then
'      .velocity.y = -.velocity.y
'    End If
'  End If

'  If SimOpts.Dxsxconnected = True Then
'    If .pos.x > SimOpts.FieldWidth Then
'      .pos.x = .pos.x - SimOpts.FieldWidth
'    ElseIf .pos.x < 0 Then
'      .pos.x = .pos.x + SimOpts.FieldWidth
'    End If
'  Else
'    If .pos.x > SimOpts.FieldWidth Or .pos.x < 0 Then
'      .velocity.x = -.velocity.x
'    End If
'  End If


'  'ShotColl = OldShotColl(n)
'  ShotColl = NewShotCollision(n)

'  End With
'End Function

'EricL 5/16/2006 Checks for collisions between shots and bots.  Takes into consideration
'motion of target bot as well as shots which potentially pass through the target bot during the cycle
'Argument: The shot number to check
'Returns: bot number of the hit bot if a collison occurred, 0 otherwise
'Side Effect: On a hit, changes the shot position to be the point of impact with the bot
*/
private static int NewShotCollision(ref int shotnum) {
  int NewShotCollision = 0;
  int robnum = 0;

  vector B0 = null;//Position of bot at time 0

  vector b = null;//Position of bot at time 0 < t < 1

  vector S0 = null;//Position of shot at time 0

  vector S1 = null;//Position of shot at time 1

  vector s = null;//Position of shot at time 0 < t < 1

  vector vs = null;//Velocity of the shot

  vector vb = null;//Velocity of the bot

  vector d = null;//Vector from bot center to shot at time 0

  decimal D2 = 0;

  decimal r = 0;//Bot radius

  decimal t = 0;//Loop counter

  decimal hitTime = 0;// time in the cycle when collision occurred.

  decimal earliestCollision = 0;//Used to find which bot was hit earliest in the cycle.

//The time in the cycle at which the earliest collision with the shot occurred.
  decimal time0 = 0;

  decimal time1 = 0;

  vector p = null;//Position Vector - Realtive positions of bot and shot over time

  decimal L1 = 0;

  decimal P2 = 0;

  decimal X = 0;

  decimal Y = 0;

  decimal DdotP = 0;

  bool usetime0 = false;

  bool usetime1 = false;


// Check for collisions with the field edges
  dynamic _WithVar_6918;
  _WithVar_6918 = Shots(shotnum);
    if (SimOpts.Updnconnected == true) {
      if (_WithVar_6918.pos.Y > SimOpts.FieldHeight) {
        _WithVar_6918.pos.Y = _WithVar_6918.pos.Y - SimOpts.FieldHeight;
      } else if (_WithVar_6918.pos.Y < 0) {
        _WithVar_6918.pos.Y = _WithVar_6918.pos.Y + SimOpts.FieldHeight;
      }
    } else {
      if (_WithVar_6918.pos.Y > SimOpts.FieldHeight) {
        _WithVar_6918.pos.Y = SimOpts.FieldHeight;
        _WithVar_6918.velocity.Y = -1 * Abs(_WithVar_6918.velocity.Y);
      } else if (_WithVar_6918.pos.Y < 0) {
        _WithVar_6918.pos.Y = 0;
        _WithVar_6918.velocity.Y = Abs(_WithVar_6918.velocity.Y);
      }
    }

    if (SimOpts.Dxsxconnected == true) {
      if (_WithVar_6918.pos.X > SimOpts.FieldWidth) {
        _WithVar_6918.pos.X = _WithVar_6918.pos.X - SimOpts.FieldWidth;
      } else if (_WithVar_6918.pos.X < 0) {
        _WithVar_6918.pos.X = _WithVar_6918.pos.X + SimOpts.FieldWidth;
      }
    } else {
      if (_WithVar_6918.pos.X > SimOpts.FieldWidth) {
        _WithVar_6918.pos.X = SimOpts.FieldWidth;
        _WithVar_6918.velocity.X = -1 * Abs(_WithVar_6918.velocity.X);
      } else if (_WithVar_6918.pos.X < 0) {
        _WithVar_6918.pos.X = 0;
        _WithVar_6918.velocity.X = Abs(_WithVar_6918.velocity.X);
      }
    }


//Initialize the return value in case no collision is found.
  NewShotCollision = 0;

//Initialize that the earliest collision to 100 to indicate no collision has been detected
  earliestCollision = 100;

  S0 = Shots(shotnum).pos;
  vs = Shots(shotnum).velocity;

  for(robnum=1; robnum<MaxRobs; robnum++) { // Walk through all the bots

//Make sure the bot is eligable to be hit by the shot.  It has to exist, it can't have been the one who
//fired the shot, it can't be a wall bot and it has to be close enough that an impact is possible.  Note that for perf reasons we
//ignore edge cases here where the field is a torus and a shot wraps around so it's possible to miss collisons in such cases.
    if (rob(robnum).exist && (Shots(shotnum).parent != robnum) && !(rob(robnum).FName == "Base.txt" && hidepred(And(Abs(Shots(shotnum).opos.X - rob(robnum).pos.X,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -) < MaxBotShotSeperation && Abs(Shots(shotnum).opos.Y - rob(robnum).pos.Y) < MaxBotShotSeperation)) {
      r = rob(robnum).radius; // + 5 ' Tweak the bot radius up a bit to handle the issue with bots appearing a little larger than then are


//Note that this routine is called before the position for both the bot and the shot is updated this cycle.  This means
//we are looking forward in time, from the current positions to where they will be at the end of this cycle.  This is why
//we can use .pos and not .opos
      B0 = rob(robnum).pos;

//Botsareus 6/22/2016 The robots actual velocity and non collision velocity can be different - correct here
      B0 = VectorSub(ref B0, ref rob(robnum).vel);
      B0 = VectorAdd(ref B0, ref rob(robnum).actvel);

      p = VectorSub(ref S0, ref B0);

      if (VectorMagnitude(ref p) < r) { // shot is inside the target at Time 0.  Did we miss the entry last cycle?  How?
        hitTime = 0;
        earliestCollision = 0;
        NewShotCollision = robnum;
goto ;
      }

      vb = rob(robnum).actvel;
      d = VectorSub(ref vs, ref vb); // Vector of velocity change from both bot and shot over time t
      P2 = VectorMagnitudeSquare(ref p); // |P|^2

      D2 = VectorMagnitudeSquare(ref d); // |D|^2
      if (D2 == 0) {
goto CheckRestOfBots;
      }
      DdotP = Dot(ref d, ref p);
      X = -DdotP;
      Y = DdotP ^ 2 - D2 * (P2 - r ^ 2);

      if (Y < 0) {
goto CheckRestOfBots; // No collision
      }

      Y = Sqr(Y);

      time0 = (X - Y) / D2;
      time1 = (X + Y) / D2;

      usetime0 = false;
      usetime1 = false;

      if (!(time0 <= 0 || time0 >= 1)) {
        usetime0 = true;
      }
      if (!(time1 <= 0 || time1 >= 1)) {
        usetime1 = true;
      }
      if ((!usetime0) && (!usetime1)) {
goto ;
      } else if (usetime0& usetime1) {
        hitTime = Min(time0, time1);
        NewShotCollision = robnum;
      } else if (usetime0) {
        hitTime = time0;
        NewShotCollision = robnum;
      } else {
        hitTime = time1;
        NewShotCollision = robnum;
      }

      if (hitTime < earliestCollision) {
        earliestCollision = hitTime;
      }

//If the collision occurred early enough in the cycle, we can assume no other bot could have been hit ealier and we can
//skip checking the rest of the bots.  This is all about perf.
      if (earliestCollision <= MinBotRadius) {
goto ;
      } else {
goto ;
      }
    }
//We jump here if we found a collision with the current bot, but it was late enough in the cycle that another
//bot could have been hit earlier in the cycle, so we keep checking the rest of the bots
//Or if we have ruled out a possibile collision between this shot and the current bot.
CheckRestOfBots:
    Next(robnum);
//We jump here if we are confident that the collision occurred early enough in the cycle that no other bot could have been
//hit before this one.  Note that this is sensitive to shot speed and minumum bot radius
FinialCollisionDetected:
    if (earliestCollision <= 1) {
//This is a total hack, but if we found a collision, any collision, then we set the position of the shot to be the point of the earliest
//collision so that in the case where a return shot is generated, that return shot starts from the point of impact and not
//from wherever the shot would have ended up at the end of the cycle had it not collided (which it did!)
      Shots(shotnum).pos = VectorAdd(ref VectorScalar(ref vs, ref earliestCollision), ref S0);
    }
    return NewShotCollision;
  }

/*
'Botsareus 10/5/2015 Bug fix for negative values in virus
*/
public static void Vshoot(ref int n, ref int thisshot) {
//here we shoot a virus

  decimal tempa = 0;

  decimal ShAngle = 0;


  if (!Shots(thisshot).exist) {
goto getout;
  }
  if (!Shots(thisshot).stored) {
goto getout;
  }

  if (rob[n].mem(VshootSys) < 0) {
    rob[n].mem(VshootSys) = 1;
  }

  tempa = CSng(rob[n].mem(VshootSys)) * 20; //.vshoot * 20
  if (tempa > 32000) {
    tempa = 32000;
  }
  if (tempa < 0) {
    tempa = 0;
  }

  Shots(thisshot).nrg = tempa;
  rob[n].nrg = rob[n].nrg - (tempa / 20) - (SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER));

  Shots(thisshot).Range = 11 + CInt((rob[n].mem(VshootSys)) / 2);
  rob[n].nrg = rob[n].nrg - CSng(rob[n].mem(VshootSys)) - (SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER));

  dynamic _WithVar_8912;
  _WithVar_8912 = Shots(thisshot);
    ShAngle = Random(ref 1, ref 1256) / 200;
    _WithVar_8912.stored = false;
    _WithVar_8912.pos.X = (rob[n].pos.X + Cos(ShAngle) * rob[n].radius);
    _WithVar_8912.pos.Y = (rob[n].pos.Y - Sin(ShAngle) * rob[n].radius);

    _WithVar_8912.velocity.X = absx(ref ShAngle, RobSize / 3, 0, 0, 0); // set shot speed x seems to not work well at high bot speeds
    _WithVar_8912.velocity.Y = absy(ref ShAngle, RobSize / 3, 0, 0, 0); // set shot speed y

    _WithVar_8912.velocity.X = _WithVar_8912.velocity.X + rob[n].actvel.X;
    _WithVar_8912.velocity.Y = _WithVar_8912.velocity.Y + rob[n].actvel.Y;

    _WithVar_8912.opos.X = _WithVar_8912.pos.X - _WithVar_8912.velocity.X;
    _WithVar_8912.opos.Y = _WithVar_8912.pos.Y - _WithVar_8912.velocity.Y;
getout:
}

public static bool MakeVirus(ref int robn, int gene) {
  bool MakeVirus = false;
  rob(robn).virusshot = newshot(ref robn, -7, Int(gene), ref 1);
  if (rob(robn).virusshot > 0) {
    MakeVirus = true;
  } else {
    MakeVirus = false;
  }
  return MakeVirus;
}

/*
' copy gene number p from robot that fired shot n into shot n dna (virus)
*/
public static bool copygene(ref int n, int p) {
  bool copygene = false;
  int t = 0;

  int parent = 0;

  int genelen = 0;

  int GeneStart = 0;

  int GeneEnding = 0;


  parent = Shots(n).parent;

  if ((p > rob(parent).genenum) || p < 1) {
// target gene is beyond the DNA bounds
    copygene = false;
goto ;
  }

  GeneStart = genepos(ref rob(parent).dna, p);
  GeneEnding = GeneEnd(ref rob(parent).dna, GeneStart);
  genelen = GeneEnding - GeneStart + 1;

  if (genelen < 1) {
    copygene = false;
goto ;
  }

  List<> Shots_6806_tmp = new List<>();
for (int redim_iter_2721=0;i<0;redim_iter_2721++) {Shots.Add(null);}

// Put an end on it just in case...
// Shots(n).DNA(genelen).tipo = 10
//Shots(n).DNA(genelen).value = 1

  for(t=0; t<genelen - 1; t++) {
    Shots(n).dna(t) = rob(parent).dna(GeneStart + t);
    Next(t);

    Shots(n).DnaLen = genelen;

    copygene = true;
getout:
    return copygene;
  }

/*
' adds gene from shot p to robot n dna
*/
public static int addgene(int n, int p) {
  int addgene = 0;
  int t = 0;

  int Insert = 0;

  int vlen = 0;//length of the DNA code of the virus

  int Position = 0;//gene position to insert the virus

  decimal power = 0;


//Dead bodies and virus immune bots can't catch a virus
  if (rob[n].Corpse || (rob[n].VirusImmune)) {
goto getout;
  }

  power = Shots(p).nrg / (Shots(p).Range * RobSize / 3) * Shots(p).value;

  if (power < rob[n].Slime * SlimeEffectiveness) {
    rob[n].Slime = rob[n].Slime - power / SlimeEffectiveness;
goto ;
  } else {
    rob[n].Slime = rob[n].Slime - power / SlimeEffectiveness;
    power = power - rob[n].Slime * SlimeEffectiveness;
    if (rob[n].Slime < 0.5m) {
      rob[n].Slime = 0;
    }
  }

  Position() = Random(ref 0, ref rob[n].genenum); //randomize the gene number
  if (Position() == 0) {
    Insert = 0;
  } else {
    Insert = GeneEnd(ref rob[n].dna, genepos(ref rob[n].dna, Position()));
    if (Insert == (rob[n].DnaLen)) {
      Insert = rob[n].DnaLen;
    }
  }

//  vlen = DnaLen(Shots(P).DNA())
  vlen = Shots(p).DnaLen;

  if (MakeSpace(ref rob[n].dna, Insert, vlen)) { //Moves genes back to make space
    for(t=Insert; t<Insert + vlen - 1; t++) {
      rob[n].dna(t + 1) == Shots(p).dna(t - Insert);
      Next(t);
    }

    makeoccurrlist(n);
    rob[n].DnaLen = DnaLen(ref rob[n].dna());
    rob[n].genenum = CountGenes(ref rob[n].dna);
    rob[n].mem(DnaLenSys) = rob[n].DnaLen;
    rob[n].mem(GenesSys) = rob[n].genenum;

    rob[n].SubSpecies = NewSubSpecies(ref n); // Infection with a virus counts as a new subspecies
    logmutation(n, "Infected with virus of length " + Str(vlen) + " during cycle " + Str(SimOpts.TotRunCycle) + " at pos " + Str(Insert));
    rob[n].Mutations = rob[n].Mutations + 1;
    rob[n].LastMut = rob[n].LastMut + 1;
getout:
    return addgene;
  }

private static shot) As Boolean IsArrayBounded(ref dynamic ArrayIn(_UNUSED) {
  shot) As Boolean IsArrayBounded = null;
  // TODO (not supported): On Error GoTo getout

  IsArrayBounded = (UBound(ArrayIn) >= LBound(ArrayIn));
  return IsArrayBounded;


getout:
  IsArrayBounded = false;
//Resume Next

  return IsArrayBounded;
}
}
