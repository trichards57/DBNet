using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Linq;

namespace DarwinBots.Modules
{
    internal static class Ties
    {
        private const int MaxTies = 10;

        public static void DeleteAllTies(robot rob)
        {
            foreach (var t in rob.Ties.ToArray())
                DeleteTie(rob, t.OtherBot);
        }

        public static void DeleteTie(robot robA, robot robB)
        {
            if (!robA.exist || !robB.exist)
                return;

            if (robA.Ties.Count == 0 || robB.Ties.Count == 0)
                return;

            var tieK = robA.Ties.FirstOrDefault(t => t.OtherBot == robB);
            var tieJ = robB.Ties.FirstOrDefault(t => t.OtherBot == robA);

            if (tieK != null)
            {
                robA.Ties.Remove(tieK);
                robA.mem[MemoryAddresses.numties] = robA.Ties.Count;
                if (robA.mem[MemoryAddresses.TIEPRES] == tieK.Port)
                {
                    var lastTie = robA.Ties.LastOrDefault();

                    robA.mem[MemoryAddresses.TIEPRES] = lastTie?.Port ?? 0;
                }
            }

            if (tieJ != null)
            {
                robB.Ties.Remove(tieJ);
                robB.mem[MemoryAddresses.numties] = robB.Ties.Count;
                if (robB.mem[MemoryAddresses.TIEPRES] == tieJ.Port)
                {
                    var lastTie = robB.Ties.LastOrDefault();

                    robB.mem[MemoryAddresses.TIEPRES] = lastTie?.Port ?? 0;
                }
            }
        }

        public static void MakeTie(robot robA, robot robB, int length, int last, int mem)
        {
            if (robA.exist == false)
                return;

            var distance = (robA.pos - robB.pos).Magnitude();

            if (distance <= length * 1.5 && ThreadSafeRandom.Local.Next(2, 92) >= robB.Slime)
            {
                DeleteTie(robA, robB);

                if (robA.Ties.Count < MaxTies && robB.Ties.Count < MaxTies)
                {
                    var tieK = new Tie
                    {
                        OtherBot = robB,
                        NaturalLength = distance,
                        Last = last,
                        Port = mem,
                        BackTie = false,
                        b = 0.02,
                        k = 0.01,
                        Type = TieType.DampedSpring
                    };
                    var tieJ = new Tie
                    {
                        OtherBot = robA,
                        NaturalLength = distance,
                        Last = last,
                        Port = robB.Ties.Count,
                        BackTie = true,
                        b = 0.02,
                        k = 0.01,
                        Type = TieType.DampedSpring
                    };
                    tieK.ReverseTie = tieJ;
                    tieJ.ReverseTie = tieK;

                    robA.Ties.Add(tieK);
                    robB.Ties.Add(tieJ);

                    robA.mem[466] = robA.Ties.Count;
                    robB.mem[466] = robB.Ties.Count;

                    robA.mem[MemoryAddresses.TIEPRES] = tieK.Mem;
                    robB.mem[MemoryAddresses.TIEPRES] = tieJ.Mem;

                    ReadTRefVars(robA, tieK);
                }
            }

            if (robB.Slime > 0)
                robB.Slime -= Math.Min(20, robB.Slime);

            robA.nrg -= SimOpt.SimOpts.Costs.TieFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (robA.Ties.Count + 1); //Tie cost to form tie
        }

        public static void ReadTie(robot rob)
        {
            if (rob.newage < 2)
                return;

            if (rob.Ties.Any())
            {
                // If there is a value in .readtie then get the trefvars from that tie else read the trefvars from the last tie created
                var tn = rob.mem[MemoryAddresses.readtiesys] != 0 ? rob.mem[MemoryAddresses.readtiesys] : rob.mem[MemoryAddresses.readtiesys];

                var tie = rob.Ties.LastOrDefault(t => t.Port == tn);

                if (tie != null)
                    ReadTRefVars(rob, tie);
            }
            EraseTRefVars(rob);
        }

