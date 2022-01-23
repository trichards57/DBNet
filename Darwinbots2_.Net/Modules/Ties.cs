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

            if (tieK != null)
            {
                var tieJ = tieK.ReverseTie;

                tieK.CleanUp();
                tieJ.CleanUp();

                robA.Ties.Remove(tieK);
                robB.Ties.Remove(tieJ);

                robA.Memory[MemoryAddresses.numties] = robA.Ties.Count;
                robB.Memory[MemoryAddresses.numties] = robB.Ties.Count;

                if (robA.Memory[MemoryAddresses.TIEPRES] == tieK.Port)
                {
                    var lastTie = robA.Ties.LastOrDefault();

                    robA.Memory[MemoryAddresses.TIEPRES] = lastTie?.Port ?? 0;
                }

                if (robB.Memory[MemoryAddresses.TIEPRES] == tieJ.Port)
                {
                    var lastTie = robB.Ties.LastOrDefault();

                    robB.Memory[MemoryAddresses.TIEPRES] = lastTie?.Port ?? 0;
                }
            }
        }

        public static void MakeTie(Robot robA, Robot robB, int length, int last, int port)
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
                        Port = port,
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

                    robA.ReadTRefVars(tieK);
                }
            }

            if (robB.Slime > 0)
                robB.Slime -= Math.Min(20, robB.Slime);

            robA.Energy -= SimOpt.SimOpts.Costs.TieFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (robA.Ties.Count + 1); //Tie cost to form tie
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

        public static void UpdateTies(Robot rob)
        {
            // this routine addresses all ties. not just ones that match .tienum
            rob.vbody = rob.Body;

            var atLeast1Tie = false;
            if (rob.IsMultibot)
            {
                foreach (var tie in rob.Ties.Where(t => !t.BackTie))
                {
                    if (rob.Memory[MemoryAddresses.ShareEnergy] > 0)
                    {
                        rob.ShareEnergy(tie);
                        tie.Sharing = true; //yellow ties
                    }
                    if (rob.Memory[MemoryAddresses.ShareWaste] > 0)
                    {
                        rob.ShareWaste(tie);
                        tie.Sharing = true; //yellow ties
                    }
                    if (rob.Memory[MemoryAddresses.ShareShell] > 0)
                    {
                        rob.ShareShell(tie);
                        tie.Sharing = true; //yellow ties
                    }
                    if (rob.Memory[MemoryAddresses.ShareSlime] > 0)
                    {
                        rob.ShareSlime(tie);
                        tie.Sharing = true; //yellow ties
                    }
                    if (rob.Memory[MemoryAddresses.sharechlr] > 0 && rob.ChloroplastsShareDelay == 0 && !rob.ChloroplastsDisabled)
                    {
                        rob.ShareChloroplasts(tie);
                        tie.Sharing = true; //yellow ties
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
            rob.Memory[MemoryAddresses.ShareEnergy] = 0;
            rob.Memory[MemoryAddresses.ShareWaste] = 0;
            rob.Memory[MemoryAddresses.ShareShell] = 0;
            rob.Memory[MemoryAddresses.ShareSlime] = 0;
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
                        var len = (int)(Math.Abs(rob.Memory[MemoryAddresses.FIXLEN]) + rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) + tie.OtherBot.GetRadius(SimOpt.SimOpts.FixedBotRadii)); // include the radius of the tied bots in the length
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
                        if (rob.TieLengthOverwrite[i])
                        {
                            var length = (int)(rob.Memory[483 + i] + rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) + rob.Ties[i].OtherBot.GetRadius(SimOpt.SimOpts.FixedBotRadii)); // include the radius of the tied bots in the length
                            if (length > 32000)
                            {
                                length = 32000; // Can happen for very big bots with very long ties.
                            }
                            rob.Ties[i].NaturalLength = length; //for first robot
                            rob.Ties[i].ReverseTie.NaturalLength = length; //for second robot. What a messed up formula
                        }
                        if (rob.TieAngleOverwrite[i])
                        {
                            rob.Ties[i].Angle = Physics.NormaliseAngle(Physics.IntToRadians(rob.Memory[479 + i]));
                            rob.Ties[i].FixedAngle = true; //EricL 4/24/2006
                        }
                        //clear input
                        rob.TieAngleOverwrite[i] = false;
                        rob.TieLengthOverwrite[i] = false;
                        //output

                        var tieAngle = Physics.Angle(rob.Position.X, rob.Position.Y, rob.Ties[i].OtherBot.Position.X, rob.Ties[i].OtherBot.Position.Y);
                        var dist = (rob.Position - rob.Ties[i].OtherBot.Position).Magnitude();
                        if (dist > 32000)
                            dist = 32000;

                        rob.Memory[483 + i] = (int)(dist - rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) - rob.Ties[i].OtherBot.GetRadius(SimOpt.SimOpts.FixedBotRadii));
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
    }
}
