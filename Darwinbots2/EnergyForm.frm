VERSION 5.00
Object = "{FE0065C0-1B7B-11CF-9D53-00AA003C9CB6}#1.1#0"; "COMCT232.OCX"
Begin VB.Form EnergyForm 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Energy Management"
   ClientHeight    =   5025
   ClientLeft      =   2760
   ClientTop       =   3750
   ClientWidth     =   5610
   Icon            =   "EnergyForm.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   5025
   ScaleWidth      =   5610
   ShowInTaskbar   =   0   'False
   Begin VB.Frame Frame1 
      Caption         =   "Incoming Energy"
      Height          =   4335
      Left            =   120
      TabIndex        =   1
      Top             =   120
      Width           =   5415
      Begin VB.Frame ffmTides 
         Caption         =   "Tide Simulator (Advanced)"
         Height          =   975
         Left            =   2040
         TabIndex        =   18
         Top             =   720
         Width           =   3015
         Begin VB.TextBox txtTideOf 
            Height          =   285
            Left            =   600
            TabIndex        =   22
            Text            =   "0"
            Top             =   600
            Width           =   825
         End
         Begin VB.TextBox txtTide 
            Height          =   285
            Left            =   600
            TabIndex        =   19
            Text            =   "0"
            Top             =   240
            Width           =   855
         End
         Begin ComCtl2.UpDown TideUpDn 
            Height          =   285
            Left            =   1560
            TabIndex        =   21
            ToolTipText     =   "Set the length of day and night in game cycles. The value entered here represents one full cycle of both."
            Top             =   240
            Width           =   255
            _ExtentX        =   450
            _ExtentY        =   503
            _Version        =   327681
            Value           =   100
            BuddyControl    =   "txtTide"
            BuddyDispid     =   196612
            OrigLeft        =   3600
            OrigTop         =   360
            OrigRight       =   3855
            OrigBottom      =   645
            Increment       =   100
            Max             =   32000
            SyncBuddy       =   -1  'True
            BuddyProperty   =   0
            Enabled         =   -1  'True
         End
         Begin ComCtl2.UpDown OfUpDown 
            Height          =   285
            Left            =   1546
            TabIndex        =   23
            ToolTipText     =   "Set the length of day and night in game cycles. The value entered here represents one full cycle of both."
            Top             =   600
            Width           =   255
            _ExtentX        =   450
            _ExtentY        =   503
            _Version        =   327681
            Value           =   100
            BuddyControl    =   "txtTideOf"
            BuddyDispid     =   196611
            OrigLeft        =   3600
            OrigTop         =   360
            OrigRight       =   3855
            OrigBottom      =   645
            Increment       =   100
            Max             =   32000
            SyncBuddy       =   -1  'True
            BuddyProperty   =   0
            Enabled         =   -1  'True
         End
         Begin VB.Label Label3 
            Caption         =   "offset"
            Height          =   255
            Left            =   120
            TabIndex        =   25
            Top             =   645
            Width           =   495
         End
         Begin VB.Label Label2 
            Caption         =   "cycles"
            Height          =   255
            Left            =   1920
            TabIndex        =   24
            Top             =   600
            Width           =   615
         End
         Begin VB.Label lblTides 
            Caption         =   "cycles (off)"
            Height          =   255
            Left            =   1920
            TabIndex        =   20
            Top             =   240
            Width           =   855
         End
      End
      Begin VB.CheckBox chkRnd 
         Caption         =   "Enable Weather"
         Height          =   255
         Left            =   240
         TabIndex        =   17
         ToolTipText     =   "A cycle of light and darkness. Veggies are not fed or repopulated during night time."
         Top             =   840
         Width           =   2475
      End
      Begin VB.Frame Frame2 
         Caption         =   "Thresholds"
         Height          =   2535
         Left            =   120
         TabIndex        =   6
         Top             =   1680
         Width           =   5175
         Begin VB.OptionButton ThresholdMode 
            Caption         =   "Advance Sun to Dawn / Dusk"
            Height          =   255
            Index           =   2
            Left            =   2400
            TabIndex        =   16
            Top             =   600
            Width           =   2535
         End
         Begin VB.OptionButton ThresholdMode 
            Caption         =   "Permanently Toggle Sun State"
            Height          =   255
            Index           =   1
            Left            =   2400
            TabIndex        =   15
            Top             =   960
            Width           =   2535
         End
         Begin VB.OptionButton ThresholdMode 
            Caption         =   "Temporarily Suspend Day Cycles"
            Height          =   255
            Index           =   0
            Left            =   2400
            TabIndex        =   13
            Top             =   240
            Width           =   2655
         End
         Begin VB.CheckBox SunDown 
            Caption         =   "Sun goes down if nrg >"
            Height          =   255
            Left            =   480
            TabIndex        =   10
            Top             =   1440
            Width           =   1935
         End
         Begin VB.CheckBox SunUp 
            Caption         =   "Sun comes up if nrg <"
            Height          =   255
            Left            =   480
            TabIndex        =   9
            Top             =   1920
            Width           =   1935
         End
         Begin VB.TextBox SunDownThreshold 
            Alignment       =   1  'Right Justify
            Height          =   285
            Left            =   2640
            TabIndex        =   8
            Text            =   "1000000"
            ToolTipText     =   "Overrides day/night if configured and forces the sun down if the sim energy is above this value"
            Top             =   1440
            Width           =   1380
         End
         Begin VB.TextBox SunUpThreshold 
            Alignment       =   1  'Right Justify
            Height          =   285
            Left            =   2640
            TabIndex        =   7
            Text            =   "500000"
            ToolTipText     =   "Brings the sun up, overriding day/night cycles if the sim energy falls below this value."
            Top             =   1920
            Width           =   1395
         End
         Begin ComCtl2.UpDown SunDownUpDn 
            Height          =   285
            Left            =   4080
            TabIndex        =   11
            ToolTipText     =   "Set the length of day and night in game cycles. The value entered here represents one full cycle of both."
            Top             =   1440
            Width           =   255
            _ExtentX        =   450
            _ExtentY        =   503
            _Version        =   327681
            Value           =   100
            AutoBuddy       =   -1  'True
            BuddyControl    =   "SunDownThreshold"
            BuddyDispid     =   196621
            OrigLeft        =   3600
            OrigTop         =   840
            OrigRight       =   3855
            OrigBottom      =   1125
            Increment       =   10000
            Max             =   10000000
            SyncBuddy       =   -1  'True
            BuddyProperty   =   0
            Enabled         =   -1  'True
         End
         Begin ComCtl2.UpDown SunUpUpDn 
            Height          =   285
            Left            =   4080
            TabIndex        =   12
            ToolTipText     =   "Set the length of day and night in game cycles. The value entered here represents one full cycle of both."
            Top             =   1920
            Width           =   255
            _ExtentX        =   450
            _ExtentY        =   503
            _Version        =   327681
            Value           =   100
            AutoBuddy       =   -1  'True
            BuddyControl    =   "SunUpThreshold"
            BuddyDispid     =   196622
            OrigLeft        =   3600
            OrigTop         =   1320
            OrigRight       =   3855
            OrigBottom      =   1605
            Increment       =   10000
            Max             =   100000000
            SyncBuddy       =   -1  'True
            BuddyProperty   =   0
            Enabled         =   -1  'True
         End
         Begin VB.Label Label1 
            Caption         =   "When Threshold reached:"
            Height          =   255
            Left            =   240
            TabIndex        =   14
            Top             =   360
            Width           =   2295
         End
      End
      Begin VB.TextBox DNLength 
         Alignment       =   1  'Right Justify
         Height          =   285
         Left            =   3000
         TabIndex        =   3
         Text            =   "1000000"
         ToolTipText     =   "Set the length of day and night in game cycles. The value entered here represents one full cycle of both."
         Top             =   360
         Width           =   1035
      End
      Begin VB.CheckBox DNCheck 
         Caption         =   "Enable Day Cycles"
         Height          =   255
         Left            =   240
         TabIndex        =   2
         ToolTipText     =   "A cycle of light and darkness. Veggies are not fed or repopulated during night time."
         Top             =   360
         Width           =   1635
      End
      Begin ComCtl2.UpDown DNCycleUpDn 
         Height          =   285
         Left            =   4080
         TabIndex        =   4
         ToolTipText     =   "Set the length of day and night in game cycles. The value entered here represents one full cycle of both."
         Top             =   360
         Width           =   255
         _ExtentX        =   450
         _ExtentY        =   503
         _Version        =   327681
         Value           =   100
         AutoBuddy       =   -1  'True
         BuddyControl    =   "DNLength"
         BuddyDispid     =   196624
         OrigLeft        =   3600
         OrigTop         =   360
         OrigRight       =   3855
         OrigBottom      =   645
         Increment       =   100
         Max             =   32000
         SyncBuddy       =   -1  'True
         BuddyProperty   =   0
         Enabled         =   -1  'True
      End
      Begin VB.Label Label40 
         Caption         =   "Period"
         Height          =   195
         Left            =   2400
         TabIndex        =   5
         ToolTipText     =   "Set the length of day and night in game cycles. The value entered here represents one full cycle of both."
         Top             =   360
         Width           =   555
      End
   End
   Begin VB.CommandButton OKButton 
      Caption         =   "OK"
      Height          =   375
      Left            =   2160
      TabIndex        =   0
      Top             =   4560
      Width           =   1215
   End
