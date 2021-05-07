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


static class DNAManipulations {
// Option Explicit
//All functions that manipulate DNA without actually mutating it should go here.
//That is, anything that searches DNA, etc.
// loads a dna file, inserting the robot in the simulation


public static int RobScriptLoad( string path) {
  var n = posto();
  preparerob(n, path); // prepares structure
  if (LoadDNA( path,  n)) { // loads and parses dna
    insertsysvars(n); // count system vars among used vars
    ScanUsedVars(n); // count other used locations
    makeoccurrlist(n); // creates the ref* array
    rob[n].DnaLen = DnaLen( rob[n].dna()); // measures dna length
    rob[n].genenum = CountGenes( rob[n].dna());
    rob[n].mem(DnaLenSys) = rob[n].DnaLen;
    rob[n].mem(GenesSys) = rob[n].genenum;
    return n; // returns the index of the created rob
  } else {
    rob[n].exist = false;
    UpdateBotBucket(n);
    return -1;
  }
}

/*
' prepares with some values the struct of a new rob
*/
private static void preparerob(ref int t, ref string path) {
  int col1 = 0;
  int col2 = 0;
  int col3 = 0;

  int k = 0;

  rob(t).pos.x = Random(ref 50, ref Form1.ScaleWidth);
  rob(t).pos.y = Random(ref 50, ref Form1.ScaleHeight);
  rob(t).aim = Random(ref 0, ref 628) / 100;
  rob(t).aimvector = VectorSet(Cos(rob(t).aim), Sin(rob(t).aim));
  rob(t).exist = true;
  rob(t).BucketPos.x = -2;
  rob(t).BucketPos.y = -2;
  UpdateBotBucket(t);

  col1 = Random(ref 50, ref 255);
  col2 = Random(ref 50, ref 255);
  col3 = Random(ref 50, ref 255);
  rob(t).color = col1 * 65536 + col2 * 256 + col3;

  rob(t).vnum = 1;
//rob(t).st.pos = 1
  rob(t).nrg = 20000;
  rob(t).Veg = false;
  k = 1;
  While(InStr(k, path, "\\") > 0);
  k = k + 1;
  Wend();
  rob(t).FName = Right((path, Len(path) - k + 1,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +);
}

public static block) As Boolean IsRobDNABounded(ref dynamic ArrayIn(_UNUSED) {
  block) As Boolean IsRobDNABounded = null;
  // TODO (not supported):   On Error GoTo done
  IsRobDNABounded = false;
  IsRobDNABounded = (UBound(ArrayIn) >= LBound(ArrayIn));
done:
  return IsRobDNABounded;
}

public static block) As Integer DnaLen(ref dynamic dna(_UNUSED) {
  block) As Integer DnaLen = null;
  DnaLen = 1;
  While(!(dna(DnaLen).tipo == 10& dna(DnaLen).value == 1) && DnaLen <= 32000& DnaLen < UBound(dna)); //Botsareus 6/16/2012 Added upper bounds check
  DnaLen = DnaLen + 1;
  Wend();

//If DnaLen = 32000 Then 'Botsareus 5/29/2012 removed pointless code
//DnaLen = 32000
//End If
  return DnaLen;
}