        public static void Regang(robot rob, Tie tie)
        {
            rob.Multibot = true;
            rob.mem[MemoryAddresses.multi] = 1;
            tie.b = 0.1;
            tie.k = 0.05;
            tie.Type = TieType.AntiRope;
            var angl = Physics.Angle(rob.pos.X, rob.pos.Y, tie.OtherBot.pos.X, tie.OtherBot.pos.Y);
            var dist = (rob.pos - tie.OtherBot.pos).Magnitude();
            if (tie.BackTie == false)
            {
                tie.Angle = Physics.NormaliseAngle(angl) - Physics.NormaliseAngle(rob.aim); // only fix the angle of the bot that created the tie
                tie.FixedAngle = true;
            }
            tie.NaturalLength = dist;
        }

        public static void TiePortCommunication(robot rob)
        {
            if (!(rob.mem[455] != 0 & rob.Ties.Count > 0 & rob.mem[MemoryAddresses.tieloc] > 0))
                return;

            if (rob.mem[MemoryAddresses.tieloc] > 0 & rob.mem[MemoryAddresses.tieloc] < 1001)
            {
                //.tieloc value
                var tie = rob.Ties.LastOrDefault(t => t.Port == rob.mem[MemoryAddresses.TIENUM]);

                if (tie == null)
                    return;

                tie.OtherBot.mem[rob.mem[MemoryAddresses.tieloc]] = rob.mem[MemoryAddresses.tieval]; //stores a value in tied robot memory location (.tieloc) specified in .tieval

                if (!tie.BackTie)
                    tie.InfoUsed = true; //draws tie white
                else
                    tie.ReverseTie.InfoUsed = true;

                rob.mem[MemoryAddresses.tieval] = 0;
                rob.mem[MemoryAddresses.tieloc] = 0;
            }
        }

        public static void UpdateTieAngles(robot rob)
        {
            // Zero these incase no ties or tienum is non-zero, but does not refer to a tieport, etc.
            rob.mem[MemoryAddresses.TIEANG] = 0;
            rob.mem[MemoryAddresses.TIELEN] = 0;

            //No point in setting the length and angle if no ties!
            if (rob.Ties.Count == 0)
                return;

            //Figure if .tienum has a value.  If it's zero, use .tiepres
            var whichTie = rob.mem[MemoryAddresses.TIENUM] != 0 ? rob.mem[MemoryAddresses.TIENUM] : rob.mem[MemoryAddresses.TIEPRES];

            if (whichTie == 0)
                return;

            //Now find the tie that corrosponds to either .tienum or .tiepres and set .tieang and .tielen accordingly
            //We count down through the ties to find the most recent tie with the specified tieport since more than one tie
            //can potentially have the same tieport and we want the most recent, which will be the one with the highest k.
            var tie = rob.Ties.LastOrDefault(t => t.Port == whichTie);

            if (tie != null)
            {
                var tieAngle = Physics.Angle(rob.pos.X, rob.pos.Y, tie.OtherBot.pos.X, tie.OtherBot.pos.Y);
                var dist = (rob.pos - tie.OtherBot.pos).Magnitude();
                //Overflow prevention.  Very long ties can happen for one cycle when bots wrap in torridal fields
                if (dist > 32000)
                    dist = 32000;

                rob.mem[MemoryAddresses.TIEANG] = (int)-(Physics.AngDiff(Physics.NormaliseAngle(tieAngle), Physics.NormaliseAngle(rob.aim)) * 200);
                rob.mem[MemoryAddresses.TIELEN] = (int)(dist - rob.Radius - tie.OtherBot.Radius);
            }
        }