End
Attribute VB_Name = "EnergyForm"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
' Copyright (c) 2006 Eric Lockard
' eric@sulaadventures.com
' All rights reserved.
'
'Redistribution and use in source and binary forms, with or without
'modification, are permitted provided that:
'
'(1) source code distributions retain the above copyright notice and this
'    paragraph in its entirety,
'(2) distributions including binary code include the above copyright notice and
'    this paragraph in its entirety in the documentation or other materials
'    provided with the distribution, and
'(3) Without the agreement of the author redistribution of this product is only allowed
'    in non commercial terms and non profit distributions.
'
'THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR IMPLIED
'WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF
'MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

Option Explicit
'Botsareus 6/12/2012 form's icon change

Private Sub chkRnd_Click() 'Botsareus 7/15/2014 Randomize the Sun On Cycles
  TmpOpts.SunOnRnd = chkRnd.value
End Sub

Private Sub DNCheck_Click()
  TmpOpts.DayNight = DNCheck.value
  If TmpOpts.DayNight = False Then TmpOpts.Daytime = True
  DNLength.Enabled = DNCheck.value
  DNCycleUpDn.Enabled = DNCheck.value
End Sub


Private Sub DNLength_Change()
  If val(DNLength.text) > 32000 Then DNLength.text = 32000
  TmpOpts.CycleLength = val(DNLength.text)
