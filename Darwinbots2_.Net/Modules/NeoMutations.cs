using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal static class NeoMutations
    {
        private const double OverTime = 30; // Time correction across all mutations

        public static void DeleteGene(robot rob, int g)
        {
            if (g <= 0 || g > rob.genenum)
                return;

            DeleteSpecificGene(rob.dna, g);
            rob.genenum = DnaManipulations.CountGenes(rob.dna);
            rob.mem[MemoryAddresses.DnaLenSys] = rob.dna.Count;
            rob.mem[MemoryAddresses.GenesSys] = rob.genenum;
            Senses.MakeOccurrList(rob);
        }

        public static void LogMutation(robot rob, string strmut)
        {
            if (SimOpt.SimOpts.TotRunCycle == 0)
                return;

            if (rob.LastMutDetail.Length > 100000000 / Globals.RobotsManager.TotalRobots)
                rob.LastMutDetail = "";

            rob.LastMutDetail = $"{strmut}\n{rob.LastMutDetail}";
        }

        public static void Mutate(robot rob, bool reproducing = false)
        {
            if (!rob.Mutables.EnableMutations || SimOpt.SimOpts.DisableMutations)
                return;

            var delta = rob.LastMut;

            if (reproducing)
            {
                if (rob.Mutables.CopyError.Probability > 0)
                    CopyError(rob);

                if (rob.Mutables.Insertion.Probability > 0)
                    Insertion(rob);

                if (rob.Mutables.Reversal.Probability > 0)
                    Reversal(rob);

                if (rob.Mutables.MajorDeletion.Probability > 0)
                    MajorDeletion(rob);

                if (rob.Mutables.MinorDeletion.Probability > 0)
                    MinorDeletion(rob);
            }
            else
            {
                if (rob.Mutables.PointMutation.Probability > 0)
                    PointMutation(rob);

                if (rob.Mutables.Delta.Probability > 0)
                    DeltaMut(rob);
            }

            delta = rob.LastMut - delta;

            if (SimOpt.SimOpts.EnableAutoSpeciation)
            {
                if (rob.Mutations > rob.dna.Count * (double)SimOpt.SimOpts.SpeciationGeneticDistance / 100)
                {
                    SimOpt.SimOpts.SpeciationForkInterval++;
                    var splitname = rob.FName.Split(")");
                    var robname = splitname[0].StartsWith("(") && int.TryParse(splitname[0][1..], out _) ? splitname[1] : rob.FName;

                    robname = "(" + SimOpt.SimOpts.SpeciationForkInterval + ")" + robname;

                    if (SimOpt.SimOpts.Specie.Count < 49)
                    {
                        rob.FName = robname;
                        rob.Mutations = 0;
                        MainEngine.AddSpecie(rob, false);
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
            rob.mem[MemoryAddresses.DnaLenSys] = rob.dna.Count;
            rob.mem[MemoryAddresses.GenesSys] = rob.genenum;
        }

        public static int NewSubSpecies(robot rob)
        {
            var i = Senses.SpeciesFromBot(rob);
            i.SubSpeciesCounter++; // increment the counter

            if (i.SubSpeciesCounter > 32000)
                i.SubSpeciesCounter = -32000; //wrap the counter if necessary

            return i.SubSpeciesCounter;
        }

        public static void SetDefaultLengths(MutationProbabilities changeme)
        {
            changeme.PointMutation = new MutationProbability { Mean = 3, StandardDeviation = 1 };
            changeme.Delta = new MutationProbability { Mean = 500, StandardDeviation = 150 };
            changeme.MinorDeletion = new MutationProbability { Mean = 1, StandardDeviation = 0 };
            changeme.Insertion = new MutationProbability { Mean = 1, StandardDeviation = 0 };
            changeme.CopyError = new MutationProbability { Mean = 1, StandardDeviation = 0 };
            changeme.MajorDeletion = new MutationProbability { Mean = 3, StandardDeviation = 1 };
            changeme.Reversal = new MutationProbability { Mean = 3, StandardDeviation = 1 };
            changeme.CopyErrorWhatToChange = 80;
            changeme.PointWhatToChange = 80;
        }

        private static void ChangeDna(robot rob, int nth, MutationType mutationType, int length = 1, int pointToChange = 50)
        {
            for (var t = nth; t < nth + length; t++)
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

        private static void CopyError(robot rob)
        {
            var floor = SimOpt.SimOpts.MutCurrMult * rob.dna.Count * (rob.Mutables.CopyError.Mean + rob.Mutables.CopyError.StandardDeviation) / (25 * OverTime);

            if (rob.Mutables.CopyError.Probability < floor)
                rob.Mutables.CopyError = rob.Mutables.CopyError with { Probability = floor };

            for (var t = 0; t < rob.dna.Count - 1; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.CopyError.Probability / SimOpt.SimOpts.MutCurrMult))
                    continue;

                var length = (int)Common.Gauss(rob.Mutables.CopyError.StandardDeviation, rob.Mutables.CopyError.Mean); //length

                ChangeDna(rob, t, MutationType.CopyError, length, rob.Mutables.CopyErrorWhatToChange);
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
            if (ThreadSafeRandom.Local.NextDouble() <= 1 - 1 / (100 * rob.Mutables.Delta.Probability / SimOpt.SimOpts.MutCurrMult))
                return;

            if (rob.Mutables.Delta.StandardDeviation == 0)
                rob.Mutables.Delta = rob.Mutables.Delta with { Mean = 50 };
            else if (rob.Mutables.Delta.Mean == 0)
                rob.Mutables.Delta = rob.Mutables.Delta with { Mean = 25 };

            MutationType temp;

            do
            {
                temp = (MutationType)ThreadSafeRandom.Local.Next(0, (int)MutationType.MaxType); //Botsareus 12/14/2013 Added new mutations
            } while (rob.Mutables.GetProbability(temp) <= 0);

            double newval;

            do
            {
                newval = Common.Gauss(rob.Mutables.Delta.Mean, rob.Mutables.Delta.Probability);
            } while (Math.Abs(rob.Mutables.GetProbability(temp) - newval) < 0.1 || newval <= 0);

            LogMutation(rob, $"Delta mutations changed {MutationToString(temp)} from 1 in {rob.Mutables.GetProbability(temp)} to 1 in {newval}");
            rob.Mutations++;
            rob.LastMut++;
            rob.Mutables.SetProbability(temp, newval);
        }

        private static void Insertion(robot rob)
        {
            var floor = rob.dna.Count * (rob.Mutables.Insertion.Mean + rob.Mutables.Insertion.StandardDeviation) / (5 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.Insertion.Probability < floor)
                rob.Mutables.Insertion = rob.Mutables.Insertion with { Probability = floor };

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.Insertion.Probability / SimOpt.SimOpts.MutCurrMult))
                    continue;

                if (rob.Mutables.Insertion.Mean == 0)
                    rob.Mutables.Insertion = rob.Mutables.Insertion with { Mean = 1 };

                int length;
                do
                {
                    length = (int)Common.Gauss(rob.Mutables.Insertion.StandardDeviation, rob.Mutables.Insertion.Mean);
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

            if (rob.Mutables.MajorDeletion.Probability < floor)
                rob.Mutables.MajorDeletion = rob.Mutables.MajorDeletion with { Probability = floor };

            if (rob.Mutables.MajorDeletion.Mean < 1)
                rob.Mutables.MajorDeletion = rob.Mutables.MajorDeletion with { Mean = 1 };

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.MajorDeletion.Probability / SimOpt.SimOpts.MutCurrMult))
                    continue;

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.Mutables.MajorDeletion.StandardDeviation, rob.Mutables.MajorDeletion.Mean);
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

            if (rob.Mutables.MinorDeletion.Probability < floor)
                rob.Mutables.MinorDeletion = rob.Mutables.MinorDeletion with { Probability = floor };

            if (rob.Mutables.MinorDeletion.Mean < 1)
                rob.Mutables.MinorDeletion = rob.Mutables.MajorDeletion with { Mean = 1 };

            for (var t = 0; t < rob.dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.Mutables.MinorDeletion.Probability / SimOpt.SimOpts.MutCurrMult))
                    continue;

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.Mutables.MinorDeletion.StandardDeviation, rob.Mutables.MinorDeletion.Mean);
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
                MutationType.MajorDeletion => "Major Deletion",
                MutationType.CopyError => "Copy Error",
                MutationType.Delta => "Delta Mutation",
                _ => ""
            };
        }

        private static void PointMutation(robot rob)
        {
            //assume the bot has a positive (>0) mutarray value for this

            var floor = rob.dna.Count * (rob.Mutables.PointMutation.Mean + rob.Mutables.PointMutation.StandardDeviation) / (400 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.PointMutation.Probability < floor)
                rob.Mutables.PointMutation = rob.Mutables.PointMutation with { Probability = floor };

            if (rob.age == 0 || rob.PointMutCycle < rob.age)
                PointMutWhereAndWhen(ThreadSafeRandom.Local.NextDouble(), rob);

            //Do it again in case we get two point mutations in a single cycle
            while (rob.age == rob.PointMutCycle && rob.age > 0 & rob.dna.Count > 1)
            {
                ChangeDna(rob, rob.PointMutBP, (MutationType)(Common.Gauss(rob.Mutables.PointMutation.StandardDeviation, rob.Mutables.PointMutation.Mean) % 32000), rob.Mutables.PointWhatToChange);
                PointMutWhereAndWhen(ThreadSafeRandom.Local.NextDouble(), rob);
            }
        }

        private static void PointMutWhereAndWhen(double randVal, robot rob)
        {
            if (rob.dna.Count == 1)
                return;

            var mutationRate = rob.Mutables.PointMutation.Probability / SimOpt.SimOpts.MutCurrMult;

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
            var floor = rob.dna.Count * (rob.Mutables.Reversal.Mean + rob.Mutables.Reversal.StandardDeviation) / (105 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.Mutables.Reversal.Probability < floor)
                rob.Mutables.Reversal = rob.Mutables.Reversal with { Probability = floor };

            for (var t = 0; t < rob.dna.Count - 1; t++)
            {
                if (!(ThreadSafeRandom.Local.NextDouble() < 1 / (rob.Mutables.Reversal.Probability / SimOpt.SimOpts.MutCurrMult)))
                    continue;

                if (rob.Mutables.Reversal.Mean < 2)
                    rob.Mutables.Reversal = rob.Mutables.Reversal with { Mean = 2 };

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.Mutables.Reversal.StandardDeviation, rob.Mutables.Reversal.Mean);
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
    }
}
