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


static class NeoMutations {
// Option Explicit
//1-(perbot+1)^(1/DNALength) = per unit
//1-(1-perunit)^DNALength = perbot
public const int PointUP = 0; //expressed as 1 chance in X per kilocycle per bp
public const int MinorDeletionUP = 1;
public const int ReversalUP = 2;
public const int InsertionUP = 3;
public const int AmplificationUP = 4;
public const int MajorDeletionUP = 5;
public const int CopyErrorUP = 6;
public const int DeltaUP = 7;
public const int TranslocationUP = 8;
public const int P2UP = 9; //Botsareus 12/10/2013 new mutation rates
public const int CE2UP = 10;
private const decimal overtime = 30; //Time correction across all mutations


static void logmutation(int n_UNUSED, string strmut_UNUSED) { //Botsareus 10/8/2015 Wrap mutations to prevent crash

/*
If SimOpts.TotRunCycle = 0 Then
Exit Sub 'Botsareus 4/28/2016 Prevents div/0
End If
 With rob[n]
  If Len(.LastMutDetail) > (100000000 / TotalRobotsDisplayed) Then
.LastMutDetail = "" 'Botsareus 4/11/2016 Bug fix - Use the total string length across all robots
End If
  .LastMutDetail = strmut + vbCrLf + .LastMutDetail
 End With
End Sub
*/
private static string MutationType(ref int thing) {
  string MutationType = "";
  MutationType = "";
  switch(thing) {
    case 0:
      MutationType = "Point Mutation";
      break;
    case 1:
      MutationType = "Minor Deletion";
      break;
    case 2:
      MutationType = "Reversal";
      break;
    case 3:
      MutationType = "Insertion";
      break;
    case 4:
      MutationType = "Amplification";
      break;
    case 5:
      MutationType = "Major Deletion";
      break;
    case 6:
      MutationType = "Copy Error";
      break;
    case 7:
      MutationType = "Delta Mutation";
break;
}

  return MutationType;
}

/*
'NEVER allow anything after end, which must be = DNALen
'ALWAYS assume that DNA is sized right
'ALWAYS size DNA correctly when mutating
*/
private static dynamic EraseUnit(out block unit) {
  dynamic EraseUnit = null;
  unit.tipo = -1;
  unit.value = -1;
  return EraseUnit;
}

public static block, ByVal beginning As Long, ByVal Length As Long, Optional DNALength As Integer = -1) As Boolean MakeSpace(ref dynamic dna(_UNUSED) {
  block, ByVal beginning As Long, ByVal Length As Long, Optional DNALength As Integer = -1) As Boolean MakeSpace = null;
//add length elements after beginning.  Beginning doesn't move places
//returns true if the space was created,
//false otherwise

  int t = 0;

  if (DNALength < 0) {
    DNALength = DnaLen(ref dna);
  }
  if (Length < 1 || beginning < 0 || beginning > DNALength - 1 || (DNALength + Length > 32000)) {
    MakeSpace = false;
goto ;
  }

  MakeSpace = true;

  List<> dna_5081_tmp = new List<>();
for (int redim_iter_7578=0;i<0;redim_iter_7578++) {dna.Add(redim_iter_7578<dna.Count ? dna(redim_iter_7578) : null);}

//Botsareus 6/25/2016 Bugfix - erase dna that was just created to insure all units get erased
  for(t=DNALength + 1; t<DNALength + Length Step 1; t++) {
    EraseUnit(dna(t));
  }

  for(t=DNALength; t<beginning + 1 Step -1; t++) {
    dna(t + Length) == dna(t);
    EraseUnit(dna(t));
    Next(t);
getout:
    return MakeSpace;
  }

public static void Delete(ref dynamic dna(_UNUSED) {
//delete elements starting at beginning
  int t = 0;

  if (DNALength < 0) {
    DNALength = DnaLen(ref dna);
  }
  if (elements < 1 || beginning < 1 || beginning > DNALength - 1) {
goto getout;
  }
// If elements + beginning > DNALength - 1 Then elements = DNALength - 1 - beginning

  for(t=beginning + elements; t<DNALength; t++) {
// TODO (not supported):     On Error GoTo step2 //small error mod
    dna(t - elements) == dna(t);
    Next(t);

step2:
    DNALength = DnaLen(ref dna);
    List<> dna_4812_tmp = new List<>();
for (int redim_iter_5346=0;i<0;redim_iter_5346++) {dna.Add(redim_iter_5346<dna.Count ? dna(redim_iter_5346) : null);}
getout:
  }

public static int NewSubSpecies(ref int n) {
  int NewSubSpecies = 0;
  int i = 0;


  i = SpeciesFromBot(ref n); // Get the index into the species array for this bot
  SimOpts.Specie(i).SubSpeciesCounter = SimOpts.Specie(i).SubSpeciesCounter + 1; // increment the counter
  if (SimOpts.Specie(i).SubSpeciesCounter > 32000) {
    SimOpts.Specie(i).SubSpeciesCounter = -32000; //wrap the counter if necessary
  }
  NewSubSpecies = SimOpts.Specie(i).SubSpeciesCounter;

  return NewSubSpecies;
}

/*
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''
*/
public static void mutate(int robn, ref bool reproducing) { //Botsareus 12/17/2013
  int Delta = 0;


  dynamic _WithVar_9838;
  _WithVar_9838 = rob(robn);
    if (!.Mutables.Mutations || SimOpts.DisableMutations) {
goto getout;
    }
    Delta = CLng(_WithVar_9838.LastMut);


    ismutating = true; //Botsareus 2/2/2013 Tells the parseor to ignore debugint and debugbool while the robot is mutating
    if (!reproducing) {
      if (_WithVar_9838.Mutables.mutarray(PointUP) > 0) {
        PointMutation(robn);
      }
      if (_WithVar_9838.Mutables.mutarray(DeltaUP) > 0& !Delta2) {
        DeltaMut(robn);
      }
      if (_WithVar_9838.Mutables.mutarray(P2UP) > 0& sunbelt) {
        PointMutation2(robn);
      }

//special case update epigenetic reset
      if (CLng(_WithVar_9838.LastMut) - Delta > 0& epireset) {
        _WithVar_9838.MutEpiReset = _WithVar_9838.MutEpiReset + (CLng(_WithVar_9838.LastMut) - Delta) ^ epiresetemp;
      }

//Delta2 point mutation change
      if (Delta2 && DeltaPM > 0) {
        if (_WithVar_9838.age % DeltaPM == 0& _WithVar_9838.age > 0) {
          int MratesMax = 0;

          MratesMax = IIf(NormMut, CLng(_WithVar_9838.DnaLen) * CLng(valMaxNormMut), 2000000000);
          byte t = 0;

          for(t=0; t<9 Step 9; t++) { //Point and Point2
            if (_WithVar_9838.Mutables.mutarray(t) < 1) {
goto skip; //Botsareus 1/3/2014 if mutation off then skip it
            }
            if (rndy() < DeltaMainChance / 100) {
              if (DeltaMainExp != 0) {
                _WithVar_9838.Mutables.mutarray(t) = _WithVar_9838.Mutables.mutarray(t) * 10 ^ ((rndy() * 2 - 1) / DeltaMainExp);
              }
              _WithVar_9838.Mutables.mutarray(t) = _WithVar_9838.Mutables.mutarray(t) + (rndy() * 2 - 1) * DeltaMainLn;
              if (_WithVar_9838.Mutables.mutarray(t) < 1) {
                _WithVar_9838.Mutables.mutarray(t) = 1;
              }
              if (_WithVar_9838.Mutables.mutarray(t) > MratesMax) {
                _WithVar_9838.Mutables.mutarray(t) = MratesMax;
              }
            }
            if (rndy() < DeltaDevChance / 100) {
              if (DeltaDevExp != 0) {
                _WithVar_9838.Mutables.StdDev(t) = _WithVar_9838.Mutables.StdDev(t) * 10 ^ ((rndy() * 2 - 1) / DeltaDevExp);
              }
              _WithVar_9838.Mutables.StdDev(t) = _WithVar_9838.Mutables.StdDev(t) + (rndy() * 2 - 1) * DeltaDevLn;
              if (DeltaDevExp != 0) {
                _WithVar_9838.Mutables.Mean(t) = _WithVar_9838.Mutables.Mean(t) * 10 ^ ((rndy() * 2 - 1) / DeltaDevExp);
              }
              _WithVar_9838.Mutables.Mean(t) = _WithVar_9838.Mutables.Mean(t) + (rndy() * 2 - 1) * DeltaDevLn;
//Max range is always 0 to 800
              if (_WithVar_9838.Mutables.StdDev(t) < 0) {
                _WithVar_9838.Mutables.StdDev(t) = 0;
              }
              if (_WithVar_9838.Mutables.StdDev(t) > 200) {
                _WithVar_9838.Mutables.StdDev(t) = 200;
              }
              if (_WithVar_9838.Mutables.Mean(t) < 1) {
                _WithVar_9838.Mutables.Mean(t) = 1;
              }
              if (_WithVar_9838.Mutables.Mean(t) > 400) {
                _WithVar_9838.Mutables.Mean(t) = 400;
              }
            }
skip:
          }
          _WithVar_9838.Mutables.PointWhatToChange = _WithVar_9838.Mutables.PointWhatToChange + (rndy() * 2 - 1) * DeltaWTC;
          if (_WithVar_9838.Mutables.PointWhatToChange < 0) {
            _WithVar_9838.Mutables.PointWhatToChange = 0;
          }
          if (_WithVar_9838.Mutables.PointWhatToChange > 100) {
            _WithVar_9838.Mutables.PointWhatToChange = 100;
          }
          _WithVar_9838.Point2MutCycle = 0;
          _WithVar_9838.PointMutCycle = 0;
        }
      }

    } else {
      if (_WithVar_9838.Mutables.mutarray(CopyErrorUP) > 0) {
        CopyError(robn);
      }
      if (_WithVar_9838.Mutables.mutarray(CE2UP) > 0& sunbelt) {
        CopyError2(robn);
      }
      if (_WithVar_9838.Mutables.mutarray(InsertionUP) > 0) {
        Insertion(robn);
      }
      if (_WithVar_9838.Mutables.mutarray(ReversalUP) > 0) {
        Reversal(robn);
      }
      if (_WithVar_9838.Mutables.mutarray(TranslocationUP) > 0& sunbelt) {
        Translocation(robn); //Botsareus Translocation and Amplification still bugy, but I want them.
      }
      if (_WithVar_9838.Mutables.mutarray(AmplificationUP) > 0& sunbelt) {
        Amplification(robn);
      }
      if (_WithVar_9838.Mutables.mutarray(MajorDeletionUP) > 0) {
        MajorDeletion(robn);
      }
      if (_WithVar_9838.Mutables.mutarray(MinorDeletionUP) > 0) {
        MinorDeletion(robn);
      }

    }

    ismutating = false; //Botsareus 2/2/2013 Tells the parseor to ignore debugint and debugbool while the robot is mutating

    Delta = CLng(_WithVar_9838.LastMut) - Delta; //Botsareus 9/4/2012 Moved delta check before overflow reset to fix an error where robot info is not being updated

//auto forking
    if (SimOpts.EnableAutoSpeciation) {
      if (CDbl(_WithVar_9838.Mutations) > CDbl(_WithVar_9838.DnaLen) * CDbl(SimOpts.SpeciationGeneticDistance / 100)) {
        string robname = "";

        List<string> splitname = new List<string> {}; // TODO - Specified Minimum Array Boundary Not Supported:                 Dim splitname() As String

//generate new specie name
        SimOpts.SpeciationForkInterval = SimOpts.SpeciationForkInterval + 1;
//remove old nick name
        splitname = Split(_WithVar_9838.FName, ")");
//if it is a nick name only
        if (Left(splitname[0], 1) == "(" && IsNumeric(Right(splitname[0], Len(splitname[0]) - 1))) {
          robname = splitname[1];
        } else {
          robname = _WithVar_9838.FName;
        }
        robname = "(" + SimOpts.SpeciationForkInterval + ")" + robname;
//do we have room for new specie?
        if (SimOpts.SpeciesNum < 49) {
          _WithVar_9838.FName = robname;
          _WithVar_9838.Mutations = 0;
          AddSpecie(robn, false);
        } else {
          SimOpts.SpeciationForkInterval = SimOpts.SpeciationForkInterval - 1;
        }
      }
    }

    if (_WithVar_9838.Mutations > 32000) {
      _WithVar_9838.Mutations = 32000; //Botsareus 5/31/2012 Prevents mutations overflow
    }
    if (_WithVar_9838.LastMut > 32000) {
      _WithVar_9838.LastMut = 32000;
    }

    if ((Delta > 0)) { //The bot has mutated.

      _WithVar_9838.GenMut = _WithVar_9838.GenMut - _WithVar_9838.LastMut;
      if (_WithVar_9838.GenMut < 0) {
        _WithVar_9838.GenMut = 0;
      }

      mutatecolors(robn, Delta);
      _WithVar_9838.SubSpecies = NewSubSpecies(ref robn);
      _WithVar_9838.genenum = CountGenes(ref rob(robn).dna());
      _WithVar_9838.DnaLen = DnaLen(ref rob(robn).dna());
      _WithVar_9838.mem(DnaLenSys) = _WithVar_9838.DnaLen;
      _WithVar_9838.mem(GenesSys) = _WithVar_9838.genenum;
    }
getout:
}

private static void Amplification(ref int robn) { //Botsareus 12/10/2013
  // TODO (not supported): On Error GoTo getout
//1. pick a spot (1 to .dnalen - 1)
//2. Run a length, copied to a temporary location
//3. Pick a new spot (1 to .dnalen - 1)
//4. Insert copied DNA

  int t = 0;

  int Length = 0;

  dynamic _WithVar_4867;
  _WithVar_4867 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_4867.DnaLen) * CDbl(_WithVar_4867.Mutables.Mean(AmplificationUP) + _WithVar_4867.Mutables.StdDev(AmplificationUP)) / (1200 * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_4867.Mutables.mutarray(AmplificationUP) < floor) {
      _WithVar_4867.Mutables.mutarray(AmplificationUP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }

    List<block> tempDNA = new List<block> {}; // TODO - Specified Minimum Array Boundary Not Supported:   Dim tempDNA() As block

    int start = 0;

    int second = 0;

    int counter = 0;

    t = 1;
    do {
      t = t + 1;
      if (rndy() < 1 / (_WithVar_4867.Mutables.mutarray(AmplificationUP) / SimOpts.MutCurrMult)) {
        Length = Gauss(_WithVar_4867.Mutables.StdDev(AmplificationUP), _WithVar_4867.Mutables.Mean(AmplificationUP));
        Length = Length % UBound(_WithVar_4867.dna);
        if (Length < 1) {
          Length = 1;
        }

        Length = Length - 1;
        Length = Length/2;
        if (t - Length < 1) {
goto skip;
        }
        if (t + Length > _WithVar_4867.DnaLen - 1) {
goto skip;
        }
        if (UBound(_WithVar_4867.dna) + CLng(Length) * 2 > 32000) {
goto skip; //Botsareus 10/5/2015 Size limit is calculated here
        }

        if (Length > 0) {
          List<block> tempDNA_1611_tmp = new List<block>();
for (int redim_iter_5563=0;i<0;redim_iter_5563++) {tempDNA.Add(null);}

          second = 0;
          for(counter=t - Length; counter<t + Length; counter++) {
            tempDNA[second] = _WithVar_4867.dna(counter);
            second = second + 1;
            Next(counter);
//we now have the appropriate length of DNA in the temporary array.

            start = Random(ref 1, ref UBound(_WithVar_4867.dna) - 2);
            MakeSpace(_WithVar_4867.dna(), start, UBound(tempDNA) + 1);

            for(counter=start + 1; counter<start + UBound(tempDNA) + 1; counter++) {
              _WithVar_4867.dna(counter) = tempDNA[counter - start - 1];
              Next(counter);

//BOTSAREUSIFIED
              _WithVar_4867.Mutations = _WithVar_4867.Mutations + 1;
              _WithVar_4867.LastMut = _WithVar_4867.LastMut + 1;
              logmutation(robn, "Amplification copied a series at" + Str(t) + Str(Length * 2 + 1) + "bps long to " + Str(start) + " during cycle" + Str(SimOpts.TotRunCycle));

            }
          }
skip:
        } while(!(t >= UBound(_WithVar_4867.dna) - 1);

//add "end" to end of the DNA
        _WithVar_4867.dna(UBound(_WithVar_4867.dna)).tipo = 10;
        _WithVar_4867.dna(UBound(_WithVar_4867.dna)).value = 1;

getout:
      rob(robn).DnaLen = DnaLen(ref rob(robn).dna()); //Botsareus 10/6/2015 Calculate new dna size
    }

private static void Translocation(ref int robn) { //Botsareus 12/10/2013
  // TODO (not supported): On Error GoTo getout
//1. pick a spot (1 to .dnalen - 1)
//2. Run a length, copied to a temporary location
//3.  Pick a new spot (1 to .dnalen - 1)
//4. Insert copied DNA

  int t = 0;

  int Length = 0;

  dynamic _WithVar_6114;
  _WithVar_6114 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_6114.DnaLen) * CDbl(_WithVar_6114.Mutables.Mean(TranslocationUP) + _WithVar_6114.Mutables.StdDev(TranslocationUP)) / (360 * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_6114.Mutables.mutarray(TranslocationUP) < floor) {
      _WithVar_6114.Mutables.mutarray(TranslocationUP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }

    List<block> tempDNA = new List<block> {}; // TODO - Specified Minimum Array Boundary Not Supported:   Dim tempDNA() As block

    int start = 0;

    int second = 0;

    int counter = 0;


    for(t=1; t<UBound(_WithVar_6114.dna) - 1; t++) {
      if (rndy() < 1 / (_WithVar_6114.Mutables.mutarray(TranslocationUP) / SimOpts.MutCurrMult)) {
        Length = Gauss(_WithVar_6114.Mutables.StdDev(TranslocationUP), _WithVar_6114.Mutables.Mean(TranslocationUP));
        Length = Length % UBound(_WithVar_6114.dna);
        if (Length < 1) {
          Length = 1;
        }

        Length = Length - 1;
        Length = Length/2;
        if (t - Length < 1) {
goto skip;
        }
        if (t + Length > UBound(_WithVar_6114.dna) - 1) {
goto skip;
        }

        if (Length > 0) {
          List<block> tempDNA_7031_tmp = new List<block>();
for (int redim_iter_9621=0;i<0;redim_iter_9621++) {tempDNA.Add(null);}

          second = 0;
          for(counter=t - Length; counter<t + Length; counter++) {
            tempDNA[second] = _WithVar_6114.dna(counter);
            second = second + 1;
            Next(counter);
//we now have the appropriate length of DNA in the temporary array.

//delete fragment
            Delete(_WithVar_6114.dna, t - Length, Length * 2 + 1); //Botsareus 12/11/2015 Bug fix

//open up a hole
            start = Random(ref 1, ref UBound(_WithVar_6114.dna) - 2);
            MakeSpace(_WithVar_6114.dna(), start, UBound(tempDNA) + 1); //Botsareus 12/11/2015 Bug fix

            for(counter=start + 1; counter<start + UBound(tempDNA) + 1; counter++) {
              _WithVar_6114.dna(counter) = tempDNA[counter - start - 1];
              Next(counter);

//BOTSAREUSIFIED
              _WithVar_6114.Mutations = _WithVar_6114.Mutations + 1;
              _WithVar_6114.LastMut = _WithVar_6114.LastMut + 1;
              logmutation(robn, "Translocation moved a series at" + Str(t) + Str(Length * 2 + 1) + "bps long to " + Str(start) + " during cycle" + Str(SimOpts.TotRunCycle));


            }
          }

skip:
          Next(t);

//add "end" to end of the DNA
          _WithVar_6114.dna(UBound(_WithVar_6114.dna)).tipo = 10;
          _WithVar_6114.dna(UBound(_WithVar_6114.dna)).value = 1;

getout:
      }

private static void CopyError2(ref int robn) { //Just like Copyerror but effects only special chars
  int DNAsize = 0;

  int e = 0;//counter

  int e2 = 0;//update generator (our position)


  dynamic _WithVar_1900;
  _WithVar_1900 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_1900.DnaLen) * CDbl(_WithVar_1900.Mutables.Mean(CopyErrorUP) + _WithVar_1900.Mutables.StdDev(CopyErrorUP)) / (5 * overtime); //Botsareus 3/22/2016 works like p2 now
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_1900.Mutables.mutarray(CE2UP) < floor) {
      _WithVar_1900.Mutables.mutarray(CE2UP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }

    DNAsize = DnaLen(ref _WithVar_1900.dna) - 1; //get aprox length

    List<bool> datahit = new List<bool> {}; // TODO - Specified Minimum Array Boundary Not Supported:     Dim datahit() As Boolean//operation repeat prevention

    List<bool> datahit_414_tmp = new List<bool>();
for (int redim_iter_5821=0;i<0;redim_iter_5821++) {datahit.Add(false);}
    for(e=1; e<DNAsize; e++) { //Botsareus 3/22/2016 Bugfix

//Botsareus 3/22/2016 keeps CopyError2 lengths the same as CopyError
      decimal calc_gauss = 0;

      calc_gauss = Gauss(_WithVar_1900.Mutables.StdDev(CopyErrorUP), _WithVar_1900.Mutables.Mean(CopyErrorUP));
      if (calc_gauss < 1) {
        calc_gauss = 1;
      }

      if (rndy() < (0.75m / (_WithVar_1900.Mutables.mutarray(CE2UP) / (SimOpts.MutCurrMult * calc_gauss)))) { //chance 'Botsareus 3/22/2016 works like p2 now
        do {
          e2 = Int(rndy() * DNAsize) + 1; //Botsareus 3/22/2016 Bugfix
        } while(!(datahit[e2] == false);
        datahit[e2] = true;

        ChangeDNA2(robn, e2, DNAsize); //Botsareus 4/10/2016 Less boilerplate code

      }
    }
}

private static void PointMutation2(ref int robn) { //Botsareus 12/10/2013
//assume the bot has a positive (>0) mutarray value for this

  int DNAsize = 0;

  int randompos = 0;


  dynamic _WithVar_5904;
  _WithVar_5904 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_5904.DnaLen) * CDbl(_WithVar_5904.Mutables.Mean(PointUP) + _WithVar_5904.Mutables.StdDev(PointUP)) / (400 * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_5904.Mutables.mutarray(P2UP) < floor) {
      _WithVar_5904.Mutables.mutarray(P2UP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }


    if (_WithVar_5904.age == 0 || _WithVar_5904.Point2MutCycle < _WithVar_5904.age) {
      Point2MutWhen(rndy(), robn);
    }

//Do it again in case we get two point mutations in a single cycle
    While(_WithVar_5904.age == _WithVar_5904.Point2MutCycle && _WithVar_5904.age > 0& _WithVar_5904.DnaLen > 1); // Avoid endless loop when .age = 0 and/or .DNALen = 1

//sysvar mutation
    DNAsize = DnaLen(ref _WithVar_5904.dna) - 1; //get aprox length
    randompos = Int(rndy() * DNAsize) + 1; //Botsareus 3/22/2016 Bug fix

    ChangeDNA2(robn, randompos, DNAsize, true); //Botsareus 4/10/2016 Less boilerplate code

    Point2MutWhen(rndy(), robn);
    Wend();
}

private static void PointMutation(ref int robn) {
//assume the bot has a positive (>0) mutarray value for this

  decimal temp = 0;

  int temp2 = 0;


  dynamic _WithVar_6253;
  _WithVar_6253 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_6253.DnaLen) * CDbl(_WithVar_6253.Mutables.Mean(PointUP) + _WithVar_6253.Mutables.StdDev(PointUP)) / (400 * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_6253.Mutables.mutarray(PointUP) < floor) {
      _WithVar_6253.Mutables.mutarray(PointUP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }


    if (_WithVar_6253.age == 0 || _WithVar_6253.PointMutCycle < _WithVar_6253.age) {
      PointMutWhereAndWhen(rndy(), robn, _WithVar_6253.PointMutBP);
    }

//Do it again in case we get two point mutations in a single cycle
    While(_WithVar_6253.age == _WithVar_6253.PointMutCycle && _WithVar_6253.age > 0& _WithVar_6253.DnaLen > 1); // Avoid endless loop when .age = 0 and/or .DNALen = 1
    temp = Gauss(_WithVar_6253.Mutables.StdDev(PointUP), _WithVar_6253.Mutables.Mean(PointUP));
    temp2 = Int(temp) % 32000; //<- Overflow was here when huge single is assigned to a Long
    ChangeDNA(robn, _WithVar_6253.PointMutBP, temp2, _WithVar_6253.Mutables.PointWhatToChange);
    PointMutWhereAndWhen(rndy(), robn, _WithVar_6253.PointMutBP);
    Wend();
}

private static void Point2MutWhen(ref decimal randval, ref int robn) {
  decimal result = 0;


  decimal mutation_rate = 0;


//If randval = 0 Then randval = 0.0001
  dynamic _WithVar_655;
  _WithVar_655 = rob(robn);
    if (_WithVar_655.DnaLen == 1) {
goto getout; // avoid divide by 0 below
    }

    mutation_rate = _WithVar_655.Mutables.mutarray(P2UP) / SimOpts.MutCurrMult;

//keeps Point2 lengths the same as Point Botsareus 1/14/2014 Checking to make sure value is >= 1
    decimal calc_gauss = 0;

    calc_gauss = Gauss(_WithVar_655.Mutables.StdDev(PointUP), _WithVar_655.Mutables.Mean(PointUP));
    if (calc_gauss < 1) {
      calc_gauss = 1;
    }

    mutation_rate = mutation_rate / calc_gauss;

    mutation_rate = mutation_rate * 1.33m; //Botsareus 4/19/2016 Adjust because changedna2 may write 2 commands

//Here we test to make sure the probability of a point mutation isn't crazy high.
//A value of 1 is the probability of mutating every base pair every 1000 cycles
//Lets not let it get lower than 1 shall we?
    if (mutation_rate < 1 && mutation_rate > 0) {
      mutation_rate = 1;
    }

//result = offset + Fix(Log(randval) / Log(1 - 1 / (1000 * .Mutables.mutarray(PointUP))))
    result = Log(1 - randval) / Log(1 - 1 / (1000 * mutation_rate));
    While(result > 1800000000); //Botsareus 3/15/2013 overflow fix
    result = result - 1800000000;
    Wend();
    _WithVar_655.Point2MutCycle = _WithVar_655.age + result / (_WithVar_655.DnaLen - 1);
getout:
}

private static void PointMutWhereAndWhen(ref decimal randval, ref int robn, ref int offset_UNUSED) {
  decimal result = 0;


  decimal mutation_rate = 0;


//If randval = 0 Then randval = 0.0001
  dynamic _WithVar_9342;
  _WithVar_9342 = rob(robn);
    if (_WithVar_9342.DnaLen == 1) {
goto getout; // avoid divide by 0 below
    }

    mutation_rate = _WithVar_9342.Mutables.mutarray(PointUP) / SimOpts.MutCurrMult;

//Here we test to make sure the probability of a point mutation isn't crazy high.
//A value of 1 is the probability of mutating every base pair every 1000 cycles
//Lets not let it get lower than 1 shall we?
    if (mutation_rate < 1 && mutation_rate > 0) {
      mutation_rate = 1;
    }

//result = offset + Fix(Log(randval) / Log(1 - 1 / (1000 * .Mutables.mutarray(PointUP))))
    result = Log(1 - randval) / Log(1 - 1 / (1000 * mutation_rate));
    While(result > 1800000000); //Botsareus 3/15/2013 overflow fix
    result = result - 1800000000;
    Wend();
    _WithVar_9342.PointMutBP = (result % (_WithVar_9342.DnaLen - 1)) + 1; //note that DNA(DNALen) = end.
//We don't mutate end.  Also note that DNA does NOT start at 0th element
    _WithVar_9342.PointMutCycle = _WithVar_9342.age + result / (_WithVar_9342.DnaLen - 1);
getout:
}

private static void DeltaMut(ref int robn) {
  int temp = 0;

  decimal newval = 0;// EricL Made newval Single instead of Long.


  dynamic _WithVar_7278;
  _WithVar_7278 = rob(robn);

    if (rndy() > 1 - 1 / (100 * _WithVar_7278.Mutables.mutarray(DeltaUP) / SimOpts.MutCurrMult)) {
      if (_WithVar_7278.Mutables.StdDev(DeltaUP) == 0) {
        _WithVar_7278.Mutables.Mean(DeltaUP) = 50;
      }
      if (_WithVar_7278.Mutables.Mean(DeltaUP) == 0) {
        _WithVar_7278.Mutables.Mean(DeltaUP) = 25;
      }

//temp = Random(0, 20)
      do {
        temp = Random(ref 0, ref 10); //Botsareus 12/14/2013 Added new mutations
      } while(!(_WithVar_7278.Mutables.mutarray(temp) <= 0);

      do {
        newval = Gauss(_WithVar_7278.Mutables.Mean(DeltaUP), _WithVar_7278.Mutables.mutarray(temp));
      } while(!(_WithVar_7278.Mutables.mutarray(temp) == newval || newval <= 0);

      logmutation(robn, "Delta mutations changed " + MutationType(temp) + " from 1 in" + Str(_WithVar_7278.Mutables.mutarray(temp)) + " to 1 in" + Str(newval));
      _WithVar_7278.Mutations = _WithVar_7278.Mutations + 1;
      _WithVar_7278.LastMut = _WithVar_7278.LastMut + 1;
      _WithVar_7278.Mutables.mutarray(temp) = newval;
    }

}

private static void CopyError(ref int robn) {
  int t = 0;

  int Length = 0;


  dynamic _WithVar_5090;
  _WithVar_5090 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_5090.DnaLen) * CDbl(_WithVar_5090.Mutables.Mean(CopyErrorUP) + _WithVar_5090.Mutables.StdDev(CopyErrorUP)) / (25 * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_5090.Mutables.mutarray(CopyErrorUP) < floor) {
      _WithVar_5090.Mutables.mutarray(CopyErrorUP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }

    for(t=1; t<(_WithVar_5090.DnaLen - 1); t++) { //note that DNA(.dnalen) = end, and we DON'T mutate that.

      if (rndy() < 1 / (rob(robn).Mutables.mutarray(CopyErrorUP) / SimOpts.MutCurrMult)) {
        Length = Gauss(rob(robn).Mutables.StdDev(CopyErrorUP), rob(robn).Mutables.Mean(CopyErrorUP)); //length

        ChangeDNA(robn, t, Length, rob(robn).Mutables.CopyErrorWhatToChange, CopyErrorUP);
      }
      Next(t);

  }

private static void ChangeDNA2(ref int robn, int nth, int DNAsize, ref bool IsPoint) {
  int randomsysvar = 0;

  string holddetail = "";

  bool special = false;

  dynamic _WithVar_3146;
  _WithVar_3146 = rob(robn);

//for .tieloc, .shoot, and functional
    do {
      randomsysvar = Int(rndy() * 256);
    } while(!(sysvarOUT(randomsysvar).Name != "");

    special = false;
//special cases
    if (nth < DNAsize - 2) {
//for .shoot store
      if (_WithVar_3146.dna(nth + 1).tipo == 0& _WithVar_3146.dna(nth + 1).value == shoot && _WithVar_3146.dna(nth + 2).tipo == 7 && _WithVar_3146.dna(nth + 2).value == 1) {
        _WithVar_3146.dna(nth).value = Choose(Int(rndy() * 7) + 1, -1, -2, -3, -4, -6, -8, sysvar(randomsysvar).value); //Botsareus 10/6/2015 Better values
        _WithVar_3146.dna(nth).tipo = 0;
        holddetail = " changed dna location " + nth + " to " + _WithVar_3146.dna(nth).value;
        special = true;
      }
//for .focuseye store
      if (_WithVar_3146.dna(nth + 1).tipo == 0& _WithVar_3146.dna(nth + 1).value == FOCUSEYE && _WithVar_3146.dna(nth + 2).tipo == 7 && _WithVar_3146.dna(nth + 2).value == 1) {
        _WithVar_3146.dna(nth).value = Int(rndy() * 9) - 4;
        _WithVar_3146.dna(nth).tipo = 0;
        holddetail = " changed dna location " + nth + " to " + _WithVar_3146.dna(nth).value;
        special = true;
      }
//for .tieloc store
      if (_WithVar_3146.dna(nth + 1).tipo == 0& _WithVar_3146.dna(nth + 1).value == tieloc && _WithVar_3146.dna(nth + 2).tipo == 7 && _WithVar_3146.dna(nth + 2).value == 1) {
        _WithVar_3146.dna(nth).value = Choose(Int(rndy() * 5) + 1, -1, -3, -4, -6, sysvar(randomsysvar).value); //Botsareus 10/6/2015 Better values 'Botsareus 3/22/2016 Better values
        _WithVar_3146.dna(nth).tipo = 0;
        holddetail = " changed dna location " + nth + " to " + _WithVar_3146.dna(nth).value;
        special = true;
      }
    }

    if (special) {
      logmutation(robn, IIf(IsPoint, "Point Mutation 2", "Copy Error 2") + holddetail + " during cycle" + Str(SimOpts.TotRunCycle));
      _WithVar_3146.Mutations = _WithVar_3146.Mutations + 1;
      _WithVar_3146.LastMut = _WithVar_3146.LastMut + 1;
    } else { //other cases
      if (nth < DNAsize - 1 && Int(rndy() * 3) == 0) { //1/3 chance functional

        _WithVar_3146.dna(nth).tipo = 0;
        _WithVar_3146.dna(nth).value = sysvarOUT(randomsysvar).value;
        holddetail = " changed dna location " + nth + " to number ." + sysvarOUT(randomsysvar).Name;
        logmutation(robn, IIf(IsPoint, "Point Mutation 2", "Copy Error 2") + holddetail + " during cycle" + Str(SimOpts.TotRunCycle));
        _WithVar_3146.Mutations = _WithVar_3146.Mutations + 1;
        _WithVar_3146.LastMut = _WithVar_3146.LastMut + 1;

        _WithVar_3146.dna(nth + 1).tipo == 7;
        _WithVar_3146.dna(nth + 1).value == 1;
        holddetail = " changed dna location " + (nth + 1) + " to store";
        logmutation(robn, IIf(IsPoint, "Point Mutation 2", "Copy Error 2") + holddetail + " during cycle" + Str(SimOpts.TotRunCycle));
        _WithVar_3146.Mutations = _WithVar_3146.Mutations + 1;
        _WithVar_3146.LastMut = _WithVar_3146.LastMut + 1;

      } else { //2/3 chance informational
        if (Int(rndy() * 5) == 0) { //1/5 chance large number (but still use a sysvar, if anything the parse will mod it)
          do {
            randomsysvar = Int(rndy() * 1000);
          } while(!(sysvar(randomsysvar).Name != "");
          _WithVar_3146.dna(nth).tipo = 0;
          _WithVar_3146.dna(nth).value = sysvar(randomsysvar).value + Int(rndy() * 32) * 1000;
          holddetail = " changed dna location " + nth + " to number " + _WithVar_3146.dna(nth).value;
        } else {
          do {
            randomsysvar = Int(rndy() * 256);
          } while(!(sysvarIN(randomsysvar).Name != "");
          _WithVar_3146.dna(nth).tipo = 1;
          _WithVar_3146.dna(nth).value = sysvarIN(randomsysvar).value;
          holddetail = " changed dna location " + nth + " to *number *." + sysvarIN(randomsysvar).Name;
        }
        logmutation(robn, IIf(IsPoint, "Point Mutation 2", "Copy Error 2") + holddetail + " during cycle" + Str(SimOpts.TotRunCycle));
        _WithVar_3146.Mutations = _WithVar_3146.Mutations + 1;
        _WithVar_3146.LastMut = _WithVar_3146.LastMut + 1;
      }
    }
}

private static void ChangeDNA(ref int robn, int nth_UNUSED, int Length_UNUSED= 1, int PointWhatToChange= 50, ref int Mtype) {
//we need to rework .lastmutdetail
  int Max = 0;

  string temp = "";

  block bp = null;

  block tempbp = null;

  string Name = "";

  string oldname = "";

  int t = 0;

  int old = 0;


  dynamic _WithVar_1153;
  _WithVar_1153 = rob(robn);

    for(t=nth; t<(nth + Length - 1); t++) { //if length is 1, it's only one bp we're mutating, remember?
      if (t >= _WithVar_1153.DnaLen) {
goto getout; //don't mutate end either
      }
      if (_WithVar_1153.dna(t).tipo == 10) {
goto getout; //mutations can't cross control barriers
      }

      if (Random(ref 0, ref 99) < PointWhatToChange) {
//''''''''''''''''''''''''''''''''''''''''
//Mutate VALUE
//''''''''''''''''''''''''''''''''''''''''
        if (_WithVar_1153.dna(t).value && Mtype == InsertionUP) {
//Insertion mutations should get a good range for value.
//Don't worry, this will get "mod"ed for non number commands.
//This doesn't count as a mutation, since the whole:
// -- Add an element, set it's tipo and value to random stuff -- is a SINGLE mutation
//we'll increment mutation counters and .lastmutdetail later.
          _WithVar_1153.dna(t).value = Gauss(500, 0); //generates values roughly between -1000 and 1000
        }


        old = _WithVar_1153.dna(t).value;
        if (_WithVar_1153.dna(t).tipo == 0 || _WithVar_1153.dna(t).tipo == 1) { //(number or *number)
          do {
            if (Abs(old) <= 1000) { //Botsareus 3/19/2016 Simplified
              if (Int(rndy() * 2) == 0) { //1/2 chance the mutation is large
                _WithVar_1153.dna(t).value = Gauss(94, _WithVar_1153.dna(t).value);
              } else {
                _WithVar_1153.dna(t).value = Gauss(7, _WithVar_1153.dna(t).value);
              }
            } else {
              _WithVar_1153.dna(t).value = Gauss(old / 10, _WithVar_1153.dna(t).value); //for very large numbers scale gauss
            }
          } while(!(_WithVar_1153.dna(t).value == old);

          _WithVar_1153.Mutations = _WithVar_1153.Mutations + 1;
          _WithVar_1153.LastMut = _WithVar_1153.LastMut + 1;

          logmutation(robn, MutationType(Mtype) + " changed " + TipoDetok(_WithVar_1153.dna(t).tipo) + " from" + Str(old) + " to" + Str(_WithVar_1153.dna(t).value) + " at position" + Str(t) + " during cycle" + Str(SimOpts.TotRunCycle));

        } else {
//find max legit value
//this should really be done a better way
          bp.tipo = _WithVar_1153.dna(t).tipo;
          Max() = 0;
          do {
            temp = "";
            Max() = Max() + 1;
            bp.value = Max();
            Parse(temp, bp);
          } while(!(temp != "");
          Max() = Max() - 1;
          if (Max() <= 1) {
goto getout; //failsafe in case its an invalid type or there's no way to mutate it
          }

          do {
            _WithVar_1153.dna(t).value = Random(ref 1, ref Max());
          } while(!(_WithVar_1153.dna(t).value == old);

          bp.tipo = _WithVar_1153.dna(t).tipo;
          bp.value = old;

          tempbp = _WithVar_1153.dna(t);

          Name = "";
          oldname = "";
          Parse(Name, tempbp); // Have to use a temp var because Parse() can change the arguments
          Parse(oldname, bp);

          _WithVar_1153.Mutations = _WithVar_1153.Mutations + 1;
          _WithVar_1153.LastMut = _WithVar_1153.LastMut + 1;

          logmutation(robn, MutationType(Mtype) + " changed value of " + TipoDetok(_WithVar_1153.dna(t).tipo) + " from " + oldname + " to " + Name + " at position" + Str(t) + " during cycle" + Str(SimOpts.TotRunCycle));
        }
      } else {
        bp.tipo = _WithVar_1153.dna(t).tipo;
        bp.value = _WithVar_1153.dna(t).value;
        do {
          _WithVar_1153.dna(t).tipo = Random(ref 0, ref 20);
        } while(!(_WithVar_1153.dna(t).tipo == bp.tipo || TipoDetok(_WithVar_1153.dna(t).tipo) == "");
        Max() = 0;
        if (_WithVar_1153.dna(t).tipo >= 2) {
          do {
            temp = "";
            Max() = Max() + 1;
            _WithVar_1153.dna(t).value = Max();
            Parse(temp, _WithVar_1153.dna(t));
          } while(!(temp != "");
          Max() = Max() - 1;
          if (Max() <= 1) {
goto getout; //failsafe in case its an invalid type or there's no way to mutate it
          }
          _WithVar_1153.dna(t).value = ((Abs(bp.value) - 1) % Max()) + 1; //put values in range 'Botsareus 4/10/2016 Bug fix
          if (_WithVar_1153.dna(t).value == 0) {
            _WithVar_1153.dna(t).value = 1;
          }
        } else {
//we do nothing, it has to be in range
        }
        tempbp = _WithVar_1153.dna(t);

        Name = "";
        oldname = "";
        Parse(Name, tempbp); // Have to use a temp var because Parse() can change the arguments
        Parse(oldname, bp);
        _WithVar_1153.Mutations = _WithVar_1153.Mutations + 1;
        _WithVar_1153.LastMut = _WithVar_1153.LastMut + 1;

        logmutation(robn, MutationType(Mtype) + " changed the " + TipoDetok(bp.tipo) + ": " + oldname + " to the " + TipoDetok(_WithVar_1153.dna(t).tipo) + ": " + Name + " at position" + Str(t) + " during cycle" + Str(SimOpts.TotRunCycle));

      }
      Next(t);
getout:
  }

private static void Insertion(ref int robn) {
  int location = 0;

  int Length = 0;

  int accum = 0;

  int t = 0;


  dynamic _WithVar_9626;
  _WithVar_9626 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_9626.DnaLen) * CDbl(_WithVar_9626.Mutables.Mean(InsertionUP) + _WithVar_9626.Mutables.StdDev(InsertionUP)) / (5 * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_9626.Mutables.mutarray(InsertionUP) < floor) {
      _WithVar_9626.Mutables.mutarray(InsertionUP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }

    for(t=1; t<(_WithVar_9626.DnaLen - 1); t++) {
      if (rndy() < 1 / (_WithVar_9626.Mutables.mutarray(InsertionUP) / SimOpts.MutCurrMult)) {
        if (_WithVar_9626.Mutables.Mean(InsertionUP) == 0) {
          _WithVar_9626.Mutables.Mean(InsertionUP) = 1;
        }
        do {
          Length = Gauss(_WithVar_9626.Mutables.StdDev(InsertionUP), _WithVar_9626.Mutables.Mean(InsertionUP));
        } while(!(Length <= 0);

        if (CLng(rob(robn).DnaLen) + CLng(Length) > 32000) {
return;

        }


        MakeSpace(_WithVar_9626.dna(), t + accum, Length, _WithVar_9626.DnaLen);
        rob(robn).DnaLen = rob(robn).DnaLen + Length;
        ChangeDNA(robn, t + 1 + accum, Length, 0, InsertionUP); //change the type first so that the mutated value is within the space of the new type
        ChangeDNA(robn, t + 1 + accum, Length, 100, InsertionUP); //set a good value up
        accum = Length + accum; //Botsareus 3/22/2016 Bugfix Since DNA expended move index down
      }
      Next(t);

  }

private static void Reversal(ref int robn) {
//reverses a length of DNA
  int Length = 0;

  int counter = 0;

  int location = 0;

  int low = 0;

  int high = 0;

  int templong = 0;

  block tempblock = null;

  int t = 0;

  int second = 0;


  dynamic _WithVar_7223;
  _WithVar_7223 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_7223.DnaLen) * CDbl(_WithVar_7223.Mutables.Mean(ReversalUP) + _WithVar_7223.Mutables.StdDev(ReversalUP)) / (105 * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_7223.Mutables.mutarray(ReversalUP) < floor) {
      _WithVar_7223.Mutables.mutarray(ReversalUP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }


    for(t=1; t<(_WithVar_7223.DnaLen - 1); t++) {
      if (rndy() < 1 / (_WithVar_7223.Mutables.mutarray(ReversalUP) / SimOpts.MutCurrMult)) {
        if (_WithVar_7223.Mutables.Mean(ReversalUP) < 2) {
          _WithVar_7223.Mutables.Mean(ReversalUP) = 2;
        }

        do {
          Length = Gauss(_WithVar_7223.Mutables.StdDev(ReversalUP), _WithVar_7223.Mutables.Mean(ReversalUP));
        } while(!(Length <= 0);

        Length = Length/2; //be sure we go an even amount to either side

        if (t - Length < 1) {
          Length = t - 1;
        }
        if (t + Length > _WithVar_7223.DnaLen - 1) {
          Length = _WithVar_7223.DnaLen - 1 - t;
        }
        if (Length > 0) {
          second = 0;
          for(counter=t - Length; counter<t - 1; counter++) {
            tempblock = _WithVar_7223.dna(counter);
            _WithVar_7223.dna(counter) = _WithVar_7223.dna(t + Length - second);
            _WithVar_7223.dna(t + Length - second) == tempblock;
            second = second + 1;
            Next(counter);

            _WithVar_7223.Mutations = _WithVar_7223.Mutations + 1;
            _WithVar_7223.LastMut = _WithVar_7223.LastMut + 1;

            logmutation(robn, "Reversal of" + Str(Length * 2 + 1) + "bps centered at " + Str(t) + " during cycle" + Str(SimOpts.TotRunCycle));

          }
        }
        Next(t);
    }

private static void MinorDeletion(ref int robn) {
  int Length = 0;
  int t = 0;

  dynamic _WithVar_3620;
  _WithVar_3620 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_3620.DnaLen) / (2.5m * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_3620.Mutables.mutarray(MinorDeletionUP) < floor) {
      _WithVar_3620.Mutables.mutarray(MinorDeletionUP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }


    if (_WithVar_3620.Mutables.Mean(MinorDeletionUP) < 1) {
      _WithVar_3620.Mutables.Mean(MinorDeletionUP) = 1;
    }
    for(t=1; t<(_WithVar_3620.DnaLen - 1); t++) {
      if (rndy() < 1 / (_WithVar_3620.Mutables.mutarray(MinorDeletionUP) / SimOpts.MutCurrMult)) {
        do {
          Length = Gauss(_WithVar_3620.Mutables.StdDev(MinorDeletionUP), _WithVar_3620.Mutables.Mean(MinorDeletionUP));
        } while(!(Length <= 0);

        if (t + Length > _WithVar_3620.DnaLen) {
          Length = _WithVar_3620.DnaLen - t; //Botsareus 3/22/2016 Bug fix
        }
        if (Length <= 0) {
return;//Botsareus 3/22/2016 Bugfix

        }

        Delete(_WithVar_3620.dna, t, Length, _WithVar_3620.DnaLen);

        _WithVar_3620.DnaLen = DnaLen(ref _WithVar_3620.dna());

        _WithVar_3620.Mutations = _WithVar_3620.Mutations + 1;
        _WithVar_3620.LastMut = _WithVar_3620.LastMut + 1;
        logmutation(robn, "Minor Deletion deleted a run of" + Str(Length) + " bps at position" + Str(t) + " during cycle" + Str(SimOpts.TotRunCycle));

      }
      Next(t);
  }

private static void MajorDeletion(ref int robn) {
  int Length = 0;
  int t = 0;

  dynamic _WithVar_7640;
  _WithVar_7640 = rob(robn);

    decimal floor = 0;

    floor = CDbl(_WithVar_7640.DnaLen) / (2.5m * overtime);
    floor = floor * SimOpts.MutCurrMult;
    if (_WithVar_7640.Mutables.mutarray(MajorDeletionUP) < floor) {
      _WithVar_7640.Mutables.mutarray(MajorDeletionUP) = floor; //Botsareus 10/5/2015 Prevent freezing
    }

    if (_WithVar_7640.Mutables.Mean(MajorDeletionUP) < 1) {
      _WithVar_7640.Mutables.Mean(MajorDeletionUP) = 1;
    }
    for(t=1; t<(_WithVar_7640.DnaLen - 1); t++) {
      if (rndy() < 1 / (_WithVar_7640.Mutables.mutarray(MajorDeletionUP) / SimOpts.MutCurrMult)) {
        do {
          Length = Gauss(_WithVar_7640.Mutables.StdDev(MajorDeletionUP), _WithVar_7640.Mutables.Mean(MajorDeletionUP));
        } while(!(Length <= 0);

        if (t + Length > _WithVar_7640.DnaLen) {
          Length = _WithVar_7640.DnaLen - t; //Botsareus 3/22/2016 Bugfix
        }
        if (Length <= 0) {
return;//Botsareus 3/22/2016 Bugfix

        }

        Delete(_WithVar_7640.dna, t, Length, _WithVar_7640.DnaLen);

        _WithVar_7640.DnaLen = DnaLen(ref _WithVar_7640.dna());

        _WithVar_7640.Mutations = _WithVar_7640.Mutations + 1;
        _WithVar_7640.LastMut = _WithVar_7640.LastMut + 1;
        logmutation(robn, "Major Deletion deleted a run of" + Str(Length) + " bps at position" + Str(t) + " during cycle" + Str(SimOpts.TotRunCycle));

      }
      Next(t);
  }

/*
' mutates robot colour in robot n a times
*/
private static void mutatecolors(ref int n, ref int a_UNUSED) {
  int color = 0;

  int r = 0;
  int g = 0;
  int b = 0;

  int counter = 0;


  color = rob[n].color;

  b = color/(65536);
  g = color/256 - b * 256;
  r = color - b * 65536 - g * 256;

  for(counter=1; counter<a; counter++) {
    switch((Random(ref 1, ref 3))) {
      case 1:
        b = b + (Random(ref 0, ref 1) * 2 - 1) * 20;
        break;
      case 2:
        g = g + (Random(ref 0, ref 1) * 2 - 1) * 20;
        break;
      case 3:
        r = r + (Random(ref 0, ref 1) * 2 - 1) * 20;
break;
}

    if (r > 255) {
      r = 255;
    }
    if (r < 0) {
      r = 0;
    }

    if (g > 255) {
      g = 255;
    }
    if (g < 0) {
      g = 0;
    }

    if (b > 255) {
      b = 255;
    }
    if (b < 0) {
      b = 0;
    }
    Next(counter);

    rob[n].color = b * 65536 + g * 256 + r;
  }

/*
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
*/
public static bool delgene(ref int n, ref int g) {
  bool delgene = false;
  int k = 0;
  int t = 0;

  k = rob[n].genenum;
  if (g > 0& g <= k) {
    DeleteSpecificGene(rob[n].dna, g);
    delgene = true;
    rob[n].DnaLen = DnaLen(ref rob[n].dna);
    rob[n].genenum = CountGenes(ref rob[n].dna);
    rob[n].mem(DnaLenSys) = rob[n].DnaLen;
    rob[n].mem(GenesSys) = rob[n].genenum;
    makeoccurrlist(n);
//Botsareus 3/14/2014 Disqualify
    if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 2) {
      dreason(rob[n].FName, rob[n].tag, "deleting a gene");
    }
    if (!SimOpts.F1 && rob[n].dq == 1 && Disqualify == 2) {
      rob[n].Dead = true; //safe kill robot
    }
  }
  return delgene;
}

/*
'Botsareus 10/5/2015 Replaced with something better
''Botsareus 9/16/2014 Save a backup of the gene that is about to get deleted
'Private Function SaveSpecificGene(ByVal n As Integer, ByVal k As Integer) As Boolean
'  Dim i As Long, f As Long
'  Dim t As Integer
'  Dim b As Integer
'  Dim datacount As Long

'  With rob[n]

'  'Make sure we have a sane memory limit
'  datacount = UBound(.dna)
'  For i = 0 To UBound(.delgenes)
'    datacount = datacount + UBound(.delgenes(i).dna)
'  Next
'  If datacount > 32000 Then GoTo getout

'  i = genepos(.dna, k)
'  If i < 0 Then GoTo getout
'  f = GeneEnd(.dna, i)

'  t = UBound(.delgenes) + 1
'  ReDim Preserve .delgenes(t)
'  .delgenes(t).position = i

'  ReDim .delgenes(t).dna(f - i + 1)
'  For b = i To f + 1
'    .delgenes(t).dna(b - i) = .dna(b)
'  Next

'  End With
'SaveSpecificGene = True
'Exit Function
'getout:
'SaveSpecificGene = False
'End Function
*/
private static void DeleteSpecificGene(ref dynamic dna(_UNUSED) {
  int i = 0;
  int f = 0;


  i = genepos(ref dna, k);
  if (i < 0) {
goto getout;
  }
  f = GeneEnd(ref dna, i);
  Delete(dna, i, f - i + 1); // EricL Added +1
getout:
}

public static void SetDefaultMutationRates(ref mutationprobs changeme, ref bool skipNorm) {
//Botsareus 12/17/2013 Figure out dna length
  int Length = 0;

  string path = "";

  if (NormMut && !skipNorm) {
    if (OptionsForm.instance.CurrSpec == 50 || OptionsForm.instance.CurrSpec == -1) { //only if current spec is selected
      Length = rob(robfocus).DnaLen;
    } else { //load dna length
      if (MaxRobs == 0) {
        List<> rob_3508_tmp = new List<>();
for (int redim_iter_5070=0;i<0;redim_iter_5070++) {rob.Add(null);}
      }
      path = TmpOpts.Specie(optionsform.CurrSpec).path + "\\" + TmpOpts.Specie(optionsform.CurrSpec).Name;
      path = Replace(path, "&#", MDIForm1.instance.MainDir);
      if (dir(path) == "") {
        path = MDIForm1.instance.MainDir + "\\Robots\\" + TmpOpts.Specie(optionsform.CurrSpec).Name;
      }
      if (LoadDNA(ref path, ref 0)) {
        Length = DnaLen(ref rob(0).dna);
      }
    }
  }

  int a = 0;

  dynamic _WithVar_6483;
  _WithVar_6483 = (changeme);

    for(a=0; a<20; a++) {
      _WithVar_6483.mutarray(a) = IIf(NormMut && !skipNorm, Length * CLng(valNormMut), 5000);
      _WithVar_6483.Mean(a) = 1;
      _WithVar_6483.StdDev(a) = 0;
      Next(a);
      if (skipNorm) {
        _WithVar_6483.mutarray(P2UP) = 0; //Botsareus 2/21/2014 Might as well disable p2 mutations if loading from the net
      }

      SetDefaultLengths(changeme);
  }

public static void SetDefaultLengths(ref mutationprobs changeme) {
  dynamic _WithVar_538;
  _WithVar_538 = (changeme);

    _WithVar_538.Mean(PointUP) = 3;
    _WithVar_538.StdDev(PointUP) = 1;

    _WithVar_538.Mean(DeltaUP) = 500;
    _WithVar_538.StdDev(DeltaUP) = 150;

    _WithVar_538.Mean(MinorDeletionUP) = 1;
    _WithVar_538.StdDev(MinorDeletionUP) = 0;

    _WithVar_538.Mean(InsertionUP) = 1;
    _WithVar_538.StdDev(InsertionUP) = 0;

    _WithVar_538.Mean(CopyErrorUP) = 1;
    _WithVar_538.StdDev(CopyErrorUP) = 0;

    _WithVar_538.Mean(MajorDeletionUP) = 3;
    _WithVar_538.StdDev(MajorDeletionUP) = 1;

    _WithVar_538.Mean(ReversalUP) = 3;
    _WithVar_538.StdDev(ReversalUP) = 1;

    _WithVar_538.CopyErrorWhatToChange = 80;
    _WithVar_538.PointWhatToChange = 80;

    _WithVar_538.Mean(AmplificationUP) = 250;
    _WithVar_538.StdDev(AmplificationUP) = 75;

    _WithVar_538.Mean(TranslocationUP) = 250;
    _WithVar_538.StdDev(TranslocationUP) = 75;

}
}
