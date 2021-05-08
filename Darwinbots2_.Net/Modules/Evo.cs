using DBNet.Forms;
using System;
using System.IO;
using System.Windows;
using static Common;
using static DNAExecution;
using static DNAManipulations;
using static DNATokenizing;
using static Globals;
using static HDRoutines;
using static Master;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Interaction;
using static NeoMutations;
using static Robots;
using static SimOptModule;
using static System.Math;
using static varspecie;
using static VBExtension;

internal static class Evo
{
    private static int oldid;

    private static double oldMx;

    public static decimal calc_exact_handycap()
    {
        return energydifXP - energydifXP2;
    }

    public static decimal calc_handycap()
    {
        return SimOpts.TotRunCycle < (CLng(hidePredCycl) * CLng(8))
            ? calc_exact_handycap() * SimOpts.TotRunCycle / (CLng(hidePredCycl) * CLng(8))
            : calc_exact_handycap();
    }

    public static void calculateZB(int robid, double Mx, int bestrob)
    {
        if (rob[bestrob].LastMut > 0)
        {
            var MratesMax = 0;//used to correct out of range mutations

            MratesMax = IIf(NormMut, rob[bestrob].DnaLen * valMaxNormMut, 2000000000);

            var goodtest = false;//no duplicate message

            if (oldid != robid && oldid != 0)
            {
                logevo("'GoodTest' reason: oldid(" + oldid + ") comp. id(" + robid + ")", x_filenumber);
                var rob = Robots.rob[bestrob];
                rob.Mutables.mutarray[PointUP] = rob.Mutables.mutarray[PointUP] * 1.15;
                if (rob.Mutables.mutarray[PointUP] > MratesMax)
                    rob.Mutables.mutarray[PointUP] = MratesMax;

                rob.Mutables.mutarray[P2UP] = rob.Mutables.mutarray[P2UP] * 1.15;
                if (rob.Mutables.mutarray[P2UP] > MratesMax)
                    rob.Mutables.mutarray[P2UP] = MratesMax;

                goodtest = true;
            }

            if (oldid == robid && Mx > oldMx)
            {
                var rob = Robots.rob[bestrob];
                rob.Mutables.mutarray[PointUP] = rob.Mutables.mutarray[PointUP] * 1.75;
                if (rob.Mutables.mutarray[PointUP] > MratesMax)
                    rob.Mutables.mutarray[PointUP] = MratesMax;

                rob.Mutables.mutarray[P2UP] = rob.Mutables.mutarray[P2UP] * 1.75;
                if (rob.Mutables.mutarray[P2UP] > MratesMax)
                    rob.Mutables.mutarray[P2UP] = MratesMax;

                ZBreadyforTest(bestrob);
            }
            else
            {
                if (!goodtest)
                    logevo("'Reset' reason: oldid(" + oldid + ") comp. id(" + robid + ") Mx(" + Mx + ") comp. oldMx(" + oldMx + ")", x_filenumber);
            }

            oldMx = Mx;
            oldid = robid;
        }
        else
        { //if robot did not mutate
            logevo("'Reset' reason: No mutations", x_filenumber);
        }
    }

    public static void logevo(string s, int Index = -1)
    {
        VBOpenFile(1, MDIForm1.instance.MainDir + "\\evolution\\log" + IIf(Index > -1, Index, "") + ".txt"); ;
        VBWriteFile(1, s + " " + DateTime.Now.ToString());
        VBCloseFile(1);
    }

    public static void UpdateLostEvo()
    {
        logevo("Evolving robot lost, decreasing difficulty.");
        Decrease_Difficulty(); //Robot simply lost, se we need to loosen up the difficulty
        exportdata();
    }