End Sub

Private Sub Form_Load()
  DNLength.text = TmpOpts.CycleLength
  DNLength.Enabled = TmpOpts.DayNight
  DNCycleUpDn.Enabled = TmpOpts.DayNight
  DNCheck.value = TmpOpts.DayNight
  
  SunUpThreshold.text = TmpOpts.SunUpThreshold
  SunUpThreshold.Enabled = TmpOpts.SunUp
  SunUpUpDn.Enabled = TmpOpts.SunUp
  SunUp.value = TmpOpts.SunUp
  
  SunDownThreshold.text = TmpOpts.SunDownThreshold
  SunDownThreshold.Enabled = TmpOpts.SunDown
  SunDownUpDn.Enabled = TmpOpts.SunDown
  SunDown.value = TmpOpts.SunDown
  ThresholdMode(TmpOpts.SunThresholdMode).value = True
  chkRnd.value = TmpOpts.SunOnRnd
  
  txtTide = TmpOpts.Tides
  txtTideOf = TmpOpts.TidesOf
End Sub

Private Sub OKButton_Click()
  Unload Me
End Sub

Private Sub SunDown_Click()
  TmpOpts.SunDown = SunDown.value
  SunDownThreshold.Enabled = SunDown.value
  SunDownUpDn.Enabled = SunDown.value
End Sub


Private Sub SunDownThreshold_Change()
  TmpOpts.SunDownThreshold = val(SunDownThreshold.text)
End Sub

Private Sub SunUp_Click()
  TmpOpts.SunUp = SunUp.value
  SunUpThreshold.Enabled = SunUp.value
  SunUpUpDn.Enabled = SunUp.value
End Sub

Private Sub SunUpThreshold_Change()
  TmpOpts.SunUpThreshold = val(SunUpThreshold.text)
End Sub

Private Sub ThresholdMode_Click(Index As Integer)
  TmpOpts.SunThresholdMode = Index
End Sub

Private Sub txtTide_Change()
  txtTide = Int(val(txtTide))
  If txtTide < 0 Then txtTide = 0
  If txtTide > 32000 Then txtTide = 32000
  If txtTide = 0 Then lblTides = "cycles (off)" Else lblTides = "cycles"
  TmpOpts.Tides = txtTide
End Sub

Private Sub txtTideOf_Change()
  txtTideOf = Int(val(txtTideOf))
  If txtTideOf < 0 Then txtTideOf = 0
  If txtTideOf > 32000 Then txtTideOf = 32000
  TmpOpts.TidesOf = txtTideOf
End Sub
