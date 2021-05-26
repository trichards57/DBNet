using DBNet.Forms;
using Iersera.DataModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static Common;
using static DNAExecution;
using static Evo;
using static Globals;
using static HDRoutines;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Multibots;
using static Obstacles;
using static Robots;
using static Senses;
using static Shots_Module;
using static SimOptModule;
using static System.Math;
using static Teleport;
using static VBExtension;
using static Vegs;

internal static class Master
{
    public static bool CostsWereZeroed = false;

    // Option Explicit
    public static int DynamicCountdown = 0;// Used to countdown the cycles until we modify the dynamic costs

    public static double energydif = 0;

    public static double energydif2 = 0;

    //Total last hide pred
    public static double energydifX = 0;

    //Total last hide pred
    public static double energydifX2 = 0;

    //Avg last hide pred
    public static double energydifXP = 0;

    //The actual handycap
    //Avg last hide pred
    public static double energydifXP2 = 0;

    // Flag used to indicate to the reinstatement threshodl that the costs were zeroed
    public static int[] PopulationLast10Cycles = new int[11];

    public static bool savenow = false;

    public static bool stagnent = false;

    //The actual handycap
    public static bool stopflag = false;

    private static double totnrgnvegs;

    public static async Task UpdateSim()
    {
        ModeChangeCycles++;
        SimOpts.TotRunCycle++;

        //Botsareus 3/22/2014 Main hidepred logic (hide pred means hide base robot a.k.a. Predator)
        var usehidepred = x_restartmode == 4 || x_restartmode == 5; //Botsareus expend to evo mode

        if (usehidepred)
        {
            //Count species for end of evo
            var Base_count = 0;
            var Mutate_count = 0;
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist)
                {
                    if (rob[t].FName == "Base.txt")
                    {
                        Base_count++;
                    }
                    if (rob[t].FName == "Mutate.txt")
                    {
                        Mutate_count++;
                    }
                }
            }
            if (Base_count > Mutate_count)
            {
                stagnent = false; //Botsareus 10/20/2015 Base went above mutate, reset stagnent flag
            }
            //See if end of evo
            if (Mutate_count == 0)
            {
                DisplayActivations = false;
                Form1.instance.Active = false;
                Form1.instance.SecTimer.Enabled = false;
                stopflag = true; //Botsareus 9/2/2014 A bug fix from Spork22
                await UpdateLostEvo();
            }
            if (Base_count == 0 & !stopflag)
            {
                DisplayActivations = false;
                Form1.instance.Active = false;
                Form1.instance.SecTimer.Enabled = false;
                await UpdateWonEvo(Form1.instance.fittest());
            }
            //Botsareus 10/19/2015 Prevents simulation from running too long
            if (SimOpts.TotRunCycle == 1000000)
            {
                stagnent = true; //Set the stagnent flag now and see what happens
            }

