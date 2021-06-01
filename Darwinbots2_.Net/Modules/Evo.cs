using DBNet.Forms;
using Iersera.DataModel;
using Iersera.Model;
using Iersera.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using static Common;
using static DNAManipulations;
using static DNATokenizing;
using static Globals;
using static Master;
using static NeoMutations;
using static Robots;
using static SimOpt;

internal static class Evo
{
    private static int oldid;

    private static double oldMx;

    public static double CalculateExactHandycap()
    {
        return energydifXP - energydifXP2;
    }

    public static double CalculateHandycap()
    {
        return SimOpts.TotRunCycle < (hidePredCycl * 8)
            ? CalculateExactHandycap() * SimOpts.TotRunCycle / (hidePredCycl * 8)
            : CalculateExactHandycap();
    }

    public static async Task CalculateZB(int robid, double Mx, robot bestrob)
    {
        if (bestrob.LastMut <= 0)
        {
            await LogEvolution("'Reset' reason: No mutations", x_filenumber);
            return;
        }

        var MratesMax = NormMut ? bestrob.dna.Count * valMaxNormMut : 2000000000;

        var goodtest = false; // no duplicate message

        if (oldid != robid && oldid != 0)
        {
            await LogEvolution($"'GoodTest' reason: oldid({oldid}) comp. id({robid})", x_filenumber);

            bestrob.Mutables.mutarray[PointUP] = Math.Min(bestrob.Mutables.mutarray[PointUP] * 1.15, MratesMax);
            bestrob.Mutables.mutarray[P2UP] = Math.Min(bestrob.Mutables.mutarray[P2UP] * 1.15, MratesMax);

            goodtest = true;
        }

        if (oldid == robid && Mx > oldMx)
        {
            bestrob.Mutables.mutarray[PointUP] = Math.Min(bestrob.Mutables.mutarray[PointUP] * 1.75, MratesMax);
            bestrob.Mutables.mutarray[P2UP] = Math.Min(bestrob.Mutables.mutarray[P2UP] * 1.75, MratesMax);

            await ZBreadyforTest(bestrob);
        }
        else if (!goodtest)
            await LogEvolution($"'Reset' reason: oldid({oldid}) comp. id({robid}) Mx({Mx}) comp. oldMx({oldMx})", x_filenumber);

        oldMx = Mx;
        oldid = robid;
    }

    public static async Task LogEvolution(string s, int idx = -1)
    {
        await File.AppendAllTextAsync($@"evolution\log{(idx > -1 ? idx : "")}.txt", $"{s} {DateTime.Now}\n");
    }

    public static async Task UpdateLostEvo()
    {
        await LogEvolution("Evolving robot lost, decreasing difficulty.");
        await DecreaseDifficulty(); //Robot simply lost, so we need to loosen up the difficulty
        await ExportData();
    }

