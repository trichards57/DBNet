using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    public enum MutationType
    {
        PointMutation,
        MinorDeletion,
        Reversal,
        Insertion,
        Amplification,
        MajorDeletion,
        CopyError,
        Delta,
        Translocation,
        PointMutation2,
        CopyError2
    }

    internal static class NeoMutations
    {
        private const double OverTime = 30; //Time correction across all mutations

        public static void DeleteGene(robot rob, int g)
        {
            if (g <= 0 || g > rob.genenum)
                return;

            DeleteSpecificGene(rob.dna, g);
            rob.genenum = DnaManipulations.CountGenes(rob.dna);
            rob.mem[Robots.DnaLenSys] = rob.dna.Count;
            rob.mem[Robots.GenesSys] = rob.genenum;
            Senses.MakeOccurrList(rob);
        }

        public static void LogMutation(robot rob, string strmut)
        {
            if (SimOpt.SimOpts.TotRunCycle == 0)
                return;

            if (rob.LastMutDetail.Length > 100000000 / Robots.TotalRobotsDisplayed)
                rob.LastMutDetail = "";

            rob.LastMutDetail = $"{strmut}\n{rob.LastMutDetail}";
        }

        public static void Mutate(robot rob, bool reproducing = false)
        {
            if (!rob.Mutables.Mutations || SimOpt.SimOpts.DisableMutations)
                return;

            var delta = rob.LastMut;

            if (!reproducing)
            {
                if (rob.Mutables.mutarray[(int)MutationType.PointMutation] > 0)
                    PointMutation(rob);

                if (rob.Mutables.mutarray[(int)MutationType.Delta] > 0 & !Globals.Delta2)
                    DeltaMut(rob);

                if (rob.Mutables.mutarray[(int)MutationType.PointMutation2] > 0 & Globals.SunBelt)
                    PointMutation2(rob);

                //special case update epigenetic reset
                if (rob.LastMut - delta > 0 & Globals.EpiReset)
                    rob.MutEpiReset += Math.Pow(rob.LastMut - delta, Globals.EpiResetEmp);

                //Delta2 point mutation change
                if (Globals.Delta2 && Globals.DeltaPm > 0 && rob.age % Globals.DeltaPm == 0 & rob.age > 0)
                {
                    var mutationRatesMax = Globals.NormMut ? rob.dna.Count * Globals.ValMaxNormMut : 2000000000;

                    foreach (var t in new[] { (int)MutationType.PointMutation, (int)MutationType.PointMutation2 })
                    {
                        //Point and Point2
                        if (rob.Mutables.mutarray[t] < 1)
                            continue;

                        if (ThreadSafeRandom.Local.NextDouble() < Globals.DeltaMainChance / 100.0)
                        {
                            if (Globals.DeltaMainExp != 0)
                                rob.Mutables.mutarray[t] = Math.Pow(rob.Mutables.mutarray[t] * 10, ((ThreadSafeRandom.Local.NextDouble() * 2 - 1) / Globals.DeltaMainExp));

                            rob.Mutables.mutarray[t] = rob.Mutables.mutarray[t] + (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * Globals.DeltaMainLn;
                            if (rob.Mutables.mutarray[t] < 1)
                                rob.Mutables.mutarray[t] = 1;

                            if (rob.Mutables.mutarray[t] > mutationRatesMax)
                                rob.Mutables.mutarray[t] = mutationRatesMax;
                        }
                        if (ThreadSafeRandom.Local.NextDouble() < Globals.DeltaDevChance / 100.0)
                        {
                            if (Globals.DeltaDevExp != 0)
                                rob.Mutables.StdDev[t] = rob.Mutables.StdDev[t] * Math.Pow(10, ((ThreadSafeRandom.Local.NextDouble() * 2 - 1) / Globals.DeltaDevExp));

                            rob.Mutables.StdDev[t] = rob.Mutables.StdDev[t] + (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * Globals.DeltaDevLn;
                            if (Globals.DeltaDevExp != 0)
                                rob.Mutables.Mean[t] = rob.Mutables.Mean[t] * Math.Pow(10, ((ThreadSafeRandom.Local.NextDouble() * 2 - 1) / Globals.DeltaDevExp));

                            rob.Mutables.Mean[t] = rob.Mutables.Mean[t] + (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * Globals.DeltaDevLn;
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

                    rob.Mutables.PointWhatToChange += (int)(ThreadSafeRandom.Local.NextDouble() * 2 - 1) * Globals.DeltaWtc;

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
                if (rob.Mutables.mutarray[(int)MutationType.CopyError] > 0)
                    CopyError(rob);

                if (rob.Mutables.mutarray[(int)MutationType.CopyError2] > 0 & Globals.SunBelt)
                    CopyError2(rob);

                if (rob.Mutables.mutarray[(int)MutationType.Insertion] > 0)
                    Insertion(rob);

                if (rob.Mutables.mutarray[(int)MutationType.Reversal] > 0)
                    Reversal(rob);

                if (rob.Mutables.mutarray[(int)MutationType.Translocation] > 0 & Globals.SunBelt)
                    Translocation(rob);

                if (rob.Mutables.mutarray[(int)MutationType.Amplification] > 0 & Globals.SunBelt)
                    Amplification(rob);

                if (rob.Mutables.mutarray[(int)MutationType.MajorDeletion] > 0)
                    MajorDeletion(rob);

                if (rob.Mutables.mutarray[(int)MutationType.MinorDeletion] > 0)
                    MinorDeletion(rob);
            }

            delta = rob.LastMut - delta; //Botsareus 9/4/2012 Moved delta check before overflow reset to fix an error where robot info is not being updated

            //auto forking
            if (SimOpt.SimOpts.EnableAutoSpeciation)
            {
                if (rob.Mutations > rob.dna.Count * (double)SimOpt.SimOpts.SpeciationGeneticDistance / 100)
                {
                    SimOpt.SimOpts.SpeciationForkInterval++;
                    var splitname = rob.FName.Split(")");
                    var robname = splitname[0].StartsWith("(") && int.TryParse(splitname[0][1..], out var _) ? splitname[1] : rob.FName;

                    robname = "(" + SimOpt.SimOpts.SpeciationForkInterval + ")" + robname;

                    if (SimOpt.SimOpts.Specie.Count < 49)
                    {
                        rob.FName = robname;
                        rob.Mutations = 0;
                        HDRoutines.AddSpecie(rob, false);
                    }
                    else
                        SimOpt.SimOpts.SpeciationForkInterval--;
                }
            }

            if (rob.Mutations > 32000)
                rob.Mutations = 32000;

            if (rob.LastMut > 32000)
                rob.LastMut = 32000;

            if (delta <= 0)
                return;

            // The bot has mutated.
            rob.GenMut -= rob.LastMut;
            if (rob.GenMut < 0)
                rob.GenMut = 0;

            MutateColours(rob, delta);
            rob.SubSpecies = NewSubSpecies(rob);
            rob.genenum = DnaManipulations.CountGenes(rob.dna);
            rob.mem[Robots.DnaLenSys] = rob.dna.Count;
            rob.mem[Robots.GenesSys] = rob.genenum;
        }

        public static int NewSubSpecies(robot rob)
        {
            var i = Senses.SpeciesFromBot(rob); // Get the index into the species array for this bot
            i.SubSpeciesCounter++; // increment the counter

            if (i.SubSpeciesCounter > 32000)
                i.SubSpeciesCounter = -32000; //wrap the counter if necessary

            return i.SubSpeciesCounter;
        }

        public static void SetDefaultLengths(MutationProbabilities changeme)
        {
            changeme.Mean[(int)MutationType.PointMutation] = 3;
            changeme.StdDev[(int)MutationType.PointMutation] = 1;

            changeme.Mean[(int)MutationType.Delta] = 500;
            changeme.StdDev[(int)MutationType.Delta] = 150;

            changeme.Mean[(int)MutationType.MinorDeletion] = 1;
            changeme.StdDev[(int)MutationType.MinorDeletion] = 0;

            changeme.Mean[(int)MutationType.Insertion] = 1;
            changeme.StdDev[(int)MutationType.Insertion] = 0;

            changeme.Mean[(int)MutationType.CopyError] = 1;
            changeme.StdDev[(int)MutationType.CopyError] = 0;

            changeme.Mean[(int)MutationType.MajorDeletion] = 3;
            changeme.StdDev[(int)MutationType.MajorDeletion] = 1;

            changeme.Mean[(int)MutationType.Reversal] = 3;
            changeme.StdDev[(int)MutationType.Reversal] = 1;

            changeme.CopyErrorWhatToChange = 80;
            changeme.PointWhatToChange = 80;

            changeme.Mean[(int)MutationType.Amplification] = 250;
            changeme.StdDev[(int)MutationType.Amplification] = 75;

            changeme.Mean[(int)MutationType.Translocation] = 250;
            changeme.StdDev[(int)MutationType.Translocation] = 75;
        }

        private static void Amplification(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[(int)MutationType.Amplification] + rob.Mutables.StdDev[(int)MutationType.Amplification]) / (1200 * OverTime);
            floor *= SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.Amplification] < floor)
                rob.Mutables.mutarray[(int)MutationType.Amplification] = floor; //Botsareus 10/5/2015 Prevent freezing

            var t = 1;
            do
            {
                t++;
                if (!(ThreadSafeRandom.Local.NextDouble() <
                      1 / (rob.Mutables.mutarray[(int)MutationType.Amplification] / SimOpt.SimOpts.MutCurrMult))) continue;
                var length = (int)Common.Gauss(rob.Mutables.StdDev[(int)MutationType.Amplification], rob.Mutables.Mean[(int)MutationType.Amplification]);
                length %= rob.dna.Count;
                if (length < 1)
                    length = 1;

                length--;
                length /= 2;

                if (t - length < 1 || t + length > rob.dna.Count - 1 || rob.dna.Count + length * 2 > 32000)
                    continue;

                if (length <= 0) continue;
                var tempDna = rob.dna.Skip(t).Take(length);

                //we now have the appropriate length of DNA in the temporary array.

                var start = ThreadSafeRandom.Local.Next(1, rob.dna.Count - 1);
                rob.dna.InsertRange(start, tempDna);

                rob.Mutations++;
                rob.LastMut++;
                LogMutation(rob, $"Amplification copied a series at {t}, {length * 2 + 1}bps long to {start} during cycle {SimOpt.SimOpts.TotRunCycle}");
            } while (t < rob.dna.Count);

            if (!(rob.dna.Last().Type == 10 && rob.dna.Last().Value == 1))
                rob.dna.Add(new DNABlock { Type = 10, Value = 1 });
        }

        private static void ChangeDna(robot rob, int nth, MutationType mutationType, int length = 1, int pointToChange = 50)
        {
            for (var t = nth; t < (nth + length); t++)
            {
                //if length is 1, it's only one bp we're mutating, remember?
                if (t >= rob.dna.Count || rob.dna[t].Type == 10)
                    return; //don't mutate end either

                if (ThreadSafeRandom.Local.Next(0, 99) < pointToChange)
                {
                    if (rob.dna[t].Value != 0 && mutationType == MutationType.Insertion)
                        rob.dna[t] = rob.dna[t] with { Value = (int)Common.Gauss(500) };

                    var old = rob.dna[t].Value;

                    if (rob.dna[t].Type is 0 or 1)
                    {
                        do
                        {
                            if (Math.Abs(old) <= 1000)
                                rob.dna[t] = rob.dna[t] with { Value = ThreadSafeRandom.Local.Next(0, 2) == 0 ? (int)Common.Gauss(94, rob.dna[t].Value) : (int)Common.Gauss(7, rob.dna[t].Value) };
                            else
                                rob.dna[t] = rob.dna[t] with { Value = (int)Common.Gauss(old / 10.0, rob.dna[t].Value) }; //for very large numbers scale gauss
                        } while (rob.dna[t].Value == old);

                        rob.Mutations++;
                        rob.LastMut++;

                        LogMutation(rob, $"{MutationToString(mutationType)} changed {DnaTokenizing.TipoDetok(rob.dna[t].Type)} from {old} to {rob.dna[t].Value} at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
                    }
                    else
                    {
                        var bp = new DNABlock
                        {
                            Type = rob.dna[t].Type
                        };

                        var max = 0;

                        do
                        {
                            max++;
                            bp = bp with { Value = max };
                        } while (DnaTokenizing.Parse(bp) != "");

                        max--;

                        if (max <= 1)
                            return;

                        do
                        {
                            rob.dna[t] = rob.dna[t] with { Value = ThreadSafeRandom.Local.Next(1, max) };
                        } while (rob.dna[t].Value == old);

                        bp = bp with { Type = rob.dna[t].Type };
                        bp = bp with { Value = old };

                        var name = DnaTokenizing.Parse(rob.dna[t]);
                        var oldName = DnaTokenizing.Parse(bp);

                        rob.Mutations++;
                        rob.LastMut++;

                        LogMutation(rob, $"{MutationToString(mutationType)} changed value of {DnaTokenizing.TipoDetok(rob.dna[t].Type)} from {oldName} to {name} at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
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
                    } while (rob.dna[t].Type == bp.Type || DnaTokenizing.TipoDetok(rob.dna[t].Type) == "");

                    var max = 0;
                    if (rob.dna[t].Type >= 2)
                    {
                        do
                        {
                            max++;
                            rob.dna[t] = rob.dna[t] with { Value = max };
                        } while (DnaTokenizing.Parse(rob.dna[t]) != "");

                        max--;

                        if (max <= 1)
                            return;

                        rob.dna[t] = rob.dna[t] with { Value = ((Math.Abs(bp.Value) - 1) % max) + 1 };
                        if (rob.dna[t].Value == 0)
                            rob.dna[t] = rob.dna[t] with { Value = 1 };
                    }

                    var name = DnaTokenizing.Parse(rob.dna[t]);
                    var oldName = DnaTokenizing.Parse(bp);
                    rob.Mutations++;
                    rob.LastMut++;

                    LogMutation(rob, $"{MutationToString(mutationType)} changed the {DnaTokenizing.TipoDetok(bp.Type)}: {oldName} to the {DnaTokenizing.TipoDetok(rob.dna[t].Type)} : {name} at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
                }
            }
        }

        private static void ChangeDna2(robot rob, int nth, int dnaSize, bool isPoint = false)
        {
            int randomSystemVariable;

            var holdDetail = "";

            //for .tieloc, .shoot, and functional
            do
            {
                randomSystemVariable = (int)(ThreadSafeRandom.Local.NextDouble() * 256);
            } while (DnaEngine.SystemVariables[randomSystemVariable].Name != "");

            var special = false;
            //special cases
            if (nth < dnaSize - 2)
            {
                //for .shoot store
                if (rob.dna[nth + 1].Type == 0 & rob.dna[nth + 1].Value == Robots.shoot && rob.dna[nth + 2].Type == 7 && rob.dna[nth + 2].Value == 1)
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
                            _ => DnaEngine.SystemVariables[randomSystemVariable].Value
                        },
                        Type = 0
                    };
                    holdDetail = $"changed dna location {nth} to {rob.dna[nth].Value}";
                    special = true;
                }
                //for .focuseye store
                if (rob.dna[nth + 1].Type == 0 & rob.dna[nth + 1].Value == Robots.FOCUSEYE && rob.dna[nth + 2].Type == 7 && rob.dna[nth + 2].Value == 1)
                {
                    rob.dna[nth] = new DNABlock { Value = (int)(ThreadSafeRandom.Local.NextDouble() * 9) - 4, Type = 0 };
                    holdDetail = $"changed dna location {nth} to {rob.dna[nth].Value}";
                    special = true;
                }
                //for .tieloc store
                if (rob.dna[nth + 1].Type == 0 & rob.dna[nth + 1].Value == Robots.tieloc && rob.dna[nth + 2].Type == 7 && rob.dna[nth + 2].Value == 1)
                {
                    rob.dna[nth] = new DNABlock
                    {
                        Value = ThreadSafeRandom.Local.Next(1, 6) switch
                        {
                            1 => -1,
                            2 => -3,
                            3 => -4,
                            4 => -6,
                            _ => DnaEngine.SystemVariables[randomSystemVariable].Value
                        },
                        Type = 0
                    };
                    holdDetail = $"changed dna location {nth} to {rob.dna[nth].Value}";
                    special = true;
                }
            }

            if (special)
            {
                LogMutation(rob, $"{(isPoint ? "Point Mutation 2" : "Copy Error 2")} {holdDetail} during cycle {SimOpt.SimOpts.TotRunCycle}");
                rob.Mutations++;
                rob.LastMut++;
            }
            else
            { //other cases
                if (nth < dnaSize - 1 && (int)(ThreadSafeRandom.Local.NextDouble() * 3) == 0)
                { //1/3 chance functional
                    rob.dna[nth] = new DNABlock { Type = 0, Value = DnaEngine.SystemVariables[randomSystemVariable].Value };
                    LogMutation(rob, $"{(isPoint ? "Point Mutation 2" : "Copy Error 2")} changed dna location {nth} to number .{DnaEngine.SystemVariables[randomSystemVariable].Name} during cycle {SimOpt.SimOpts.TotRunCycle}");
                    rob.Mutations++;
                    rob.LastMut++;

                    rob.dna[nth + 1] = new DNABlock { Type = 7, Value = 1 };
                    LogMutation(rob, $"{(isPoint ? "Point Mutation 2" : "Copy Error 2")} changed dna location {nth + 1} to store during cycle {SimOpt.SimOpts.TotRunCycle}");
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
                            randomSystemVariable = (int)(ThreadSafeRandom.Local.NextDouble() * 1000);
                        } while (DnaEngine.SystemVariables[randomSystemVariable].Name != "");
                        rob.dna[nth] = new DNABlock { Type = 0, Value = DnaEngine.SystemVariables[randomSystemVariable].Value + (int)(ThreadSafeRandom.Local.NextDouble() * 32) * 1000 };
                        holdDetail = $"changed dna location {nth} to number {rob.dna[nth].Value}";
                    }
                    else
                    {
                        do
                        {
                            randomSystemVariable = (int)(ThreadSafeRandom.Local.NextDouble() * 256);
                        } while (DnaEngine.SystemVariables[randomSystemVariable].Name != "");
                        rob.dna[nth] = new DNABlock { Type = 1, Value = DnaEngine.SystemVariables[randomSystemVariable].Value };
                        holdDetail = $"changed dna location {nth} to *number *.{DnaEngine.SystemVariables[randomSystemVariable].Name}";
                    }
                    LogMutation(rob, $"{(isPoint ? "Point Mutation 2" : "Copy Error 2")} {holdDetail} during cycle {SimOpt.SimOpts.TotRunCycle}");
                    rob.Mutations++;
                    rob.LastMut++;
                }
            }
        }

        private static void CopyError(robot rob)
        {
            var floor = SimOpt.SimOpts.MutCurrMult * rob.dna.Count * (rob.Mutables.Mean[(int)MutationType.CopyError] + rob.Mutables.StdDev[(int)MutationType.CopyError]) / (25 * OverTime);

            if (rob.Mutables.mutarray[(int)MutationType.CopyError] < floor)
                rob.Mutables.mutarray[(int)MutationType.CopyError] = floor; //Botsareus 10/5/2015 Prevent freezing

            for (var t = 0; t < rob.dna.Count - 1; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.mutarray[(int)MutationType.CopyError] / SimOpt.SimOpts.MutCurrMult))
                    continue;

                var length = (int)Common.Gauss(rob.Mutables.StdDev[(int)MutationType.CopyError], rob.Mutables.Mean[(int)MutationType.CopyError]); //length

                ChangeDna(rob, t, MutationType.CopyError, length, rob.Mutables.CopyErrorWhatToChange);
            }
        }

        private static void CopyError2(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[(int)MutationType.CopyError] + rob.Mutables.StdDev[(int)MutationType.CopyError]) / (5 * OverTime); //Botsareus 3/22/2016 works like p2 now
            floor *= SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.CopyError2] < floor)
                rob.Mutables.mutarray[(int)MutationType.CopyError2] = floor; //Botsareus 10/5/2015 Prevent freezing

            var dnaSize = DnaManipulations.DnaLen(rob.dna) - 1; //get aprox length

            var dataHit = new HashSet<int>();

            for (var e = 0; e < rob.dna.Count; e++)
            {
                var calcGauss = Common.Gauss(rob.Mutables.StdDev[(int)MutationType.CopyError], rob.Mutables.Mean[(int)MutationType.CopyError]);
                if (calcGauss < 1)
                    calcGauss = 1;

                if (!(ThreadSafeRandom.Local.NextDouble() < (0.75 / (rob.Mutables.mutarray[(int)MutationType.CopyError2] / (SimOpt.SimOpts.MutCurrMult * calcGauss)))))
                    continue;

                int e2;

                do
                {
                    e2 = (int)(ThreadSafeRandom.Local.NextDouble() * dnaSize + 1);
                } while (!dataHit.Contains(e2));
                dataHit.Add(e2);

                ChangeDna2(rob, e2, dnaSize); //Botsareus 4/10/2016 Less boilerplate code
            }
        }

        private static void DeleteSpecificGene(List<DNABlock> dna, int k)
        {
            var i = DnaManipulations.GenePosition(dna, k);
            if (i < 0)
                return;

            var f = DnaManipulations.GeneEnd(dna, i);
            dna.RemoveRange(i, f - i); // EricL Added +1
        }

        private static void DeltaMut(robot rob)
        {
            if (ThreadSafeRandom.Local.NextDouble() <= 1 - 1 / (100 * rob.Mutables.mutarray[(int)MutationType.Delta] / SimOpt.SimOpts.MutCurrMult))
                return;

            if (rob.Mutables.StdDev[(int)MutationType.Delta] == 0)
                rob.Mutables.Mean[(int)MutationType.Delta] = 50;
            else if (rob.Mutables.Mean[(int)MutationType.Delta] == 0)
                rob.Mutables.Mean[(int)MutationType.Delta] = 25;

            MutationType temp;

            do
            {
                temp = (MutationType)ThreadSafeRandom.Local.Next(0, 10); //Botsareus 12/14/2013 Added new mutations
            } while (rob.Mutables.mutarray[(int)temp] <= 0);

            double newval;

            do
            {
                newval = Common.Gauss(rob.Mutables.Mean[(int)MutationType.Delta], rob.Mutables.mutarray[(int)temp]);
            } while (rob.Mutables.mutarray[(int)temp] == newval || newval <= 0);

            LogMutation(rob, $"Delta mutations changed {MutationToString(temp)} from 1 in {rob.Mutables.mutarray[(int)temp]} to 1 in {newval}");
            rob.Mutations++;
            rob.LastMut++;
            rob.Mutables.mutarray[(int)temp] = newval;
        }

        private static void Insertion(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[(int)MutationType.Insertion] + rob.Mutables.StdDev[(int)MutationType.Insertion]) / (5 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.Insertion] < floor)
                rob.Mutables.mutarray[(int)MutationType.Insertion] = floor; //Botsareus 10/5/2015 Prevent freezing

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.mutarray[(int)MutationType.Insertion] / SimOpt.SimOpts.MutCurrMult))
                    continue;

                if (rob.Mutables.Mean[(int)MutationType.Insertion] == 0)
                    rob.Mutables.Mean[(int)MutationType.Insertion] = 1;

                int length;
                do
                {
                    length = (int)Common.Gauss(rob.Mutables.StdDev[(int)MutationType.Insertion], rob.Mutables.Mean[(int)MutationType.Insertion]);
                } while (length <= 0);

                if (rob.dna.Count + length > 32000)
                    return;

                for (var i = 0; i < length; i++)
                {
                    rob.dna.Insert(t + 1 + i, new DNABlock
                    {
                        Type = 0,
                        Value = 100
                    });
                }

                ChangeDna(rob, t + 1, MutationType.Insertion, length, 0); //change the type first so that the mutated value is within the space of the new type
                ChangeDna(rob, t + 1, MutationType.Insertion, length, 100); //set a good value up

                t += length;
            }
        }

        private static void MajorDeletion(robot rob)
        {
            var floor = rob.dna.Count / (2.5 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.MajorDeletion] < floor)
                rob.Mutables.mutarray[(int)MutationType.MajorDeletion] = floor; //Botsareus 10/5/2015 Prevent freezing

            if (rob.Mutables.Mean[(int)MutationType.MajorDeletion] < 1)
                rob.Mutables.Mean[(int)MutationType.MajorDeletion] = 1;

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.mutarray[(int)MutationType.MajorDeletion] / SimOpt.SimOpts.MutCurrMult))
                    continue;

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.Mutables.StdDev[(int)MutationType.MajorDeletion], rob.Mutables.Mean[(int)MutationType.MajorDeletion]);
                } while (length <= 0);

                if (t + length > rob.dna.Count)
                    length = rob.dna.Count - t;

                if (length <= 0)
                    return;

                rob.dna.RemoveRange(t, length);

                rob.Mutations++;
                rob.LastMut++;
                LogMutation(rob, $"Major Deletion deleted a run of {length} bps at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
            }
        }

        private static void MinorDeletion(robot rob)
        {
            var floor = rob.dna.Count / (2.5 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.MinorDeletion] < floor)
                rob.Mutables.mutarray[(int)MutationType.MinorDeletion] = floor;

            if (rob.Mutables.Mean[(int)MutationType.MinorDeletion] < 1)
                rob.Mutables.Mean[(int)MutationType.MinorDeletion] = 1;

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.mutarray[(int)MutationType.MinorDeletion] / SimOpt.SimOpts.MutCurrMult))
                    continue;

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.Mutables.StdDev[(int)MutationType.MinorDeletion], rob.Mutables.Mean[(int)MutationType.MinorDeletion]);
                } while (length <= 0);

                if (t + length > rob.dna.Count)
                    length = rob.dna.Count - t;

                if (length <= 0)
                    return;

                rob.dna.RemoveRange(t, length);

                rob.Mutations++;
                rob.LastMut++;
                LogMutation(rob, $"Minor Deletion deleted a run of {length} bps at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
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

        private static string MutationToString(MutationType thing)
        {
            return thing switch
            {
                MutationType.PointMutation => "Point Mutation",
                MutationType.MinorDeletion => "Minor Deletion",
                MutationType.Reversal => "Reversal",
                MutationType.Insertion => "Insertion",
                MutationType.Amplification => "Amplification",
                MutationType.MajorDeletion => "Major Deletion",
                MutationType.CopyError => "Copy Error",
                MutationType.Delta => "Delta Mutation",
                MutationType.Translocation => "Translocation",
                MutationType.PointMutation2 => "Point Mutation 2",
                MutationType.CopyError2 => "Copy Error 2",
                _ => ""
            };
        }

        private static void Point2MutWhen(double randval, robot rob)
        {
            //If randval = 0 Then randval = 0.0001

            if (rob.dna.Count == 1)
                return;

            var mutationRate = rob.Mutables.mutarray[(int)MutationType.PointMutation2] / SimOpt.SimOpts.MutCurrMult;

            //keeps Point2 lengths the same as Point Botsareus 1/14/2014 Checking to make sure value is >= 1

            var calcGauss = Common.Gauss(rob.Mutables.StdDev[(int)MutationType.PointMutation], rob.Mutables.Mean[(int)MutationType.PointMutation]);
            if (calcGauss < 1)
                calcGauss = 1;

            mutationRate /= calcGauss;

            mutationRate *= 1.33; //Botsareus 4/19/2016 Adjust because changedna2 may write 2 commands

            //Here we test to make sure the probability of a point mutation isn't crazy high.
            //A value of 1 is the probability of mutating every base pair every 1000 cycles
            //Lets not let it get lower than 1 shall we?
            if (mutationRate is < 1 and > 0)
                mutationRate = 1;

            var result = Math.Log(1 - randval, 1 - 1 / (1000 * mutationRate));

            while (result > 1800000000)
                result -= 1800000000;

            rob.Point2MutCycle = (int)(rob.age + result / (rob.dna.Count - 1));
        }

        private static void PointMutation(robot rob)
        {
            //assume the bot has a positive (>0) mutarray value for this

            var floor = rob.dna.Count * (rob.Mutables.Mean[(int)MutationType.PointMutation] + rob.Mutables.StdDev[(int)MutationType.PointMutation]) / (400 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.PointMutation] < floor)
                rob.Mutables.mutarray[(int)MutationType.PointMutation] = floor; //Botsareus 10/5/2015 Prevent freezing

            if (rob.age == 0 || rob.PointMutCycle < rob.age)
                PointMutWhereAndWhen(ThreadSafeRandom.Local.NextDouble(), rob);

            //Do it again in case we get two point mutations in a single cycle
            while (rob.age == rob.PointMutCycle && rob.age > 0 & rob.dna.Count > 1)
            {
                ChangeDna(rob, rob.PointMutBP, (MutationType)(Common.Gauss(rob.Mutables.StdDev[(int)MutationType.PointMutation], rob.Mutables.Mean[(int)MutationType.PointMutation]) % 32000), rob.Mutables.PointWhatToChange);
                PointMutWhereAndWhen(ThreadSafeRandom.Local.NextDouble(), rob);
            }
        }

        private static void PointMutation2(robot rob)
        {
            //Botsareus 12/10/2013
            //assume the bot has a positive (>0) mutarray value for this

            var floor = rob.dna.Count * (rob.Mutables.Mean[(int)MutationType.PointMutation] + rob.Mutables.StdDev[(int)MutationType.PointMutation]) / (400 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.PointMutation2] < floor)
                rob.Mutables.mutarray[(int)MutationType.PointMutation2] = floor;

            if (rob.age == 0 || rob.Point2MutCycle < rob.age)
                Point2MutWhen(ThreadSafeRandom.Local.NextDouble(), rob);

            //Do it again in case we get two point mutations in a single cycle
            while (rob.age == rob.Point2MutCycle && rob.age > 0 & rob.dna.Count > 1)
            {
                //sysvar mutation
                var dnaSize = DnaManipulations.DnaLen(rob.dna) - 1; //get aprox length
                var randomPos = (int)(ThreadSafeRandom.Local.NextDouble() * dnaSize) + 1;

                ChangeDna2(rob, randomPos, dnaSize, true);

                Point2MutWhen(ThreadSafeRandom.Local.NextDouble(), rob);
            }
        }

        private static void PointMutWhereAndWhen(double randVal, robot rob)
        {
            if (rob.dna.Count == 1)
                return;

            var mutationRate = rob.Mutables.mutarray[(int)MutationType.PointMutation] / SimOpt.SimOpts.MutCurrMult;

            //Here we test to make sure the probability of a point mutation isn't crazy high.
            //A value of 1 is the probability of mutating every base pair every 1000 cycles
            //Lets not let it get lower than 1 shall we?
            if (mutationRate is < 1 and > 0)
                mutationRate = 1;

            var result = Math.Log(1 - randVal, 1 - 1 / (1000 * mutationRate));

            while (result > 1800000000)
                result -= 1800000000;

            rob.PointMutBP = (int)(result % (rob.dna.Count - 1)) + 1;
            rob.PointMutCycle = rob.age + (int)result / (rob.dna.Count - 1);
        }

        private static void Reversal(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[(int)MutationType.Reversal] + rob.Mutables.StdDev[(int)MutationType.Reversal]) / (105 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.Reversal] < floor)
                rob.Mutables.mutarray[(int)MutationType.Reversal] = floor; //Botsareus 10/5/2015 Prevent freezing

            for (var t = 0; t < (rob.dna.Count - 1); t++)
            {
                if (!(ThreadSafeRandom.Local.NextDouble() <
                      1 / (rob.Mutables.mutarray[(int)MutationType.Reversal] / SimOpt.SimOpts.MutCurrMult))) continue;
                if (rob.Mutables.Mean[(int)MutationType.Reversal] < 2)
                    rob.Mutables.Mean[(int)MutationType.Reversal] = 2;

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.Mutables.StdDev[(int)MutationType.Reversal], rob.Mutables.Mean[(int)MutationType.Reversal]);
                } while (length <= 0);

                length /= 2; //be sure we go an even amount to either side

                if (t - length < 1)
                    length = t - 1;

                if (t + length > rob.dna.Count - 1)
                    length = rob.dna.Count - 1 - t;

                if (length <= 0)
                    continue;

                var second = 0;

                for (var counter = t - length; counter < t - 1; counter++)
                {
                    var tempBlock = rob.dna[counter];
                    rob.dna[counter] = rob.dna[t + length - second];
                    rob.dna[t + length - second] = tempBlock;
                    second++;
                }

                rob.Mutations++;
                rob.LastMut++;

                LogMutation(rob, $"Reversal of {length * 2 + 1} bps centered at {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
            }
        }

        private static void Translocation(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Mean[(int)MutationType.Translocation] + rob.Mutables.StdDev[(int)MutationType.Translocation]) / (360 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.mutarray[(int)MutationType.Translocation] < floor)
            {
                rob.Mutables.mutarray[(int)MutationType.Translocation] = floor; //Botsareus 10/5/2015 Prevent freezing
            }

            var tempDna = new List<DNABlock>();

            for (var t = 1; t < rob.dna.Count - 1; t++)
            {
                if (!(ThreadSafeRandom.Local.NextDouble() < 1 / (rob.Mutables.mutarray[(int)MutationType.Translocation] / SimOpt.SimOpts.MutCurrMult)))
                    continue;

                var length = (int)Common.Gauss(rob.Mutables.StdDev[(int)MutationType.Translocation], rob.Mutables.Mean[(int)MutationType.Translocation]);
                length %= rob.dna.Count;

                if (length < 1)
                    length = 1;

                length--;
                length /= 2;

                if (t - length < 1 || t + length > rob.dna.Count - 1 || length <= 0)
                    continue;

                for (var counter = t - length; counter < t + length; counter++)
                    tempDna.Add(rob.dna[counter]);

                //we now have the appropriate length of DNA in the temporary array.

                //delete fragment
                rob.dna.RemoveRange(t - length, length * 2 + 1);
                var start = ThreadSafeRandom.Local.Next(1, rob.dna.Count - 1);
                rob.dna.InsertRange(start, tempDna);

                rob.Mutations++;
                rob.LastMut++;
                LogMutation(rob, $"Translocation moved a series at {t} {length * 2 + 1} bps long to {start} during cycle {SimOpt.SimOpts.TotRunCycle}");
            }

            //add "end" to end of the DNA
            rob.dna[-1] = new DNABlock { Type = 10, Value = 1 };
        }
    }
}