    public static void UpdateLostF1()
    {
        logevo("Evolving robot lost the test, increasing difficulty.");

        if (y_eco_im > 0)
        {
            byte ecocount = 0;

            for (ecocount = 1; ecocount < 15; ecocount++)
            {
                RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount);
            }
        }
        else
        {
            File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Test.txt"); ;
            File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate"); ;
            //reset base robot
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt", MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
            y_Stgwins = 0;
        }
        x_restartmode = 4;
        Increase_Difficulty(); //Robot lost, might as well have never mutated
        exportdata();
    }

    public static void UpdateWonEvo(int bestrob)
    { //passing best robot
        if (rob[bestrob].Mutations > 0 & (totnvegsDisplayed >= 15 || y_eco_im == 0))
        {
            logevo("Evolving robot changed, testing robot.");
            //F1 mode init
            if (y_eco_im == 0)
            {
                salvarob(bestrob, MDIForm1.instance.MainDir + "\\evolution\\Test.txt");
            }
            else
            {
                //The Eco Calc

                //Step1 disable simulation execution
                DisplayActivations = false;
                Form1.instance.Active = false;
                Form1.instance.SecTimer.Enabled = false;

                //Step2 calculate cumelative genetic distance
                Form1.instance.GraphLab.Visibility = Visibility.Visible;

                var maxgdi = new double[MaxRobs];

                for (var t = 1; t < MaxRobs; t++)
                {
                    if (rob[t].exist && !rob[t].Veg && rob[t].FName != "Corpse" && !(rob[t].FName == "Base.txt" && hidepred))
                    {
                        //calculate cumelative genetic distance
                        for (var t2 = 1; t2 < MaxRobs; t2++)
                        {
                            if (t != t2)
                            {
                                if (rob[t2].exist && !rob[t2].Corpse && rob[t2].FName == rob[t].FName)
                                { // Must exist, and be of same species
                                    maxgdi[t] = maxgdi[t] + DoGeneticDistance(t, t2);
                                }
                            }
                        }
                        Form1.instance.GraphLab.Content = "Calculating eco result: " + Int(t / MaxRobs * 100) + "%";
                        DoEvents();
                    }
                }

                Form1.instance.GraphLab.Visibility = Visibility.Hidden;

                //step3 calculate robots

                for (var ecocount = 1; ecocount < 15; ecocount++)
                {
                    var sEnergy = (IIf(intFindBestV2 > 100, 100, intFindBestV2)) / 100;
                    var sPopulation = (IIf(intFindBestV2 < 100, 100, 200 - intFindBestV2)) / 100;

                    double Mx = 0;
                    var fit = 0;
                    for (var t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist && !rob[t].Veg && rob[t].FName != "Corpse" && !(rob[t].FName == "Base.txt" && hidepred))
                        {
                            Form1.instance.TotalOffspring = 1;
                            var s = Form1.instance.score(t, 1, 10, 0) + rob[t].nrg + rob[t].body * 10; //Botsareus 5/22/2013 Advanced fit test
                            if (s < 0)
                                s = 0; //Botsareus 9/23/2016 Bug fix

                            s = Pow(Form1.instance.TotalOffspring, sPopulation) * Pow(s, sEnergy);
                            s *= maxgdi[t];
                            if (s >= Mx)
                            {
                                Mx = s;
                                fit = t;
                            }
                        }
                    }

                    //save and kill the robot
                    if (Dir(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount, vbDirectory) == "")
                    {
                        MkDir(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount);
                    }
                    salvarob(fit, MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.txt");
                    rob[fit].exist = false;
                }
            }
            x_restartmode = 6;
        }
        else
        {
            logevo("Evolving robot never changed, increasing difficulty.");
            //Increase mutation rates
            scale_mutations();
            //Robot never mutated so we need to tighten up the difficulty
            Increase_Difficulty();
        }
        exportdata();
    }

    public static void UpdateWonF1()
    {
        //figure out next opponent
        y_Stgwins++;
        var currenttest = x_filenumber - y_Stgwins * (x_filenumber ^ (1 / 3));
        if (currenttest < 0 || x_filenumber == 0 || y_eco_im > 0)
        { //check for x_filenumber is zero here to prevent endless loop
            logevo("Evolving robot won all tests, setting up stage " + (x_filenumber + 1));
            Next_Stage(); //Robot won, go to next stage
            x_restartmode = 4;
        }
        else
        {
            //copy a robot for current test
            logevo("Robot is currently under test against stage " + currenttest);
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + currenttest + ".txt", MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
        }
        exportdata();
    }

    public static void ZBfailedtest()
    {
        logevo("Zerobot failed the test, evolving further.");
        File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Test.txt"); ;
        File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate"); ;
        x_restartmode = 7;
        //restart now
        VBOpenFile(1, App.path + "\\restartmode.gset"); ;
        Write(1, x_restartmode);
        Write(1, x_filenumber);
        VBCloseFile(1);
        //Botsareus 10/9/2016 If runs out of stages restart everything
        if (x_filenumber > 500)
        {
            //erase the folder
            RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution");

            //make folder again
            RecursiveMkDir(MDIForm1.instance.MainDir + "\\evolution");
            RecursiveMkDir(MDIForm1.instance.MainDir + "\\evolution\\stages");

            //populate folder init
            byte ecocount = 0;

            for (ecocount = 1; ecocount < 8; ecocount++)
            {
                //generate folders for multi
                MkDir(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount);
                MkDir(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount);
                //generate the zb file (multi)
                VBOpenFile(1, MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt"); ;
                int zerocount = 0;

                for (zerocount = 1; zerocount < y_zblen; zerocount++)
                {
                    Write(1, 0);
                }
                VBCloseFile(1);
                FileCopy(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
            }

            //Botsareus 10/22/2015 the stages are singuler
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\baserob1\\Base.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage0.txt");

            //restart
            VBOpenFile(1, App.path + "\\restartmode.gset"); ;
            Write(1, 7);
            Write(1, 0);
            VBCloseFile(1);
        }
        //Restart
        DisplayActivations = false;
        Form1.instance.Active = false;
        Form1.instance.SecTimer.Enabled = false;
        VBOpenFile(1, App.path + "\\Safemode.gset"); ;
        Write(1, false);
        VBCloseFile(1);
        VBOpenFile(1, App.path + "\\autosaved.gset"); ;
        Write(1, false);
        VBCloseFile(1);
        restarter();
    }

    public static void ZBpassedtest()
    {
        MsgBox("Zerobot evolution complete.", vbInformation, "Zerobot evo");
        DisplayActivations = false;
        Form1.instance.Active = false;
        Form1.instance.SecTimer.Enabled = false;
    }

    private static void Decrease_Difficulty()
    {
        //Botsareus 12/11/2015 renormalize the mutation rates
        renormalize_mutations();

        if (!LFORdir)
        {
            LFORdir = true;
            LFORcorr /= 2;
        }
        LFOR += LFORcorr / n10(LFOR);
        if (LFOR > 150)
        {
            LFOR = 150;
        }

        hidePredCycl = (int)(Init_hidePredCycl + 300 * rndy() - 150);

        if (hidePredCycl < 150)
        {
            hidePredCycl = 150;
        }
        if (hidePredCycl > 15000)
        {
            hidePredCycl = 15000;
        }

        //Botsareus 8/17/2016 Revert one stage, should not apply to eco evo
        if (LFORcorr < 0.00000005 && y_eco_im == 0 & x_filenumber > 0)
        {
            logevo("Reverting one stage.");
            revert();
        }
    }

    private static void exportdata()
    {
        //save main data
        VBOpenFile(1, MDIForm1.instance.MainDir + "\\evolution\\data.gset"); ;
        Write(1, LFOR); //LFOR init
        Write(1, LFORdir); //dir True means decrease diff
        Write(1, LFORcorr); //corr

        Write(1, hidePredCycl); //hidePredCycl

        Write(1, curr_dna_size); //curr_dna_size
        Write(1, target_dna_size); //target_dna_size

        Write(1, Init_hidePredCycl);

        Write(1, y_Stgwins);
        VBCloseFile(1);
        //save restart mode
        if (x_restartmode == 5)
            x_restartmode = 4;

        VBOpenFile(1, App.path + "\\restartmode.gset"); ;
        Write(1, x_restartmode);
        Write(1, x_filenumber);
        VBCloseFile(1);
        //Restart
        VBOpenFile(1, App.path + "\\Safemode.gset"); ;
        Write(1, false);
        VBCloseFile(1);
        VBOpenFile(1, App.path + "\\autosaved.gset"); ;
        Write(1, false);
        VBCloseFile(1);
        restarter();
    }

    private static void Increase_Difficulty()
    {
        if (LFORdir)
        {
            LFORdir = false;
            LFORcorr /= 2;
        }
        //Botsare us 7/01/2014 a little mod here, more sane floor on lfor

        var tmpLFOR = LFOR;
        LFOR = LFOR - LFORcorr / n10(LFOR);
        if (LFOR < 1 / n10(tmpLFOR))
        {
            LFOR = 1 / n10(tmpLFOR);
        }
        if (LFOR < 0.01)
        {
            LFOR = 0.01;
        }

        hidePredCycl = (int)(Init_hidePredCycl + 300 * rndy() - 150);

        if (hidePredCycl < 150)
            hidePredCycl = 150;

        if (hidePredCycl > 15000)
            hidePredCycl = 15000;

        //In really rear cases start emping up difficulty using other means
        if (LFOR == 0.01)
        {
            hidePredCycl = 150;
            Init_hidePredCycl = 150;
        }
    }

    private static int n10(double a)
    {
        return a <= 1 ? (int)Pow(10, 1 - (Log(a * 2) / Log(10))) : 1;
    }

    private static void Next_Stage()
    {
        //Reset F1 test
        y_Stgwins = 0;

        //Configure settings

        LFORdir = !LFORdir;
        LFORcorr = 5;
        Init_hidePredCycl = hidePredCycl;

        if (y_normsize)
        { //This stuff should only happen if y_normalize is enabled
            var gotdnalen = 0;

            if (LoadDNA(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", 0))
            {
                gotdnalen = DnaLen(rob[0].dna);
            }

            var sizechangerate = (5000 - target_dna_size) / 4750;
            if (sizechangerate < 0)
                sizechangerate = 0;

            if (gotdnalen < target_dna_size)
            {
                curr_dna_size = gotdnalen + 5; //current dna size is 5 more then old size
                if ((gotdnalen >= (target_dna_size - 15)) && (gotdnalen <= (target_dna_size + sizechangerate * 75)))
                {
                    target_dna_size = target_dna_size + (sizechangerate * 250) + 10;
                }
            }
            else
            {
                curr_dna_size = target_dna_size;
                if ((gotdnalen >= (target_dna_size - 15)) && (gotdnalen <= (target_dna_size + sizechangerate * 75)))
                {
                    target_dna_size = target_dna_size + (sizechangerate * 250) + 10;
                }
            }
        }

        //Configure robots

        //next stage
        x_filenumber++;

        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stagerob" + ecocount + "\\stage" + x_filenumber + ".txt");
                FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\stages\\stagerob" + ecocount + "\\stage" + x_filenumber + ".mrate");
            }
        }
        else
        {
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt");
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".mrate");
        }

        //kill main dir robots
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount);
                RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount);
            }
        }
        else
        {
            File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Base.txt"); ;
            File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt"); ;
            if (Dir(MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate") != "")
            {
                File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate"); ;
            }
        }

        //copy robots
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                MkDir(MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount);
                MkDir(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount);
                FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt");
                FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
                FileCopy(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount + "\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");
            }
        }
        else
        {
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt");
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");
        }

        //kill test robot
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                RecursiveRmDir(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount);
            }
        }
        else
        {
            File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Test.txt"); ;
            File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate"); ;
        }
    }

    private static void renormalize_mutations()
    {
        var val = 5 / LFORcorr;
        val *= 90;
        mutationprobs norm = null;
        var length = 0;

        mutationprobs filem;
        double tot;
        double rez;

        if (y_eco_im == 0)
        {
            //load mutations

            // TODO (not supported):     On Error GoTo nofile

            filem = Load_mrates(MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");

            //calculate normalized rate

            tot = 0;
            var i = 0;
            for (var a = 0; a < 10; a++)
            {
                if (filem.mutarray[a] > 0)
                {
                    tot += filem.mutarray[a];
                    i++;
                }
            }
            rez = tot / i * 3;

            if (LoadDNA(MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt", 0))
            {
                length = DnaLen(rob[0].dna);
            }

            if (rez > IIf(NormMut, length * CLng(valMaxNormMut), 2000000000.0))
            {
                rez = IIf(NormMut, length * CLng(valMaxNormMut), 2000000000.0);
            }
            //norm holds normalized mutation rates

            for (var a = 0; a < 10; a++)
            {
                norm.mutarray[a] = rez;
                norm.Mean[a] = 1;
                norm.StdDev[a] = 0;
            }

            SetDefaultLengths(norm);

            //renormalize mutations

            filem.CopyErrorWhatToChange = (int)((filem.CopyErrorWhatToChange * (val - 1) + norm.CopyErrorWhatToChange) / val);
            filem.PointWhatToChange = (int)((filem.PointWhatToChange * (val - 1) + norm.PointWhatToChange) / val);

            for (var a = 0; a < 10; a++)
            {
                if (filem.mutarray[a] > 0)
                {
                    filem.mutarray[a] = (filem.mutarray[a] * (val - 1) + norm.mutarray[a]) / val;
                }
                filem.Mean[a] = (filem.Mean[a] * (val - 1) + norm.Mean[a]) / val;
                filem.StdDev[a] = (filem.StdDev[a] * (val - 1) + norm.StdDev[a]) / val;
            }

            //save mutations

            Save_mrates(filem, MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");
        }
        else
        {
            int ecocount;
            for (ecocount = 1; ecocount < 15; ecocount++)
            {
                //load mutations

                filem = Load_mrates(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");

                //calculate normalized rate

                tot = 0;
                var i = 0;
                for (var a = 0; a < 10; a++)
                {
                    if (filem.mutarray[a] > 0)
                    {
                        tot += filem.mutarray[a];
                        i++;
                    }
                }
                rez = tot / i * 3;

                if (LoadDNA(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt", ref 0))
                    length = DnaLen(rob[0].dna);

                if (rez > IIf(NormMut, length * CLng(valMaxNormMut), 2000000000.0))
                    rez = IIf(NormMut, length * CLng(valMaxNormMut), 2000000000.0);
                //norm holds normalized mutation rates

                for (var a = 0; a < 10; a++)
                {
                    norm.mutarray[a] = rez;
                    norm.Mean[a] = 1;
                    norm.StdDev[a] = 0;
                }

                SetDefaultLengths(norm);

                //renormalize mutations

                filem.CopyErrorWhatToChange = (int)((filem.CopyErrorWhatToChange * (val - 1) + norm.CopyErrorWhatToChange) / val);
                filem.PointWhatToChange = (int)((filem.PointWhatToChange * (val - 1) + norm.PointWhatToChange) / val);

                for (var a = 0; a < 10; a++)
                {
                    if (filem.mutarray[a] > 0)
                    {
                        filem.mutarray[a] = (filem.mutarray[a] * (val - 1) + norm.mutarray[a]) / val;
                    }
                    filem.Mean[a] = (filem.Mean[a] * (val - 1) + norm.Mean[a]) / val;
                    filem.StdDev[a] = (filem.StdDev[a] * (val - 1) + norm.StdDev[a]) / val;
                }

                //save mutations

                Save_mrates(filem, MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");
            }
        }
    }

    private static void revert()
    {
        //Kill a stage
        File.Delete(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt"); ;
        File.Delete(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".mrate"); ;
        //Update file number
        x_filenumber--;
        //Move files
        FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt", MDIForm1.instance.MainDir + "\\evolution\\Base.txt");
        FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt", MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt");
        FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".mrate", MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");
        //Reset data
        LFORcorr = 5;
        LFOR = (LFOR + 10) / 2; //normalize LFOR toward 10
        var fdnalen = 0;

        if (LoadDNA(MDIForm1.instance.MainDir + "\\evolution\\Mutate.txt", 0))
        {
            fdnalen = DnaLen(rob[0].dna);
        }
        curr_dna_size = fdnalen + 5;
    }

    private static void scale_mutations()
    {
        var val = 5 / LFORcorr;
        val *= 5;
        int a;
        mutationprobs filem;
        double tot;
        double rez;

        if (y_eco_im == 0)
        {
            //load mutations

            // TODO (not supported):     On Error GoTo nofile

            filem = Load_mrates(MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");

            tot = 0;
            var i = 0;
            for (a = 0; a < 10; a++)
            {
                if (filem.mutarray[a] > 0)
                {
                    tot += filem.mutarray[a];
                    i++;
                }
            }
            rez = tot / i;

            for (a = 0; a < 10; a++)
            {
                if (filem.mutarray[a] > 0)
                {
                    //The lower the value, the faster it reaches 1
                    var holdrate = filem.mutarray[a];

                    if (holdrate >= (Log(6) / Log(4) * rez))
                    {
                        holdrate = (Log(6) / Log(4) * rez) - 1;
                    }
                    holdrate = Pow(holdrate / 6 * 4, (holdrate / rez));
                    if (holdrate < 1)
                    {
                        holdrate = 1;
                    }

                    filem.mutarray[a] = (filem.mutarray[a] * (val - 1) + holdrate) / val;
                }
            }

            //save mutations

            Save_mrates(filem, MDIForm1.instance.MainDir + "\\evolution\\Mutate.mrate");
        }
        else
        {
            byte ecocount;
            for (ecocount = 1; ecocount < 15; ecocount++)
            {
                //load mutations

                // TODO (not supported):         On Error GoTo nextrob

                filem = Load_mrates(MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");

                tot = 0;
                var i = 0;
                for (a = 0; a < 10; a++)
                {
                    if (filem.mutarray[a] > 0)
                    {
                        tot += filem.mutarray[a];
                        i++;
                    }
                }
                rez = tot / i;

                for (a = 0; a < 10; a++)
                {
                    if (filem.mutarray[a] > 0)
                    {
                        //The lower the value, the faster it reaches 1
                        var holdrate = filem.mutarray[a];

                        if (holdrate >= (Log(6) / Log(4) * rez))
                        {
                            holdrate = (Log(6) / Log(4) * rez) - 1;
                        }
                        holdrate = Pow(holdrate / 6 * 4, (holdrate / rez));
                        if (holdrate < 1)
                        {
                            holdrate = 1;
                        }

                        filem.mutarray[a] = (filem.mutarray[a] * (val - 1) + holdrate) / val;
                    }
                }

                //save mutations

                Save_mrates(filem, MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");
            }
        }
    }


    private static void ZBreadyforTest(int bestrob)
    {
        salvarob(bestrob, MDIForm1.instance.MainDir + "\\evolution\\Test.txt");
        //the robot did evolve, so lets update
        x_filenumber++;
        FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".txt");
        FileCopy(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate", MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + x_filenumber + ".mrate");

        //what is our lowest index?
        var lowestindex = x_filenumber - 7;
        if (lowestindex < 0)
        {
            lowestindex = 0;
        }

        logevo("Progress.");
        for (var ecocount = 1; ecocount < 8; ecocount++)
        {
            //calculate index and copy robots
            var dbn = lowestindex + (ecocount - 1) % (x_filenumber + 1);
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + dbn + ".txt", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
            if (Dir(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + dbn + ".mrate") != "")
            {
                FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + dbn + ".mrate", MDIForm1.instance.MainDir + "\\evolution\\mutaterob" + ecocount + "\\Mutate.mrate");
            }
            FileCopy(MDIForm1.instance.MainDir + "\\evolution\\stages\\stage" + dbn + ".txt", MDIForm1.instance.MainDir + "\\evolution\\baserob" + ecocount + "\\Base.txt");
        }

        x_restartmode = 9;
        SimOpts.TotRunCycle = 8001; //make sure we skip the message
                                    //restart now
        VBOpenFile(1, App.path + "\\restartmode.gset"); ;
        Write(1, x_restartmode);
        Write(1, x_filenumber);
        VBCloseFile(1);
        //Restart
        DisplayActivations = false;
        Form1.instance.Active = false;
        Form1.instance.SecTimer.Enabled = false;
        VBOpenFile(1, App.path + "\\Safemode.gset"); ;
        Write(1, false);
        VBCloseFile(1);
        VBOpenFile(1, App.path + "\\autosaved.gset"); ;
        Write(1, false);
        VBCloseFile(1);
        restarter();
    }
}
