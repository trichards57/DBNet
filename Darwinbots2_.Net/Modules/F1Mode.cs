using DBNet.Forms;
using Iersera.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Common;
using static DNAExecution;
using static Evo;
using static Globals;
using static HDRoutines;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Robots;
using static SimOptModule;
using static VBExtension;

internal static class F1Mode
{
    public static bool ContestMode = false;

    public static int Contests = 0;

    public static decimal F1count = 0;

    public static bool FirstCycle = false;

    public static int MaxCycles = 0;

    public static int MaxPop = 0;

    public static int Maxrounds = 0;

    public static int MinRounds = 0;

    public static int optMaxCycles = 0;

    public static int optMinRounds = 0;

    public static bool Over = false;

    //For F1 Contests:
    public static List<pop> PopArray = new List<pop>(new pop[21]);

    public static bool RestartMode = false;

    public static int ReStarts = 0;

    //For restarts
    public static string robotA = "";

    public static string robotB = "";

    public static int SampFreq = 0;

    public static bool StartAnotherRound = false;

    public static int TotSpecies = 0;

    //for settings only
    //For League mode
    private static int eye11 = 0;

    private static int oldpop1;

    private static int oldpop2;

    private static bool setoldpop;

    public static async Task Countpop()
    {
        for (var t = 1; t < 20; t++)
        {
            PopArray[t].population = 0;
            PopArray[t].exist = 0;
        }

        for (var t = 1; t < MaxRobs; t++)
        {
            if (!rob[t].Veg && !rob[t].Corpse && rob[t].exist)
            {
                for (var SpeciePointer = 1; SpeciePointer < TotSpecies; SpeciePointer++)
                {
                    var realname = Left(rob[t].FName, Len(rob[t].FName) - 4);
                    if (realname == PopArray[SpeciePointer].SpName)
                    {
                        PopArray[SpeciePointer].population = PopArray[SpeciePointer].population + 1;
                        PopArray[SpeciePointer].exist = 1;
                        break;
                    }
                }
            }
        }

        if (Contests < MinRounds)
            Contest_Form.instance.Contests.Content = Contests + 1;

        Contest_Form.instance.Maxrounds.Content = IIf(optMinRounds < Maxrounds && (string)Contest_Form.instance.Winner.Content == "" || Maxrounds == 0, optMinRounds, Maxrounds);
        var SpeciesLeft = 0;
        for (var p = 1; p < TotSpecies; p++)
        {
            SpeciesLeft += PopArray[p].exist;
        }
        if (SpeciesLeft == 1 && Contests + 1 <= MinRounds && Over == false)
        {
            for (var t = 1; t < TotSpecies; t++)
            {
                if (PopArray[t].population != 0)
                {
                    PopArray[t].Wins = PopArray[t].Wins + 1;
                }
            }
        }
        Contest_Form.instance.Visibility = Visibility.Visible;
        if (PopArray[1].SpName != "")
        {
            Contest_Form.instance.Robname1.Content = PopArray[1].SpName;
            Contest_Form.instance.wins1.Content = Str(PopArray[1].Wins);
            Contest_Form.instance.Pop1.Content = Str(PopArray[1].population);
        }
        else
        {
            Contest_Form.instance.Robname1.Content = "";
            Contest_Form.instance.wins1.Content = "";
            Contest_Form.instance.Pop1.Content = "";
        }
        if (PopArray[2].SpName != "")
        {
            Contest_Form.instance.Robname2.Content = PopArray[2].SpName;
            Contest_Form.instance.Wins2.Content = Str(PopArray[2].Wins);
            Contest_Form.instance.Pop2.Content = Str(PopArray[2].population);
        }
        else
        {
            Contest_Form.instance.Robname2.Content = "";
            Contest_Form.instance.Wins2.Content = "";
            Contest_Form.instance.Pop2.Content = "";
        }
        if (PopArray[3].SpName != "")
        {
            Contest_Form.instance.Robname3.Content = PopArray[3].SpName;
            Contest_Form.instance.Wins3.Content = Str(PopArray[3].Wins);
            Contest_Form.instance.Pop3.Content = Str(PopArray[3].population);
        }
        else
        {
            Contest_Form.instance.Robname3.Content = "";
            Contest_Form.instance.Wins3.Content = "";
            Contest_Form.instance.Pop3.Content = "";
        }
        if (PopArray[4].SpName != "")
        {
            Contest_Form.instance.Robname4.Content = PopArray[4].SpName;
            Contest_Form.instance.Wins4.Content = Str(PopArray[4].Wins);
            Contest_Form.instance.Pop4.Content = Str(PopArray[4].population);
        }
        else
        {
            Contest_Form.instance.Robname4.Content = "";
            Contest_Form.instance.Wins4.Content = "";
            Contest_Form.instance.Pop4.Content = "";
        }
        if (PopArray[5].SpName != "")
        {
            Contest_Form.instance.Robname5.Content = PopArray[5].SpName;
            Contest_Form.instance.Wins5.Content = Str(PopArray[5].Wins);
            Contest_Form.instance.Pop5.Content = Str(PopArray[5].population);
        }
        else
        {
            Contest_Form.instance.Robname5.Content = "";
            Contest_Form.instance.Wins5.Content = "";
            Contest_Form.instance.Pop5.Content = "";
        }

        //Botsareus 2/11/2014 Population control
        if (MaxPop > 0)
        {
            if (PopArray[1].population > MaxPop || PopArray[2].population > MaxPop)
            {
                int erase1;
                int erase2;
                if (PopArray[1].population > PopArray[2].population)
                {
                    erase1 = MaxPop - PopArray[1].population;
                    erase2 = erase1 * (PopArray[2].population / PopArray[1].population);
                }
                else
                {
                    erase2 = MaxPop - PopArray[2].population;
                    erase1 = erase2 * (PopArray[1].population / PopArray[2].population);
                }

                var selectrobot = 0;
                double calcminenergy;
                for (var l = 0; l < -erase1; l++)
                { //only erase robots with lowest energy
                    calcminenergy = 320000;
                    for (var t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            if (Left(rob[t].FName, Len(rob[t].FName) - 4) == PopArray[1].SpName)
                            {
                                if ((rob[t].nrg + rob[t].body * 10) < calcminenergy)
                                {
                                    calcminenergy = (rob[t].nrg + rob[t].body * 10);
                                    selectrobot = t;
                                }
                            }
                        }
                    }
                    KillRobot(selectrobot);
                }

                for (var l = 0; l < -erase2; l++)
                { //only erase robots with lowest energy
                    calcminenergy = 320000;
                    for (var t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            if (Left(rob[t].FName, Len(rob[t].FName) - 4) == PopArray[2].SpName)
                            {
                                if ((rob[t].nrg + rob[t].body * 10) < calcminenergy)
                                {
                                    calcminenergy = (rob[t].nrg + rob[t].body * 10);
                                    selectrobot = t;
                                }
                            }
                        }
                    }
                    KillRobot(selectrobot);
                }
            }
        }

        if (optMaxCycles > 0)
        { //Botsareus 2/14/2014 The max cycles code
            if (SimOpts.TotRunCycle < 500 & !setoldpop)
            { //reset old pop
                if (PopArray[1].population > 0 & PopArray[2].population > 0)
                {
                    oldpop1 = PopArray[1].population;
                    oldpop2 = PopArray[2].population;
                    setoldpop = true;
                }
            }
            if (ModeChangeCycles > 1000)
            {
                if (PopArray[1].population > PopArray[2].population)
                {
                    if ((PopArray[1].population - oldpop1) < (PopArray[2].population - oldpop2) && PopArray[2].population > 10)
                    {
                        optMaxCycles += 1000 / (1 / (PopArray[1].population / PopArray[2].population - 1) + 1);
                    }
                }
                if (PopArray[2].population > PopArray[1].population)
                {
                    if ((PopArray[2].population - oldpop2) < (PopArray[1].population - oldpop1) && PopArray[1].population > 10)
                    {
                        optMaxCycles += 1000 / (1 / (PopArray[2].population / PopArray[1].population - 1) + 1);
                    }
                }
                oldpop1 = PopArray[1].population;
                oldpop2 = PopArray[2].population;
                ModeChangeCycles = 0;
            }
            if (SimOpts.TotRunCycle > optMaxCycles)
            { //Botsareus 2/14/2014 kill losing species
                if (PopArray[1].population > PopArray[2].population)
                {
                    for (var t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            if (Left(rob[t].FName, Len(rob[t].FName) - 4) == PopArray[2].SpName)
                            {
                                KillRobot(t);
                            }
                        }
                    }
                }
                if (PopArray[2].population > PopArray[1].population)
                {
                    for (var t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            if (Left(rob[t].FName, Len(rob[t].FName) - 4) == PopArray[1].SpName)
                            {
                                KillRobot(t);
                            }
                        }
                    }
                }
            }
        }

        //counts population of robots at regular intervals
        //for auto-combat mode and for automatic reset of starting conditions

        var Winner = "";
        //Botsareus 2/11/2014 check here for max per contestent
        if (Maxrounds > 0)
        {
            for (var t = 1; t < TotSpecies; t++)
            {
                if (PopArray[t].Wins > Maxrounds - 1)
                {
                    Winner = PopArray[t].SpName;
                }
            }
        }

        F1count = 0;
        var Wins = Math.Sqrt(MinRounds) + MinRounds / 2;
        if (SpeciesLeft == 0)
        { //in very rear cases both robots are dead when checking, start another round
            StartAnotherRound = true;
            startnovid = loadstartnovid; //Botsareus bugfix for no vedio
        }

        if (SpeciesLeft == 1 && Contests + 1 <= MinRounds)
        {
            if (Contests + 1 == MinRounds && Over == false)
            { //contest is over now
                for (var t = 1; t < TotSpecies; t++)
                {
                    if (PopArray[t].Wins > Wins)
                    {
                        Winner = PopArray[t].SpName;
                        Over = true;
                        DisplayActivations = false;
                        Form1.instance.Active = false;
                        Form1.instance.SecTimer.Enabled = false;
                        switch (x_restartmode)
                        { //all new league components start with "x_"
                            case 10:
                                if (Winner == "robotA")
                                {
                                    FileCopy($@"{MDIForm1.instance.MainDir}\league\robotA.txt", $@"{MDIForm1.instance.MainDir}\league\seeded\{robotA}");
                                }
                                await Iersera.DataModel.RestartMode.Save(10, x_filenumber);
                                await SafeMode.Save(false);
                                restarter();
                                break;

                            case 6:
                                if (Winner == "Test")
                                    await UpdateWonF1();
                                if (Winner == "Base")
                                    await UpdateLostF1();
                                break;

                            case 0:
                                MessageBox.Show(Winner + " has won.");
                                MinRounds = optMinRounds;
                                break;

                            case 2:
                                //R E S T A R T  N E X T
                                //first we make sure next round folder is there
                                Directory.CreateDirectory($@"{MDIForm1.instance.MainDir}\league\round{x_filenumber + 1}");

                                if (Winner == "robotA")
                                    File.Copy($@"{MDIForm1.instance.MainDir}\league\robotA.txt", $@"{MDIForm1.instance.MainDir}\league\round{x_filenumber + 1}\{robotA}");

                                if (Winner == "robotB")
                                    FileCopy($@"{MDIForm1.instance.MainDir}\league\robotB.txt", $@"{MDIForm1.instance.MainDir}\league\round{x_filenumber + 1}\{robotB}");

                                await SafeMode.Save(false);

                                restarter();
                                break;

                            case 3:
                                if (Winner == "robotA")
                                    await populateladder();

                                if (Winner == "robotB")
                                {
                                    //move file to current position
                                    robotB = Directory.GetFiles(leagueSourceDir, "*.*").First();
                                    movetopos($"{leagueSourceDir}\\{robotB}", x_filenumber);
                                    //reset filenumber
                                    x_filenumber = 0;
                                    //start another round
                                    await populateladder();
                                }
                                break;
                        }
                        return;
                    }
                    else
                    {
                        Winner = "Statistical Draw. Extending contest.";
                    }
                }
                Contest_Form.instance.Winner.Content = Winner;

                if (Winner != "Statistical Draw. Extending contest.")
                    Contest_Form.instance.Winner1.Content = "Winner";
                else
                    MinRounds++;
            }

            if (Contests + 1 <= MinRounds && Over == false)
            {
                Contests++;
                StartAnotherRound = true;
                startnovid = loadstartnovid; //Botsareus bugfix for no vedio
                SimOpts.TotRunCycle = 0;
                setoldpop = false;
            }
            else
                StartAnotherRound = false;
        }
    }

    public static void dreason(string Name, string tag, string reason)
    {
        //format the tag
        if (tag[..45].Trim() == string.Empty)
            tag = "";
        else
            tag = $"({tag[..45].Trim()})";

        File.AppendAllText($@"{MDIForm1.instance.MainDir}\Disqualifications.txt", $"Robot \"{Name}\"{tag} has been disqualified for {reason}.");

        //kill species
        for (var t = 1; t < MaxRobs; t++)
        {
            if (!rob[t].Veg && !rob[t].Corpse && rob[t].exist && rob[t].FName == Name)
                KillRobot(t);
        }
    }

    public static void FindSpecies()
    {
        var robcol = new int[11];

        TotSpecies = 0;

        if (Contests == 0)
            ResetContest();

        for (var t = 1; t < 20; t++)
        {
            PopArray[t].SpName = "";
            PopArray[t].population = 0;
        }

        Contest_Form.instance.Show();
        Contest_Form.instance.Contests.Content = Str(Contests);

        for (var t = 0; t < MaxRobs; t++)
        {
            if (!rob[t].Veg && !rob[t].Corpse && rob[t].exist)
            {
                //counts species of robots at beginning of simulation
                for (var SpeciePointer = 1; SpeciePointer < 20; SpeciePointer++)
                {
                    var realname = Left(rob[t].FName, Len(rob[t].FName) - 4);
                    if (realname == PopArray[SpeciePointer].SpName)
                    {
                        PopArray[SpeciePointer].population = PopArray[SpeciePointer].population + 1;
                        break;
                    }
                    if (PopArray[SpeciePointer].SpName == "")
                    {
                        TotSpecies++;
                        PopArray[SpeciePointer].SpName = realname;
                        PopArray[SpeciePointer].population = PopArray[SpeciePointer].population + 1;
                        robcol[SpeciePointer] = rob[t].color;
                        break;
                    }
                }
            }
        }
        if (TotSpecies == 1)
        {
            ContestMode = false;
            MDIForm1.instance.F1Piccy.Visibility = Visibility.Hidden;
            Contest_Form.instance.Visibility = Visibility.Hidden;
            MsgBox("You have only selected one species for combat. Formula 1 mode disabled", vbOKOnly);
            return;
        }
        //Botsareus 2/11/2014 reset time limit and stuff
        if (TotSpecies > 2 && (MaxCycles > 0 || MaxPop > 0))
        {
            optMaxCycles = 0;
            MaxPop = 0;
            MsgBox("You have selected more then two species for combat. Cycle limit and max population disabled", vbOKOnly);
        }
        else
            optMaxCycles = MaxCycles;

        if (PopArray[1].SpName != "")
        {
            Contest_Form.instance.Robname1.Content = PopArray[1].SpName;
            Contest_Form.instance.wins1.Content = Str(PopArray[1].Wins);
            Contest_Form.instance.Pop1.Content = Str(PopArray[1].population);
            Contest_Form.instance.Robname1.ForeColor = robcol[1];
            Contest_Form.instance.Option1_1.setVisible(true);
        }
        else
        {
            Contest_Form.instance.Robname1.Content = "";
            Contest_Form.instance.wins1.Content = "";
            Contest_Form.instance.Pop1.Content = "";
            Contest_Form.instance.Option1_1.setVisible(false);
        }
        if (PopArray[2].SpName != "")
        {
            Contest_Form.instance.Robname2.Content = PopArray[2].SpName;
            Contest_Form.instance.Wins2.Content = Str(PopArray[2].Wins);
            Contest_Form.instance.Pop2.Content = Str(PopArray[2].population);
            Contest_Form.instance.Robname2.ForeColor = robcol[2];
            Contest_Form.instance.Option1_2.setVisible(true);
        }
        else
        {
            Contest_Form.instance.Robname2.Content = "";
            Contest_Form.instance.Wins2.Content = "";
            Contest_Form.instance.Pop2.Content = "";
            Contest_Form.instance.Option1_2.setVisible(false);
        }
        if (PopArray[3].SpName != "")
        {
            Contest_Form.instance.Robname3.Content = PopArray[3].SpName;
            Contest_Form.instance.Wins3.Content = Str(PopArray[3].Wins);
            Contest_Form.instance.Pop3.Content = Str(PopArray[3].population);
            Contest_Form.instance.Robname3.ForeColor = robcol[3];
            Contest_Form.instance.Option1_3.setVisible(true);
        }
        else
        {
            Contest_Form.instance.Robname3.Content = "";
            Contest_Form.instance.Wins3.Content = "";
            Contest_Form.instance.Pop3.Content = "";
            Contest_Form.instance.Option1_3.setVisible(false);
        }
        if (PopArray[4].SpName != "")
        {
            Contest_Form.instance.Robname4.Content = PopArray[4].SpName;
            Contest_Form.instance.Wins4.Content = Str(PopArray[4].Wins);
            Contest_Form.instance.Pop4.Content = Str(PopArray[4].population);
            Contest_Form.instance.Robname4.ForeColor = robcol[4];
            Contest_Form.instance.Option1_4.setVisible(true);
        }
        else
        {
            Contest_Form.instance.Robname4.Content = "";
            Contest_Form.instance.Wins4.Content = "";
            Contest_Form.instance.Pop4.Content = "";
            Contest_Form.instance.Option1_4.setVisible(false);
        }
        if (PopArray[5].SpName != "")
        {
            Contest_Form.instance.Robname5.Content = PopArray[5].SpName;
            Contest_Form.instance.Wins5.Content = Str(PopArray[5].Wins);
            Contest_Form.instance.Pop5.Content = Str(PopArray[5].population);
            Contest_Form.instance.Robname5.ForeColor = robcol[5];
            Contest_Form.instance.Option1_5.setVisible(true);
        }
        else
        {
            Contest_Form.instance.Robname5.Content = "";
            Contest_Form.instance.Wins5.Content = "";
            Contest_Form.instance.Pop5.Content = "";
            Contest_Form.instance.Option1_5.setVisible(false);
        }

        if (ContestMode)
            Contest_Form.instance.Visibility = Visibility.Visible;
    }

    public static async Task populateladder()
    {
        //populate one step ladder round
        File.Delete($@"{MDIForm1.instance.MainDir}\league\robotA.txt");
        File.Delete($@"{MDIForm1.instance.MainDir}\league\robotB.txt");

        //update file number
        x_filenumber++;
        await Iersera.DataModel.RestartMode.Save(3, x_filenumber);

        string file_name;

        //files in stepladder

        var files = Directory.GetFiles($@"{MDIForm1.instance.MainDir}\league\stepladder");

        if (x_filenumber > files.Length)
        {
            //if filenumber maxed out we need to move robot and reset filenumber
            //move file to last position
            file_name = Directory.GetFiles(leagueSourceDir, "*.*").First();
            movetopos($@"{leagueSourceDir}\{file_name}", x_filenumber);

            //reset file number
            x_filenumber = 1;
            await Iersera.DataModel.RestartMode.Save(3, x_filenumber);
        }

        //RobotB
        file_name = Directory.GetFiles(leagueSourceDir, "*.*").First();
        if (file_name == "")
        {
            x_restartmode = 0;
            File.Delete("restartmode.gset"); ;
            MessageBox.Show($@"Go to {MDIForm1.instance.MainDir}\league\stepladder to view your results.", "League Complete!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return;
        }
        else
            File.Copy($@"{leagueSourceDir}\{file_name}", $@"{MDIForm1.instance.MainDir}\league\robotB.txt");

        //RobotA
        //find a file prefixed i
        for (var j = 1; j < files.Length; j++)
        {
            var tmpname = Path.GetFileName(files[j]);
            if (tmpname == x_filenumber + "-*")
                FileCopy(files[j], MDIForm1.instance.MainDir + "\\league\\robotA.txt");
        }

        await SafeMode.Save(false);

        restarter();
    }

    //for eye fudging.  Search 'fudge' to see what I mean
    public static void ResetContest()
    {
        Contests = 0;
        Contest_Form.instance.Winner.Content = "";
        Contest_Form.instance.Winner1.Content = "";
        for (var t = 1; t < 5; t++)
        {
            PopArray[t].SpName = "";
            PopArray[t].population = 0;
            PopArray[t].Wins = 0;
        }
    }

    // Option Explicit
    public class pop
    {
        public int exist = 0;
        public int population = 0;
        public string SpName = "";
        public int Wins = 0;
    }
}
