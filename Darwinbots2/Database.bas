Attribute VB_Name = "Database"
Option Explicit

'
' D A T A B A S E   R E C O R D I N G
'

'for snapshots
Dim SnapName As String

' creates a new database

Public Sub CreateArchive(Name As String)
  Dim t As Integer
  If Name = "" Then Exit Sub
  Open Name For Output As 2
    Print #2, "Rob id,Parent id,Founder name,Generation,Birth cycle,Age,Mutations,New mutations,Dna length,Offspring number,Total pop,Vegs pop,Robs pop,kills";
    For t = 1 To 100
      Print #2, ",gene" & t;
    Next t
    Print #2, "end "
  Close 2
End Sub

' opens a previously created database
Public Sub OpenDB(Name)
  If Name = "" Then Exit Sub
  Open Name For Append As 2
  optionsform.DBOptsEnable False
End Sub

' closes a db
Public Sub CloseDatabase()
  Close 2
  optionsform.DBOptsEnable True
End Sub

Private Sub SnapBrowse() 'creates a file to store the snapshot into
  Form1.CommonDialog1.InitDir = MDIForm1.MainDir + "/database"
  Form1.CommonDialog1.DialogTitle = "Select a name for your snapshot file."
  Form1.CommonDialog1.Filter = "Snapshot Database (*.snp)|*.snp"
  Form1.CommonDialog1.ShowSave
  SnapName = Form1.CommonDialog1.FileName
End Sub

Public Sub Snapshot()
  'records a snapshot of all living robots in a snapshot database
  Dim v As String
  Dim t As Integer
  Dim a As String
  Dim d As String
  Dim ti As Integer
  Dim va As Integer
  Dim rn As Integer
  Dim OK As Boolean
  
  On Error GoTo fine
  SnapBrowse
  Open SnapName For Output As 3
  Print #3, "Rob id,Parent id,Founder name,Generation,Birth cycle,Age,Mutations,New mutations,Dna length,Offspring number,Total pop,kills";

  For t = 1 To 100
    Print #3, ",gene" & t;
  Next t

  Print #3, "end "
  v = ","

  For rn = 1 To MaxRobs
    OK = True

    If rob(rn).exist And OK Then
      With rob(rn)
        Print #3, sstr(.AbsNum); v; sstr(.parent); v; .FName; v; sstr(.generation); v; sstr(.BirthCycle); v; sstr(.age); v; sstr(.Mutations); v;
        Print #3, sstr(.LastMut); v; sstr(.DnaLen); v; sstr(.SonNumber); v; sstr(TotalRobots); v; sstr(.Kills); v;
        
        d = ""
        d = DetokenizeDNA(rn, False)
        Print #3, d; v
      End With
    End If
  Next rn
  Close 3
  MsgBox ("Saved snapshot successfully.")
  GoTo getout

fine:
  Close 3
  If Err.Number = 70 Then
    MsgBox ("That file is already open in another program")
  Else
    d = "File error " + Str$(Err.Number) + Err.Description
    MsgBox (d)
  End If
getout:
End Sub

' adds a record
Public Sub AddRecord(rn As Integer)
  Dim v As String
  Dim t As Integer
  Dim a As String
  Dim d As String
  Dim ti As Integer
  Dim va As Integer
  Dim gene As String
  
  On Error GoTo fine
  v = ","
  With rob(rn)
  Print #2, sstr(.AbsNum); v; sstr(.parent); v; .FName; v; sstr(.generation); v; sstr(.BirthCycle); v; sstr(.age); v; sstr(.Mutations); v;
  Print #2, sstr(.LastMut); v; sstr(.DnaLen); v; sstr(.SonNumber); v; sstr(TotalRobots); v; sstr(.Kills); v;

  d = ""
  d = d + DetokenizeDNA(rn, False)
  Print #2, d
  End With
  GoTo getout

fine:
  d = "File error " + Str$(Err.Number) + Err.Description
  If Err.Number = 70 Then
    MsgBox ("The Database file is already open in another program")
  Else
    MsgBox (d)
  End If
getout:
End Sub

' when I wasn't aware of the existence of CStr()
Private Function sstr(X) As String
  sstr = Right(Str(X), Len(Str(X)) - 1)
End Function
