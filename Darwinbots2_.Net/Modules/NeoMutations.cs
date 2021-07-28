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

        public static void DeleteGene(Robot rob, int g)
        {
            if (g <= 0 || g > rob.NumberOfGenes)
                return;

            DeleteSpecificGene(rob.Dna, g);
            rob.NumberOfGenes = DnaManipulations.CountGenes(rob.Dna);
            rob.Memory[MemoryAddresses.DnaLenSys] = rob.Dna.Count;
            rob.Memory[MemoryAddresses.GenesSys] = rob.NumberOfGenes;
            Senses.MakeOccurrList(rob);
        }

        public static void Mutate(IRobotManager robotManager, Robot rob, bool reproducing = false)
        {
            if (!rob.MutationProbabilities.EnableMutations || SimOpt.SimOpts.DisableMutations)
                return;

            var delta = rob.LastMutation;

            if (reproducing)
            {
                if (rob.MutationProbabilities.CopyError.Probability > 0)
                    CopyError(rob);

                if (rob.MutationProbabilities.Insertion.Probability > 0)
                    Insertion(rob);

                if (rob.MutationProbabilities.Reversal.Probability > 0)
                    Reversal(rob);

                if (rob.MutationProbabilities.MajorDeletion.Probability > 0)
                    MajorDeletion(rob);

                if (rob.MutationProbabilities.MinorDeletion.Probability > 0)
                    MinorDeletion(rob);
            }
            else
            {
                if (rob.MutationProbabilities.PointMutation.Probability > 0)
                    PointMutation(rob);

                if (rob.MutationProbabilities.Delta.Probability > 0)
                    DeltaMut(rob);
            }

            delta = rob.LastMutation - delta;

            if (SimOpt.SimOpts.EnableAutoSpeciation)
            {
                if (rob.Mutations > rob.Dna.Count * (double)SimOpt.SimOpts.SpeciationGeneticDistance / 100)
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

            if (rob.LastMutation > 32000)
                rob.LastMutation = 32000;

            if (delta <= 0)
                return;

            // The bot has mutated.
            rob.GenMut -= rob.LastMutation;
            if (rob.GenMut < 0)
                rob.GenMut = 0;

            MutateColours(rob, delta);
            rob.SubSpecies = NewSubSpecies(rob);
            rob.NumberOfGenes = DnaManipulations.CountGenes(rob.Dna);
            rob.Memory[MemoryAddresses.DnaLenSys] = rob.Dna.Count;
            rob.Memory[MemoryAddresses.GenesSys] = rob.NumberOfGenes;
        }

        public static int NewSubSpecies(Robot rob)
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

        private static void ChangeDna(Robot rob, int nth, MutationType mutationType, int length = 1, int pointToChange = 50)
        {
            for (var t = nth; t < nth + length; t++)
            {
                //if length is 1, it's only one bp we're mutating, remember?
                if (t >= rob.Dna.Count || rob.Dna[t].Type == 10)
                    return; //don't mutate end either

                if (ThreadSafeRandom.Local.Next(0, 99) < pointToChange)
                {
                    if (rob.Dna[t].Value != 0 && mutationType == MutationType.Insertion)
                        rob.Dna[t] = new DnaBlock(rob.Dna[t].Type, (int)Common.Gauss(500));

                    var old = rob.Dna[t].Value;

                    if (rob.Dna[t].Type is 0 or 1)
                    {
                        do
                        {
                            if (Math.Abs(old) <= 1000)
                                rob.Dna[t] = new DnaBlock(rob.Dna[t].Type, ThreadSafeRandom.Local.Next(0, 2) == 0 ? (int)Common.Gauss(94, rob.Dna[t].Value) : (int)Common.Gauss(7, rob.Dna[t].Value));
                            else
                                rob.Dna[t] = new DnaBlock(rob.Dna[t].Type, (int)Common.Gauss(old / 10.0, rob.Dna[t].Value)); //for very large numbers scale gauss
                        } while (rob.Dna[t].Value == old);

                        rob.Mutations++;
                        rob.LastMutation++;

                        rob.LogMutation($"{MutationToString(mutationType)} changed {DnaTokenizing.TipoDetok(rob.Dna[t].Type)} from {old} to {rob.Dna[t].Value} at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
                    }
                    else
                    {
                        var bp = new DnaBlock
                        {
                            Type = rob.Dna[t].Type
                        };

                        var max = 0;

                        do
                        {
                            max++;
                            bp = new DnaBlock(bp.Type, max);
                        } while (DnaTokenizing.Parse(bp) != "");

                        max--;

                        if (max <= 1)
                            return;

                        do
                        {
                            rob.Dna[t] = new DnaBlock(rob.Dna[t].Type, ThreadSafeRandom.Local.Next(1, max));
                        } while (rob.Dna[t].Value != old);

                        bp = new DnaBlock(rob.Dna[t].Type, old);

                        var name = DnaTokenizing.Parse(rob.Dna[t]);
                        var oldName = DnaTokenizing.Parse(bp);

                        rob.Mutations++;
                        rob.LastMutation++;

                        rob.LogMutation($"{MutationToString(mutationType)} changed value of {DnaTokenizing.TipoDetok(rob.Dna[t].Type)} from {oldName} to {name} at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
                    }
                }
                else
                {
                    var bp = new DnaBlock
                    {
                        Type = rob.Dna[t].Type,
                        Value = rob.Dna[t].Value
                    };
                    do
                    {
                        rob.Dna[t] = new DnaBlock(ThreadSafeRandom.Local.Next(0, 20), rob.Dna[t].Value);
                    } while (rob.Dna[t].Type == bp.Type || DnaTokenizing.TipoDetok(rob.Dna[t].Type) == "");

                    var max = 0;
                    if (rob.Dna[t].Type >= 2)
                    {
                        do
                        {
                            max++;
                            rob.Dna[t] = new DnaBlock(rob.Dna[t].Type, max);
                        } while (DnaTokenizing.Parse(rob.Dna[t]) != "");

                        max--;

                        if (max <= 1)
                            return;

                        rob.Dna[t] = new DnaBlock(rob.Dna[t].Type, (Math.Abs(bp.Value) - 1) % max + 1);
                        if (rob.Dna[t].Value == 0)
                            rob.Dna[t] = new DnaBlock(rob.Dna[t].Type, 1);
                    }

                    var name = DnaTokenizing.Parse(rob.Dna[t]);
                    var oldName = DnaTokenizing.Parse(bp);
                    rob.Mutations++;
                    rob.LastMutation++;

                    rob.LogMutation($"{MutationToString(mutationType)} changed the {DnaTokenizing.TipoDetok(bp.Type)}: {oldName} to the {DnaTokenizing.TipoDetok(rob.Dna[t].Type)} : {name} at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
                }
            }
        }

        private static void CopyError(Robot rob)
        {
            var floor = SimOpt.SimOpts.MutCurrMult * rob.Dna.Count * (rob.MutationProbabilities.CopyError.Mean + rob.MutationProbabilities.CopyError.StandardDeviation) / (25 * OverTime);

            if (rob.MutationProbabilities.CopyError.Probability < floor)
                rob.MutationProbabilities.CopyError = rob.MutationProbabilities.CopyError with { Probability = floor };

            for (var t = 0; t < rob.Dna.Count - 1; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.MutationProbabilities.CopyError.Probability / SimOpt.SimOpts.MutCurrMult))
                    continue;

                var length = (int)Common.Gauss(rob.MutationProbabilities.CopyError.StandardDeviation, rob.MutationProbabilities.CopyError.Mean); //length

                ChangeDna(rob, t, MutationType.CopyError, length, rob.MutationProbabilities.CopyErrorWhatToChange);
            }
        }

        private static void DeleteSpecificGene(List<DnaBlock> dna, int k)
        {
            var i = DnaManipulations.GenePosition(dna, k);
            if (i < 0)
                return;

            var f = DnaManipulations.GeneEnd(dna, i);
            dna.RemoveRange(i, f - i); // EricL Added +1
        }

        private static void DeltaMut(Robot rob)
        {
            if (ThreadSafeRandom.Local.NextDouble() <= 1 - 1 / (100 * rob.MutationProbabilities.Delta.Probability / SimOpt.SimOpts.MutCurrMult))
                return;

            if (rob.MutationProbabilities.Delta.StandardDeviation == 0)
                rob.MutationProbabilities.Delta = rob.MutationProbabilities.Delta with { Mean = 50 };
            else if (rob.MutationProbabilities.Delta.Mean == 0)
                rob.MutationProbabilities.Delta = rob.MutationProbabilities.Delta with { Mean = 25 };

            MutationType temp;

            do
            {
                temp = (MutationType)ThreadSafeRandom.Local.Next(0, (int)MutationType.MaxType); //Botsareus 12/14/2013 Added new mutations
            } while (rob.MutationProbabilities.GetProbability(temp) <= 0);

            double newval;

            do
            {
                newval = Common.Gauss(rob.MutationProbabilities.Delta.Mean, rob.MutationProbabilities.Delta.Probability);
            } while (Math.Abs(rob.MutationProbabilities.GetProbability(temp) - newval) < 0.1 || newval <= 0);

            rob.LogMutation($"Delta mutations changed {MutationToString(temp)} from 1 in {rob.MutationProbabilities.GetProbability(temp)} to 1 in {newval}");
            rob.Mutations++;
            rob.LastMutation++;
            rob.MutationProbabilities.SetProbability(temp, newval);
        }

        private static void Insertion(Robot rob)
        {
            var floor = rob.Dna.Count * (rob.MutationProbabilities.Insertion.Mean + rob.MutationProbabilities.Insertion.StandardDeviation) / (5 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.MutationProbabilities.Insertion.Probability < floor)
                rob.MutationProbabilities.Insertion = rob.MutationProbabilities.Insertion with { Probability = floor };

            for (var t = 0; t < rob.Dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.MutationProbabilities.Insertion.Probability / SimOpt.SimOpts.MutCurrMult))
                    continue;

                if (rob.MutationProbabilities.Insertion.Mean == 0)
                    rob.MutationProbabilities.Insertion = rob.MutationProbabilities.Insertion with { Mean = 1 };

                int length;
                do
                {
                    length = (int)Common.Gauss(rob.MutationProbabilities.Insertion.StandardDeviation, rob.MutationProbabilities.Insertion.Mean);
                } while (length <= 0);

                if (rob.Dna.Count + length > 32000)
                    return;

                for (var i = 0; i < length; i++)
                {
                    rob.Dna.Insert(t + 1 + i, new DnaBlock
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

        private static void MajorDeletion(Robot rob)
        {
            var floor = rob.Dna.Count / (2.5 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.MutationProbabilities.MajorDeletion.Probability < floor)
                rob.MutationProbabilities.MajorDeletion = rob.MutationProbabilities.MajorDeletion with { Probability = floor };

            if (rob.MutationProbabilities.MajorDeletion.Mean < 1)
                rob.MutationProbabilities.MajorDeletion = rob.MutationProbabilities.MajorDeletion with { Mean = 1 };

            for (var t = 0; t < rob.Dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.MutationProbabilities.MajorDeletion.Probability / SimOpt.SimOpts.MutCurrMult))
                    continue;

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.MutationProbabilities.MajorDeletion.StandardDeviation, rob.MutationProbabilities.MajorDeletion.Mean);
                } while (length <= 0);

                if (t + length > rob.Dna.Count)
                    length = rob.Dna.Count - t;

                if (length <= 0)
                    return;

                rob.Dna.RemoveRange(t, length);

                rob.Mutations++;
                rob.LastMutation++;
                rob.LogMutation($"Major Deletion deleted a run of {length} bps at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
            }
        }

        private static void MinorDeletion(Robot rob)
        {
            var floor = rob.Dna.Count / (2.5 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.MutationProbabilities.MinorDeletion.Probability < floor)
                rob.MutationProbabilities.MinorDeletion = rob.MutationProbabilities.MinorDeletion with { Probability = floor };

            if (rob.MutationProbabilities.MinorDeletion.Mean < 1)
                rob.MutationProbabilities.MinorDeletion = rob.MutationProbabilities.MajorDeletion with { Mean = 1 };

            for (var t = 0; t < rob.Dna.Count; t++)
            {
                if (ThreadSafeRandom.Local.NextDouble() >= 1 / (rob.MutationProbabilities.MinorDeletion.Probability / SimOpt.SimOpts.MutCurrMult))
                    continue;

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.MutationProbabilities.MinorDeletion.StandardDeviation, rob.MutationProbabilities.MinorDeletion.Mean);
                } while (length <= 0);

                if (t + length > rob.Dna.Count)
                    length = rob.Dna.Count - t;

                if (length <= 0)
                    return;

                rob.Dna.RemoveRange(t, length);

                rob.Mutations++;
                rob.LastMutation++;
                rob.LogMutation($"Minor Deletion deleted a run of {length} bps at position {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
            }
        }

        private static void MutateColours(Robot rob, int a)
        {
            var b = (int)rob.Color.R;
            var g = (int)rob.Color.G;
            var r = (int)rob.Color.B;

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

            rob.Color = Color.FromRgb((byte)r, (byte)g, (byte)b);
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

        private static void PointMutation(Robot rob)
        {
            //assume the bot has a positive (>0) mutarray value for this

            var floor = rob.Dna.Count * (rob.MutationProbabilities.PointMutation.Mean + rob.MutationProbabilities.PointMutation.StandardDeviation) / (400 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.MutationProbabilities.PointMutation.Probability < floor)
                rob.MutationProbabilities.PointMutation = rob.MutationProbabilities.PointMutation with { Probability = floor };

            if (rob.Age == 0 || rob.PointMutationCycle < rob.Age)
                PointMutWhereAndWhen(ThreadSafeRandom.Local.NextDouble(), rob);

            //Do it again in case we get two point mutations in a single cycle
            while (rob.Age == rob.PointMutationCycle && rob.Age > 0 & rob.Dna.Count > 1)
            {
                ChangeDna(rob, rob.PointMutationBasePair, (MutationType)(Common.Gauss(rob.MutationProbabilities.PointMutation.StandardDeviation, rob.MutationProbabilities.PointMutation.Mean) % 32000), rob.MutationProbabilities.PointWhatToChange);
                PointMutWhereAndWhen(ThreadSafeRandom.Local.NextDouble(), rob);
            }
        }

        private static void PointMutWhereAndWhen(double randVal, Robot rob)
        {
            if (rob.Dna.Count == 1)
                return;

            var mutationRate = rob.MutationProbabilities.PointMutation.Probability / SimOpt.SimOpts.MutCurrMult;

            //Here we test to make sure the probability of a point mutation isn't crazy high.
            //A value of 1 is the probability of mutating every base pair every 1000 cycles
            //Lets not let it get lower than 1 shall we?
            if (mutationRate is < 1 and > 0)
                mutationRate = 1;

            var result = Math.Log(1 - randVal, 1 - 1 / (1000 * mutationRate));

            while (result > 1800000000)
                result -= 1800000000;

            rob.PointMutationBasePair = (int)(result % (rob.Dna.Count - 1)) + 1;
            rob.PointMutationCycle = rob.Age + (int)result / (rob.Dna.Count - 1);
        }

        private static void Reversal(Robot rob)
        {
            var floor = rob.Dna.Count * (rob.MutationProbabilities.Reversal.Mean + rob.MutationProbabilities.Reversal.StandardDeviation) / (105 * OverTime) * SimOpt.SimOpts.MutCurrMult;

            if (rob.MutationProbabilities.Reversal.Probability < floor)
                rob.MutationProbabilities.Reversal = rob.MutationProbabilities.Reversal with { Probability = floor };

            for (var t = 0; t < rob.Dna.Count - 1; t++)
            {
                if (!(ThreadSafeRandom.Local.NextDouble() < 1 / (rob.MutationProbabilities.Reversal.Probability / SimOpt.SimOpts.MutCurrMult)))
                    continue;

                if (rob.MutationProbabilities.Reversal.Mean < 2)
                    rob.MutationProbabilities.Reversal = rob.MutationProbabilities.Reversal with { Mean = 2 };

                int length;

                do
                {
                    length = (int)Common.Gauss(rob.MutationProbabilities.Reversal.StandardDeviation, rob.MutationProbabilities.Reversal.Mean);
                } while (length <= 0);

                length /= 2; //be sure we go an even amount to either side

                if (t - length < 1)
                    length = t - 1;

                if (t + length > rob.Dna.Count - 1)
                    length = rob.Dna.Count - 1 - t;

                if (length <= 0)
                    continue;

                var second = 0;

                for (var counter = t - length; counter < t - 1; counter++)
                {
                    var tempBlock = rob.Dna[counter];
                    rob.Dna[counter] = rob.Dna[t + length - second];
                    rob.Dna[t + length - second] = tempBlock;
                    second++;
                }

                rob.Mutations++;
                rob.LastMutation++;

                rob.LogMutation($"Reversal of {length * 2 + 1} bps centered at {t} during cycle {SimOpt.SimOpts.TotRunCycle}");
            }
        }
    }
}
