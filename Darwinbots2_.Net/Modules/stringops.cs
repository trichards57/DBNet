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


static class stringops {
// some useful string operations


static string extractpath(ref string path_UNUSED) {
  string extractpath = "";

/*
Dim k As Integer
  Dim OK As Integer
  If path <> "" Then
    k = 1
    While InStr(k, path, "\\") > 0
      OK = k
      k = InStr(k, path, "\\") + 1
    Wend
'EricL - If condition below added March 15, 2006
    If k = 1 Then
      extractpath = ""
    Else
      extractpath = Left(path, k - 2)
    End If
  End If
End Function
*/
static string extractname(ref string path_UNUSED) {
  string extractname = "";

/*
Dim k As Integer
  Dim OK As Integer
  If path <> "" Then
    k = 1
    While InStr(k, path, "\\") > 0
      OK = k
      k = InStr(k, path, "\\") + 1
    Wend
    extractname = Right(path, Len(path) - k + 1)
  End If
End Function
*/
static string extractexactname(ref string Name_UNUSED) {//Botsareus 3/16/2014 Bug fix
  string extractexactname = "";

/*
'Botsareus 10/21/2015 Bug fix monitoring corpses
If Name = "Corpse" Then
extractexactname = "Corpse"
Exit Function
End If
'Botsareus 12/11/2013 Extracts the name portion of fname
Dim sp() As String
sp = Split(Name, ".")
Dim t As String
Dim i As Byte
For i = 0 To UBound(sp) - 1
t = t & sp(i) & IIf(i = UBound(sp) - 1, "", ".")
Next
extractexactname = t
End Function
*/
static string relpath(ref string path_UNUSED) {
  string relpath = "";

/*
If path Like MDIForm1.MainDir + "*" Then
    path = "&#" + Right(path, Len(path) - Len(MDIForm1.MainDir))
  End If
  relpath = path
End Function
*/
static string respath(ref string path_UNUSED) {
  string respath = "";

/*
If Left(path, 2) = "&#" Then
    path = MDIForm1.MainDir + Right(path, Len(path) - 2)
  End If
  If path = "" Then
path = MDIForm1.MainDir + "\\Robots"
End If
  respath = path
End Function
*/
static string ConvertCommasToDecimal(ref string s_UNUSED) {
  string ConvertCommasToDecimal = "";

/*
ConvertCommasToDecimal = Replace(s, ",", ".")

End Function

'Botsareus 1/29/2014 replace special chars function
*/
static string replacechars(ref string s_UNUSED) {
  string replacechars = "";
}
