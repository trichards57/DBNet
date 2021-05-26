using DBNet.Forms;
using Iersera.DataModel;
using Iersera.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Common;
using static Evo;
using static Globals;
using static HDRoutines;
using static Robots;
using static SimOpt;

internal static class F1Mode
{
    public static bool ContestMode = false;
    public static int Contests = 0;
    public static decimal F1count = 0;
    public static int MaxCycles = 0;
    public static int MaxPop = 0;
    public static int Maxrounds = 0;
    public static int MinRounds = 0;
    public static int optMinRounds = 0;
    public static bool Over = false;
    public static List<PopulationItem> PopArray = new();
    public static bool RestartMode = false;
    public static int ReStarts = 0;
    public static string robotA = "";
    public static string robotB = "";
    public static int SampFreq = 0;
    public static bool StartAnotherRound = false;
    private static int oldpop1;
    private static int oldpop2;
    private static int optMaxCycles = 0;
    private static bool setoldpop;
    private static int TotSpecies = 0;

    public static async Task Countpop()
    {
        foreach (var pop in PopArray)
        {
            pop.population = 0;
            pop.exist = 0;
        }

        foreach (var r in rob.Where(r => !r.Veg && !r.Corpse && r.exist))
        {
            var pop = PopArray.FirstOrDefault(p => p.SpName == Path.GetFileNameWithoutExtension(r.FName));

            if (pop != null)
            {
                pop.population++;
                pop.exist = 1;
            }
        }

        var SpeciesLeft = 0;

        SpeciesLeft = PopArray.Count(p => p.exist == 1);

        if (SpeciesLeft == 1 && Contests + 1 <= MinRounds && Over == false)
        {
            foreach (var pop in PopArray.Where(p => p.population > 0))
            {
                pop.Wins++;
            }
        }

        if (MaxPop > 0 && (PopArray[0].population > MaxPop || PopArray[1].population > MaxPop))
        {
            int erase1;
            int erase2;
            if (PopArray[0].population > PopArray[1].population)
            {
                erase1 = MaxPop - PopArray[0].population;
                erase2 = erase1 * (PopArray[1].population / PopArray[1].population);
            }
            else
            {
                erase2 = MaxPop - PopArray[1].population;
                erase1 = erase2 * (PopArray[0].population / PopArray[1].population);
            }

            var selectrobot = 0;
            double calcminenergy;

            for (var l = 0; l < -erase1; l++)
            {
                //only erase robots with lowest energy
                calcminenergy = 320000;
                for (var t = 1; t < MaxRobs; t++)
                {
                    if (rob[t].exist && Path.GetFileNameWithoutExtension(rob[t].FName) == PopArray[1].SpName && (rob[t].nrg + rob[t].body * 10) < calcminenergy)
                    {
                        calcminenergy = (rob[t].nrg + rob[t].body * 10);
                        selectrobot = t;
                    }
                }
                KillRobot(selectrobot);
            }

            for (var l = 0; l < -erase2; l++)
            {
                //only erase robots with lowest energy
                calcminenergy = 320000;
                for (var t = 1; t < MaxRobs; t++)
                {
                    if (rob[t].exist && Path.GetFileNameWithoutExtension(rob[t].FName) == PopArray[2].SpName && (rob[t].nrg + rob[t].body * 10) < calcminenergy)
                    {
                        calcminenergy = (rob[t].nrg + rob[t].body * 10);
                        selectrobot = t;
                    }
                }
                KillRobot(selectrobot);
            }
        }

        if (optMaxCycles > 0)
        { //Botsareus 2/14/2014 The max cycles code
            if (SimOpts.TotRunCycle < 500 & !setoldpop && PopArray[1].population > 0 & PopArray[2].population > 0)
            {
                oldpop1 = PopArray[1].population;
                oldpop2 = PopArray[2].population;
                setoldpop = true;
            }
            if (ModeChangeCycles > 1000)
            {
                if (PopArray[1].population > PopArray[2].population && (PopArray[1].population - oldpop1) < (PopArray[2].population - oldpop2) && PopArray[2].population > 10)
                {
                    optMaxCycles += 1000 / (1 / (PopArray[1].population / PopArray[2].population - 1) + 1);
                }
                if (PopArray[2].population > PopArray[1].population && (PopArray[2].population - oldpop2) < (PopArray[1].population - oldpop1) && PopArray[1].population > 10)
                {
                    optMaxCycles += 1000 / (1 / (PopArray[2].population / PopArray[1].population - 1) + 1);
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
                        if (rob[t].exist && Path.GetFileNameWithoutExtension(rob[t].FName) == PopArray[2].SpName)
                        {
                            KillRobot(t);
                        }
                    }
                }
                if (PopArray[2].population > PopArray[1].population)
                {
                    for (var t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist && Path.GetFileNameWithoutExtension(rob[t].FName) == PopArray[1].SpName)
                        {
                            KillRobot(t);
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

                        switch (x_restartmode)
                        {
                            //all new league components start with "x_"
                            case 10:
                                if (Winner == "robotA")
                                {
                                    File.Copy(@"league\robotA.txt", $@"league\seeded\{robotA}");
                                }
                                await Iersera.DataModel.RestartMode.Save(10, x_filenumber);
                                await SafeMode.Save(false);
                                Restarter();
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
                                Directory.CreateDirectory($@"league\round{x_filenumber + 1}");

                                if (Winner == "robotA")
                                    File.Copy(@"league\robotA.txt", $@"league\round{x_filenumber + 1}\{robotA}");

                                if (Winner == "robotB")
                                    File.Copy(@"league\robotB.txt", $@"league\round{x_filenumber + 1}\{robotB}");

                                await SafeMode.Save(false);

                                Restarter();
                                break;

                            case 3:
                                if (Winner == "robotA")
                                    await populateladder();

                                if (Winner == "robotB")
                                {
                                    //move file to current position
                                    robotB = Directory.GetFiles(leagueSourceDir, "*.*").First();
                                    movetopos($@"{leagueSourceDir}\{robotB}", x_filenumber);
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

        File.AppendAllText("Disqualifications.txt", $"Robot \"{Name}\"{tag} has been disqualified for {reason}.");

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

        for (var t = 0; t < MaxRobs; t++)
        {
            if (!rob[t].Veg && !rob[t].Corpse && rob[t].exist)
            {
                //counts species of robots at beginning of simulation
                for (var SpeciePointer = 1; SpeciePointer < 20; SpeciePointer++)
                {
                    var realname = Path.GetFileNameWithoutExtension(rob[t].FName);
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
            MessageBox.Show("You have only selected one species for combat. Formula 1 mode disabled", "F1 Mode Disabled", MessageBoxButton.OK);
            return;
        }
        //Botsareus 2/11/2014 reset time limit and stuff
        if (TotSpecies > 2 && (MaxCycles > 0 || MaxPop > 0))
        {
            optMaxCycles = 0;
            MaxPop = 0;
            MessageBox.Show("You have selected more then two species for combat. Cycle limit and max population disabled", "Limits Disabled", MessageBoxButton.OK);
        }
        else
            optMaxCycles = MaxCycles;
    }

    public static async Task populateladder()
    {
        //populate one step ladder round
        File.Delete(@"league\robotA.txt");
        File.Delete(@"league\robotB.txt");

        //update file number
        x_filenumber++;
        await Iersera.DataModel.RestartMode.Save(3, x_filenumber);

        string file_name;

        //files in stepladder

        var files = Directory.GetFiles(@"league\stepladder");

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
            MessageBox.Show($@"Go to league\stepladder to view your results.", "League Complete!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return;
        }
        else
            File.Copy($@"{leagueSourceDir}\{file_name}", @"league\robotB.txt");

        //RobotA
        //find a file prefixed i
        for (var j = 1; j < files.Length; j++)
        {
            var tmpname = Path.GetFileName(files[j]);
            if (tmpname == x_filenumber + "-*")
                File.Copy(files[j], @"league\robotA.txt");
        }

        await SafeMode.Save(false);

        Restarter();
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
}