/*
' compiles a list of used locations
' (used to introduce gradually new locations
' with mutation, but abandoned)
*/
public static void ScanUsedVars(ref int n) {
  int t = 0;

  int k = 0;

  int a = 0;

  bool used = false;

  used = false;
  While(!(rob[n].dna(t).tipo == 10& rob[n].dna(t).value == 1));
  t = t + 1;
  if (UBound(rob[n].dna()) < t) {
goto getout;
  }
  if (rob[n].dna(t).tipo == 1) {
    a = rob[n].dna(t).value;
    for(k=1; k<rob[n].maxusedvars; k++) {
      if (rob[n].usedvars(k) == a) {
        used = true;
      }
      Next(k);
      if (!used) {
        rob[n].maxusedvars = rob[n].maxusedvars + 1;
        if (UBound(rob[n].usedvars()) >= rob[n].maxusedvars) {
          rob[n].usedvars(rob[n].maxusedvars) = a;
        }
      }
      used = false;
    }
    Wend();
getout:
  }

/*
' inserts sysvars among used vars
*/
public static void insertsysvars(ref int n) {
  int t = 0;

  t = 1;
  While(sysvar(t).Name != "");
  rob[n].usedvars(t) = sysvar(t).value;
  t = t + 1;
  Wend();
  rob[n].maxusedvars = t - 1;
}

/*
' inserts a new private variable in the private vars list
*/
public static void insertvar(ref int n, out string a) {
  string b = "";

  string c = "";

  int pos = 0;

  a = Right(a, Len(a) - 4);
  pos = InStr(a, " ");
  b = Left(a, pos - 1);
  c = Right(a, Len(a) - pos);
  rob[n].vars(rob[n].vnum).Name = b;
  rob[n].vars(rob[n].vnum).value = val(c);
  rob[n].vnum = rob[n].vnum + 1;
}

public static void interpretUSE(ref int n, out string a) {
  string b = "";

  int pos = 0;

  a = Right(a, Len(a) - 4);

  if ((a == "NewMove")) {
    rob[n].NewMove = true;
  }
}

/*
'''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''handle the stacks ''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''

'intstack.pos points to the Least Upper Bound element of the stack
*/
public static void PushIntStack(int value) {
  int a = 0;


  if (IntStack.pos >= 101) { //next push will overfill
    for(a=0; a<99; a++) {
      IntStack.val(a) = IntStack.val(a + 1);
      Next(a);
      IntStack.val(100) = 0;
      IntStack.pos = 100;
    }

    IntStack.val(IntStack.pos) = value;
    IntStack.pos = IntStack.pos + 1;
  }

public static int PopIntStack() {
  int PopIntStack = 0;
  IntStack.pos = IntStack.pos - 1;

  if (IntStack.pos == -1) {
    IntStack.pos = 0;
    IntStack.val(0) = 0;
  }

  PopIntStack = IntStack.val(IntStack.pos);
  return PopIntStack;
}

public static void ClearIntStack() {
  IntStack.pos = 0;
  IntStack.val(0) = 0;
}

public static void DupIntStack() {
  int a = 0;


  if (IntStack.pos == 0) {
return;

  } else {
    a = PopIntStack();
    PushIntStack(a);
    PushIntStack(a);
  }
}

public static void SwapIntStack() {
  int a = 0;

  int b = 0;


  if (IntStack.pos <= 1) { // 1 or 0 values on the stack
return;

  } else {
    a = PopIntStack();
    b = PopIntStack();
    PushIntStack(a);
    PushIntStack(b);
  }
}

public static void OverIntStack() {
//a b -> a b a

  int a = 0;

  int b = 0;


  if (IntStack.pos == 0) {
return;

  }
  if (IntStack.pos == 1) { // 1 value on the stack
    PushIntStack(0);
return;

  } else {
    b = PopIntStack();
    a = PopIntStack();
    PushIntStack(a);
    PushIntStack(b);
    PushIntStack(a);
  }
}

public static void PushBoolStack(bool value) { //change to a linked list so there is no stack limit or wasted memory soemtime in the future
  int a = 0;


  if (Condst.pos >= 101) { //next push will overfill
    for(a=0; a<99; a++) {
      Condst.val(a) = Condst.val(a + 1);
      Next(a);
      Condst.val(100) = 0;
      Condst.pos = 100;
    }

    Condst.val(Condst.pos) = value;
    Condst.pos = Condst.pos + 1;
  }

public static void ClearBoolStack() {
  Condst.pos = 0;
  Condst.val(0) = 0;
}

public static void DupBoolStack() {
  bool a = false;


  if (Condst.pos == 0) {
return;

  } else {
    a = PopBoolStack();
    PushBoolStack(a);
    PushBoolStack(a);
  }
}

public static void SwapBoolStack() {
  bool a = false;

  bool b = false;


  if (Condst.pos <= 1) {
return;//Do nothing

  } else { // 2 or more things on stack
    a = PopBoolStack();
    b = PopBoolStack();
    PushBoolStack(a);
    PushBoolStack(b);
  }
}

public static void OverBoolStack() {
//a b -> a b a
  bool a = false;

  bool b = false;


  if (Condst.pos == 0) {
return;//Do nothing.  Nothing on stack.

  }
  if (Condst.pos == 1) { //Only 1 thing on stack.
    PushBoolStack(true);
return;

  } else {
    b = PopBoolStack();
    a = PopBoolStack();
    PushBoolStack(a);
    PushBoolStack(b);
    PushBoolStack(a);
  }
}

public static int PopBoolStack() {
  int PopBoolStack = 0;
  Condst.pos = Condst.pos - 1;

  if (Condst.pos == -1) {
    Condst.pos = 0;
    PopBoolStack = -5;
    return PopBoolStack;//returns a weird value if there's nothing on the stack

  }

  PopBoolStack = Condst.val(Condst.pos);
  return PopBoolStack;
}

public static block) As Integer CountGenes(ref dynamic dna(_UNUSED) {
  block) As Integer CountGenes = null;
  int counter = 0;

  int k = 0;

  int genenum = 0;

  bool ingene = false;


  ingene = false;

  counter = 1;

  While(counter <= 32000& counter <= UBound(dna)); //Botsareus 5/29/2012 Added upper bounds check
  if (dna(counter).tipo == 10& dna(counter).value == 1) {
goto getout;
  }
// If a Start or Else
  if (dna(counter).tipo == 9 && (dna(counter).value == 2 || dna(counter).value == 3)) {
    if (!ingene) { //that does not follow a Cond
      CountGenes = CountGenes + 1;
    }
    ingene = false; // that follows a cond
  }
// If a Cond
  if (dna(counter).tipo == 9 && (dna(counter).value == 1)) {
    ingene = true;
    CountGenes = CountGenes + 1;
  }
// If a stop
  if (dna(counter).tipo == 9 && dna(counter).value == 4) {
    ingene = false;
  }
  counter = counter + 1;
  Wend();
getout:
  return CountGenes;
}

public static block, ByVal inizio As Long) As Integer NextStop(ref dynamic dna(_UNUSED) {
  block, ByVal inizio As Long) As Integer NextStop = null;
  NextStop = inizio;
  While(!((dna(NextStop).tipo == 9 && (dna(NextStop).value == 4)) || dna(NextStop).tipo == 10) && (NextStop <= 32000));
//_ And (DNA(NextStop).value = 2 Or DNA(NextStop).value = 3 Or DNA(NextStop).value = 4))
  NextStop = NextStop + 1;
  Wend();
  return NextStop;
}

/*
'Returns the position of the last base pair of the gene beginnign at position
*/
public static block, ByVal position As Integer) As Integer GeneEnd(ref dynamic dna(_UNUSED) {
  block, ByVal position As Integer) As Integer GeneEnd = null;
  bool condgene = false;

  condgene = false;

  GeneEnd = position();
  if (dna(GeneEnd).tipo == 9 && dna(GeneEnd).value == 1) {
    condgene = true;
  }

  While(GeneEnd + 1 <= 32000);
  if ((dna(GeneEnd + 1).tipo == 10)) {
goto getout; // end of genome
  }
  if ((dna(GeneEnd + 1).tipo == 9 && ((dna(GeneEnd + 1).value == 1) || dna(GeneEnd + 1).value == 4))) { // cond or stop
    if ((dna(GeneEnd + 1).value == 4)) {
      GeneEnd = GeneEnd + 1; // Include the stop as part of the gene
    }
goto ;
  }
  if ((dna(GeneEnd + 1).tipo == 9 && ((dna(GeneEnd + 1).value == 2) || dna(GeneEnd + 1).value == 3)) && !condgene) {
goto getout; // start or else
  }
  if ((dna(GeneEnd + 1).tipo == 9 && ((dna(GeneEnd + 1).value == 2) || dna(GeneEnd + 1).value == 3)) && condgene) {
    condgene = false; // start or else
  }
  GeneEnd = GeneEnd + 1;
  if ((GeneEnd + 1) > UBound(dna)) {
goto getout; //Botsareus 5/29/2012 Added upper bounds check
  }
  Wend();
getout:
  return GeneEnd;
}

