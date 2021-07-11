using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static DarwinBots.Modules.Common;
using static DarwinBots.Modules.DnaManipulations;
using static DarwinBots.Modules.DnaTokenizing;
using static DarwinBots.Modules.Globals;
using static DarwinBots.Modules.HDRoutines;
using static DarwinBots.Modules.Robots;
using static DarwinBots.Modules.Senses;
using static DarwinBots.Modules.SimOpt;

namespace DarwinBots.Modules
{
    internal static class NeoMutations
    {
        public const int AmplificationUP = 4;
        public const int CE2UP = 10;
        public const int CopyErrorUP = 6;
        public const int DeltaUP = 7;
        public const int InsertionUP = 3;
        public const int MajorDeletionUP = 5;
        public const int MinorDeletionUP = 1;
        public const int P2UP = 9;
        public const int PointUP = 0; //expressed as 1 chance in X per kilocycle per bp
        public const int ReversalUP = 2;
        public const int TranslocationUP = 8;
        private const double overtime = 30; //Time correction across all mutations

        public static bool DeleteGene(robot rob, int g)
        {
            if (g <= 0 || g > rob.genenum)
                return false;

            DeleteSpecificGene(rob.dna, g);
            rob.genenum = CountGenes(rob.dna);
            rob.mem[DnaLenSys] = rob.dna.Count;
            rob.mem[GenesSys] = rob.genenum;
            MakeOccurrList(rob);

            return true;
        }

        public static void LogMutation(robot rob, string strmut)
        {
            if (SimOpts.TotRunCycle == 0)
                return;

            if (rob.LastMutDetail.Length > 100000000 / TotalRobotsDisplayed)
                rob.LastMutDetail = "";

            rob.LastMutDetail = $"{strmut}\n{rob.LastMutDetail}";
        }

