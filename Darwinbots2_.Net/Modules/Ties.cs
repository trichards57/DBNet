using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Linq;

namespace DarwinBots.Modules
{
    internal static class Ties
    {
        private const int MaxTies = 10;

        public static void DeleteAllTies(Robot rob)
        {
            foreach (var t in rob.Ties.ToArray())
                DeleteTie(rob, t.OtherBot);
        }

        public static void DeleteTie(Robot robA, Robot robB)
        {
            if (!robA.Exists || !robB.Exists)
                return;

            if (robA.Ties.Count == 0 || robB.Ties.Count == 0)
                return;

            var tieK = robA.Ties.FirstOrDefault(t => t.OtherBot == robB);
            var tieJ = robB.Ties.FirstOrDefault(t => t.OtherBot == robA);

            if (tieK != null)
            {
                robA.Ties.Remove(tieK);
                robA.Memory[MemoryAddresses.numties] = robA.Ties.Count;
                if (robA.Memory[MemoryAddresses.TIEPRES] == tieK.Port)
                {
                    var lastTie = robA.Ties.LastOrDefault();

                    robA.Memory[MemoryAddresses.TIEPRES] = lastTie?.Port ?? 0;
                }
            }

            if (tieJ != null)
            {
                robB.Ties.Remove(tieJ);
                robB.Memory[MemoryAddresses.numties] = robB.Ties.Count;
                if (robB.Memory[MemoryAddresses.TIEPRES] == tieJ.Port)
                {
                    var lastTie = robB.Ties.LastOrDefault();

                    robB.Memory[MemoryAddresses.TIEPRES] = lastTie?.Port ?? 0;
                }
            }
        }

        public static void MakeTie(Robot robA, Robot robB, int length, int last, int mem)
        {
            if (robA.Exists == false)
                return;

            var distance = (robA.Position - robB.Position).Magnitude();

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

                    robA.Memory[466] = robA.Ties.Count;
                    robB.Memory[466] = robB.Ties.Count;

                    robA.Memory[MemoryAddresses.TIEPRES] = tieK.Mem;
                    robB.Memory[MemoryAddresses.TIEPRES] = tieJ.Mem;

                    ReadTRefVars(robA, tieK);
                }
            }

            if (robB.Slime > 0)
                robB.Slime -= Math.Min(20, robB.Slime);