public static block, ByVal inizio As Long) As Integer PrevStop(ref dynamic dna(_UNUSED) {
  block, ByVal inizio As Long) As Integer PrevStop = null;
  PrevStop = inizio;
  While(!((dna(PrevStop).tipo == 9 && dna(PrevStop).value != 4) || dna(PrevStop).tipo == 10));
  PrevStop = PrevStop - 1;
  if (PrevStop < 1) {
goto getout;
  }
  Wend();
getout:
  return PrevStop;
}

/*
'returns position of gene n
*/
public static block, ByVal n As Integer) As Integer genepos(ref dynamic dna(_UNUSED) {
  block, ByVal n As Integer) As Integer genepos = null;
  int k = 0;

  int genenum = 0;

  bool ingene = false;


  ingene = false;
  genepos = 0;
  k = 1;

  if (n == 0) {
    genepos = 0;
goto ;
  }

  While(k > 0& genepos == 0& k <= 32000);
//A start or else
  if (dna(k).tipo == 9 && (dna(k).value == 2 || dna(k).value == 3)) {
    if (!ingene) { // Does not follow a cond.  Make it a new gene
      genenum = genenum + 1;
      if (genenum == n) {
        genepos = k;
goto ;
      }
    } else {
      ingene = false; // First Start or Else following a cond
    }
  }

// If a Cond
  if (dna(k).tipo == 9 && (dna(k).value == 1)) {
    ingene = true;
    genenum = genenum + 1;
    if (genenum == n) {
      genepos = k;
goto ;
    }
  }
// If a stop
  if (dna(k).tipo == 9 && dna(k).value == 4) {
    ingene = false;
  }

  k = k + 1;
  if (dna(k).tipo == 10& dna(k).value == 1) {
    k = -1;
  }
  Wend();
getout:
  return genepos;
}

/*
' executes program of robot n with genes activation display on
*/
public static void exechighlight(int n) {
//Dim ga() As Boolean
//Dim k As Integer
//ReDim ga(rob[n].genenum)
//k = 1
// scans the list of genes entry points
// verifying conditions and jumping to body execution
//While rob[n].condlist(k) > 0
//  currgene = k
//  If COND(n, rob[n].condlist(k) + 1) Then
//    ga(k) = True
//    'corpo (n)
//  Else
//    ga(k) = False
//  End If
//  k = k + 1
//Wend
  ActivForm.DrawGrid(rob[n].ga); // EricL March 15, 2006 - This line uncommented
}
}
