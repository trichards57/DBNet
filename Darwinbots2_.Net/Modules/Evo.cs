using DBNet.Forms;
using Iersera.DataModel;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using static Common;
using static DNAExecution;
using static DNAManipulations;
using static DNATokenizing;
using static Globals;
using static Master;
using static NeoMutations;
using static Robots;
using static SimOptModule;
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

    public static async Task calculateZB(int robid, double Mx, int bestrob)
    {
        if (rob[bestrob].LastMut <= 0)
        {
            await logevo("'Reset' reason: No mutations", x_filenumber);
            return;
        }

        var MratesMax = NormMut ? rob[bestrob].DnaLen * valMaxNormMut : 2000000000;

        var goodtest = false;//no duplicate message

        if (oldid != robid && oldid != 0)
        {
            await logevo($"'GoodTest' reason: oldid({oldid}) comp. id({robid})", x_filenumber);

            rob[bestrob].Mutables.mutarray[PointUP] = Math.Min(rob[bestrob].Mutables.mutarray[PointUP] * 1.15, MratesMax);
            rob[bestrob].Mutables.mutarray[P2UP] = Math.Min(rob[bestrob].Mutables.mutarray[P2UP] * 1.15, MratesMax);

            goodtest = true;
        }

        if (oldid == robid && Mx > oldMx)
        {
            rob[bestrob].Mutables.mutarray[PointUP] = Math.Min(rob[bestrob].Mutables.mutarray[PointUP] * 1.75, MratesMax);
            rob[bestrob].Mutables.mutarray[P2UP] = Math.Min(rob[bestrob].Mutables.mutarray[P2UP] * 1.75, MratesMax);

            await ZBreadyforTest(bestrob);
        }
        else if (!goodtest)
            await logevo($"'Reset' reason: oldid({oldid}) comp. id({robid}) Mx({Mx}) comp. oldMx({oldMx})", x_filenumber);

        oldMx = Mx;
        oldid = robid;
    }

    public static async Task logevo(string s, int idx = -1)
    {
        await File.AppendAllTextAsync($@"{MDIForm1.instance.MainDir}\evolution\log{(idx > -1 ? idx : "")}.txt", $"{s} {DateTime.Now.ToString()}\n");
    }

    public static async Task UpdateLostEvo()
    {
        await logevo("Evolving robot lost, decreasing difficulty.");
        await Decrease_Difficulty(); //Robot simply lost, se we need to loosen up the difficulty
        await exportdata();
    }

    public static async Task UpdateLostF1()
    {
        await logevo("Evolving robot lost the test, increasing difficulty.");

        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
                Directory.Delete($@"{MDIForm1.instance.MainDir}\evolution\testrob{ecocount}", true);
        }
        else
        {
            File.Delete($@"{MDIForm1.instance.MainDir}\evolution\Test.txt"); ;
            File.Delete($@"{MDIForm1.instance.MainDir}\evolution\Test.mrate"); ;
            //reset base robot
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.txt", $@"{MDIForm1.instance.MainDir}\evolution\Base.txt");
            y_Stgwins = 0;
        }
        x_restartmode = 4;
        Increase_Difficulty(); //Robot lost, might as well have never mutated
        await exportdata();
    }

    public static async Task UpdateWonEvo(int bestrob)
    {
        if (rob[bestrob].Mutations > 0 & (totnvegsDisplayed >= 15 || y_eco_im == 0))
        {
            await logevo("Evolving robot changed, testing robot.");
            //F1 mode init
            if (y_eco_im == 0)
            {
                salvarob(bestrob, $@"{MDIForm1.instance.MainDir}\evolution\Test.txt");
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
                        Form1.instance.GraphLab.Content = $"Calculating eco result: {t / MaxRobs * 100}%";
                        DoEvents();
                    }
                }

                Form1.instance.GraphLab.Visibility = Visibility.Hidden;

                //step3 calculate robots

                for (var ecocount = 1; ecocount < 15; ecocount++)
                {
                    var sEnergy = (intFindBestV2 > 100 ? 100 : intFindBestV2) / 100;
                    var sPopulation = (intFindBestV2 < 100 ? 100 : 200 - intFindBestV2) / 100;

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

                            s = Math.Pow(Form1.instance.TotalOffspring, sPopulation) * Math.Pow(s, sEnergy);
                            s *= maxgdi[t];
                            if (s >= Mx)
                            {
                                Mx = s;
                                fit = t;
                            }
                        }
                    }

                    //save and kill the robot
                    Directory.CreateDirectory($@"{MDIForm1.instance.MainDir}\evolution\testrob{ecocount}");

                    salvarob(fit, $@"{MDIForm1.instance.MainDir}\evolution\testrob{ecocount}\Test.txt");
                    rob[fit].exist = false;
                }
            }
            x_restartmode = 6;
        }
        else
        {
            await logevo("Evolving robot never changed, increasing difficulty.");
            //Increase mutation rates
            scale_mutations();
            //Robot never mutated so we need to tighten up the difficulty
            Increase_Difficulty();
        }
        await exportdata();
    }

    public static async Task UpdateWonF1()
    {
        //figure out next opponent
        y_Stgwins++;
        var currenttest = x_filenumber - y_Stgwins * (x_filenumber ^ (1 / 3));
        if (currenttest < 0 || x_filenumber == 0 || y_eco_im > 0)
        { //check for x_filenumber is zero here to prevent endless loop
            await logevo($"Evolving robot won all tests, setting up stage {x_filenumber + 1}");
            await Next_Stage(); //Robot won, go to next stage
            x_restartmode = 4;
        }
        else
        {
            //copy a robot for current test
            await logevo($"Robot is currently under test against stage {currenttest}");
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{currenttest}.txt", $@"{MDIForm1.instance.MainDir}\evolution\Base.txt");
        }
        await exportdata();
    }

    public static async Task ZBfailedtest()
    {
        await logevo("Zerobot failed the test, evolving further.");
        File.Delete($@"{MDIForm1.instance.MainDir}\evolution\Test.txt");
        File.Delete($@"{MDIForm1.instance.MainDir}\evolution\Test.mrate");
        x_restartmode = 7;

        await RestartMode.Save(x_restartmode, x_filenumber);

        //Botsareus 10/9/2016 If runs out of stages restart everything
        if (x_filenumber > 500)
        {
            //erase the folder
            Directory.Delete($@"{MDIForm1.instance.MainDir}\evolution", true);

            //make folder again
            Directory.CreateDirectory($@"{MDIForm1.instance.MainDir}\evolution\stages");

            //populate folder init
            for (var ecocount = 1; ecocount < 8; ecocount++)
            {
                //generate folders for multi
                Directory.CreateDirectory($@"{MDIForm1.instance.MainDir}\evolution\baserob{ecocount}");
                Directory.CreateDirectory($@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}");
                //generate the zb file (multi)
                using (var fileWriter = new StreamWriter($@"{MDIForm1.instance.MainDir}\evolution\baserob{ecocount}\Base.txt"))
                {
                    for (var zerocount = 1; zerocount < y_zblen; zerocount++)
                        await fileWriter.WriteLineAsync("0");
                }
                File.Copy($@"{MDIForm1.instance.MainDir}\evolution\baserob{ecocount}\Base.txt", $@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.txt");
            }

            //Botsareus 10/22/2015 the stages are singuler
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\baserob1\Base.txt", $@"{MDIForm1.instance.MainDir}\evolution\stages\stage0.txt");

            //restart
            await RestartMode.Save(7, 0);
        }
        //Restart
        DisplayActivations = false;
        Form1.instance.Active = false;
        Form1.instance.SecTimer.Enabled = false;

        await SafeMode.Save(false);
        await AutoSaved.Save(false);

        restarter();
    }

    public static void ZBpassedtest()
    {
        MessageBox.Show("Zerobot evolution complete.", "Zerobot evo", MessageBoxButton.OK, MessageBoxImage.Information);
        DisplayActivations = false;
        Form1.instance.Active = false;
        Form1.instance.SecTimer.Enabled = false;
    }

    private static async Task Decrease_Difficulty()
    {
        //Botsareus 12/11/2015 renormalize the mutation rates
        await renormalize_mutations();

        if (!LFORdir)
        {
            LFORdir = true;
            LFORcorr /= 2;
        }

        LFOR = Math.Min(LFOR + LFORcorr / n10(LFOR), 150);

        hidePredCycl = Math.Clamp((int)(Init_hidePredCycl + 300 * rndy() - 150), 150, 15000);

        //Botsareus 8/17/2016 Revert one stage, should not apply to eco evo
        if (LFORcorr < 0.00000005 && y_eco_im == 0 & x_filenumber > 0)
        {
            await logevo("Reverting one stage.");
            await revert();
        }
    }

    private static async Task exportdata()
    {
        var data = new EvoData
        {
            LFOR = LFOR,
            LFORdir = LFORdir,
            LFORcorr = LFORcorr,
            hidePredCycl = hidePredCycl,
            curr_dna_size = curr_dna_size,
            target_dna_size = target_dna_size,
            Init_hidePredCycl = Init_hidePredCycl,
            y_Stgwins = y_Stgwins
        };

        await File.WriteAllTextAsync($@"{MDIForm1.instance.MainDir}\evolution\data.gset", JsonSerializer.Serialize(data));

        //save restart mode
        if (x_restartmode == 5)
            x_restartmode = 4;

        await RestartMode.Save(x_restartmode, x_filenumber);
        await SafeMode.Save(false);
        await AutoSaved.Save(false);

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
        LFOR -= LFORcorr / n10(LFOR);
        if (LFOR < 1 / n10(tmpLFOR))
            LFOR = Math.Max(1 / n10(tmpLFOR), 0.01);

        hidePredCycl = Math.Clamp((int)(Init_hidePredCycl + 300 * rndy() - 150), 150, 15000);

        //In really rare cases start emping up difficulty using other means
        if (LFOR == 0.01)
        {
            hidePredCycl = 150;
            Init_hidePredCycl = 150;
        }
    }

    private static int n10(double a)
    {
        return a <= 1 ? (int)Math.Pow(10, 1 - (Math.Log(a * 2) / Math.Log(10))) : 1;
    }

    private static async Task Next_Stage()
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

            if (await LoadDNA(MDIForm1.instance.MainDir + "\\evolution\\Test.txt", 0))
                gotdnalen = DnaLen(rob[0].dna);

            var sizechangerate = Math.Max((5000 - target_dna_size) / 4750, 0);

            if (gotdnalen < target_dna_size)
            {
                curr_dna_size = gotdnalen + 5; //current dna size is 5 more then old size
                if ((gotdnalen >= (target_dna_size - 15)) && (gotdnalen <= (target_dna_size + sizechangerate * 75)))
                    target_dna_size += (sizechangerate * 250) + 10;
            }
            else
            {
                curr_dna_size = target_dna_size;
                if ((gotdnalen >= (target_dna_size - 15)) && (gotdnalen <= (target_dna_size + sizechangerate * 75)))
                    target_dna_size += (sizechangerate * 250) + 10;
            }
        }

        //Configure robots

        //next stage
        x_filenumber++;

        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                File.Copy($@"{MDIForm1.instance.MainDir}\evolution\testrob{ecocount}\Test.txt", $@"{MDIForm1.instance.MainDir}\evolution\stages\stagerob{ecocount}\stage{x_filenumber}.txt");
                File.Copy($@"{MDIForm1.instance.MainDir}\evolution\testrob{ecocount}\Test.mrate", $@"{MDIForm1.instance.MainDir}\evolution\stages\stagerob{ecocount}\stage{x_filenumber}.mrate");
            }
        }
        else
        {
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\Test.txt", $@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.txt");
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\Test.mrate", $@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.mrate");
        }

        //kill main dir robots
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                Directory.Delete($@"{MDIForm1.instance.MainDir}\evolution\baserob{ecocount}", true);
                Directory.Delete($@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}", true);
            }
        }
        else
        {
            File.Delete($@"{MDIForm1.instance.MainDir}\evolution\Base.txt");
            File.Delete($@"{MDIForm1.instance.MainDir}\evolution\Mutate.txt");

            if (File.Exists($@"{MDIForm1.instance.MainDir}\evolution\Mutate.mrate"))
                File.Delete($@"{MDIForm1.instance.MainDir}\evolution\Mutate.mrate"); ;
        }

        //copy robots
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                Directory.CreateDirectory($@"{MDIForm1.instance.MainDir}\evolution\baserob{ecocount}");
                Directory.CreateDirectory($@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}");
                File.Copy($@"{MDIForm1.instance.MainDir}\evolution\testrob{ecocount}\Test.txt", $@"{MDIForm1.instance.MainDir}\evolution\baserob{ecocount}\Base.txt");
                File.Copy($@"{MDIForm1.instance.MainDir}\evolution\testrob{ecocount}\Test.txt", $@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.txt");
                File.Copy($@"{MDIForm1.instance.MainDir}\evolution\testrob{ecocount}\Test.mrate", $@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.mrate");
            }
        }
        else
        {
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\Test.txt", $@"{MDIForm1.instance.MainDir}\evolution\Base.txt");
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\Test.txt", $@"{MDIForm1.instance.MainDir}\evolution\Mutate.txt");
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\Test.mrate", $@"{MDIForm1.instance.MainDir}\evolution\Mutate.mrate");
        }

        //kill test robot
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
                Directory.Delete(MDIForm1.instance.MainDir + "\\evolution\\testrob" + ecocount, true);
        }
        else
        {
            File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Test.txt"); ;
            File.Delete(MDIForm1.instance.MainDir + "\\evolution\\Test.mrate"); ;
        }
    }

    private static async Task renormalize_mutations()
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

            filem = Load_mrates($@"{MDIForm1.instance.MainDir}\evolution\Mutate.mrate");

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

            if (await LoadDNA($@"{MDIForm1.instance.MainDir}\evolution\Mutate.txt", 0))
                length = DnaLen(rob[0].dna);

            rez = Math.Min(rez, NormMut ? length * CLng(valMaxNormMut) : 2000000000.0);
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

            Save_mrates(filem, $@"{MDIForm1.instance.MainDir}\evolution\Mutate.mrate");
        }
        else
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                //load mutations

                filem = Load_mrates($@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.mrate");

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

                if (await LoadDNA($@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.txt", 0))
                    length = DnaLen(rob[0].dna);

                rez = Math.Min(rez, NormMut ? length * CLng(valMaxNormMut) : 2000000000.0);
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

                Save_mrates(filem, $@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.mrate");
            }
        }
    }

    private static async Task revert()
    {
        //Kill a stage
        File.Delete($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.txt"); ;
        File.Delete($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.mrate"); ;
        //Update file number
        x_filenumber--;
        //Move files
        File.Copy($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.txt", $@"{MDIForm1.instance.MainDir}\evolution\Base.txt");
        File.Copy($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.txt", $@"{MDIForm1.instance.MainDir}\evolution\Mutate.txt");
        File.Copy($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.mrate", $@"{MDIForm1.instance.MainDir}\evolution\Mutate.mrate");
        //Reset data
        LFORcorr = 5;
        LFOR = (LFOR + 10) / 2; //normalize LFOR toward 10
        var fdnalen = 0;

        if (await LoadDNA($@"{MDIForm1.instance.MainDir}\evolution\Mutate.txt", 0))
            fdnalen = DnaLen(rob[0].dna);

        curr_dna_size = fdnalen + 5;
    }

    private static void scale_mutations()
    {
        var val = 5 / LFORcorr;
        val *= 5;
        mutationprobs filem;
        double tot;
        double rez;

        if (y_eco_im == 0)
        {
            //load mutations

            filem = Load_mrates($@"{MDIForm1.instance.MainDir}\evolution\Mutate.mrate");

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
            rez = tot / i;

            for (var a = 0; a < 10; a++)
            {
                if (filem.mutarray[a] > 0)
                {
                    //The lower the value, the faster it reaches 1
                    var holdrate = filem.mutarray[a];

                    if (holdrate >= (Math.Log(6, 4) * rez))
                        holdrate = (Math.Log(6, 4) * rez) - 1;

                    holdrate = Math.Max(Math.Pow(holdrate / 6 * 4, holdrate / rez), 1);

                    filem.mutarray[a] = (filem.mutarray[a] * (val - 1) + holdrate) / val;
                }
            }

            //save mutations

            Save_mrates(filem, $@"{MDIForm1.instance.MainDir}\evolution\Mutate.mrate");
        }
        else
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                //load mutations

                filem = Load_mrates($@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.mrate");

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
                rez = tot / i;

                for (var a = 0; a < 10; a++)
                {
                    if (filem.mutarray[a] > 0)
                    {
                        //The lower the value, the faster it reaches 1
                        var holdrate = filem.mutarray[a];

                        if (holdrate >= (Math.Log(6, 4) * rez))
                            holdrate = (Math.Log(6, 4) * rez) - 1;

                        holdrate = Math.Max(Math.Pow(holdrate / 6 * 4, holdrate / rez), 1);

                        filem.mutarray[a] = (filem.mutarray[a] * (val - 1) + holdrate) / val;
                    }
                }

                //save mutations

                Save_mrates(filem, $@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.mrate");
            }
        }
    }

    private static async Task ZBreadyforTest(int bestrob)
    {
        salvarob(bestrob, $@"{MDIForm1.instance.MainDir}\evolution\Test.txt");
        //the robot did evolve, so lets update
        x_filenumber++;
        File.Copy($@"{MDIForm1.instance.MainDir}\evolution\Test.txt", $@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.txt");
        File.Copy($@"{MDIForm1.instance.MainDir}\evolution\Test.mrate", $@"{MDIForm1.instance.MainDir}\evolution\stages\stage{x_filenumber}.mrate");

        //what is our lowest index?
        var lowestindex = Math.Max(x_filenumber - 7, 0);

        await logevo("Progress.");

        for (var ecocount = 1; ecocount < 8; ecocount++)
        {
            //calculate index and copy robots
            var dbn = lowestindex + (ecocount - 1) % (x_filenumber + 1);
            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{dbn}.txt", $@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.txt");
            if (File.Exists($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{dbn}.mrate"))
                File.Copy($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{dbn}.mrate", $@"{MDIForm1.instance.MainDir}\evolution\mutaterob{ecocount}\Mutate.mrate");

            File.Copy($@"{MDIForm1.instance.MainDir}\evolution\stages\stage{dbn}.txt", $@"{MDIForm1.instance.MainDir}\evolution\baserob{ecocount}\Base.txt");
        }

        x_restartmode = 9;
        SimOpts.TotRunCycle = 8001; //make sure we skip the message
                                    //restart now

        await RestartMode.Save(x_restartmode, x_filenumber);

        //Restart
        DisplayActivations = false;
        Form1.instance.Active = false;
        Form1.instance.SecTimer.Enabled = false;

        await SafeMode.Save(false);
        await AutoSaved.Save(false);

        restarter();
    }
}