            robA.Energy -= SimOpt.SimOpts.Costs.TieFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (robA.Ties.Count + 1); //Tie cost to form tie
        }

        public static void ReadTie(Robot rob)
        {
            if (rob.NewAge < 2)
                return;

            if (rob.Ties.Any())
            {
                // If there is a value in .readtie then get the trefvars from that tie else read the trefvars from the last tie created
                var tn = rob.Memory[MemoryAddresses.readtiesys] != 0 ? rob.Memory[MemoryAddresses.readtiesys] : rob.Memory[MemoryAddresses.TIEPRES];

                var tie = rob.Ties.LastOrDefault(t => t.Port == tn);

                if (tie != null)
                    ReadTRefVars(rob, tie);
            }
            EraseTRefVars(rob);
        }

        public static void Regang(Robot rob, Tie tie)
        {
            rob.IsMultibot = true;
            rob.Memory[MemoryAddresses.multi] = 1;
            tie.b = 0.1;
            tie.k = 0.05;
            tie.Type = TieType.AntiRope;
            var angl = Physics.Angle(rob.Position.X, rob.Position.Y, tie.OtherBot.Position.X, tie.OtherBot.Position.Y);
            var dist = (rob.Position - tie.OtherBot.Position).Magnitude();
            if (tie.BackTie == false)
            {
                tie.Angle = Physics.NormaliseAngle(angl) - Physics.NormaliseAngle(rob.Aim); // only fix the angle of the bot that created the tie
                tie.FixedAngle = true;
            }
            tie.NaturalLength = dist;
        }

        public static void TiePortCommunication(Robot rob)
        {
            if (!(rob.Memory[455] != 0 & rob.Ties.Count > 0 & rob.Memory[MemoryAddresses.tieloc] > 0))
                return;

            if (rob.Memory[MemoryAddresses.tieloc] > 0 & rob.Memory[MemoryAddresses.tieloc] < 1001)
            {
                //.tieloc value
                var tie = rob.Ties.LastOrDefault(t => t.Port == rob.Memory[MemoryAddresses.TIENUM]);

                if (tie == null)
                    return;

                tie.OtherBot.Memory[rob.Memory[MemoryAddresses.tieloc]] = rob.Memory[MemoryAddresses.tieval]; //stores a value in tied robot memory location (.tieloc) specified in .tieval

                if (!tie.BackTie)
                    tie.InfoUsed = true; //draws tie white
                else
                    tie.ReverseTie.InfoUsed = true;

                rob.Memory[MemoryAddresses.tieval] = 0;
                rob.Memory[MemoryAddresses.tieloc] = 0;
            }
        }

        public static void UpdateTieAngles(Robot rob)
        {
            // Zero these incase no ties or tienum is non-zero, but does not refer to a tieport, etc.
            rob.Memory[MemoryAddresses.TIEANG] = 0;
            rob.Memory[MemoryAddresses.TIELEN] = 0;

            //No point in setting the length and angle if no ties!
            if (rob.Ties.Count == 0)
                return;

            //Figure if .tienum has a value.  If it's zero, use .tiepres
            var whichTie = rob.Memory[MemoryAddresses.TIENUM] != 0 ? rob.Memory[MemoryAddresses.TIENUM] : rob.Memory[MemoryAddresses.TIEPRES];

            if (whichTie == 0)
                return;

            //Now find the tie that corrosponds to either .tienum or .tiepres and set .tieang and .tielen accordingly
            //We count down through the ties to find the most recent tie with the specified tieport since more than one tie
            //can potentially have the same tieport and we want the most recent, which will be the one with the highest k.
            var tie = rob.Ties.LastOrDefault(t => t.Port == whichTie);

            if (tie != null)
            {
                var tieAngle = Physics.Angle(rob.Position.X, rob.Position.Y, tie.OtherBot.Position.X, tie.OtherBot.Position.Y);
                var dist = (rob.Position - tie.OtherBot.Position).Magnitude();
                //Overflow prevention.  Very long ties can happen for one cycle when bots wrap in torridal fields
                if (dist > 32000)
                    dist = 32000;

                rob.Memory[MemoryAddresses.TIEANG] = (int)-(Physics.AngDiff(Physics.NormaliseAngle(tieAngle), Physics.NormaliseAngle(rob.Aim)) * 200);
                rob.Memory[MemoryAddresses.TIELEN] = (int)(dist - rob.Radius - tie.OtherBot.Radius);
            }
        }

        public static void UpdateTies(IRobotManager robotManager, Robot rob)
        {
            // this routine addresses all ties. not just ones that match .tienum
            rob.vbody = rob.Body;

            var atLeast1Tie = false;
            if (rob.IsMultibot)
            {
                foreach (var tie in rob.Ties)
                {
                    // while there is a tie that points to another robot that this bot created.
                    if (!tie.BackTie)
                    {
                        if (rob.Memory[830] > 0)
                        {
                            robotManager.ShareEnergy(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                        if (rob.Memory[831] > 0)
                        {
                            robotManager.ShareWaste(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                        if (rob.Memory[832] > 0)
                        {
                            robotManager.ShareShell(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                        if (rob.Memory[833] > 0)
                        {
                            robotManager.ShareSlime(rob, tie);
                            tie.Sharing = true; //yellow ties
                        }
                        if (rob.Memory[MemoryAddresses.sharechlr] > 0 & rob.ChloroplastsShareDelay == 0 & !rob.ChloroplastsDisabled)
                        { //Panda 8/31/2013 code to share chloroplasts 'Botsareus 8/16/2014 chloroplast sharing disable
                            robotManager.ShareChloroplasts(rob, tie);
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

            if (rob.MultibotTimer > 0)
            {
                if (atLeast1Tie)
                    rob.MultibotTimer++;
                else
                    rob.MultibotTimer--;

                if (rob.MultibotTimer > 210)
                    rob.MultibotTimer = 210;

                if (rob.MultibotTimer < 10)
                    rob.IsDead = true; //safe kill robot
            }

            // Zero the sharing sysvars
            rob.Memory[830] = 0;
            rob.Memory[831] = 0;
            rob.Memory[832] = 0;
            rob.Memory[833] = 0;
            rob.Memory[MemoryAddresses.sharechlr] = 0;
            rob.Memory[MemoryAddresses.numties] = rob.Ties.Count;

            if (rob.Ties.Count == 0)
            {
                rob.IsMultibot = false;
                rob.Memory[MemoryAddresses.multi] = 0;
                return;
            }

            // Handle the deltie sysvar.  Bot is trying to delete one or more ties
            if (rob.Memory[MemoryAddresses.DELTIE] != 0)
            {
                foreach (var tie in rob.Ties.Where(t => t.Port == rob.Memory[MemoryAddresses.DELTIE]).ToArray())
                    DeleteTie(rob, tie.OtherBot);

                rob.Memory[MemoryAddresses.DELTIE] = 0; //resets .deltie command
            }

            var tn = rob.Memory[MemoryAddresses.TIENUM];
            if (tn == 0)
                tn = rob.Memory[MemoryAddresses.TIEPRES];

            if (tn == 0)
                return;

            if (rob.IsMultibot)
            {
                foreach (var tie in rob.Ties.Where(t => t.Type == TieType.AntiRope && t.Port == tn))
                {
                    if (rob.Memory[MemoryAddresses.FIXANG] != 32000)
                    {
                        if (rob.Memory[MemoryAddresses.FIXANG] >= 0)
                        {
                            tie.Angle = Physics.IntToRadians(rob.Memory[MemoryAddresses.FIXANG]);
                            tie.FixedAngle = true;
                        }
                        else
                        {
                            tie.FixedAngle = false;
                        }
                    }

                    //TieLen Section
                    if (rob.Memory[MemoryAddresses.FIXLEN] != 0)
                    {
                        var len = (int)(Math.Abs(rob.Memory[MemoryAddresses.FIXLEN]) + rob.Radius + tie.OtherBot.Radius); // include the radius of the tied bots in the length
                        if (len > 32000)
                            len = 32000; // Can happen for very big bots with very long ties.

                        tie.NaturalLength = len;
                        tie.ReverseTie.NaturalLength = len;
                    }

                    //EricL 5/7/2006 Added Stifftie section.  This never made it into the 2.4 code
                    if (rob.Memory[MemoryAddresses.stifftie] != 0)
                    {
                        rob.Memory[MemoryAddresses.stifftie] = rob.Memory[MemoryAddresses.stifftie] % 100;

                        if (rob.Memory[MemoryAddresses.stifftie] == 0)
                            rob.Memory[MemoryAddresses.stifftie] = 100;

                        if (rob.Memory[MemoryAddresses.stifftie] < 0)
                            rob.Memory[MemoryAddresses.stifftie] = 1;

                        tie.b = 0.005 * rob.Memory[MemoryAddresses.stifftie]; // May need to tweak the multiplier here vares from 0.0025 to .1
                        tie.ReverseTie.b = 0.005 * rob.Memory[MemoryAddresses.stifftie]; // May need to tweak the multiplier here
                        tie.k = 0.0025 * rob.Memory[MemoryAddresses.stifftie]; //May need to tweak the multiplier here:  varies from 0.00125 to 0.05
                        tie.ReverseTie.k = 0.0025 * rob.Memory[MemoryAddresses.stifftie]; // May need to tweak the multiplier here: varies from 0.00125 to 0.05
                    }
                }
            }

            rob.Memory[MemoryAddresses.FIXANG] = 32000;
            rob.Memory[MemoryAddresses.FIXLEN] = 0;
            rob.Memory[MemoryAddresses.stifftie] = 0;

            //Botsareus 3/22/2013 Complete fix for tielen...tieang 1...4
            if (rob.IsMultibot)
            {
                for (var i = 0; i < Math.Min(4, rob.Ties.Count); i++)
                {
                    if (rob.Ties[i].Type == TieType.AntiRope)
                    {
                        //input
                        if (rob.TieLengthOverwrite[i - 1])
                        {
                            var length = (int)(rob.Memory[483 + i] + rob.Radius + rob.Ties[i].OtherBot.Radius); // include the radius of the tied bots in the length
                            if (length > 32000)
                            {
                                length = 32000; // Can happen for very big bots with very long ties.
                            }
                            rob.Ties[i].NaturalLength = length; //for first robot
                            rob.Ties[i].ReverseTie.NaturalLength = length; //for second robot. What a messed up formula
                        }
                        if (rob.TieAngleOverwrite[i - 1])
                        {
                            rob.Ties[i].Angle = Physics.NormaliseAngle(Physics.IntToRadians(rob.Memory[479 + i]));
                            rob.Ties[i].FixedAngle = true; //EricL 4/24/2006
                        }
                        //clear input
                        rob.TieAngleOverwrite[i - 1] = false;
                        rob.TieLengthOverwrite[i - 1] = false;
                        //output

                        var tieAngle = Physics.Angle(rob.Position.X, rob.Position.Y, rob.Ties[i].OtherBot.Position.X, rob.Ties[i].OtherBot.Position.Y);
                        var dist = (rob.Position - rob.Ties[i].OtherBot.Position).Magnitude();
                        if (dist > 32000)
                            dist = 32000;

                        rob.Memory[483 + i] = (int)(dist - rob.Radius - rob.Ties[i].OtherBot.Radius);
                        rob.Memory[479 + i] = (int)Physics.NormaliseAngle(Physics.NormaliseAngle(tieAngle) - Physics.NormaliseAngle(rob.Aim)) * 200;
                    }
                }
            }

            if (rob.Memory[MemoryAddresses.tieport1 + 2] < 0)
            {
                //we are checking values that are negative such as -1 or -6
                if (rob.Memory[MemoryAddresses.tieport1 + 2] == -1 && rob.Memory[MemoryAddresses.tieport1 + 3] != 0)
                {
                    //tieloc = -1 and .tieval not zero
                    var l = rob.Memory[MemoryAddresses.tieport1 + 3]; // l is amount of energy to exchange, positive to give nrg away, negative to take it

                    //Limits on Tie feeding as a function of body attempting to do the feeding (or sharing)
                    if (rob.Body < 0)
                        l = 0; // If your body has gone negative, you can't take or give nrg.

                    if (rob.Energy < 0)
                        l = 0; // If you nrg has gone negative, you can't take or give nrg.

                    if (rob.Age == 0)
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
                            if (l > rob.Energy)
                                l = (int)rob.Energy; // Can't give away more nrg than you have

                            tie.OtherBot.Energy = tie.OtherBot.Energy + l * 0.7; //tied robot receives energy

                            if (tie.OtherBot.Energy > 32000)
                                tie.OtherBot.Energy = 32000;

                            tie.OtherBot.Body = tie.OtherBot.Body + l * 0.029; //tied robot stores some fat
                            if (tie.OtherBot.Body > 32000)
                                tie.OtherBot.Body = 32000;

                            tie.OtherBot.Waste = tie.OtherBot.Waste + l * 0.01; //tied robot receives waste

                            rob.Energy -= l; //tying robot gives up energy
                        }

                        //Taking nrg
                        if (l < 0)
                        {
                            if (l < -tie.OtherBot.Energy)
                                l = (int)-tie.OtherBot.Energy; // Can't give away more nrg than you have

                            //Poison
                            var ptag = Math.Abs(l / 4);
                            if (tie.OtherBot.Poison > ptag)
                            {
                                //target robot has poison
                                if (tie.OtherBot.FName != rob.FName)
                                {
                                    //can't poison your brother
                                    rob.IsPoisoned = true;
                                    rob.PoisonCountdown += ptag;
                                    if (rob.PoisonCountdown > 32000)
                                        rob.PoisonCountdown = 32000;

                                    l = 0;

                                    tie.OtherBot.Poison = tie.OtherBot.Poison - ptag;
                                    tie.OtherBot.Memory[827] = (int)tie.OtherBot.Poison;

                                    if (tie.OtherBot.Memory[834] > 0)
                                    {
                                        rob.PoisonLocation = ((tie.OtherBot.Memory[834] - 1) % 1000) + 1; //sets .Ploc to targets .mem(ploc) EricL - 3/29/2006 Added Mod to fix overflow
                                        if (rob.PoisonLocation == 340)
                                        {
                                            rob.PoisonLocation = 0;
                                        }
                                    }
                                    else
                                    {
                                        do
                                        {
                                            rob.PoisonLocation = ThreadSafeRandom.Local.Next(1, 1000);
                                        } while (rob.PoisonLocation != 340);
                                    }

                                    rob.PoisonValue = tie.OtherBot.Memory[839];
                                }
                            }

                            rob.Energy -= l * 0.7; //tying robot receives energy
                            if (rob.Energy > 32000)
                                rob.Energy = 32000;

                            rob.Body -= l * 0.029; //tying robot stores some fat
                            if (rob.Body > 32000)
                                rob.Body = 32000;

                            rob.Waste -= l * 0.01; //tying robot adds waste

                            tie.OtherBot.Energy = tie.OtherBot.Energy + l; //Take the nrg

                            if (tie.OtherBot.Energy <= 0 & !tie.OtherBot.IsDead && !tie.OtherBot.IsCorpse)
                            {
                                rob.Kills++;
                                if (rob.Kills > 32000)
                                    rob.Kills = 32000;

                                rob.Memory[220] = rob.Kills;
                            }
                        }

                        if (!tie.BackTie)
                            tie.EnergyUsed = true; //red ties
                        else
                            tie.ReverseTie.EnergyUsed = true; //red ties
                    }
                }

                if (rob.Memory[MemoryAddresses.tieport1 + 2] == -3 && rob.Memory[MemoryAddresses.tieport1 + 3] != 0)
                {
                    //inject or steal venom
                    var l = rob.Memory[MemoryAddresses.tieport1 + 3]; //amount of venom to take or inject

                    l = Math.Clamp(l, -100, 100);

                    foreach (var tie in rob.Ties.Where(t => t.Port == tn))
                    {
                        if (l > rob.Venom)
                            l = (int)rob.Venom;

                        if (l > 0)
                        {
                            //works the same as a venom injection
                            tie.OtherBot.ParalyzedCountdown = tie.OtherBot.ParalyzedCountdown + l; //paralysis counter set
                            if (tie.OtherBot.ParalyzedCountdown > 32000)
                                tie.OtherBot.ParalyzedCountdown = 32000;

                            tie.OtherBot.IsParalyzed = true; //robot paralyzed

                            if (rob.Memory[835] > 0)
                            {
                                tie.OtherBot.VirusLocation = ((rob.Memory[835] - 1) % 1000) + 1;
                                if (tie.OtherBot.VirusLocation == 340)
                                    tie.OtherBot.VirusLocation = 0;
                            }
                            else
                            {
                                do
                                {
                                    tie.OtherBot.VirusLocation = ThreadSafeRandom.Local.Next(1, 1000);
                                } while (tie.OtherBot.VirusLocation == 340);
                            }

                            tie.OtherBot.VirusValue = rob.Memory[836];
                            rob.Venom -= l;
                            rob.Memory[825] = (int)rob.Venom;
                        }

                        if (l < 0)
                        {
                            //Taking venom
                            if (l < -tie.OtherBot.Venom)
                                l = (int)-tie.OtherBot.Venom; // Can't give away more venom than you have

                            //robot steals venom from tied target
                            tie.OtherBot.Venom = tie.OtherBot.Venom + l;
                            rob.Venom -= l;
                            if (rob.Venom > 32000)
                                rob.Venom = 32000;

                            rob.Memory[825] = (int)rob.Venom;
                        }

                        if (!tie.BackTie)
                            tie.EnergyUsed = true; //red ties
                        else
                            tie.ReverseTie.EnergyUsed = true; //red ties
                    }
                }

                if (rob.Memory[MemoryAddresses.tieport1 + 2] == -4 && rob.Memory[MemoryAddresses.tieport1 + 3] != 0)
                {
                    //trade waste via ties
                    var l = rob.Memory[MemoryAddresses.tieport1 + 3]; // l is amount of waste to exchange, positive to give waste away, negative to take it

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
                            rob.PermanentWaste += l * 0.01; //some waste is converted into perminent waste rather then given away
                        }

                        //taking waste
                        if (l < 0)
                        {
                            if (l < -tie.OtherBot.Waste)
                                l = (int)-tie.OtherBot.Waste;

                            rob.Waste -= l * 0.99; //robot reseaves waste from opponent
                            tie.OtherBot.Waste = tie.OtherBot.Waste + l; //opponent losing some waste
                            tie.OtherBot.PermanentWaste = tie.OtherBot.PermanentWaste - l * 0.01; //some waste is converted into perminent waste rather then given away
                        }

                        if (!tie.BackTie)
                            tie.EnergyUsed = true; //red ties
                        else
                            tie.ReverseTie.EnergyUsed = true; //red ties
                    }
                }

                if (rob.Memory[MemoryAddresses.tieport1 + 2] == -6 && rob.Memory[MemoryAddresses.tieport1 + 3] != 0)
                {
                    //tieloc = -6 and .tieval not zero
                    var l = rob.Memory[MemoryAddresses.tieport1 + 3]; // l is amount of body to exchange, positive to give body away, negative to take it

                    //Limits on Tie feeding as a function of body attempting to do the feeding (or sharing)
                    if (rob.Body < 0)
                        l = 0; // If your body has gone negative, you can't take or give body.

                    if (rob.Energy < 0)
                        l = 0; // If you nrg has gone negative, you can't take or give body.

                    if (rob.Age == 0)
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

                            tie.OtherBot.Energy = tie.OtherBot.Energy + l * 0.03; //tied robot receives energy
                            if (tie.OtherBot.Energy > 32000)
                                tie.OtherBot.Energy = 32000;

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
                            if (tie.OtherBot.Poison > ptag)
                            {
                                //target robot has poison
                                if (tie.OtherBot.FName != rob.FName)
                                {
                                    //can't poison your brother
                                    rob.IsPoisoned = true;
                                    rob.PoisonCountdown += ptag;
                                    if (rob.PoisonCountdown > 32000)
                                        rob.PoisonCountdown = 32000;

                                    l = 0;

                                    tie.OtherBot.Poison = tie.OtherBot.Poison - ptag;
                                    tie.OtherBot.Memory[827] = (int)tie.OtherBot.Poison;

                                    if (tie.OtherBot.Memory[834] > 0)
                                    {
                                        rob.PoisonLocation = ((tie.OtherBot.Memory[834] - 1) % 1000) + 1; //sets .Ploc to targets .mem(ploc) EricL - 3/29/2006 Added Mod to fix overflow
                                        if (rob.PoisonLocation == 340)
                                            rob.PoisonLocation = 0;
                                    }
                                    else
                                    {
                                        do
                                        {
                                            rob.PoisonLocation = ThreadSafeRandom.Local.Next(1, 1000);
                                        } while (rob.PoisonLocation == 340);
                                    }

                                    rob.PoisonValue = tie.OtherBot.Memory[839];
                                }
                            }

                            rob.Energy -= l * 0.03; //tying robot receives energy
                            if (rob.Energy > 32000)
                                rob.Energy = 32000;

                            rob.Body -= l * 0.987; //tying robot stores some fat 'Botsareus 3/23/2016 Bugfix
                            if (rob.Body > 32000)
                                rob.Body = 32000;

                            rob.Waste -= l * 0.01; //tying robot adds waste

                            tie.OtherBot.Body = tie.OtherBot.Body + l; //Take the body

                            if (tie.OtherBot.Body <= 0 & tie.OtherBot.IsDead == false)
                            {
                                //Botsareus 3/11/2014 Tie feeding kills
                                rob.Kills++;
                                if (rob.Kills > 32000)
                                    rob.Kills = 32000;

                                rob.Memory[220] = rob.Kills;
                            }
                        }

                        if (!tie.BackTie)
                            tie.EnergyUsed = true; //red ties
                        else
                            tie.ReverseTie.EnergyUsed = true; //red ties
                    }
                }

                rob.Memory[MemoryAddresses.tieport1 + 2] = 0;
                rob.Memory[MemoryAddresses.tieport1 + 3] = 0;
            }

            rob.Memory[MemoryAddresses.tieport1 + 5] = 0; // .tienum should be reset every cycle
        }

        private static void EraseTRefVars(Robot rob)
        {
            // Zero the trefvars as all ties have gone.  Perf -> Could set a flag to not do this everytime
            for (var counter = 456; counter < 465; counter++)
                rob.Memory[counter] = 0;

            rob.Memory[MemoryAddresses.trefbody] = 0;
            rob.Memory[475] = 0;
            rob.Memory[478] = 0;
            rob.Memory[479] = 0;
            for (var counter = 0; counter < 10; counter++)
                rob.Memory[MemoryAddresses.trefxpos + counter] = 0;

            //These are .tin trefvars
            for (var counter = 420; counter < 429; counter++)
                rob.Memory[counter] = 0;
        }

        private static void ReadTRefVars(Robot rob, Tie tie)
        {
            if (tie.OtherBot.Energy < 32000 & tie.OtherBot.Energy > -32000)
                rob.Memory[MemoryAddresses.trefnrg] = (int)tie.OtherBot.Energy; //copies tied robot's energy into memory cell *trefnrg

            if (tie.OtherBot.Age < 32000)
                rob.Memory[465] = tie.OtherBot.Age + 1; //copies age of tied robot into *refvar
            else
                rob.Memory[465] = 32000;

            if (tie.OtherBot.Body < 32000 & tie.OtherBot.Body > -32000)
                rob.Memory[MemoryAddresses.trefbody] = (int)tie.OtherBot.Body; //copies tied robot's body value
            else
                rob.Memory[MemoryAddresses.trefbody] = 32000;

            for (var l = 1; l < 8; l++)
                rob.Memory[455 + l] = tie.OtherBot.occurr[l];

            if (rob.Memory[476] > 0 & rob.Memory[476] <= 1000)
            {
                //tmemval and tmemloc couple used to read a specific memory value from tied robot.
                rob.Memory[475] = tie.OtherBot.Memory[rob.Memory[476]];
            }

            rob.Memory[478] = tie.OtherBot.IsFixed ? 1 : 0;
            rob.Memory[479] = tie.OtherBot.Memory[MemoryAddresses.AimSys];
            rob.Memory[MemoryAddresses.trefxpos] = tie.OtherBot.Memory[219];
            rob.Memory[MemoryAddresses.trefypos] = tie.OtherBot.Memory[217];
            rob.Memory[MemoryAddresses.trefvelyourup] = tie.OtherBot.Memory[MemoryAddresses.velup];
            rob.Memory[MemoryAddresses.trefvelyourdn] = tie.OtherBot.Memory[MemoryAddresses.veldn];
            rob.Memory[MemoryAddresses.trefvelyoursx] = tie.OtherBot.Memory[MemoryAddresses.velsx];
            rob.Memory[MemoryAddresses.trefvelyourdx] = tie.OtherBot.Memory[MemoryAddresses.veldx];

            tie.OtherBot.Velocity = DoubleVector.Clamp(tie.OtherBot.Velocity, -16000, 16000);

            rob.Memory[MemoryAddresses.trefvelmyup] = (int)(tie.OtherBot.Velocity.X * Math.Cos(rob.Aim) + Math.Sin(rob.Aim) * tie.OtherBot.Velocity.Y * -1 - rob.Memory[MemoryAddresses.velup]); //gives velocity from mybots frame of reference
            rob.Memory[MemoryAddresses.trefvelmydn] = rob.Memory[MemoryAddresses.trefvelmyup] * -1;
            rob.Memory[MemoryAddresses.trefvelmydx] = (int)(tie.OtherBot.Velocity.Y * Math.Cos(rob.Aim) + Math.Sin(rob.Aim) * tie.OtherBot.Velocity.X - rob.Memory[MemoryAddresses.veldx]);
            rob.Memory[MemoryAddresses.trefvelmysx] = rob.Memory[MemoryAddresses.trefvelmydx] * -1;
            rob.Memory[MemoryAddresses.trefvelscalar] = tie.OtherBot.Memory[MemoryAddresses.velscalar];
            rob.Memory[MemoryAddresses.trefshell] = (int)tie.OtherBot.Shell;

            //These are the tie in/pairs
            for (var l = 410; l < 419; l++)
                rob.Memory[l + 10] = tie.OtherBot.Memory[l];
        }
    }
}