        public static void Mutate(robot rob, bool reproducing = false)
        {
            if (!rob.Mutables.Mutations || SimOpts.DisableMutations)
                return;

            var Delta = rob.LastMut;

            if (!reproducing)
            {
                if (rob.Mutables.mutarray[PointUP] > 0)
                    PointMutation(rob);

                if (rob.Mutables.mutarray[DeltaUP] > 0 & !Delta2)
                    DeltaMut(rob);

                if (rob.Mutables.mutarray[P2UP] > 0 & sunbelt)
                    PointMutation2(rob);

                //special case update epigenetic reset
                if (rob.LastMut - Delta > 0 & epireset)
                    rob.MutEpiReset += Math.Pow(rob.LastMut - Delta, epiresetemp);

                //Delta2 point mutation change
                if (Delta2 && DeltaPM > 0 && rob.age % DeltaPM == 0 & rob.age > 0)
                {
                    var MratesMax = NormMut ? rob.dna.Count * valMaxNormMut : 2000000000;

                    foreach (var t in new[] { PointUP, P2UP })
                    {
                        //Point and Point2
                        if (rob.Mutables.mutarray[t] < 1)
                            continue;

                        if (ThreadSafeRandom.Local.NextDouble() < DeltaMainChance / 100)
                        {
                            if (DeltaMainExp != 0)
                                rob.Mutables.mutarray[t] = Math.Pow(rob.Mutables.mutarray[t] * 10, ((ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaMainExp));

                            rob.Mutables.mutarray[t] = rob.Mutables.mutarray[t] + (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaMainLn;
                            if (rob.Mutables.mutarray[t] < 1)
                                rob.Mutables.mutarray[t] = 1;

                            if (rob.Mutables.mutarray[t] > MratesMax)
                                rob.Mutables.mutarray[t] = MratesMax;
                        }
                        if (ThreadSafeRandom.Local.NextDouble() < DeltaDevChance / 100)
                        {
                            if (DeltaDevExp != 0)
                                rob.Mutables.StdDev[t] = rob.Mutables.StdDev[t] * Math.Pow(10, ((ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaDevExp));

                            rob.Mutables.StdDev[t] = rob.Mutables.StdDev[t] + (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaDevLn;
                            if (DeltaDevExp != 0)
                                rob.Mutables.Mean[t] = rob.Mutables.Mean[t] * Math.Pow(10, ((ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaDevExp));

                            rob.Mutables.Mean[t] = rob.Mutables.Mean[t] + (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaDevLn;
                            //Max range is always 0 to 800
                            if (rob.Mutables.StdDev[t] < 0)
                                rob.Mutables.StdDev[t] = 0;

                            if (rob.Mutables.StdDev[t] > 200)
                                rob.Mutables.StdDev[t] = 200;

                            if (rob.Mutables.Mean[t] < 1)
                                rob.Mutables.Mean[t] = 1;

                            if (rob.Mutables.Mean[t] > 400)
                                rob.Mutables.Mean[t] = 400;
                        }
                    }

                    rob.Mutables.PointWhatToChange += (int)(ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaWTC;

                    if (rob.Mutables.PointWhatToChange < 0)
                        rob.Mutables.PointWhatToChange = 0;

                    if (rob.Mutables.PointWhatToChange > 100)
                        rob.Mutables.PointWhatToChange = 100;

                    rob.Point2MutCycle = 0;
                    rob.PointMutCycle = 0;
                }
            }
            else
            {
                if (rob.Mutables.mutarray[CopyErrorUP] > 0)
                    CopyError(rob);

                if (rob.Mutables.mutarray[CE2UP] > 0 & sunbelt)
                    CopyError2(rob);

                if (rob.Mutables.mutarray[InsertionUP] > 0)
                    Insertion(rob);

                if (rob.Mutables.mutarray[ReversalUP] > 0)
                    Reversal(rob);

                if (rob.Mutables.mutarray[TranslocationUP] > 0 & sunbelt)
                    Translocation(rob);

                if (rob.Mutables.mutarray[AmplificationUP] > 0 & sunbelt)
                    Amplification(rob);

                if (rob.Mutables.mutarray[MajorDeletionUP] > 0)
                    MajorDeletion(rob);

                if (rob.Mutables.mutarray[MinorDeletionUP] > 0)
                    MinorDeletion(rob);
            }

            Delta = rob.LastMut - Delta; //Botsareus 9/4/2012 Moved delta check before overflow reset to fix an error where robot info is not being updated

            //auto forking
            if (SimOpts.EnableAutoSpeciation)
            {
                if (rob.Mutations > rob.dna.Count * (double)SimOpts.SpeciationGeneticDistance / 100)
                {
                    SimOpts.SpeciationForkInterval++;
                    var splitname = rob.FName.Split(")");
                    var robname = splitname[0].StartsWith("(") && int.TryParse(splitname[0][1..], out var _) ? splitname[1] : rob.FName;

                    robname = "(" + SimOpts.SpeciationForkInterval + ")" + robname;

                    if (SimOpts.Specie.Count < 49)
                    {
                        rob.FName = robname;
                        rob.Mutations = 0;
                        AddSpecie(rob, false);
                    }
                    else
                        SimOpts.SpeciationForkInterval--;
                }
            }

            if (rob.Mutations > 32000)
                rob.Mutations = 32000;

            if (rob.LastMut > 32000)
                rob.LastMut = 32000;

            if (Delta > 0)
            { //The bot has mutated.
                rob.GenMut -= rob.LastMut;
                if (rob.GenMut < 0)
                    rob.GenMut = 0;

                MutateColours(rob, Delta);
                rob.SubSpecies = NewSubSpecies(rob);
                rob.genenum = CountGenes(rob.dna);
                rob.mem[DnaLenSys] = rob.dna.Count;
                rob.mem[GenesSys] = rob.genenum;
            }
        }

        public static int NewSubSpecies(robot rob)
        {
            var i = SpeciesFromBot(rob); // Get the index into the species array for this bot
            i.SubSpeciesCounter++; // increment the counter

            if (i.SubSpeciesCounter > 32000)
                i.SubSpeciesCounter = -32000; //wrap the counter if necessary

            return i.SubSpeciesCounter;
        }

        public static void SetDefaultLengths(MutationProbabilities changeme)
        {
            changeme.Mean[PointUP] = 3;
            changeme.StdDev[PointUP] = 1;

            changeme.Mean[DeltaUP] = 500;
            changeme.StdDev[DeltaUP] = 150;

            changeme.Mean[MinorDeletionUP] = 1;
            changeme.StdDev[MinorDeletionUP] = 0;

            changeme.Mean[InsertionUP] = 1;
            changeme.StdDev[InsertionUP] = 0;

            changeme.Mean[CopyErrorUP] = 1;
            changeme.StdDev[CopyErrorUP] = 0;

            changeme.Mean[MajorDeletionUP] = 3;
            changeme.StdDev[MajorDeletionUP] = 1;

            changeme.Mean[ReversalUP] = 3;
            changeme.StdDev[ReversalUP] = 1;

            changeme.CopyErrorWhatToChange = 80;
            changeme.PointWhatToChange = 80;

            changeme.Mean[AmplificationUP] = 250;
            changeme.StdDev[AmplificationUP] = 75;

            changeme.Mean[TranslocationUP] = 250;
            changeme.StdDev[TranslocationUP] = 75;
        }

        private static void Amplification(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[AmplificationUP] + rob.Mutables.StdDev[AmplificationUP]) / (1200 * overtime);
            floor *= SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[AmplificationUP] < floor)
                rob.Mutables.mutarray[AmplificationUP] = floor; //Botsareus 10/5/2015 Prevent freezing

            var t = 1;
            do
            {
                t++;
                if (!(ThreadSafeRandom.Local.NextDouble() <
                      1 / (rob.Mutables.mutarray[AmplificationUP] / SimOpts.MutCurrMult))) continue;
                var Length = (int)Gauss(rob.Mutables.StdDev[AmplificationUP], rob.Mutables.Mean[AmplificationUP]);
                Length %= rob.dna.Count;
                if (Length < 1)
                    Length = 1;

                Length--;
                Length /= 2;

                if (t - Length < 1 || t + Length > rob.dna.Count - 1 || rob.dna.Count + Length * 2 > 32000)
                    continue;

                if (Length <= 0) continue;
                var tempDNA = rob.dna.Skip(t).Take(Length);

                //we now have the appropriate length of DNA in the temporary array.

                var start = ThreadSafeRandom.Local.Next(1, rob.dna.Count - 1);
                rob.dna.InsertRange(start, tempDNA);

                rob.Mutations++;
                rob.LastMut++;
                LogMutation(rob, $"Amplification copied a series at {t}, {Length * 2 + 1}bps long to {start} during cycle {SimOpts.TotRunCycle}");
            } while (t < rob.dna.Count);

            if (!(rob.dna.Last().Type == 10 && rob.dna.Last().Value == 1))
                rob.dna.Add(new DNABlock { Type = 10, Value = 1 });
        }

        private static void ChangeDNA(robot rob, int nth, int Mtype, int Length = 1, int PointWhatToChange = 50)
        {
            for (var t = nth; t < (nth + Length); t++)
            {
                //if length is 1, it's only one bp we're mutating, remember?
                if (t >= rob.dna.Count || rob.dna[t].Type == 10)
                    return; //don't mutate end either

                if (ThreadSafeRandom.Local.Next(0, 99) < PointWhatToChange)
                {
                    if (rob.dna[t].Value != 0 && Mtype == InsertionUP)
                        rob.dna[t] = rob.dna[t] with { Value = (int)Gauss(500, 0) };

                    var old = rob.dna[t].Value;

                    if (rob.dna[t].Type == 0 || rob.dna[t].Type == 1)
                    {
                        do
                        {
                            if (Math.Abs(old) <= 1000)
                                rob.dna[t] = rob.dna[t] with { Value = ThreadSafeRandom.Local.Next(0, 2) == 0 ? (int)Gauss(94, rob.dna[t].Value) : (int)Gauss(7, rob.dna[t].Value) };
                            else
                                rob.dna[t] = rob.dna[t] with { Value = (int)Gauss(old / 10, rob.dna[t].Value) }; //for very large numbers scale gauss
                        } while (rob.dna[t].Value == old);

                        rob.Mutations++;
                        rob.LastMut++;

                        LogMutation(rob, $"{MutationType(Mtype)} changed {TipoDetok(rob.dna[t].Type)} from {old} to {rob.dna[t].Value} at position {t} during cycle {SimOpts.TotRunCycle}");
                    }
                    else
                    {
                        var bp = new DNABlock
                        {
                            Type = rob.dna[t].Type
                        };

                        var Max = 0;

                        do
                        {
                            Max++;
                            bp = bp with { Value = Max };
                        } while (Parse(bp) != "");

                        Max--;

                        if (Max <= 1)
                            return;

                        do
                        {
                            rob.dna[t] = rob.dna[t] with { Value = ThreadSafeRandom.Local.Next(1, Max) };
                        } while (rob.dna[t].Value == old);

                        bp = bp with { Type = rob.dna[t].Type };
                        bp = bp with { Value = old };

                        var Name = Parse(rob.dna[t]);
                        var oldname = Parse(bp);

                        rob.Mutations++;
                        rob.LastMut++;

                        LogMutation(rob, $"{MutationType(Mtype)} changed value of {TipoDetok(rob.dna[t].Type)} from {oldname} to {Name} at position {t} during cycle {SimOpts.TotRunCycle}");
                    }
                }
                else
                {
                    var bp = new DNABlock
                    {
                        Type = rob.dna[t].Type,
                        Value = rob.dna[t].Value
                    };
                    do
                    {
                        rob.dna[t] = rob.dna[t] with { Type = ThreadSafeRandom.Local.Next(0, 20) };
                    } while (rob.dna[t].Type == bp.Type || TipoDetok(rob.dna[t].Type) == "");

                    var Max = 0;
                    if (rob.dna[t].Type >= 2)
                    {
                        do
                        {
                            Max++;
                            rob.dna[t] = rob.dna[t] with { Value = Max };
                        } while (Parse(rob.dna[t]) != "");

                        Max--;

                        if (Max <= 1)
                            return;

                        rob.dna[t] = rob.dna[t] with { Value = ((Math.Abs(bp.Value) - 1) % Max) + 1 }; //put values in range 'Botsareus 4/10/2016 Bug fix
                        if (rob.dna[t].Value == 0)
                            rob.dna[t] = rob.dna[t] with { Value = 1 };
                    }

                    var Name = Parse(rob.dna[t]);
                    var oldname = Parse(bp);
                    rob.Mutations++;
                    rob.LastMut++;

                    LogMutation(rob, $"{MutationType(Mtype)} changed the {TipoDetok(bp.Type)}: {oldname} to the {TipoDetok(rob.dna[t].Type)} : {Name} at position {t} during cycle {SimOpts.TotRunCycle}");
                }
            }
        }

        private static void ChangeDNA2(robot rob, int nth, int DNAsize, bool IsPoint = false)
        {
            int randomsysvar;

            var holddetail = "";

            //for .tieloc, .shoot, and functional
            do
            {
                randomsysvar = (int)(ThreadSafeRandom.Local.NextDouble() * 256);
            } while (DnaEngine.SystemVariables[randomsysvar].Name != "");

            var special = false;
            //special cases
            if (nth < DNAsize - 2)
            {
                //for .shoot store
                if (rob.dna[nth + 1].Type == 0 & rob.dna[nth + 1].Value == shoot && rob.dna[nth + 2].Type == 7 && rob.dna[nth + 2].Value == 1)
                {
                    rob.dna[nth] = new DNABlock
                    {
                        Value = ThreadSafeRandom.Local.Next(1, 8) switch
                        {
                            1 => -1,
                            2 => -2,
                            3 => -3,
                            4 => -4,
                            5 => -6,
                            6 => -8,
                            _ => DnaEngine.SystemVariables[randomsysvar].Value
                        },
                        Type = 0
                    };
                    holddetail = $"changed dna location {nth} to {rob.dna[nth].Value}";
                    special = true;
                }
                //for .focuseye store
                if (rob.dna[nth + 1].Type == 0 & rob.dna[nth + 1].Value == FOCUSEYE && rob.dna[nth + 2].Type == 7 && rob.dna[nth + 2].Value == 1)
                {
                    rob.dna[nth] = new DNABlock { Value = (int)(ThreadSafeRandom.Local.NextDouble() * 9) - 4, Type = 0 };
                    holddetail = $"changed dna location {nth} to {rob.dna[nth].Value}";
                    special = true;
                }
                //for .tieloc store
                if (rob.dna[nth + 1].Type == 0 & rob.dna[nth + 1].Value == tieloc && rob.dna[nth + 2].Type == 7 && rob.dna[nth + 2].Value == 1)
                {
                    rob.dna[nth] = new DNABlock
                    {
                        Value = ThreadSafeRandom.Local.Next(1, 6) switch
                        {
                            1 => -1,
                            2 => -3,
                            3 => -4,
                            4 => -6,
                            _ => DnaEngine.SystemVariables[randomsysvar].Value
                        },
                        Type = 0
                    };
                    holddetail = $"changed dna location {nth} to {rob.dna[nth].Value}";
                    special = true;
                }
            }

            if (special)
            {
                LogMutation(rob, $"{(IsPoint ? "Point Mutation 2" : "Copy Error 2")} {holddetail} during cycle {SimOpts.TotRunCycle}");
                rob.Mutations++;
                rob.LastMut++;
            }
            else
            { //other cases
                if (nth < DNAsize - 1 && (int)(ThreadSafeRandom.Local.NextDouble() * 3) == 0)
                { //1/3 chance functional
                    rob.dna[nth] = new DNABlock { Type = 0, Value = DnaEngine.SystemVariables[randomsysvar].Value };
                    LogMutation(rob, $"{(IsPoint ? "Point Mutation 2" : "Copy Error 2")} changed dna location {nth} to number .{DnaEngine.SystemVariables[randomsysvar].Name} during cycle {SimOpts.TotRunCycle}");
                    rob.Mutations++;
                    rob.LastMut++;

                    rob.dna[nth + 1] = new DNABlock { Type = 7, Value = 1 };
                    LogMutation(rob, $"{(IsPoint ? "Point Mutation 2" : "Copy Error 2")} changed dna location {nth + 1} to store during cycle {SimOpts.TotRunCycle}");
                    rob.Mutations++;
                    rob.LastMut++;
                }
                else
                {
                    // 2/3 chance informational
                    if ((int)(ThreadSafeRandom.Local.NextDouble() * 5) == 0)
                    {
                        // 1/5 chance large number (but still use a sysvar, if anything the parse will mod it)
                        do
                        {
                            randomsysvar = (int)(ThreadSafeRandom.Local.NextDouble() * 1000);
                        } while (DnaEngine.SystemVariables[randomsysvar].Name != "");
                        rob.dna[nth] = new DNABlock { Type = 0, Value = DnaEngine.SystemVariables[randomsysvar].Value + (int)(ThreadSafeRandom.Local.NextDouble() * 32) * 1000 };
                        holddetail = $"changed dna location {nth} to number {rob.dna[nth].Value}";
                    }
                    else
                    {
                        do
                        {
                            randomsysvar = (int)(ThreadSafeRandom.Local.NextDouble() * 256);
                        } while (DnaEngine.SystemVariables[randomsysvar].Name != "");
                        rob.dna[nth] = new DNABlock { Type = 1, Value = DnaEngine.SystemVariables[randomsysvar].Value };
                        holddetail = $"changed dna location {nth} to *number *.{DnaEngine.SystemVariables[randomsysvar].Name}";
                    }
                    LogMutation(rob, $"{(IsPoint ? "Point Mutation 2" : "Copy Error 2")} {holddetail} during cycle {SimOpts.TotRunCycle}");
                    rob.Mutations++;
                    rob.LastMut++;
                }
            }
        }

        private static void CopyError(robot rob)
        {
            var floor = SimOpts.MutCurrMult * rob.dna.Count * (rob.Mutables.Mean[CopyErrorUP] + rob.Mutables.StdDev[CopyErrorUP]) / (25 * overtime);

            if (rob.Mutables.mutarray[CopyErrorUP] < floor)
                rob.Mutables.mutarray[CopyErrorUP] = floor; //Botsareus 10/5/2015 Prevent freezing

            for (var t = 0; t < rob.dna.Count - 1; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.mutarray[CopyErrorUP] / SimOpts.MutCurrMult))
                    continue;

                var Length = (int)Gauss(rob.Mutables.StdDev[CopyErrorUP], rob.Mutables.Mean[CopyErrorUP]); //length

                ChangeDNA(rob, t, CopyErrorUP, Length, rob.Mutables.CopyErrorWhatToChange);
            }
        }

        private static void CopyError2(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[CopyErrorUP] + rob.Mutables.StdDev[CopyErrorUP]) / (5 * overtime); //Botsareus 3/22/2016 works like p2 now
            floor *= SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[CE2UP] < floor)
                rob.Mutables.mutarray[CE2UP] = floor; //Botsareus 10/5/2015 Prevent freezing

            var DNAsize = DnaLen(rob.dna) - 1; //get aprox length

            var datahit = new HashSet<int>(); // TODO - Specified Minimum Array Boundary Not Supported:     Dim datahit() As Boolean//operation repeat prevention

            for (var e = 0; e < rob.dna.Count; e++)
            {
                var calc_gauss = Gauss(rob.Mutables.StdDev[CopyErrorUP], rob.Mutables.Mean[CopyErrorUP]);
                if (calc_gauss < 1)
                    calc_gauss = 1;

                if (ThreadSafeRandom.Local.NextDouble() < (0.75 / (rob.Mutables.mutarray[CE2UP] / (SimOpts.MutCurrMult * calc_gauss))))
                {
                    int e2;

                    do
                    {
                        e2 = (int)(ThreadSafeRandom.Local.NextDouble() * DNAsize + 1);
                    } while (!datahit.Contains(e2));
                    datahit.Add(e2);

                    ChangeDNA2(rob, e2, DNAsize); //Botsareus 4/10/2016 Less boilerplate code
                }
            }
        }

        private static void DeleteSpecificGene(List<DNABlock> dna, int k)
        {
            var i = GenePosition(dna, k);
            if (i < 0)
                return;

            var f = GeneEnd(dna, i);
            dna.RemoveRange(i, f - i); // EricL Added +1
        }

        private static void DeltaMut(robot rob)
        {
            if (ThreadSafeRandom.Local.NextDouble() <= 1 - 1 / (100 * rob.Mutables.mutarray[DeltaUP] / SimOpts.MutCurrMult))
                return;

            if (rob.Mutables.StdDev[DeltaUP] == 0)
                rob.Mutables.Mean[DeltaUP] = 50;
            else if (rob.Mutables.Mean[DeltaUP] == 0)
                rob.Mutables.Mean[DeltaUP] = 25;

            int temp;

            do
            {
                temp = ThreadSafeRandom.Local.Next(0, 10); //Botsareus 12/14/2013 Added new mutations
            } while (rob.Mutables.mutarray[temp] <= 0);

            double newval;

            do
            {
                newval = Gauss(rob.Mutables.Mean[DeltaUP], rob.Mutables.mutarray[temp]);
            } while (rob.Mutables.mutarray[temp] == newval || newval <= 0);

            LogMutation(rob, $"Delta mutations changed {MutationType(temp)} from 1 in {rob.Mutables.mutarray[temp]} to 1 in {newval}");
            rob.Mutations++;
            rob.LastMut++;
            rob.Mutables.mutarray[temp] = newval;
        }

        private static void Insertion(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[InsertionUP] + rob.Mutables.StdDev[InsertionUP]) / (5 * overtime) * SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[InsertionUP] < floor)
                rob.Mutables.mutarray[InsertionUP] = floor; //Botsareus 10/5/2015 Prevent freezing

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.mutarray[InsertionUP] / SimOpts.MutCurrMult))
                    continue;

                if (rob.Mutables.Mean[InsertionUP] == 0)
                    rob.Mutables.Mean[InsertionUP] = 1;

                int Length;
                do
                {
                    Length = (int)Gauss(rob.Mutables.StdDev[InsertionUP], rob.Mutables.Mean[InsertionUP]);
                } while (Length <= 0);

                if (rob.dna.Count + Length > 32000)
                    return;

                for (var i = 0; i < Length; i++)
                    rob.dna.Insert(t + 1 + i, new DNABlock
                    {
                        Type = 0,
                        Value = 100
                    });

                ChangeDNA(rob, t + 1, InsertionUP, Length, 0); //change the type first so that the mutated value is within the space of the new type
                ChangeDNA(rob, t + 1, InsertionUP, Length, 100); //set a good value up

                t += Length;
            }
        }

        private static void MajorDeletion(robot rob)
        {
            var floor = rob.dna.Count / (2.5 * overtime) * SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[MajorDeletionUP] < floor)
                rob.Mutables.mutarray[MajorDeletionUP] = floor; //Botsareus 10/5/2015 Prevent freezing

            if (rob.Mutables.Mean[MajorDeletionUP] < 1)
                rob.Mutables.Mean[MajorDeletionUP] = 1;

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.mutarray[MajorDeletionUP] / SimOpts.MutCurrMult))
                    continue;

                int Length;

                do
                {
                    Length = (int)Gauss(rob.Mutables.StdDev[MajorDeletionUP], rob.Mutables.Mean[MajorDeletionUP]);
                } while (Length <= 0);

                if (t + Length > rob.dna.Count)
                    Length = rob.dna.Count - t;

                if (Length <= 0)
                    return;

                rob.dna.RemoveRange(t, Length);

                rob.Mutations++;
                rob.LastMut++;
                LogMutation(rob, $"Major Deletion deleted a run of {Length} bps at position {t} during cycle {SimOpts.TotRunCycle}");
            }
        }

        private static void MinorDeletion(robot rob)
        {
            var floor = rob.dna.Count / (2.5 * overtime) * SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[MinorDeletionUP] < floor)
                rob.Mutables.mutarray[MinorDeletionUP] = floor;

            if (rob.Mutables.Mean[MinorDeletionUP] < 1)
                rob.Mutables.Mean[MinorDeletionUP] = 1;

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.mutarray[MinorDeletionUP] / SimOpts.MutCurrMult))
                    continue;

                int Length;

                do
                {
                    Length = (int)Gauss(rob.Mutables.StdDev[MinorDeletionUP], rob.Mutables.Mean[MinorDeletionUP]);
                } while (Length <= 0);

                if (t + Length > rob.dna.Count)
                    Length = rob.dna.Count - t; //Botsareus 3/22/2016 Bug fix

                if (Length <= 0)
                    return;//Botsareus 3/22/2016 Bugfix

                rob.dna.RemoveRange(t, Length);

                rob.Mutations++;
                rob.LastMut++;
                LogMutation(rob, $"Minor Deletion deleted a run of {Length} bps at position {t} during cycle {SimOpts.TotRunCycle}");
            }
        }

        private static void MutateColours(robot rob, int a)
        {
            var b = (int)rob.color.R;
            var g = (int)rob.color.G;
            var r = (int)rob.color.B;

            for (var counter = 1; counter < a; counter++)
            {
                switch (ThreadSafeRandom.Local.Next(1, 4))
                {
                    case 1:
                        b += (byte)(ThreadSafeRandom.Local.Next(-20, 21));
                        break;

                    case 2:
                        g += (byte)(ThreadSafeRandom.Local.Next(-20, 21));
                        break;

                    case 3:
                        r += (byte)(ThreadSafeRandom.Local.Next(-20, 21));
                        break;
                }

                r = Math.Clamp(r, 0, 255);
                g = Math.Clamp(g, 0, 255);
                b = Math.Clamp(b, 0, 255);
            }

            rob.color = Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

        private static string MutationType(int thing)
        {
            return thing switch
            {
                0 => "Point Mutation",
                1 => "Minor Deletion",
                2 => "Reversal",
                3 => "Insertion",
                4 => "Amplification",
                5 => "Major Deletion",
                6 => "Copy Error",
                7 => "Delta Mutation",
                _ => "",
            };
        }

        private static void Point2MutWhen(double randval, robot rob)
        {
            //If randval = 0 Then randval = 0.0001

            if (rob.dna.Count == 1)
                return;

            var mutation_rate = rob.Mutables.mutarray[P2UP] / SimOpts.MutCurrMult;

            //keeps Point2 lengths the same as Point Botsareus 1/14/2014 Checking to make sure value is >= 1

            var calc_gauss = Gauss(rob.Mutables.StdDev[PointUP], rob.Mutables.Mean[PointUP]);
            if (calc_gauss < 1)
                calc_gauss = 1;

            mutation_rate /= calc_gauss;

            mutation_rate *= 1.33; //Botsareus 4/19/2016 Adjust because changedna2 may write 2 commands

            //Here we test to make sure the probability of a point mutation isn't crazy high.
            //A value of 1 is the probability of mutating every base pair every 1000 cycles
            //Lets not let it get lower than 1 shall we?
            if (mutation_rate < 1 && mutation_rate > 0)
                mutation_rate = 1;

            var result = Math.Log(1 - randval, 1 - 1 / (1000 * mutation_rate));

            while (result > 1800000000)
                result -= 1800000000;

            rob.Point2MutCycle = (int)(rob.age + result / (rob.dna.Count - 1));
        }

        private static void PointMutation(robot rob)
        {
            //assume the bot has a positive (>0) mutarray value for this

            var floor = rob.dna.Count * (rob.Mutables.Mean[PointUP] + rob.Mutables.StdDev[PointUP]) / (400 * overtime) * SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[PointUP] < floor)
                rob.Mutables.mutarray[PointUP] = floor; //Botsareus 10/5/2015 Prevent freezing

            if (rob.age == 0 || rob.PointMutCycle < rob.age)
                PointMutWhereAndWhen(ThreadSafeRandom.Local.NextDouble(), rob);

            //Do it again in case we get two point mutations in a single cycle
            while (rob.age == rob.PointMutCycle && rob.age > 0 & rob.dna.Count > 1)
            {
                ChangeDNA(rob, rob.PointMutBP, (int)Gauss(rob.Mutables.StdDev[PointUP], rob.Mutables.Mean[PointUP]) % 32000, rob.Mutables.PointWhatToChange);
                PointMutWhereAndWhen(ThreadSafeRandom.Local.NextDouble(), rob);
            }
        }

        private static void PointMutation2(robot rob)
        {
            //Botsareus 12/10/2013
            //assume the bot has a positive (>0) mutarray value for this

            var floor = rob.dna.Count * (rob.Mutables.Mean[PointUP] + rob.Mutables.StdDev[PointUP]) / (400 * overtime) * SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[P2UP] < floor)
                rob.Mutables.mutarray[P2UP] = floor; //Botsareus 10/5/2015 Prevent freezing

            if (rob.age == 0 || rob.Point2MutCycle < rob.age)
                Point2MutWhen(ThreadSafeRandom.Local.NextDouble(), rob);

            //Do it again in case we get two point mutations in a single cycle
            while (rob.age == rob.Point2MutCycle && rob.age > 0 & rob.dna.Count > 1)
            {
                //sysvar mutation
                var DNAsize = DnaLen(rob.dna) - 1; //get aprox length
                var randompos = (int)(ThreadSafeRandom.Local.NextDouble() * DNAsize) + 1; //Botsareus 3/22/2016 Bug fix

                ChangeDNA2(rob, randompos, DNAsize, true); //Botsareus 4/10/2016 Less boilerplate code

                Point2MutWhen(ThreadSafeRandom.Local.NextDouble(), rob);
            }
        }

        private static void PointMutWhereAndWhen(double randval, robot rob)
        {
            if (rob.dna.Count == 1)
                return;

            var mutation_rate = rob.Mutables.mutarray[PointUP] / SimOpts.MutCurrMult;

            //Here we test to make sure the probability of a point mutation isn't crazy high.
            //A value of 1 is the probability of mutating every base pair every 1000 cycles
            //Lets not let it get lower than 1 shall we?
            if (mutation_rate < 1 && mutation_rate > 0)
            {
                mutation_rate = 1;
            }

            var result = Math.Log(1 - randval, 1 - 1 / (1000 * mutation_rate));

            while (result > 1800000000)
                result -= 1800000000;

            rob.PointMutBP = (int)(result % (rob.dna.Count - 1)) + 1;
            rob.PointMutCycle = rob.age + (int)result / (rob.dna.Count - 1);
        }

        private static void Reversal(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[ReversalUP] + rob.Mutables.StdDev[ReversalUP]) / (105 * overtime) * SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[ReversalUP] < floor)
                rob.Mutables.mutarray[ReversalUP] = floor; //Botsareus 10/5/2015 Prevent freezing

            for (var t = 0; t < (rob.dna.Count - 1); t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() < 1 / (rob.Mutables.mutarray[ReversalUP] / SimOpts.MutCurrMult))
                {
                    if (rob.Mutables.Mean[ReversalUP] < 2)
                        rob.Mutables.Mean[ReversalUP] = 2;

                    int Length;

                    do
                    {
                        Length = (int)Gauss(rob.Mutables.StdDev[ReversalUP], rob.Mutables.Mean[ReversalUP]);
                    } while (Length <= 0);

                    Length /= 2; //be sure we go an even amount to either side

                    if (t - Length < 1)
                        Length = t - 1;

                    if (t + Length > rob.dna.Count - 1)
                        Length = rob.dna.Count - 1 - t;

                    if (Length > 0)
                    {
                        var second = 0;
                        for (var counter = t - Length; counter < t - 1; counter++)
                        {
                            var tempblock = rob.dna[counter];
                            rob.dna[counter] = rob.dna[t + Length - second];
                            rob.dna[t + Length - second] = tempblock;
                            second++;
                        }

                        rob.Mutations++;
                        rob.LastMut++;

                        LogMutation(rob, $"Reversal of {Length * 2 + 1} bps centered at {t} during cycle {SimOpts.TotRunCycle}");
                    }
                }
            }
        }

        private static void Translocation(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[TranslocationUP] + rob.Mutables.StdDev[TranslocationUP]) / (360 * overtime) * SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[TranslocationUP] < floor)
            {
                rob.Mutables.mutarray[TranslocationUP] = floor; //Botsareus 10/5/2015 Prevent freezing
            }

            var tempDNA = new List<DNABlock>();

            for (var t = 1; t < rob.dna.Count - 1; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() < 1 / (rob.Mutables.mutarray[TranslocationUP] / SimOpts.MutCurrMult))
                {
                    var Length = (int)Gauss(rob.Mutables.StdDev[TranslocationUP], rob.Mutables.Mean[TranslocationUP]);
                    Length %= rob.dna.Count;

                    if (Length < 1)
                        Length = 1;

                    Length--;
                    Length /= 2;

                    if (t - Length < 1 || t + Length > rob.dna.Count - 1 || Length <= 0)
                        continue;

                    for (var counter = t - Length; counter < t + Length; counter++)
                        tempDNA.Add(rob.dna[counter]);

                    //we now have the appropriate length of DNA in the temporary array.

                    //delete fragment
                    rob.dna.RemoveRange(t - Length, Length * 2 + 1); //Botsareus 12/11/2015 Bug fix
                    var start = ThreadSafeRandom.Local.Next(1, rob.dna.Count - 1);
                    rob.dna.InsertRange(start, tempDNA);

                    rob.Mutations++;
                    rob.LastMut++;
                    LogMutation(rob, $"Translocation moved a series at {t} {Length * 2 + 1} bps long to {start} during cycle {SimOpts.TotRunCycle}");
                }
            }

            //add "end" to end of the DNA
            rob.dna[-1] = new DNABlock { Type = 10, Value = 1 };
        }
    }
}