        public static void UpdateTies(robot rob)
        {
            // this routine addresses all ties. not just ones that match .tienum
            rob.vbody = rob.Body;

            var atLeast1Tie = false;
            if (rob.Multibot)
            {
                foreach (var tie in rob.Ties)
                {
                    // while there is a tie that points to another robot that this bot created.
                    if (!tie.BackTie)
                    {
                        if (rob.mem[830] > 0)
                        {
                            Globals.RobotsManager.ShareEnergy(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                        if (rob.mem[831] > 0)
                        {
                            Globals.RobotsManager.ShareWaste(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                        if (rob.mem[832] > 0)
                        {
                            Globals.RobotsManager.ShareShell(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                        if (rob.mem[833] > 0)
                        {
                            Globals.RobotsManager.ShareSlime(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                        if (rob.mem[MemoryAddresses.sharechlr] > 0 & rob.Chlr_Share_Delay == 0 & !rob.NoChlr)
                        { //Panda 8/31/2013 code to share chloroplasts 'Botsareus 8/16/2014 chloroplast sharing disable
                            Globals.RobotsManager.ShareChloroplasts(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                    }
                    rob.vbody += tie.OtherBot.Body;
                    if (rob.FName == tie.OtherBot.FName)
                    {
                        atLeast1Tie = true;
                    }
                }
            }

            if (rob.multibot_time > 0)
            {
                if (atLeast1Tie)
                    rob.multibot_time++;
                else
                    rob.multibot_time--;

                if (rob.multibot_time > 210)
                    rob.multibot_time = 210;

                if (rob.multibot_time < 10)
                    rob.Dead = true; //safe kill robot
            }

            // Zero the sharing sysvars
            rob.mem[830] = 0;
            rob.mem[831] = 0;
            rob.mem[832] = 0;
            rob.mem[833] = 0;
            rob.mem[MemoryAddresses.sharechlr] = 0;
            rob.mem[MemoryAddresses.numties] = rob.Ties.Count;

            if (rob.Ties.Count == 0)
            {
                rob.Multibot = false;
                rob.mem[MemoryAddresses.multi] = 0;
                return;
            }

            // Handle the deltie sysvar.  Bot is trying to delete one or more ties
            if (rob.mem[MemoryAddresses.DELTIE] != 0)
            {
                foreach (var tie in rob.Ties.Where(t => t.Port == rob.mem[MemoryAddresses.DELTIE]).ToArray())
                    DeleteTie(rob, tie.OtherBot);

                rob.mem[MemoryAddresses.DELTIE] = 0; //resets .deltie command
            }

            var tn = rob.mem[MemoryAddresses.TIENUM];
            if (tn == 0)
                tn = rob.mem[MemoryAddresses.TIEPRES];

            if (tn == 0)
                return;

            if (rob.Multibot)
            {
                foreach (var tie in rob.Ties.Where(t => t.Type == TieType.AntiRope && t.Port == tn))
                {
                    if (rob.mem[MemoryAddresses.FIXANG] != 32000)
                    {
                        if (rob.mem[MemoryAddresses.FIXANG] >= 0)
                        {
                            tie.Angle = Physics.IntToRadians(rob.mem[MemoryAddresses.FIXANG]);
                            tie.FixedAngle = true;
                        }
                        else
                        {
                            tie.FixedAngle = false;
                        }
                    }

                    //TieLen Section
                    if (rob.mem[MemoryAddresses.FIXLEN] != 0)
                    {
                        var len = (int)(Math.Abs(rob.mem[MemoryAddresses.FIXLEN]) + rob.Radius + tie.OtherBot.Radius); // include the radius of the tied bots in the length
                        if (len > 32000)
                            len = 32000; // Can happen for very big bots with very long ties.

                        tie.NaturalLength = len;
                        tie.ReverseTie.NaturalLength = len;
                    }

                    //EricL 5/7/2006 Added Stifftie section.  This never made it into the 2.4 code
                    if (rob.mem[MemoryAddresses.stifftie] != 0)
                    {
                        rob.mem[MemoryAddresses.stifftie] = rob.mem[MemoryAddresses.stifftie] % 100;

                        if (rob.mem[MemoryAddresses.stifftie] == 0)
                            rob.mem[MemoryAddresses.stifftie] = 100;

                        if (rob.mem[MemoryAddresses.stifftie] < 0)
                            rob.mem[MemoryAddresses.stifftie] = 1;

                        tie.b = 0.005 * rob.mem[MemoryAddresses.stifftie]; // May need to tweak the multiplier here vares from 0.0025 to .1
                        tie.ReverseTie.b = 0.005 * rob.mem[MemoryAddresses.stifftie]; // May need to tweak the multiplier here
                        tie.k = 0.0025 * rob.mem[MemoryAddresses.stifftie]; //May need to tweak the multiplier here:  varies from 0.00125 to 0.05
                        tie.ReverseTie.k = 0.0025 * rob.mem[MemoryAddresses.stifftie]; // May need to tweak the multiplier here: varies from 0.00125 to 0.05
                    }
                }
            }

            rob.mem[MemoryAddresses.FIXANG] = 32000;
            rob.mem[MemoryAddresses.FIXLEN] = 0;
            rob.mem[MemoryAddresses.stifftie] = 0;

            //Botsareus 3/22/2013 Complete fix for tielen...tieang 1...4
            if (rob.Multibot)
            {
                for (var i = 0; i < Math.Min(4, rob.Ties.Count); i++)
                {
                    if (rob.Ties[i].Type == TieType.AntiRope)
                    {
                        //input
                        if (rob.TieLenOverwrite[i - 1])
                        {
                            var length = (int)(rob.mem[483 + i] + rob.Radius + rob.Ties[i].OtherBot.Radius); // include the radius of the tied bots in the length
                            if (length > 32000)
                            {
                                length = 32000; // Can happen for very big bots with very long ties.
                            }
                            rob.Ties[i].NaturalLength = length; //for first robot
                            rob.Ties[i].ReverseTie.NaturalLength = length; //for second robot. What a messed up formula
                        }
                        if (rob.TieAngOverwrite[i - 1])
                        {
                            rob.Ties[i].Angle = Physics.NormaliseAngle(Physics.IntToRadians(rob.mem[479 + i]));
                            rob.Ties[i].FixedAngle = true; //EricL 4/24/2006
                        }
                        //clear input
                        rob.TieAngOverwrite[i - 1] = false;
                        rob.TieLenOverwrite[i - 1] = false;
                        //output

                        var tieAngle = Physics.Angle(rob.pos.X, rob.pos.Y, rob.Ties[i].OtherBot.pos.X, rob.Ties[i].OtherBot.pos.Y);
                        var dist = (rob.pos - rob.Ties[i].OtherBot.pos).Magnitude();
                        if (dist > 32000)
                            dist = 32000;

                        rob.mem[483 + i] = (int)(dist - rob.Radius - rob.Ties[i].OtherBot.Radius);
                        rob.mem[479 + i] = (int)Physics.NormaliseAngle(Physics.NormaliseAngle(tieAngle) - Physics.NormaliseAngle(rob.aim)) * 200;
                    }
                }
            }

            if (rob.mem[MemoryAddresses.tieport1 + 2] < 0)
            {
                //we are checking values that are negative such as -1 or -6
                if (rob.mem[MemoryAddresses.tieport1 + 2] == -1 && rob.mem[MemoryAddresses.tieport1 + 3] != 0)
                {
                    //tieloc = -1 and .tieval not zero
                    var l = rob.mem[MemoryAddresses.tieport1 + 3]; // l is amount of energy to exchange, positive to give nrg away, negative to take it

                    //Limits on Tie feeding as a function of body attempting to do the feeding (or sharing)
                    if (rob.Body < 0)
                        l = 0; // If your body has gone negative, you can't take or give nrg.

                    if (rob.nrg < 0)
                        l = 0; // If you nrg has gone negative, you can't take or give nrg.

                    if (rob.age == 0)
                        l = 0; // The just born can't trasnfer nrg

                    if (l > 1000)
                        l = 1000; // Upper limt on sharing

                    if (l < -3000)
                        l = -3000; // Upper limit on tie feeding

                    foreach (var tie in rob.Ties.Where(t => t.Port == tn))
                    {
                        //tie actually points at something
                        //tienum indicates this tie
                        //Giving nrg away
                        if (l > 0)
                        {
                            if (l > rob.nrg)
                                l = (int)rob.nrg; // Can't give away more nrg than you have

                            tie.OtherBot.nrg = tie.OtherBot.nrg + l * 0.7; //tied robot receives energy

                            if (tie.OtherBot.nrg > 32000)
                                tie.OtherBot.nrg = 32000;

                            tie.OtherBot.Body = tie.OtherBot.Body + l * 0.029; //tied robot stores some fat
                            if (tie.OtherBot.Body > 32000)
                                tie.OtherBot.Body = 32000;

                            tie.OtherBot.Waste = tie.OtherBot.Waste + l * 0.01; //tied robot receives waste

                            rob.nrg -= l; //tying robot gives up energy
                        }

                        //Taking nrg
                        if (l < 0)
                        {
                            if (l < -tie.OtherBot.nrg)
                                l = (int)-tie.OtherBot.nrg; // Can't give away more nrg than you have

                            //Poison
                            var ptag = Math.Abs(l / 4);
                            if (tie.OtherBot.poison > ptag)
                            {
                                //target robot has poison
                                if (tie.OtherBot.FName != rob.FName)
                                {
                                    //can't poison your brother
                                    rob.Poisoned = true;
                                    rob.Poisoncount += ptag;
                                    if (rob.Poisoncount > 32000)
                                        rob.Poisoncount = 32000;

                                    l = 0;

                                    tie.OtherBot.poison = tie.OtherBot.poison - ptag;
                                    tie.OtherBot.mem[827] = (int)tie.OtherBot.poison;

                                    if (tie.OtherBot.mem[834] > 0)
                                    {
                                        rob.Ploc = ((tie.OtherBot.mem[834] - 1) % 1000) + 1; //sets .Ploc to targets .mem(ploc) EricL - 3/29/2006 Added Mod to fix overflow
                                        if (rob.Ploc == 340)
                                        {
                                            rob.Ploc = 0;
                                        }
                                    }
                                    else
                                    {
                                        do
                                        {
                                            rob.Ploc = ThreadSafeRandom.Local.Next(1, 1000);
                                        } while (rob.Ploc != 340);
                                    }

                                    rob.Pval = tie.OtherBot.mem[839];
                                }
                            }

                            rob.nrg -= l * 0.7; //tying robot receives energy
                            if (rob.nrg > 32000)
                                rob.nrg = 32000;

                            rob.Body -= l * 0.029; //tying robot stores some fat
                            if (rob.Body > 32000)
                                rob.Body = 32000;

                            rob.Waste -= l * 0.01; //tying robot adds waste

                            tie.OtherBot.nrg = tie.OtherBot.nrg + l; //Take the nrg

                            if (tie.OtherBot.nrg <= 0 & !tie.OtherBot.Dead && !tie.OtherBot.Corpse)
                            {
                                rob.Kills++;
                                if (rob.Kills > 32000)
                                    rob.Kills = 32000;

                                rob.mem[220] = rob.Kills;
                            }
                        }

                        if (!tie.BackTie)
                            tie.EnergyUsed = true; //red ties
                        else
                            tie.ReverseTie.EnergyUsed = true; //red ties
                    }
                }

                if (rob.mem[MemoryAddresses.tieport1 + 2] == -3 && rob.mem[MemoryAddresses.tieport1 + 3] != 0)
                {
                    //inject or steal venom
                    var l = rob.mem[MemoryAddresses.tieport1 + 3]; //amount of venom to take or inject

                    l = Math.Clamp(l, -100, 100);

                    foreach (var tie in rob.Ties.Where(t => t.Port == tn))
                    {
                        if (l > rob.venom)
                            l = (int)rob.venom;

                        if (l > 0)
                        {
                            //works the same as a venom injection
                            tie.OtherBot.Paracount = tie.OtherBot.Paracount + l; //paralysis counter set
                            if (tie.OtherBot.Paracount > 32000)
                                tie.OtherBot.Paracount = 32000;

                            tie.OtherBot.Paralyzed = true; //robot paralyzed

                            if (rob.mem[835] > 0)
                            {
                                tie.OtherBot.Vloc = ((rob.mem[835] - 1) % 1000) + 1;
                                if (tie.OtherBot.Vloc == 340)
                                    tie.OtherBot.Vloc = 0;
                            }
                            else
                            {
                                do
                                {
                                    tie.OtherBot.Vloc = ThreadSafeRandom.Local.Next(1, 1000);
                                } while (tie.OtherBot.Vloc == 340);
                            }

                            tie.OtherBot.Vval = rob.mem[836];
                            rob.venom -= l;
                            rob.mem[825] = (int)rob.venom;
                        }

                        if (l < 0)
                        {
                            //Taking venom
                            if (l < -tie.OtherBot.venom)
                                l = (int)-tie.OtherBot.venom; // Can't give away more venom than you have

                            //robot steals venom from tied target
                            tie.OtherBot.venom = tie.OtherBot.venom + l;
                            rob.venom -= l;
                            if (rob.venom > 32000)
                                rob.venom = 32000;

                            rob.mem[825] = (int)rob.venom;
                        }

                        if (!tie.BackTie)
                            tie.EnergyUsed = true; //red ties
                        else
                            tie.ReverseTie.EnergyUsed = true; //red ties
                    }
                }

                if (rob.mem[MemoryAddresses.tieport1 + 2] == -4 && rob.mem[MemoryAddresses.tieport1 + 3] != 0)
                {
                    //trade waste via ties
                    var l = rob.mem[MemoryAddresses.tieport1 + 3]; // l is amount of waste to exchange, positive to give waste away, negative to take it

                    //limits on giving or taking waste
                    if (l > 1000)
                        l = 1000;

                    if (l < -1000)
                        l = -1000;

                    foreach (var tie in rob.Ties.Where(t => t.Port == tn))
                    {
                        //giving waste away
                        if (l > 0)
                        {
                            if (l > rob.Waste)
                                l = (int)rob.Waste;

                            tie.OtherBot.Waste = tie.OtherBot.Waste + l * 0.99;
                            rob.Waste -= l;
                            rob.Pwaste += l * 0.01; //some waste is converted into perminent waste rather then given away
                        }

                        //taking waste
                        if (l < 0)
                        {
                            if (l < -tie.OtherBot.Waste)
                                l = (int)-tie.OtherBot.Waste;

                            rob.Waste -= l * 0.99; //robot reseaves waste from opponent
                            tie.OtherBot.Waste = tie.OtherBot.Waste + l; //opponent losing some waste
                            tie.OtherBot.Pwaste = tie.OtherBot.Pwaste - l * 0.01; //some waste is converted into perminent waste rather then given away
                        }

                        if (!tie.BackTie)
                            tie.EnergyUsed = true; //red ties
                        else
                            tie.ReverseTie.EnergyUsed = true; //red ties
                    }
                }

                if (rob.mem[MemoryAddresses.tieport1 + 2] == -6 && rob.mem[MemoryAddresses.tieport1 + 3] != 0)
                {
                    //tieloc = -6 and .tieval not zero
                    var l = rob.mem[MemoryAddresses.tieport1 + 3]; // l is amount of body to exchange, positive to give body away, negative to take it

                    //Limits on Tie feeding as a function of body attempting to do the feeding (or sharing)
                    if (rob.Body < 0)
                        l = 0; // If your body has gone negative, you can't take or give body.

                    if (rob.nrg < 0)
                        l = 0; // If you nrg has gone negative, you can't take or give body.

                    if (rob.age == 0)
                        l = 0; // The just born can't trasnfer body

                    if (l > 100)
                        l = 100; // Upper limt on sharing

                    if (l < -300)
                        l = -300; // Upper limit on tie feeding

                    foreach (var tie in rob.Ties.Where(t => t.Port == tn))
                    {
                        if (l > 0)
                        {
                            if (l > rob.Body)
                                l = (int)rob.Body; // Can't give away more body than you have

                            tie.OtherBot.nrg = tie.OtherBot.nrg + l * 0.03; //tied robot receives energy
                            if (tie.OtherBot.nrg > 32000)
                                tie.OtherBot.nrg = 32000;

                            tie.OtherBot.Body = tie.OtherBot.Body + l * 0.987; //tied robot stores some fat 'Botsareus 3/23/2016 Bugfix
                            if (tie.OtherBot.Body > 32000)
                                tie.OtherBot.Body = 32000;

                            tie.OtherBot.Waste = tie.OtherBot.Waste + l * 0.01; //tied robot receives waste

                            rob.Body -= l; //tying robot gives up energy
                        }

                        //Taking body
                        if (l < 0)
                        {
                            if (l < -tie.OtherBot.Body)
                            {
                                l = (int)-tie.OtherBot.Body; // Can't give away more body than you have
                            }

                            //Poison (Yes tiefeeding body is a reason enough to get poisoned)
                            var ptag = Math.Abs(l / 4);
                            if (tie.OtherBot.poison > ptag)
                            {
                                //target robot has poison
                                if (tie.OtherBot.FName != rob.FName)
                                {
                                    //can't poison your brother
                                    rob.Poisoned = true;
                                    rob.Poisoncount += ptag;
                                    if (rob.Poisoncount > 32000)
                                        rob.Poisoncount = 32000;

                                    l = 0;

                                    tie.OtherBot.poison = tie.OtherBot.poison - ptag;
                                    tie.OtherBot.mem[827] = (int)tie.OtherBot.poison;

                                    if (tie.OtherBot.mem[834] > 0)
                                    {
                                        rob.Ploc = ((tie.OtherBot.mem[834] - 1) % 1000) + 1; //sets .Ploc to targets .mem(ploc) EricL - 3/29/2006 Added Mod to fix overflow
                                        if (rob.Ploc == 340)
                                            rob.Ploc = 0;
                                    }
                                    else
                                    {
                                        do
                                        {
                                            rob.Ploc = ThreadSafeRandom.Local.Next(1, 1000);
                                        } while (rob.Ploc == 340);
                                    }

                                    rob.Pval = tie.OtherBot.mem[839];
                                }
                            }

                            rob.nrg -= l * 0.03; //tying robot receives energy
                            if (rob.nrg > 32000)
                                rob.nrg = 32000;

                            rob.Body -= l * 0.987; //tying robot stores some fat 'Botsareus 3/23/2016 Bugfix
                            if (rob.Body > 32000)
                                rob.Body = 32000;

                            rob.Waste -= l * 0.01; //tying robot adds waste

                            tie.OtherBot.Body = tie.OtherBot.Body + l; //Take the body

                            if (tie.OtherBot.Body <= 0 & tie.OtherBot.Dead == false)
                            {
                                //Botsareus 3/11/2014 Tie feeding kills
                                rob.Kills++;
                                if (rob.Kills > 32000)
                                    rob.Kills = 32000;

                                rob.mem[220] = rob.Kills;
                            }
                        }

                        if (!tie.BackTie)
                            tie.EnergyUsed = true; //red ties
                        else
                            tie.ReverseTie.EnergyUsed = true; //red ties
                    }
                }

                rob.mem[MemoryAddresses.tieport1 + 2] = 0;
                rob.mem[MemoryAddresses.tieport1 + 3] = 0;
            }

            rob.mem[MemoryAddresses.tieport1 + 5] = 0; // .tienum should be reset every cycle
        }

        private static void EraseTRefVars(robot rob)
        {
            // Zero the trefvars as all ties have gone.  Perf -> Could set a flag to not do this everytime
            for (var counter = 456; counter < 465; counter++)
                rob.mem[counter] = 0;

            rob.mem[MemoryAddresses.trefbody] = 0;
            rob.mem[475] = 0;
            rob.mem[478] = 0;
            rob.mem[479] = 0;
            for (var counter = 0; counter < 10; counter++)
                rob.mem[MemoryAddresses.trefxpos + counter] = 0;

            //These are .tin trefvars
            for (var counter = 420; counter < 429; counter++)
                rob.mem[counter] = 0;
        }

        private static void ReadTRefVars(robot rob, Tie tie)
        {
            if (tie.OtherBot.nrg < 32000 & tie.OtherBot.nrg > -32000)
                rob.mem[MemoryAddresses.trefnrg] = (int)tie.OtherBot.nrg; //copies tied robot's energy into memory cell *trefnrg

            if (tie.OtherBot.age < 32000)
                rob.mem[465] = tie.OtherBot.age + 1; //copies age of tied robot into *refvar
            else
                rob.mem[465] = 32000;

            if (tie.OtherBot.Body < 32000 & tie.OtherBot.Body > -32000)
                rob.mem[MemoryAddresses.trefbody] = (int)tie.OtherBot.Body; //copies tied robot's body value
            else
                rob.mem[MemoryAddresses.trefbody] = 32000;

            for (var l = 1; l < 8; l++)
                rob.mem[455 + l] = tie.OtherBot.occurr[l];

            if (rob.mem[476] > 0 & rob.mem[476] <= 1000)
            {
                //tmemval and tmemloc couple used to read a specific memory value from tied robot.
                rob.mem[475] = tie.OtherBot.mem[rob.mem[476]];
                if (rob.mem[479] > MemoryAddresses.EyeStart && rob.mem[479] < MemoryAddresses.EyeEnd)
                    tie.OtherBot.View = true;
            }

            rob.mem[478] = tie.OtherBot.Fixed ? 1 : 0;
            rob.mem[479] = tie.OtherBot.mem[MemoryAddresses.AimSys];
            rob.mem[MemoryAddresses.trefxpos] = tie.OtherBot.mem[219];
            rob.mem[MemoryAddresses.trefypos] = tie.OtherBot.mem[217];
            rob.mem[MemoryAddresses.trefvelyourup] = tie.OtherBot.mem[MemoryAddresses.velup];
            rob.mem[MemoryAddresses.trefvelyourdn] = tie.OtherBot.mem[MemoryAddresses.veldn];
            rob.mem[MemoryAddresses.trefvelyoursx] = tie.OtherBot.mem[MemoryAddresses.velsx];
            rob.mem[MemoryAddresses.trefvelyourdx] = tie.OtherBot.mem[MemoryAddresses.veldx];

            tie.OtherBot.vel = DoubleVector.Clamp(tie.OtherBot.vel, -16000, 16000);

            rob.mem[MemoryAddresses.trefvelmyup] = (int)(tie.OtherBot.vel.X * Math.Cos(rob.aim) + Math.Sin(rob.aim) * tie.OtherBot.vel.Y * -1 - rob.mem[MemoryAddresses.velup]); //gives velocity from mybots frame of reference
            rob.mem[MemoryAddresses.trefvelmydn] = rob.mem[MemoryAddresses.trefvelmyup] * -1;
            rob.mem[MemoryAddresses.trefvelmydx] = (int)(tie.OtherBot.vel.Y * Math.Cos(rob.aim) + Math.Sin(rob.aim) * tie.OtherBot.vel.X - rob.mem[MemoryAddresses.veldx]);
            rob.mem[MemoryAddresses.trefvelmysx] = rob.mem[MemoryAddresses.trefvelmydx] * -1;
            rob.mem[MemoryAddresses.trefvelscalar] = tie.OtherBot.mem[MemoryAddresses.velscalar];
            rob.mem[MemoryAddresses.trefshell] = (int)tie.OtherBot.shell;

            //These are the tie in/pairs
            for (var l = 410; l < 419; l++)
                rob.mem[l + 10] = tie.OtherBot.mem[l];
        }
    }
}