        Mode:
            if (ModeChangeCycles > (hidePredCycl / 1.2m + hidePredOffset))
            {
                //Botsareus 11/5/2015 If lfor max lower limit wait for mutate pop to match base pop
                if (LFOR == 150 & Mutate_count < Base_count && hidepred)
                {
                    ModeChangeCycles -= 100;
                    goto Mode;
                }
                //calculate new energy handycap
                energydif2 += energydif / ModeChangeCycles; //inverse handycap
                if (hidepred)
                {
                    double holdXP = 0;

                    holdXP = (energydifX - (energydif / ModeChangeCycles)) / LFOR;
                    if (holdXP < energydifXP)
                    {
                        energydifXP = holdXP;
                    }
                    else
                    {
                        energydifXP = (energydifXP * 9 + holdXP) / 10;
                    }

                    //inverse handycap
                    energydifXP2 = (energydifX2 - energydif2) / LFOR;
                    if (energydifXP2 > 0)
                    {
                        energydifXP2 = 0;
                    }
                    if ((energydifXP - energydifXP2) > 0.1)
                    {
                        energydifXP2 = energydifXP - 0.1;
                    }
                    energydifX2 = energydif2;
                    energydif2 = 0;
                }
                energydifX = energydif / ModeChangeCycles;
                energydif = 0;
                //Botsareus 6/12/2016 An attempt to get rid of 'chasers' without using any reposition code:
                if (hidepred)
                {
                    //Erase offensive shots
                    for (var t = 1; t < maxshotarray; t++)
                    {
                        var shot = Shots[t];
                        if (shot.shottype == -1 || shot.shottype == -6)
                        {
                            shot.exist = false;
                            shot.flash = false;
                        }
                    }

                    //Reposition robots the safe way
                    var k2 = 0;//robots moved total
                    int k;

                    do
                    {
                        k = 0;
                        for (var t = 1; t < MaxRobs; t++)
                        {
                            if (rob[t].exist && rob[t].FName == "Base.txt")
                            {
                                for (var i = 1; i < MaxRobs; i++)
                                {
                                    if (rob[i].exist && rob[i].FName == "Mutate.txt")
                                    {
                                        double ingdist = 0;
                                        //calculate ingagment distance
                                        if (rob[t].body > rob[i].body)
                                        {
                                            if (rob[t].body > 10)
                                            {
                                                ingdist = Log(rob[t].body) * 60 + 41;
                                            }
                                            else
                                            {
                                                ingdist = 40;
                                            }
                                        }
                                        else
                                        {
                                            if (rob[i].body > 10)
                                            {
                                                ingdist = Log(rob[i].body) * 60 + 41;
                                            }
                                            else
                                            {
                                                ingdist = 40;
                                            }
                                        }
                                        ingdist = rob[t].radius + rob[i].radius + ingdist + 40; //both radii plus shot dist plus offset 1 shot travel dist

                                        var posdif = VectorSub(rob[t].pos, rob[i].pos);
                                        if (posdif.Magnitude() < ingdist)
                                        {
                                            //if the distance between the robots is less then ingagment distance
                                            var rob = Robots.rob[i];
                                            ingdist -= posdif.Magnitude(); //ingdist becomes offset dist
                                            var newpoz = rob.pos - posdif.Unit() * ingdist;
                                            var pozdif = newpoz - rob.pos;

                                            if (rob.numties > 0)
                                            {
                                                var clist = new int[51];
                                                int tk;

                                                clist[0] = i;
                                                ListCells(clist);
                                                //move multibot
                                                tk = 1;
                                                while (clist[tk] > 0)
                                                {
                                                    //Botsareus 7/15/2016 Only own species
                                                    if (Robots.rob[clist[tk]].FName == "Mutate.txt")
                                                    {
                                                        Robots.rob[clist[tk]].pos.X = Robots.rob[clist[tk]].pos.X + pozdif.X;
                                                        Robots.rob[clist[tk]].pos.Y = Robots.rob[clist[tk]].pos.Y + pozdif.Y;
                                                    }
                                                    tk++;
                                                }
                                            }
                                            rob.pos += pozdif;
                                            k++;
                                            k2++;
                                        }
                                    }
                                }
                            }
                        }
                    } while (!(k == 0 || k2 > (3200 + Mutate_count * 0.9m))); //Scales as mutate_count scales
                }
                //change hide pred
                hidepred = !hidepred;
                hidePredOffset = (int)(hidePredCycl / 3 * rndy());
                ModeChangeCycles = 0;
            }
        }

        //provides the mutation rates oscillation Botsareus 8/3/2013 moved to UpdateSim)
        if (SimOpts.MutOscill)
        { //Botsareus 10/8/2015 Yet another redo, sine wave optional
            if ((SimOpts.MutCycMax + SimOpts.MutCycMin) > 0)
            {
                if (SimOpts.MutOscillSine)
                {
                    var fullrange = SimOpts.TotRunCycle % (SimOpts.MutCycMax + SimOpts.MutCycMin);
                    if (fullrange < SimOpts.MutCycMax)
                    {
                        SimOpts.MutCurrMult = Pow(20, Sin(fullrange / SimOpts.MutCycMax * PI));
                    }
                    else
                    {
                        SimOpts.MutCurrMult = Pow(20, (Sin((fullrange - SimOpts.MutCycMax) / SimOpts.MutCycMin * PI));
                    }
                }
                else
                {
                    var fullrange = SimOpts.TotRunCycle % (SimOpts.MutCycMax + SimOpts.MutCycMin);
                    if (fullrange < SimOpts.MutCycMax)
                    {
                        SimOpts.MutCurrMult = 16;
                    }
                    else
                    {
                        SimOpts.MutCurrMult = 1 / 16;
                    }
                }
            }
        }

        TotalSimEnergyDisplayed = TotalSimEnergy[CurrentEnergyCycle];
        CurrentEnergyCycle = SimOpts.TotRunCycle % 100;
        TotalSimEnergy[CurrentEnergyCycle] = 0;

        var CurrentPopulation = totnvegsDisplayed;
        if (SimOpts.Costs[DYNAMICCOSTINCLUDEPLANTS] != 0)
        {
            CurrentPopulation += totvegsDisplayed; //Include Plants in target population
        }

        //If (SimOpts.TotRunCycle + 200) Mod 2000 = 0 Then MsgBox "sup" & SimOpts.TotRunCycle 'debug only

        if (SimOpts.TotRunCycle % 10 == 0)
        {
            for (var i = 10; i >= 2; i--)
            {
                PopulationLast10Cycles[i] = PopulationLast10Cycles[i - 1];
            }
            PopulationLast10Cycles[1] = CurrentPopulation;
        }

        if (SimOpts.Costs[USEDYNAMICCOSTS] > 0)
        {
            var AmountOff = CurrentPopulation - SimOpts.Costs[DYNAMICCOSTTARGET];

            //If we are more than X% off of our target population either way AND the population isn't moving in the
            //the direction we want or hasn't moved at all in the past 10 cycles then adjust the cost multiplier
            var UpperRange = TmpOpts.Costs[DYNAMICCOSTTARGETUPPERRANGE] * 0.01 * SimOpts.Costs[DYNAMICCOSTTARGET];
            var LowerRange = TmpOpts.Costs[DYNAMICCOSTTARGETLOWERRANGE] * 0.01 * SimOpts.Costs[DYNAMICCOSTTARGET];

            if (CurrentPopulation == PopulationLast10Cycles[10])
            {
                DynamicCountdown--;
                if (DynamicCountdown < -10)
                {
                    DynamicCountdown = -10;
                }
            }
            else
            {
                DynamicCountdown = 10;
            }

            if ((AmountOff > UpperRange && (PopulationLast10Cycles[10] < CurrentPopulation || DynamicCountdown <= 0)) || (AmountOff < -LowerRange && (PopulationLast10Cycles[10] > CurrentPopulation || DynamicCountdown <= 0)))
            {
                var CorrectionAmount = AmountOff > UpperRange ? AmountOff - UpperRange : Abs(AmountOff) - LowerRange;

                //Adjust the multiplier. The idea is to rachet this over time as bots evolve to be more effecient.
                //We don't muck with it if the bots are within X% of the target.  If they are outside the target, then
                //we adjust only if the populatiuon isn't heading towards the range and then we do it my an amount that is a function
                //of how far of the range we are (not how far from the target itself) and the sensitivity set in the sim
                SimOpts.Costs[COSTMULTIPLIER] = SimOpts.Costs[COSTMULTIPLIER] + (0.0000001 * CorrectionAmount * Sign(AmountOff) * SimOpts.Costs[DYNAMICCOSTSENSITIVITY]);

                //Don't let the costs go negative if the user doesn't want them to
                if ((SimOpts.Costs[ALLOWNEGATIVECOSTX] != 1))
                {
                    if (SimOpts.Costs[COSTMULTIPLIER] < 0)
                    {
                        SimOpts.Costs[COSTMULTIPLIER] = 0;
                    }
                }
                DynamicCountdown = 10; // Reset the countdown timer
            }
        }

        if ((CurrentPopulation < SimOpts.Costs[BOTNOCOSTLEVEL]) && (SimOpts.Costs[COSTMULTIPLIER] != 0))
        {
            CostsWereZeroed = true;
            SimOpts.OldCostX = SimOpts.Costs[COSTMULTIPLIER];
            SimOpts.Costs[COSTMULTIPLIER] = 0; // The population has fallen below the threshold to 0 all costs
        }
        else if ((CurrentPopulation > SimOpts.Costs[COSTXREINSTATEMENTLEVEL]) && CostsWereZeroed)
        {
            CostsWereZeroed = false; // Set the flag so we don't do this again unless they get zeored again
            SimOpts.Costs[COSTMULTIPLIER] = SimOpts.OldCostX;
        }

        //Store new energy handycap
        for (var t = 1; t < MaxRobs; t++)
        {
            if (rob[t].exist)
            {
                if (rob[t].FName == "Mutate.txt" && hidepred)
                {
                    if (rob[t].LastMut > 0)
                    { //4/5/2016 Handycap freshly mutated robots more than other robots
                        rob[t].nrg = rob[t].nrg + CalculateHandycap();
                    }
                    else
                    {
                        rob[t].nrg = rob[t].nrg + CalculateHandycap() / 2;
                    }
                }
            }
        }

        //core evo
        double avrnrgStart = 0;
        if (usehidepred)
        {
            //Calculate average energy before sim update
            avrnrgStart = 0;
            var i = 0;
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].FName == "Mutate.txt" && rob[t].exist)
                {
                    if (rob[t].LastMut > 0)
                    { //4/17/2014 New rule from Botsareus, only handycap fresh robots
                        i++;
                        avrnrgStart += rob[t].nrg;
                    }
                }
            }
            if (i > 0)
            {
                avrnrgStart /= i;
            }
        }

        ExecRobs();
        if (datirob.instance.Visibility == Visibility.Visible && datirob.instance.ShowMemoryEarlyCycle)
        {
            datirob.instance.infoupdate(robfocus, rob[robfocus].nrg, rob[robfocus].parent, rob[robfocus].Mutations, rob[robfocus].age, rob[robfocus].SonNumber, 1, rob[robfocus].FName, rob[robfocus].genenum, rob[robfocus].LastMut, rob[robfocus].generation, rob[robfocus].DnaLen, rob[robfocus].LastOwner, rob[robfocus].Waste, rob[robfocus].body, rob[robfocus].mass, rob[robfocus].venom, rob[robfocus].shell, rob[robfocus].Slime, rob[robfocus].chloroplasts);
        }

        //updateshots can write to bot sense, so we need to clear bot senses before updating shots
        for (var t = 1; t < MaxRobs; t++)
        {
            if (rob[t].exist)
            {
                if ((rob[t].DisableDNA == false))
                {
                    EraseSenses(t);
                }
            }
        }

        //it is time for some overwrites by playerbot mode
        if (MDIForm1.instance.pbOn.Checked)
        {
            for (var t = 1; t < MaxRobs; t++)
            {
                var rob = Robots.rob[t];
                if (rob.exist)
                {
                    if (t == robfocus || rob.highlight)
                    {
                        if (!(Mouse_loc.X == 0 & Mouse_loc.Y == 0))
                        {
                            rob.mem[SetAim] = angnorm(angle(rob.pos.X, rob.pos.Y, Mouse_loc.X, Mouse_loc.Y)) * 200;
                        }
                        for (var i = 1; i < UBound(PB_keys); i++)
                        {
                            if (PB_keys[i].Active != PB_keys[i].Invert)
                            {
                                rob.mem[PB_keys[i].memloc] = PB_keys[i].value;
                            }
                        }
                    }
                }
            }
        }

        UpdateShots();

        //Botsareus 6/22/2016 to figure actual velocity of the bot incase there is a collision event
        for (var t = 1; t < MaxRobs; t++)
        {
            if (rob[t].exist && !(rob[t].FName == "Base.txt" && hidepred))
            {
                rob[t].opos = rob[t].pos;
            }
        }

        UpdateBots();

        //to figure actual velocity of the bot incase there is a collision event
        for (var t = 1; t < MaxRobs; t++)
        {
            if (rob[t].exist && !(rob[t].FName == "Base.txt" && hidepred))
            {
                //Only if the robots position was already configured
                if (!(rob[t].opos.X == 0 & rob[t].opos.Y == 0))
                {
                    rob[t].actvel = VectorSub(rob[t].pos, rob[t].opos);
                }
            }
        }

        if (numObstacles > 0)
        {
            MoveObstacles();
        }
        if (numTeleporters > 0)
        {
            UpdateTeleporters();
        }

        int AllChlr = 0;//Panda 8/13/2013 The new way to figure total number vegys
        for (var t = 1; t < MaxRobs; t++)
        { //Panda 8/14/2013 to figure how much vegys to repopulate across all robots
            if (rob[t].exist && !(rob[t].FName == "Base.txt" & hidepred))
            { //Botsareus 8/14/2013 We have to make sure the robot is alive first
                AllChlr += (int)rob[t].chloroplasts;
            }
        }

        TotalChlr = AllChlr / 16000; //Panda 8/23/2013 Calculate total unit chloroplasts

        if (TotalChlr < CLng(SimOpts.MinVegs))
        { //Panda 8/23/2013 Only repopulate vegs when total chlroplasts below value
            if (totvegsDisplayed != -1)
            {
                VegsRepopulate(); //Will be -1 first cycle after loading a sim.  Prevents spikes.
            }
        }

        feedvegs(SimOpts.MaxEnergy);

        if (usehidepred)
        {
            //Calculate average energy after sim update
            double avrnrgEnd = 0;
            var i = 0;
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].FName == "Mutate.txt" && rob[t].exist)
                {
                    if (rob[t].LastMut > 0)
                    { //4/17/2014 New rule from Botsareus, only handycap fresh robots
                        i++;
                        avrnrgEnd += rob[t].nrg;
                    }
                }
            }
            if (i > 0)
            {
                avrnrgEnd /= i;
                energydif = energydif - avrnrgStart + avrnrgEnd;
            }
        }

        //okay, time to store some values for RGB monitor
        if (MDIForm1.instance.MonitorOn.DefaultProperty)
        {
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist)
                {
                    rob[t].monitor_r = rob[t].mem[frmMonitorSet.instance.Monitor_mem_r];
                    rob[t].monitor_g = rob[t].mem[frmMonitorSet.instance.Monitor_mem_g];
                    rob[t].monitor_b = rob[t].mem[frmMonitorSet.instance.Monitor_mem_b];
                }
            }
        }

        //Kill some robots to prevent of memory
        var totlen = 0;
        for (var t = 1; t < MaxRobs; t++)
        {
            if (rob[t].exist)
            {
                totlen += rob[t].DnaLen;
            }
        }
        if (totlen > 4000000)
        {
            var selectrobot = 0;

            var maxdel = 1500 * (TotalRobotsDisplayed * 425 / totlen);

            for (var i = 0; i < maxdel; i++)
            {
                var calcminenergy = 320000.0; //only erase robots with lowest energy
                for (var t = 1; t < MaxRobs; t++)
                {
                    if (rob[t].exist)
                    {
                        if ((rob[t].nrg + rob[t].body * 10) < calcminenergy)
                        {
                            calcminenergy = (rob[t].nrg + rob[t].body * 10);
                            selectrobot = t;
                        }
                    }
                }
                KillRobot(selectrobot);
            }
        }
        if (totlen > 3000000)
        {
            for (var t = 1; t < MaxRobs; t++)
            {
                rob[t].LastMutDetail = "";
            }
        }

        //Botsareus 5/6/2013 The safemode system
        if (UseSafeMode)
        { //special modes does not apply, may need to expended to other restart modes
            if (IIf(UseIntRnd, savenow, SimOpts.TotRunCycle % 2000 == 0 & SimOpts.TotRunCycle > 0))
            { //Botsareus 10/19/2015 Safe mode uses different logic under use internet as randomizer
                if (x_restartmode == 0 || x_restartmode == 4 || x_restartmode == 5 || x_restartmode == 7 || x_restartmode == 8)
                { //Botsareus 10/5/2015 restartmodes 1, 2, 3, 6 and 9 are test only and do not need autosaves
                    SaveSimulation("saves\\lastautosave.sim");
                    //Botsareus 5/13/2013 delete local copy
                    if (File.Exists("saves\\localcopy.sim"))
                    {
                        File.Delete("saves\\localcopy.sim");
                    }
                    await AutoSaved.Save(true);

                    savenow = false;
                }
            }
        }

        //R E S T A R T  N E X T
        //Botsareus 1/31/2014 seeding
        if (x_restartmode == 1)
        {
            if (SimOpts.TotRunCycle == 2000)
            {
                File.Copy("league\\Test.txt", NamefileRecursive($"league\\seeded\\{totnvegsDisplayed}.txt"));
                await Iersera.DataModel.RestartMode.Save(x_restartmode, x_filenumber);
                Restarter();
            }
        }

        //Z E R O B O T
        //evo mode
        if (x_restartmode == 7 || x_restartmode == 8)
        {
            if (SimOpts.TotRunCycle % 50 == 0 & SimOpts.TotRunCycle > 0)
            {
                Form1.instance.fittest();
            }
            var Mutate_count = 0;
            //Botsareus 10/192015 count robots to see if time to restart zb evo
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist)
                {
                    if (rob[t].FName == "Mutate.txt")
                    {
                        Mutate_count++;
                    }
                }
            }
            if (Mutate_count == 0)
            {
                //Restart
                await LogEvolution("A restart is needed.");

                DisplayActivations = false;
                Form1.instance.Active = false;
                Form1.instance.SecTimer.Enabled = false;

                await SafeMode.Save(false);
                await AutoSaved.Save(false);

                Restarter();
            }
        }

        double cmptotnrgnvegs = 0;

        //test mode
        if (x_restartmode == 9)
        {
            if (SimOpts.TotRunCycle == 1)
            { //record starting energy
                for (var t = 1; t < MaxRobs; t++)
                {
                    if (rob[t].exist)
                    {
                        if (rob[t].FName == "Test.txt")
                        {
                            totnrgnvegs = totnrgnvegs + rob[t].nrg + rob[t].body * 10;
                        }
                    }
                }
            }
            if (SimOpts.TotRunCycle == 8000)
            { //ending energy must be more
                for (var t = 1; t < MaxRobs; t++)
                {
                    if (rob[t].exist)
                    {
                        if (rob[t].FName == "Test.txt")
                        {
                            cmptotnrgnvegs = cmptotnrgnvegs + rob[t].nrg + rob[t].body * 10;
                        }
                    }
                }
                if (totnvegsDisplayed > 10 & cmptotnrgnvegs > totnrgnvegs * 2)
                { //did population and energy x2?
                    ZBPassedTest();
                }
                else
                {
                    await ZBFailedTest();
                }
            }
        }
    }
}