    public static async Task UpdateLostF1()
    {
        await LogEvolution("Evolving robot lost the test, increasing difficulty.");

        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
                Directory.Delete($@"evolution\testrob{ecocount}", true);
        }
        else
        {
            File.Delete($@"evolution\Test.txt"); ;
            File.Delete($@"evolution\Test.mrate"); ;
            //reset base robot
            File.Copy($@"evolution\stages\stage{x_filenumber}.txt", $@"evolution\Base.txt");
            y_Stgwins = 0;
        }
        x_restartmode = 4;
        IncreaseDifficulty(); //Robot lost, might as well have never mutated
        await ExportData();
    }

    public static async Task UpdateWonEvo(robot bestrob)
    {
        if (bestrob.Mutations > 0 & (totnvegsDisplayed >= 15 || y_eco_im == 0))
        {
            await LogEvolution("Evolving robot changed, testing robot.");
            //F1 mode init
            if (y_eco_im == 0)
            {
                await HDRoutines.salvarob(bestrob, @"evolution\Test.txt");
            }
            else
            {
                //The Eco Calc

                var maxgdi = new Dictionary<robot, double>();

                foreach (var robT in rob.Where(r => r.exist && !r.Veg && r.FName != "Corpse" && !(r.FName == "Base.txt" && hidepred)))
                {
                    foreach (var robT2 in rob.Where(r => r != robT && r.exist && !r.Corpse && r.FName == robT.FName))
                        maxgdi[robT] += DoGeneticDistance(robT, robT2);
                }

                //step3 calculate robots

                for (var ecocount = 1; ecocount < 15; ecocount++)
                {
                    var sEnergy = (intFindBestV2 > 100 ? 100 : intFindBestV2) / 100;
                    var sPopulation = (intFindBestV2 < 100 ? 100 : 200 - intFindBestV2) / 100;

                    double Mx = 0;
                    robot fit = null;
                    foreach (var robT in rob.Where(r => r.exist && !r.Veg && r.FName != "Corpse" && !(r.FName == "Base.txt" && hidepred)))
                    {
                        Form1.instance.TotalOffspring = 1;
                        var s = Form1.instance.score(robT, 1, 10, 0) + robT.nrg + robT.body * 10; //Botsareus 5/22/2013 Advanced fit test
                        if (s < 0)
                            s = 0; //Botsareus 9/23/2016 Bug fix

                        s = Math.Pow(Form1.instance.TotalOffspring, sPopulation) * Math.Pow(s, sEnergy);
                        s *= maxgdi[robT];
                        if (s >= Mx)
                        {
                            Mx = s;
                            fit = robT;
                        }
                    }

                    //save and kill the robot
                    Directory.CreateDirectory($@"evolution\testrob{ecocount}");

                    await HDRoutines.salvarob(fit, $@"evolution\testrob{ecocount}\Test.txt");
                    fit.exist = false;
                }
            }
            x_restartmode = 6;
        }
        else
        {
            await LogEvolution("Evolving robot never changed, increasing difficulty.");
            //Increase mutation rates
            await ScaleMutations();
            //Robot never mutated so we need to tighten up the difficulty
            IncreaseDifficulty();
        }
        await ExportData();
    }

    public static async Task UpdateWonF1()
    {
        //figure out next opponent
        y_Stgwins++;
        var currenttest = x_filenumber - y_Stgwins * (x_filenumber ^ (1 / 3));
        if (currenttest < 0 || x_filenumber == 0 || y_eco_im > 0)
        {
            //check for x_filenumber is zero here to prevent endless loop
            await LogEvolution($"Evolving robot won all tests, setting up stage {x_filenumber + 1}");
            await NextStage(); //Robot won, go to next stage
            x_restartmode = 4;
        }
        else
        {
            //copy a robot for current test
            await LogEvolution($"Robot is currently under test against stage {currenttest}");
            File.Copy($@"evolution\stages\stage{currenttest}.txt", $@"evolution\Base.txt");
        }
        await ExportData();
    }

    public static async Task ZBFailedTest()
    {
        await LogEvolution("Zerobot failed the test, evolving further.");
        File.Delete(@"evolution\Test.txt");
        File.Delete(@"evolution\Test.mrate");
        x_restartmode = 7;

        await RestartMode.Save(x_restartmode, x_filenumber);

        //Botsareus 10/9/2016 If runs out of stages restart everything
        if (x_filenumber > 500)
        {
            //erase the folder
            Directory.Delete(@"evolution", true);

            //make folder again
            Directory.CreateDirectory(@"evolution\stages");

            //populate folder init
            for (var ecocount = 1; ecocount < 8; ecocount++)
            {
                //generate folders for multi
                Directory.CreateDirectory($@"evolution\baserob{ecocount}");
                Directory.CreateDirectory($@"evolution\mutaterob{ecocount}");
                //generate the zb file (multi)
                using (var fileWriter = new StreamWriter($@"evolution\baserob{ecocount}\Base.txt"))
                {
                    for (var zerocount = 1; zerocount < y_zblen; zerocount++)
                        await fileWriter.WriteLineAsync("0");
                }
                File.Copy($@"evolution\baserob{ecocount}\Base.txt", $@"evolution\mutaterob{ecocount}\Mutate.txt");
            }

            File.Copy(@"evolution\baserob1\Base.txt", @"evolution\stages\stage0.txt");

            //restart
            await RestartMode.Save(7, 0);
        }
        //Restart

        await SafeMode.Save(false);
        await AutoSaved.Save(false);

        Restarter();
    }

    public static void ZBPassedTest()
    {
        MessageBox.Show("Zerobot evolution complete.", "Zerobot evo", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private static async Task DecreaseDifficulty()
    {
        await RenormalizeMutations();

        if (!LFORdir)
        {
            LFORdir = true;
            LFORcorr /= 2;
        }

        LFOR = Math.Min(LFOR + LFORcorr / n10(LFOR), 150);

        hidePredCycl = Math.Clamp(Init_hidePredCycl + ThreadSafeRandom.Local.Next(-150, 150), 150, 15000);

        //Botsareus 8/17/2016 Revert one stage, should not apply to eco evo
        if (LFORcorr < 0.00000005 && y_eco_im == 0 & x_filenumber > 0)
        {
            await LogEvolution("Reverting one stage.");
            await Revert();
        }
    }

    private static async Task ExportData()
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

        await File.WriteAllTextAsync(@"evolution\data.gset", JsonSerializer.Serialize(data));

        if (x_restartmode == 5)
            x_restartmode = 4;

        await RestartMode.Save(x_restartmode, x_filenumber);
        await SafeMode.Save(false);
        await AutoSaved.Save(false);

        Restarter();
    }

    private static void IncreaseDifficulty()
    {
        if (LFORdir)
        {
            LFORdir = false;
            LFORcorr /= 2;
        }

        var tmpLFOR = LFOR;
        LFOR -= LFORcorr / n10(LFOR);
        if (LFOR < 1 / n10(tmpLFOR))
            LFOR = Math.Max(1 / n10(tmpLFOR), 0.01);

        hidePredCycl = Math.Clamp(Init_hidePredCycl + ThreadSafeRandom.Local.Next(-150, 150), 150, 15000);

        //In really rare cases start emping up difficulty using other means
        if (LFOR == 0.01)
        {
            hidePredCycl = 150;
            Init_hidePredCycl = 150;
        }
    }

    private static int n10(double a)
    {
        return a <= 1 ? (int)Math.Pow(10, 1 - Math.Log10(a * 2)) : 1;
    }

    private static async Task NextStage()
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

            var rob = new robot();

            if (await LoadDNA(@"evolution\Test.txt", rob))
                gotdnalen = DnaLen(rob.dna);

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
                File.Copy($@"evolution\testrob{ecocount}\Test.txt", $@"evolution\stages\stagerob{ecocount}\stage{x_filenumber}.txt");
                File.Copy($@"evolution\testrob{ecocount}\Test.mrate", $@"evolution\stages\stagerob{ecocount}\stage{x_filenumber}.mrate");
            }
        }
        else
        {
            File.Copy(@"evolution\Test.txt", $@"evolution\stages\stage{x_filenumber}.txt");
            File.Copy(@"evolution\Test.mrate", $@"evolution\stages\stage{x_filenumber}.mrate");
        }

        //kill main dir robots
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                Directory.Delete($@"evolution\baserob{ecocount}", true);
                Directory.Delete($@"evolution\mutaterob{ecocount}", true);
            }
        }
        else
        {
            File.Delete(@"evolution\Base.txt");
            File.Delete(@"evolution\Mutate.txt");

            if (File.Exists(@"evolution\Mutate.mrate"))
                File.Delete(@"evolution\Mutate.mrate"); ;
        }

        //copy robots
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                Directory.CreateDirectory($@"evolution\baserob{ecocount}");
                Directory.CreateDirectory($@"evolution\mutaterob{ecocount}");
                File.Copy($@"evolution\testrob{ecocount}\Test.txt", $@"evolution\baserob{ecocount}\Base.txt");
                File.Copy($@"evolution\testrob{ecocount}\Test.txt", $@"evolution\mutaterob{ecocount}\Mutate.txt");
                File.Copy($@"evolution\testrob{ecocount}\Test.mrate", $@"evolution\mutaterob{ecocount}\Mutate.mrate");
            }
        }
        else
        {
            File.Copy(@"evolution\Test.txt", @"evolution\Base.txt");
            File.Copy(@"evolution\Test.txt", @"evolution\Mutate.txt");
            File.Copy(@"evolution\Test.mrate", @"evolution\Mutate.mrate");
        }

        //kill test robot
        if (y_eco_im > 0)
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
                Directory.Delete($@"evolution\testrob{ecocount}", true);
        }
        else
        {
            File.Delete(@"\evolution\Test.txt"); ;
            File.Delete(@"\evolution\Test.mrate"); ;
        }
    }

    private static async Task RenormalizeMutations()
    {
        var val = 5 / LFORcorr;
        val *= 90;
        MutationProbabilities norm = null;
        var length = 0;

        MutationProbabilities filem;
        double tot;
        double rez;

        if (y_eco_im == 0)
        {
            //load mutations

            filem = await HDRoutines.LoadMutationRates(@"evolution\Mutate.mrate");

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

            var rob = new robot();

            if (await LoadDNA(@"evolution\Mutate.txt", rob))
                length = DnaLen(rob.dna);

            rez = Math.Min(rez, NormMut ? length * valMaxNormMut : 2000000000.0);
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

            await HDRoutines.Save_mrates(filem, @"evolution\Mutate.mrate");
        }
        else
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                //load mutations

                filem = await HDRoutines.LoadMutationRates($@"evolution\mutaterob{ecocount}\Mutate.mrate");

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

                var rob = new robot();

                if (await LoadDNA($@"evolution\mutaterob{ecocount}\Mutate.txt", rob))
                    length = DnaLen(rob.dna);

                rez = Math.Min(rez, NormMut ? length * valMaxNormMut : DNAExecution.MaxIntValue);
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

                await HDRoutines.Save_mrates(filem, $@"evolution\mutaterob{ecocount}\Mutate.mrate");
            }
        }
    }

    private static async Task Revert()
    {
        //Kill a stage
        File.Delete($@"evolution\stages\stage{x_filenumber}.txt"); ;
        File.Delete($@"evolution\stages\stage{x_filenumber}.mrate"); ;
        //Update file number
        x_filenumber--;
        //Move files
        File.Copy($@"evolution\stages\stage{x_filenumber}.txt", @"evolution\Base.txt");
        File.Copy($@"evolution\stages\stage{x_filenumber}.txt", @"evolution\Mutate.txt");
        File.Copy($@"evolution\stages\stage{x_filenumber}.mrate", @"evolution\Mutate.mrate");
        //Reset data
        LFORcorr = 5;
        LFOR = (LFOR + 10) / 2; //normalize LFOR toward 10
        var fdnalen = 0;

        var rob = new robot();
        if (await LoadDNA(@"evolution\Mutate.txt", rob))
            fdnalen = DnaLen(rob.dna);

        curr_dna_size = fdnalen + 5;
    }

    private static async Task ScaleMutations()
    {
        var val = 5 / LFORcorr;
        val *= 5;
        MutationProbabilities filem;
        double tot;
        double rez;

        if (y_eco_im == 0)
        {
            //load mutations

            filem = await HDRoutines.LoadMutationRates(@"evolution\Mutate.mrate");

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

            await HDRoutines.Save_mrates(filem, @"evolution\Mutate.mrate");
        }
        else
        {
            for (var ecocount = 1; ecocount < 15; ecocount++)
            {
                //load mutations

                filem = await HDRoutines.LoadMutationRates($@"evolution\mutaterob{ecocount}\Mutate.mrate");

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

                await HDRoutines.Save_mrates(filem, $@"evolution\mutaterob{ecocount}\Mutate.mrate");
            }
        }
    }

    private static async Task ZBreadyforTest(robot bestrob)
    {
        await HDRoutines.salvarob(bestrob, @"evolution\Test.txt");
        //the robot did evolve, so lets update
        x_filenumber++;
        File.Copy(@"evolution\Test.txt", $@"evolution\stages\stage{x_filenumber}.txt");
        File.Copy(@"evolution\Test.mrate", $@"evolution\stages\stage{x_filenumber}.mrate");

        //what is our lowest index?
        var lowestindex = Math.Max(x_filenumber - 7, 0);

        await LogEvolution("Progress.");

        for (var ecocount = 1; ecocount < 8; ecocount++)
        {
            //calculate index and copy robots
            var dbn = lowestindex + (ecocount - 1) % (x_filenumber + 1);
            File.Copy($@"evolution\stages\stage{dbn}.txt", $@"evolution\mutaterob{ecocount}\Mutate.txt");
            if (File.Exists($@"evolution\stages\stage{dbn}.mrate"))
                File.Copy($@"evolution\stages\stage{dbn}.mrate", $@"evolution\mutaterob{ecocount}\Mutate.mrate");

            File.Copy($@"evolution\stages\stage{dbn}.txt", $@"evolution\baserob{ecocount}\Base.txt");
        }

        x_restartmode = 9;
        SimOpts.TotRunCycle = 8001; //make sure we skip the message
                                    //restart now

        await RestartMode.Save(x_restartmode, x_filenumber);

        //Restart

        await SafeMode.Save(false);
        await AutoSaved.Save(false);

        Restarter();
    }
}
