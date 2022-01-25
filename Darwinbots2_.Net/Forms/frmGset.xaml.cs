using System.Windows;
using static Globals;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Interaction;
using static System.Math;
using static VBConstants;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class frmGset : Window
    {
        private static frmGset _instance;

        // Option Explicit //False
        //Botsareus 3/15/2013 The global settings form
        private string robpath = "";

        public frmGset()
        {
            InitializeComponent();
        }

        public static frmGset instance { set { _instance = null; } get { return _instance ?? (_instance = new frmGset()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = frmGset.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        private void btnBrowseRob_Click(object sender, RoutedEventArgs e)
        {
            // TODO (not supported): On Error GoTo fine
            OptionsForm.instance.CommonDialog1.FileName = "";
            OptionsForm.instance.CommonDialog1.Filter = "Dna file(*.txt)|*.txt"; //Botsareus 1/11/2013 DNA only
            OptionsForm.instance.CommonDialog1.InitDir = MDIForm1.instance.MainDir + "\\robots";
            OptionsForm.instance.CommonDialog1.DialogTitle = WSchoosedna;
            OptionsForm.instance.CommonDialog1.ShowOpen();
            if (OptionsForm.instance.CommonDialog1.FileName != "")
            { //Botsareus 1/11/2013 Do not insert robot if filename is blank
                txtRob.Text = OptionsForm.instance.CommonDialog1.FileName;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //cancel has been pressed
            Unload();
        }

        private void btnEvoRES_Click(object sender, RoutedEventArgs e)
        {
            frmRestriOps.instance.res_state = 4;
            frmRestriOps.instance.Show(vbModal);
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            MsgBox("Survival mode consists of a base species and a mutating species. The base species gets 'turned on and off.'" + " The mutating species get there energy changed during the time the base species are 'not updating.'" + " This is a handicap for the Base species. Because, a negative value means mutating species are losing energy." + vbCrLf + vbCrLf + "Formula1:" + vbCrLf + "new_value = ((Average energy gain with Base.txt on (species average, time average)) minus " + "(Average energy gain with Base.txt off (species average, time average))) / LFOR " + vbCrLf + "If new_value is less then old_value then current_value = new_value else current_value = (old_value * 9 + new_value) / 10 " + vbCrLf + "Formula 2:" + vbCrLf + "((Average energy gain last on/off cycle (species average, time average)) minus " + "(Average energy gain this on/off cycle (species average, time average))) / LFOR * 2 " + vbCrLf + "Result = Formula1 minus Formula2 or just Formula1 if Formula2 is greater then zero. Result takes full effect after 6 on/off cycles." + vbCrLf + vbCrLf + "For current simulation's data go to the help menu." + vbCrLf + vbCrLf + "www.darwinbots.com", vbInformation, "Dev. By: Paul Kononov a.k.a. Botsareus");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ////Botsareus 5/10/2013 Make sure txtCD points to a valid directory
            //  if (!FolderExists(txtCD.text) && chkUseCD.value == 1) {
            //    MsgBox("Please use a valid directory for the main directory.", vbCritical);
            //return;

            //  }

            //  if (chkUseCD.value == 0) {
            ////delete the maindir setting if no longer used
            //    if (dir(App.path + "\\Maindir.gset") != "") {
            //      File.Delete((App.path + "\\Maindir.gset"));;
            //    }
            //  } else {
            ////write current path to setting
            //    VBOpenFile(1, App.path + "\\Maindir.gset");;
            //    Write(1, txtCD.Text);
            //    VBCloseFile(1);
            //  }

            //  string specielist = "";

            //  specielist = "";
            //  int i = 0;

            ////Botsareus 1/31/2014 The league modes
            //  if (txtSourceDir.Visibility) {
            //    if (txtCD.Text != MDIForm1.instance.MainDir) {
            //      MsgBox("Can not start a league while changing main directory.", vbCritical);
            //      txtCD.Text = MDIForm1.instance.MainDir;
            //return;

            //    }
            //    if (txtSourceDir.text == MDIForm1.instance.MainDir + "\\league" || txtSourceDir.text == MDIForm1.instance.MainDir + "\\league\\*") {
            //      MsgBox("League source directory can not be the same as league engine directory.", vbCritical);
            //return;

            //    }
            //    if (!FolderExists(txtSourceDir.text)) {
            //      MsgBox("Please use a valid directory for the league source directory.", vbCritical);
            //return;

            //    }
            //    if (FolderExists(MDIForm1.instance.MainDir + "\\league")) {
            //      if (MsgBox("The current league will be restarted. Continue?", vbYesNo | vbQuestion) == vbYes) {
            //        RecursiveRmDir(MDIForm1.instance.MainDir + "\\league");
            //      } else {
            //return;

            //      }
            //    }
            ////create folder
            //    RecursiveMkDir(MDIForm1.instance.MainDir + "\\league");
            //    RecursiveMkDir(MDIForm1.instance.MainDir + "\\league\\seeded");
            //    RecursiveMkDir(MDIForm1.instance.MainDir + "\\league\\stepladder");
            ////generate list of species that are not repopulating
            //    for(i=0; i<UBound(TmpOpts.Specie); i++) {
            //      if (TmpOpts.Specie[i].Veg == false && TmpOpts.Specie[i].Name != "") {
            //        specielist = specielist + TmpOpts.Specie[i].Name + vbNewLine;
            //      }
            //    }
            ////remove all nonrepopulating robots
            //    if (specielist != "") {
            //      if (MsgBox("The following robots must be removed first:" + vbCrLf + vbCrLf + specielist + vbCrLf + "Continue?", vbYesNo | vbQuestion) == vbYes) {
            //                        for (i = 0; i < UBound(TmpOpts.Specie); i++) {
            //                            if (TmpOpts.Specie[i].Veg == false && TmpOpts.Specie[i].Name != "") {
            //            OptionsForm.instance.SpecList.SelectedIndex = i;
            //            OptionsForm.instance.DelSpec_Click();
            //            i = i - 1;
            //          }
            //        }
            //      } else {
            //        if (x_restartmode == 1 || x_restartmode == 2 || x_restartmode == 3 || x_restartmode == 10) { //Botsareus 10/6/2015 Bug fix
            //          File.Delete(App.path + "\\restartmode.gset");;
            //          x_restartmode = 0;
            //        }
            //return;

            //      }
            //    }
            //  }

            ////2/16/2014 Evolution modes
            //  if (chkSurvivalSimple.value == 1 || chkSurvivalEco.value == 1) {
            //    if (txtCD.Text != MDIForm1.instance.MainDir) {
            //      MsgBox("Can not start a evolution mode while changing main directory.", vbCritical);
            //      txtCD.Text = MDIForm1.instance.MainDir;
            //return;

            //    }
            //    if (extractpath(txtRob.Text) == MDIForm1.instance.MainDir + "\\evolution" || extractpath(txtRob.Text) == MDIForm1.instance.MainDir + "\\evolution\\*") {
            //      MsgBox("Robot source directory can not be the same as evolution engine directory.", vbCritical);
            //return;

            //    }
            //    if (dir(txtRob.Text) == "" || txtRob.Text == "") {
            //      MsgBox("Please use a valid file name for robot.", vbCritical);
            //return;

            //    }
            //    if (FolderExists(MDIForm1.instance.MainDir + "\\evolution")) {
            //      if (MsgBox("The current evolution mode will be restarted. Continue?", vbYesNo | vbQuestion) == vbYes) {
            //        RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution");
            //      } else {
            //return;

            //      }
            //    }
            ////the folders
            //    RecursiveMkDir(MDIForm1.instance.MainDir + "\\evolution");
            //    RecursiveMkDir(MDIForm1.instance.MainDir + "\\evolution\\stages");
            ////generate list of species that are not repopulating
            //    for(i=0; i<UBound(TmpOpts.Specie); i++) {
            //      if (TmpOpts.Specie(i).Veg == false && TmpOpts.Specie(i).Name != "") {
            //        specielist = specielist + TmpOpts.Specie(i).Name + vbNewLine;
            //      }
            //    }
            ////remove all nonrepopulating robots
            //    if (specielist != "") {
            //      if (MsgBox("The following robots must be removed first:" + vbCrLf + vbCrLf + specielist + vbCrLf + "Continue?", vbYesNo | vbQuestion) == vbYes) {
            //        for(i=0; i<UBound(TmpOpts.Specie); i++) {
            //          if (TmpOpts.Specie(i).Veg == false && TmpOpts.Specie(i).Name != "") {
            //            OptionsForm.instance.SpecList.SelectedIndex = i;
            //            OptionsForm.instance.DelSpec_Click();
            //            i = i - 1;
            //          }
            //        }
            //      } else {
            //        if (x_restartmode == 4 || x_restartmode == 5 || x_restartmode == 6) { //Botsareus 10/6/2015 Bug fix
            //          File.Delete(App.path + "\\restartmode.gset");;
            //          x_restartmode = 0;
            //        }
            //return;

            //      }
            //    }
            //  }
            //  if (chkZBmode.value) {
            //    if (txtCD.Text != MDIForm1.instance.MainDir) {
            //      MsgBox("Can not start zerobot evolution mode while changing main directory.", vbCritical);
            //      txtCD.Text = MDIForm1.instance.MainDir;
            //return;

            //    }
            //    if (FolderExists(MDIForm1.instance.MainDir + "\\evolution")) {
            //      if (MsgBox("The current evolution mode will be restarted. Continue?", vbYesNo | vbQuestion) == vbYes) {
            //        RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution");
            //      } else {
            //return;

            //      }
            //    }
            ////the folders
            //    RecursiveMkDir(MDIForm1.instance.MainDir + "\\evolution");
            //    RecursiveMkDir(MDIForm1.instance.MainDir + "\\evolution\\stages");
            ////generate list of species that are not repopulating
            //    for(i=0; i<UBound(TmpOpts.Specie); i++) {
            //      if (TmpOpts.Specie(i).Veg == false && TmpOpts.Specie(i).Name != "") {
            //        specielist = specielist + TmpOpts.Specie(i).Name + vbNewLine;
            //      }
            //    }
            ////remove all nonrepopulating robots
            //    if (specielist != "") {
            //      if (MsgBox("The following robots must be removed first:" + vbCrLf + vbCrLf + specielist + vbCrLf + "Continue?", vbYesNo | vbQuestion) == vbYes) {
            //        for(i=0; i<UBound(TmpOpts.Specie); i++) {
            //          if (TmpOpts.Specie(i).Veg == false && TmpOpts.Specie(i).Name != "") {
            //            OptionsForm.instance.SpecList.SelectedIndex = i;
            //            OptionsForm.instance.DelSpec_Click();
            //            i = i - 1;
            //          }
            //        }
            //      } else {
            //        if (x_restartmode == 7 || x_restartmode == 8 || x_restartmode == 9) { //Botsareus 10/6/2015 Bug fix
            //          File.Delete(App.path + "\\restartmode.gset");;
            //          x_restartmode = 0;
            //        }
            //return;

            //      }
            //    }
            //  }

            ////prompt that settings will take place when you restart db
            //  MsgBox("Global settings will take effect the next time DarwinBots starts.", vbInformation);

            ////save all settings
            //  VBOpenFile(1, MDIForm1.MainDir + "\\Global.gset");;
            //  Write(1, chkScreenRatio.IsChecked);
            //  Write(1, Val(txtBodyFix.Text));
            //  Write(1, chkGreedy.IsChecked);
            //  Write(1, chkchseedstartnew.IsChecked );
            //  Write(1, chkchseedloadsim.IsChecked );
            //  Write(1, chkSafeMode.IsChecked);
            //  Write(1, sldFindBest.Value);
            //  Write(1, chkOldColor.IsChecked);
            //  Write(1, chkNoBoyMsg.IsChecked );
            //  Write(1, chkNoVid.IsChecked );
            //  Write(1, chkEpiReset.IsChecked );
            //  Write(1, Val(txtMEmp.Text));
            //  Write(1, Val(txtOP.Text));
            //  Write(1, chkSunbelt.IsChecked );
            //  Write(1, chkDelta2.IsChecked );
            //  Write(1, Val(txtMainExp.Text));
            //  Write(1, Val(txtMainLn.Text));
            //  Write(1, Val(txtDevExp.Text));
            //  Write(1, Val(txtDevLn.Text));
            //  Write(1, Val(txtPMinter.Text));
            //  Write(1, chkNorm.IsChecked );
            //  Write(1, Val(txtDnalen.Text));
            //  Write(1, Val(txtMxDnalen.Text));
            //  Write(1, Val(txtWTC.Text));
            //  Write(1, Val(sldMain.Value));
            //  Write(1, Val(sldDev.Value));
            //  Write(1, IIf(chkAddarob.Value == 1, MDIForm1.instance.MainDir + "\\league\\singlerob", txtSourceDir.Text));
            //  Write(1, (chkStepladder.IsChecked.Value  && chkTournament.IsChecked.Value ));

            //  foreach(var tmpopt in new[] { optFudging_0, optFudging_1, optFudging_2 }) {
            //    if (tmpopt.Value) {
            //      Write(1, tmpopt.Name.Split('_').Last());
            //    }
            //  }

            //  Write(1, Val(txtStartChlr.Text));

            //  foreach(var tmpopt in optDisqua) {
            //    if (tmpopt.value) {
            //      Write(1, tmpopt.Index);
            //    }
            //  }

            ////evolution

            //  Write(1, txtRob.Text);
            //  Write(1, chkShowGraphs.value == 1);
            //  Write(1, chkNormSize.value == 1);
            //  Write(1, val(txtCycSM.Text));
            //  Write(1, val(txtLFOR.Text));

            //  Write(1, false); //Replacing with better rules

            //  Write(1, val(txtZlength.Text));

            ////Restrictions

            //  Write(1, x_res_kill_chlr);
            //  Write(1, x_res_kill_mb);
            //  Write(1, x_res_other);

            //  Write(1, y_res_kill_chlr);
            //  Write(1, y_res_kill_mb);
            //  Write(1, y_res_kill_dq);
            //  Write(1, y_res_other);

            //  Write(1, x_res_kill_mb_veg);
            //  Write(1, x_res_other_veg);

            //  Write(1, y_res_kill_mb_veg);
            //  Write(1, y_res_kill_dq_veg);
            //  Write(1, y_res_other_veg);

            //  Write(1, chkGraphUp.value == 1);
            //  Write(1, chkHide.value == 1);

            ////New way to preserve epigenetic memory

            //  Write(1, chkEpiGene.value == 1);

            ////Use internet as randomizer

            //  Write(1, chkIntRnd.value == 1);

            //  VBCloseFile(1);

            ////Botsareus 1/31/2014 Setup a league
            //  if (txtSourceDir.Visibility) {
            ////R E S T A R T  I N I T
            //    if (chkStepladder.IsChecked == 1 && chkTournament.IsChecked == 0) {
            //      optionsform.savesett(MDIForm1.instance.MainDir + "\\settings\\lastexit.set");
            //      string file_name = "";

            //      Collection files = null;

            //      if (chkAddarob.IsChecked == 1) { //Botsareus 6/28/2014 Just add one robot
            //        files = getfiles(txtSourceDir.text);
            ////move all robots
            //        for(i=1; i<files.count; i++) {
            //          FileCopy(files(i), MDIForm1.instance.MainDir + "\\league\\stepladder\\" + extractname(files(i)));
            //        }
            ////move a single robot
            //        MkDir(MDIForm1.instance.MainDir + "\\league\\singlerob");
            //        FileCopy(robpath, MDIForm1.instance.MainDir + "\\league\\singlerob\\" + extractname(robpath));
            //        VBOpenFile(1, MDIForm1.MainDir + "\\league\\singlerob\\" + extractname(robpath));;
            //        VBWriteFile(1, vbCrLf + "'#tag:" + extractname(robpath));;
            //        VBCloseFile(1);();
            //        leagueSourceDir = MDIForm1.instance.MainDir + "\\league\\singlerob";
            //        x_filenumber = 0;
            //        populateladder();
            //      } else {
            //        leagueSourceDir = txtSourceDir.text;
            ////add tags to files
            //        files = getfiles(leagueSourceDir);
            //        for(i=1; i<files.count; i++) {
            //          VBOpenFile(1, files(i));;
            //          VBWriteFile(1, vbCrLf + "'#tag:" + extractname(files(i)));;
            //          VBCloseFile(1);();
            //        }

            //        file_name = dir$(leagueSourceDir + "\\*.*");
            //        FileCopy(leagueSourceDir + "\\" + file_name, MDIForm1.instance.MainDir + "\\league\\stepladder\\1-" + file_name);
            //        File.Delete(leagueSourceDir + "\\" + file_name);;
            //        x_filenumber = 0;
            //        populateladder();
            //      }
            //    } else if (chkFilter.value == 1) {
            //      optionsform.savesett(MDIForm1.instance.MainDir + "\\settings\\lastexit.set"); //save last settings
            //      FileCopy(robpath, MDIForm1.instance.MainDir + "\\league\\robotB.txt");
            //      VBOpenFile(1, App.path + "\\restartmode.gset");;
            //      Write(1, 10);
            //      Write(1, 1);
            //      VBCloseFile(1);();
            //      VBOpenFile(1, App.path + "\\Safemode.gset");;
            //      Write(1, false);
            //      VBCloseFile(1);();
            //      VBOpenFile(1, App.path + "\\autosaved.gset");;
            //      Write(1, false);
            //      VBCloseFile(1);();
            //      Call(restarter());
            //    } else {
            //      optionsform.savesett(MDIForm1.instance.MainDir + "\\settings\\lastexit.set"); //save last settings
            //      VBOpenFile(1, App.path + "\\restartmode.gset");;
            //      Write(1, 1);
            //      Write(1, 1);
            //      VBCloseFile(1);();
            //      VBOpenFile(1, App.path + "\\Safemode.gset");;
            //      Write(1, false);
            //      VBCloseFile(1);();
            //      VBOpenFile(1, App.path + "\\autosaved.gset");;
            //      Write(1, false);
            //      VBCloseFile(1);();
            //      Call(restarter());
            //    }
            //  }

            ////2/16/2014 Evolution modes
            //  if (chkSurvivalSimple.value == 1 || chkSurvivalEco.value == 1) {
            ////let us init basic survival evolution mode
            ////copy robots
            //    if (chkSurvivalSimple.value == 1) {
            //      FileCopy(txtRob.Text, MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
            //      FileCopy(txtRob.Text, MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt");
            //      FileCopy(txtRob.Text, MDIForm1.instance.MainDir + "\\evolution\\stages\\stage0.txt");
            //    } else {
            ////Botsareus 12/11/2015 Copy robot to append tag
            //      FileCopy(txtRob.Text, MDIForm1.instance.MainDir + "\\evolution\\tmp.txt");
            ////Botsareus 10/21/2015 Append tag at start of eco evo
            //      VBOpenFile(1, MDIForm1.MainDir + "\\evolution\\tmp.txt");;
            //      VBWriteFile(1, vbCrLf + "'#tag:" + extractname(txtRob));;
            //      VBCloseFile(1);();
            //      byte ecocount = 0;

            //      for(ecocount=1; ecocount<15; ecocount++) { //now uses tmp file with appended tag
            //        MkDir(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount);
            //        FileCopy(MDIForm1.instance.MainDir + "\\evolution\\tmp.txt", MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt");
            //        MkDir(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount);
            //        FileCopy(MDIForm1.instance.MainDir + "\\evolution\\tmp.txt", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
            //        MkDir(MDIForm1.instance.MainDir + "\\evolution\\stages\\stagerob" + ecocount);
            //        FileCopy(MDIForm1.instance.MainDir + "\\evolution\\tmp.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stagerob" + ecocount + "\\stage0.txt");
            //      }
            ////Botsareus 12/11/2015 Kill tmp robot file
            //      File.Delete(MDIForm1.MainDir + "\\evolution\\tmp.txt");;
            //    }
            ////generate mrate filename
            //    string mratefn = "";

            //    mratefn = extractpath(txtRob.Text) + "\\" + extractexactname(extractname(txtRob.Text)) + ".mrate";
            //    if (dir(mratefn) != "") {
            ////copy mrates
            //      if (chkSurvivalSimple.value == 1) {
            //        FileCopy(mratefn, MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");
            //        FileCopy(mratefn, MDIForm1.instance.MainDir + "\\evolution\\stages\\stage0.mrate");
            //      } else {
            //        for(ecocount=1; ecocount<15; ecocount++) {
            //          FileCopy(mratefn, MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");
            //          FileCopy(mratefn, MDIForm1.instance.MainDir + "\\evolution\\stages\\stagerob" + ecocount + "\\stage0.mrate");
            //        }
            //      }
            //    }
            ////calculate robot's size
            //    int Length = 0;

            ////we have to calculate length of robot here
            //    List<> rob_4027_tmp = new List<>();
            //for (int redim_iter_6270=0;i<0;redim_iter_6270++) {rob.Add(null);}
            //    if (LoadDNA(txtRob.Text, 0)) {
            //      Length = DnaLen(rob(0).dna);
            //    }
            ////generate data file
            //    VBOpenFile(1, MDIForm1.MainDir + "\\evolution\\data.gset");;
            //    Write(1, val(txtLFOR.Text)); //LFOR init
            //    Write(1, true); //dir
            //    Write(1, 5); //corr

            //    Write(1, val(txtCycSM.Text)); //hidePredCycl

            //    Write(1, val(Length + CInt(5))); //curr_dna_size
            //    Write(1, TargetDNASize(Length)); //target_dna_size

            //    Write(1, val(txtCycSM.Text)); //Init hidePredCycl

            //    Write(1, 0); //stgwins
            //    VBCloseFile(1);();
            ////for eco evo
            //    if (chkSurvivalEco.value == 1) {
            //      VBOpenFile(1, App.path + "\\im.gset");;
            //      Write(1, chkIM.value);
            //      VBCloseFile(1);();
            //    } else {
            //      if (dir(App.path + "\\im.gset") != "") {
            //        File.Delete(App.path + "\\im.gset");;
            //      }
            //    }
            ////other
            //    optionsform.savesett(MDIForm1.instance.MainDir + "\\settings\\lastexit.set"); //save last settings
            //    VBOpenFile(1, App.path + "\\restartmode.gset");;
            //    Write(1, 4);
            //    Write(1, 0);
            //    VBCloseFile(1);();
            //    VBOpenFile(1, App.path + "\\Safemode.gset");;
            //    Write(1, false);
            //    VBCloseFile(1);();
            //    VBOpenFile(1, App.path + "\\autosaved.gset");;
            //    Write(1, false);
            //    VBCloseFile(1);();
            //    Call(restarter());
            //  }
            ////4/14/2014
            //  if (chkZBmode.value == 1) {
            //    for(ecocount=1; ecocount<8; ecocount++) {
            ////generate folders for multi
            //      MkDir(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount);
            //      MkDir(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount);
            ////generate the zb file (multi)
            //      VBOpenFile(1, MDIForm1.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt");;
            //      int zerocount = 0;

            //      for(zerocount=1; zerocount<val(txtZlength.Text); zerocount++) {
            //        Write(1, 0);
            //      }
            //      VBCloseFile(1);();
            //      FileCopy(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
            //    }
            ////Botsareus 10/22/2015 the stages are singuler
            //    FileCopy(MDIForm1.instance.MainDir + "\\evolution\\baserob1\\Base.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage0.txt");
            //    optionsform.savesett(MDIForm1.instance.MainDir + "\\settings\\lastexit.set"); //save last settings
            ////other
            //    VBOpenFile(1, App.path + "\\restartmode.gset");;
            //    Write(1, 7);
            //    Write(1, 0);
            //    VBCloseFile(1);();
            //    VBOpenFile(1, App.path + "\\Safemode.gset");;
            //    Write(1, false);
            //    VBCloseFile(1);();
            //    VBOpenFile(1, App.path + "\\autosaved.gset");;
            //    Write(1, false);
            //    VBCloseFile(1);();
            //    Call(restarter());
            //  }

            ////unload
            //  Unload(this);
        }

        private void chkAddarob_Click(object sender, RoutedEventArgs e)
        {
            //  if (chkAddarob.value) {
            //    if (MsgBox("Make sure all robots in source dir. have a prefix \"1-\" \"2-\" etc. and #tag metadata contains name of robot.", vbOKCancel) == vbCancel) {
            //goto fine;
            //    }
            //    // TODO (not supported): On Error GoTo fine
            //    OptionsForm.instance.CommonDialog1.FileName = "";
            //    OptionsForm.instance.CommonDialog1.Filter = "Dna file(*.txt)|*.txt"; //Botsareus 1/11/2013 DNA only
            //    OptionsForm.instance.CommonDialog1.InitDir = MDIForm1.instance.MainDir + "\\robots";
            //    OptionsForm.instance.CommonDialog1.DialogTitle = WSchoosedna;
            //    OptionsForm.instance.CommonDialog1.ShowOpen();
            //    if (OptionsForm.instance.CommonDialog1.FileName != "") { //Botsareus 1/11/2013 Do not insert robot if filename is blank
            //      robpath = OptionsForm.instance.CommonDialog1.FileName;
            //    } else {
            //goto ;
            //    }
            //  }
            //return;

            //fine:
            //  chkAddarob.value = 0;
        }

        private void chkDelta2_Click(object sender, RoutedEventArgs e)
        {
            if (chkDelta2.IsChecked.Value && Visibility == Visibility.Visible)
            {
                if (MsgBox("Enabling Delta2 mutations may slow down your simulation considerably as it may optimize for extreme mutation rates. Are you sure?", vbExclamation | vbYesNo, "Global Darwinbots Settings") == vbNo)
                {
                    chkDelta2.IsChecked = false; //Botsareus 9/13/2014 Warnings for Shvarz
                }
            }
            lblD2.Visibility = chkDelta2.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
            lblPMDelta2.Visibility = chkDelta2.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
            txtPMinter.Visibility = chkDelta2.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
            lblWTC.Visibility = chkDelta2.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
            txtWTC.Visibility = chkDelta2.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;

            if (!chkDelta2.IsChecked.Value)
            {
                lblMmain.setVisible(false);
                lblMmean.setVisible(false);
                txtMainExp.setVisible(false);
                txtMainLn.setVisible(false);
                txtDevExp.setVisible(false);
                txtDevLn.setVisible(false);
                lblChance.setVisible(false);
                sldMain.setVisible(false);
                sldDev.setVisible(false);
            }
        }

        private void chkEpiReset_Click(object sender, RoutedEventArgs e)
        {
            txtMEmp.IsEnabled = chkEpiReset.IsChecked.Value;
            txtOP.IsEnabled = chkEpiReset.IsChecked.Value;
        }

        private void chkFilter_Click(object sender, RoutedEventArgs e)
        {
            if (chkFilter.IsChecked.Value)
            {
                chkTournament.IsChecked = false;
                chkStepladder.IsChecked = false;
                chkSurvivalSimple.IsChecked = false;
                chkZBmode.IsChecked = false;
                chkSurvivalEco.IsChecked = false;
                lblSource.setVisible(true);
                txtSourceDir.setVisible(true);
                OptionsForm.instance.CommonDialog1.FileName = "";
                OptionsForm.instance.CommonDialog1.Filter = "Dna file(*.txt)|*.txt"; //Botsareus 1/11/2013 DNA only
                OptionsForm.instance.CommonDialog1.InitDir = MDIForm1.instance.MainDir + "\\robots";
                OptionsForm.instance.CommonDialog1.DialogTitle = WSchoosedna;
                OptionsForm.instance.CommonDialog1.ShowOpen();
                if (OptionsForm.instance.CommonDialog1.FileName != "")
                { //Botsareus 1/11/2013 Do not insert robot if filename is blank
                    robpath = OptionsForm.instance.CommonDialog1.FileName;
                }
                else
                {
                    goto fine;
                }
            }
            else
            {
                if (!(chkStepladder.IsChecked.Value) && !(chkTournament.IsChecked.Value))
                {
                    lblSource.setVisible(false);
                    txtSourceDir.setVisible(false);
                }
            }
            return;

        fine:
            chkFilter.IsChecked = false;
        }

        private void chkIntRnd_Click(object sender, RoutedEventArgs e)
        {
            if (chkIntRnd.IsChecked.Value && Visibility == Visibility.Visible)
            {
                MsgBox("The pictures are loaded from urls listed in the " + App.path + "\\web.gset file.", vbInformation);
            }
        }

        private void chkNorm_Click(object sender, RoutedEventArgs e)
        {
            chkNorm_Click();
        }

        private void chkNorm_Click()
        {
            txtDnalen.Visibility = chkNorm.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
            lblDnalen.Visibility = chkNorm.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
            txtMxDnalen.Visibility = chkNorm.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void chkStepladder_Click(object sender, RoutedEventArgs e)
        {
            chkStepladder_Click();
        }

        private void chkStepladder_Click()
        {
            if (chkStepladder.IsChecked.Value)
            {
                lblSource.setVisible(true);
                txtSourceDir.setVisible(true);
                chkSurvivalSimple.IsChecked = false;
                chkSurvivalEco.IsChecked = false;
                chkZBmode.IsChecked = false;
                chkFilter.IsChecked = false;
            }
            else
            {
                if (chkTournament.IsChecked == false && chkFilter.IsChecked == false)
                {
                    lblSource.setVisible(false);
                    txtSourceDir.setVisible(false);
                }
            }
            //default chloroplasts
            txtStartChlr.Text = "16000";
            chkAddarob.Visibility = chkStepladder.IsChecked.Value && !chkTournament.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void chkSurvivalEco_Click(object sender, RoutedEventArgs e)
        {
            chkSurvivalEco_Click();
        }

        private void chkSurvivalEco_Click()
        {
            if (chkSurvivalEco.value == 1)
            {
                chkStepladder.value = 0;
                chkTournament.value = 0;
                chkZBmode.value = 0;
                chkSurvivalSimple.value = 0;
                chkIM.setVisible(true);
                chkNormSize.setVisible(false);
                chkNormSize.value = 0;
                chkFilter.value = 0;
            }
            ffmSurvival.Visibility = chkSurvivalEco.value == 1;
            //default chloroplasts
            txtStartChlr.Text = 16000; //default chloroplasts
        }

        private void chkSurvivalSimple_Click(object sender, RoutedEventArgs e)
        {
            chkSurvivalSimple_Click();
        }

        private void chkSurvivalSimple_Click()
        {
            if (chkSurvivalSimple.value == 1)
            {
                chkStepladder.value = 0;
                chkTournament.value = 0;
                chkFilter.value = 0;
                chkZBmode.value = 0;
                chkSurvivalEco.value = 0;
                chkNormSize.setVisible(true);
                chkIM.setVisible(false);
            }
            ffmSurvival.Visibility = chkSurvivalSimple.value == 1;
            //default chloroplasts
            txtStartChlr.Text = 16000; //default chloroplasts
        }

        private void chkTournament_Click(object sender, RoutedEventArgs e)
        {
            chkTournament_Click();
        }

        private void chkTournament_Click()
        {
            if (chkTournament.value == 1)
            {
                chkStepladder.Caption = "Stepladder league (starts between 16 and 31 robots)";
                lblSource.setVisible(true);
                txtSourceDir.setVisible(true);
                chkSurvivalSimple.value = 0;
                chkZBmode.value = 0;
                chkSurvivalEco.value = 0;
                chkFilter.value = 0;
            }
            else
            {
                chkStepladder.Caption = "Stepladder league";
                if (!(chkStepladder.value) == 1 && !(chkFilter.value) == 1)
                {
                    lblSource.setVisible(false);
                    txtSourceDir.setVisible(false);
                }
            }
            //default chloroplasts
            txtStartChlr.Text = 16000;
            chkAddarob.Visibility = chkStepladder.value == 1 && chkTournament.value == 0;
        }

        private void chkUseCD_Click(object sender, RoutedEventArgs e)
        {
            chkUseCD_Click();
        }

        private void chkUseCD_Click()
        {
            if (chkUseCD.value == 1)
            {
                if (Visible)
                {
                    MsgBox("If you are running parallel simulations on a single computer, make sure you disable this setting or make the path is unique for each instance. Also, don't forget to have each Darwin.exe in a separate directory");
                }
                txtCD.IsEnabled = true;
            }
            else
            {
                txtCD.IsEnabled = false;
            }
        }

        private void chkZBmode_Click(object sender, RoutedEventArgs e)
        {
            chkZBmode_Click();
        }

        private void chkZBmode_Click()
        {
            if (chkZBmode.value == 1)
            {
                chkStepladder.value = 0;
                chkTournament.value = 0;
                chkSurvivalSimple.value = 0;
                chkNormSize.value = 0;
                chkShowGraphs.value = 0;
                chkSurvivalEco.value = 0;
                chkFilter.value = 0;
            }
            ffmZeroBot.Visibility = chkZBmode.value == 1;
            txtStartChlr.Text = 16000; //default chloroplasts
        }

        private void Command1_Click(object sender, RoutedEventArgs e)
        {
            Command1_Click();
        }

        private void Command1_Click()
        {
            frmRestriOps.instance.res_state = 2;
            frmRestriOps.Show(vbModal);
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Form_Load();
        }

        private void Form_Load()
        {
            //load all global settings into controls
            chkScreenRatio.value = IIf(screenratiofix, 1, 0);
            txtBodyFix.Text = bodyfix;
            chkGreedy.IsChecked = IIf(reprofix, 1, 0);
            chkchseedstartnew.value = IIf(chseedstartnew, 1, 0);
            chkchseedloadsim.value = IIf(chseedloadsim, 1, 0);
            chkNoBoyMsg.value = IIf(loadboylabldisp, 1, 0); //some global settings change within simulation
            chkNoVid.value = IIf(loadstartnovid, 1, 0); //some global settings change within simulation
            txtCD.Text = MDIForm1.instance.MainDir;
            //only eanable txtCD and chkUseCD if maindir.gset exisits
            if (dir(App.path + "\\Maindir.gset") != "")
            {
                chkUseCD.value = 1;
                txtCD.IsEnabled = true;
            }
            else
            {
                chkUseCD.value = 0;
                txtCD.IsEnabled = false;
            }
            //are we using safemode
            chkSafeMode.IsChecked = IIf(UseSafeMode, 1, 0);
            //find best
            sldFindBest.value = intFindBestV2;
            //use old color
            chkOldColor.IsChecked = IIf(UseOldColor, 1, 0);
            //epigenetic reset
            chkEpiReset.IsChecked = IIf(epireset, 1, 0);
            txtMEmp.Text = epiresetemp;
            txtOP.Text = epiresetOP;
            txtMEmp.IsEnabled = chkEpiReset.value == 1;
            txtOP.IsEnabled = chkEpiReset.value == 1;
            //Eclipse mutations
            chkSunbelt.value = IIf(sunbelt, 1, 0);
            //Delta2
            chkDelta2.value = IIf(Delta2, 1, 0);
            txtMainExp.Text = DeltaMainExp;
            txtMainLn.Text = DeltaMainLn;
            txtDevExp.Text = DeltaDevExp;
            txtDevLn.Text = DeltaDevLn;
            txtPMinter.Text = DeltaPM;
            txtWTC.Text = DeltaWTC;
            sldMain.DefaultProperty = DeltaMainChance;
            sldDev.DefaultProperty = DeltaDevChance;
            //Norm Mut
            chkNorm.IsChecked = IIf(NormMut, 1, 0);
            txtDnalen.Text = valNormMut;
            txtMxDnalen.Text = valMaxNormMut;
            //Set values Delta2 and Norm mut
            lblD2.Visibility = chkDelta2.value == 1;
            lblPMDelta2.Visibility = chkDelta2.value == 1;
            txtPMinter.Visibility = chkDelta2.value == 1;
            lblWTC.Visibility = chkDelta2.value == 1;
            txtWTC.Visibility = chkDelta2.value == 1;

            txtDnalen.Visibility = chkNorm.value == 1;
            lblDnalen.Visibility = chkNorm.value == 1;
            txtMxDnalen.Visibility = chkNorm.value == 1;

            txtSourceDir.Text = leagueSourceDir;
            optFudging(x_fudge).value = true;

            txtStartChlr.text = StartChlr;

            optDisqua(Disqualify).value = true;

            txtRob.Text = y_robdir;
            chkShowGraphs.IsChecked = IIf(y_graphs, 1, 0);
            chkNormSize.IsChecked = IIf(y_normsize, 1, 0);
            txtCycSM.Text = y_hidePredCycl;
            txtLFOR.Text = y_LFOR;

            txtZlength.Text = y_zblen;

            if (y_eco_im > 0)
            {
                chkIM.value = y_eco_im - 1;
            }

            chkGraphUp.value = IIf(GraphUp, 1, 0);
            chkHide.value = IIf(HideDB, 1, 0);

            chkEpiGene.IsChecked = IIf(UseEpiGene, 1, 0);

            chkIntRnd.IsChecked = IIf(UseIntRnd, 1, 0);
        }

        private void lblD2_Click(object sender, RoutedEventArgs e)
        {
            lblD2_Click();
        }

        private void lblD2_Click()
        {
            lblMmain.setVisible(true);
            lblMmean.setVisible(true);
            txtMainExp.setVisible(true);
            txtMainLn.setVisible(true);
            txtDevExp.setVisible(true);
            txtDevLn.setVisible(true);
            lblChance.setVisible(true);
            sldMain.setVisible(true);
            sldDev.setVisible(true);
            lblD2.setVisible(false);
        }

        private void txtBodyFix_LostFocus(object sender, RoutedEventArgs e)
        {
            txtBodyFix_LostFocus();
        }

        private void txtBodyFix_LostFocus()
        {
            //make sure the value is sane
            txtBodyFix.Text = Abs(val(txtBodyFix.Text));
            if (txtBodyFix.Text > 32100)
            {
                txtBodyFix.Text = 32100;
            }
            if (txtBodyFix.Text < 2500)
            {
                if (MsgBox("It is not recommended to set 'Cheating Prevention' below 2500 because it may result in your robots never getting enough body to survive. Do you want to set to 2500 instead?", vbExclamation | vbYesNo, "Global Darwinbots Settings") == vbYes)
                {
                    txtBodyFix.Text = 2500; //Botsareus 9/13/2014 Warnings for Shvarz
                }
            }
            if (txtBodyFix.Text < 1000)
            {
                txtBodyFix.Text = 1000;
            }
        }

        private void txtCycSM_LostFocus(object sender, RoutedEventArgs e)
        {
            txtCycSM_LostFocus();
        }

        private void txtCycSM_LostFocus()
        {
            //make sure the value is sane
            txtCycSM.Text = Int(Abs(val(txtCycSM.Text)));
            if (txtCycSM.Text < 150)
            {
                txtCycSM.Text = 150;
            }
            if (txtCycSM.Text > 15000)
            {
                txtCycSM.Text = 15000;
            }
        }

        private void txtDevExp_LostFocus(object sender, RoutedEventArgs e)
        {
            txtDevExp_LostFocus();
        }

        private void txtDevExp_LostFocus()
        {
            //make sure the value is sane
            txtDevExp.Text = Abs(val(txtDevExp.Text));
            if (txtDevExp.Text == 0)
            {
                return;
            }
            if (txtDevExp.Text < 0.4m)
            {
                txtDevExp.Text = 0.4m;
            }
            if (txtDevExp.Text > 25)
            {
                txtDevExp.Text = 25;
            }
        }

        private void txtDevLn_LostFocus(object sender, RoutedEventArgs e)
        {
            txtDevLn_LostFocus();
        }

        private void txtDevLn_LostFocus()
        {
            //make sure the value is sane
            txtDevLn.Text = Abs(val(txtDevLn.Text));
            if (txtDevLn.Text > 5000)
            {
                txtDevLn.Text = 3000;
            }
        }

        private void txtDnalen_LostFocus(object sender, RoutedEventArgs e)
        {
            txtDnalen_LostFocus();
        }

        private void txtDnalen_LostFocus()
        {
            //make sure the value is sane
            txtDnalen.Text = Abs(val(txtDnalen.Text));
            if (txtDnalen.Text < 1)
            {
                txtDnalen.Text = 1;
            }
            if (txtDnalen.Text > 2000)
            {
                txtDnalen.Text = 2000;
            }
        }

        private void txtLFOR_LostFocus(object sender, RoutedEventArgs e)
        {
            txtLFOR_LostFocus();
        }

        private void txtLFOR_LostFocus()
        {
            //make sure the value is sane
            txtLFOR.Text = Abs(val(txtLFOR.Text));
            if (txtLFOR.Text < 0.01m)
            {
                txtLFOR.Text = 0.01m;
            }
            if (txtLFOR.Text > 150)
            {
                txtLFOR.Text = 150;
            }
        }

        private void txtMainExp_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMainExp_LostFocus();
        }

        private void txtMainExp_LostFocus()
        {
            //make sure the value is sane
            txtMainExp.Text = Abs(val(txtMainExp.Text));
            if (txtMainExp.Text == 0)
            {
                return;
            }
            if (txtMainExp.Text < 0.4m)
            {
                txtMainExp.Text = 0.4m;
            }
            if (txtMainExp.Text > 25)
            {
                txtMainExp.Text = 25;
            }
        }

        private void txtMainLn_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMainLn_LostFocus();
        }

        private void txtMainLn_LostFocus()
        {
            //make sure the value is sane
            txtMainLn.Text = Round(Abs(val(txtMainLn.Text)));
            if (txtMainLn.Text > 5000)
            {
                txtMainLn.Text = 3000;
            }
        }

        private void txtMEmp_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMEmp_LostFocus();
        }

        private void txtMEmp_LostFocus()
        {
            //make sure the value is sane
            txtMEmp.Text = Abs(val(txtMEmp.Text));
            if (txtMEmp.Text > 5)
            {
                txtMEmp.Text = 5;
            }
        }

        private void txtMxDnalen_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMxDnalen_LostFocus();
        }

        private void txtMxDnalen_LostFocus()
        {
            //make sure the value is sane
            txtMxDnalen.Text = Abs(val(txtMxDnalen.Text));
            if (txtMxDnalen.Text < 1)
            {
                txtMxDnalen.Text = 1;
            }
            if (txtMxDnalen.Text > 32000)
            {
                txtMxDnalen.Text = 32000;
            }
        }

        private void txtOP_LostFocus(object sender, RoutedEventArgs e)
        {
            txtOP_LostFocus();
        }

        private void txtOP_LostFocus()
        {
            //make sure the value is sane
            txtOP.Text = Abs(val(txtOP.Text));
            if (txtOP.Text > 32000)
            {
                txtOP.Text = 32000;
            }
        }

        private void txtPMinter_LostFocus(object sender, RoutedEventArgs e)
        {
            txtPMinter_LostFocus();
        }

        private void txtPMinter_LostFocus()
        {
            //make sure the value is sane
            txtPMinter.Text = Round(Abs(val(txtPMinter.Text)));
            if (txtPMinter.Text > 32000)
            {
                txtPMinter.Text = 32000;
            }
        }

        private void txtStartChlr_LostFocus(object sender, RoutedEventArgs e)
        {
            txtStartChlr_LostFocus();
        }

        private void txtStartChlr_LostFocus()
        {
            //make sure the value is sane
            txtStartChlr.Text = Abs(val(txtStartChlr.Text));
            if (txtStartChlr.Text > 32000)
            {
                txtStartChlr.Text = 32000;
            }
        }

        private void txtWTC_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txtWTC_Change();
        }

        private void txtWTC_Change()
        {
            //make sure the value is sane
            txtWTC.Text = Abs(val(txtWTC.Text));
            if (txtWTC.Text > 100)
            {
                txtWTC.Text = 100;
            }
        }

        private void txtZlength_LostFocus(object sender, RoutedEventArgs e)
        {
            txtZlength_LostFocus();
        }

        private void txtZlength_LostFocus()
        {
            //make sure the value is sane
            txtZlength.Text = Abs(Int(val(txtZlength.Text)));
            if (txtZlength.Text > 32000)
            {
                txtZlength.Text = 32000;
            }
            if (txtZlength.Text < 4)
            {
                txtZlength.Text = 4;
            }
        }
    }
}
