﻿using DarwinBots.Modules;

namespace DarwinBots.Model
{
    public record MutationProbability
    {
        public double Mean { get; init; }
        public double Probability { get; init; }
        public double StandardDeviation { get; init; }
    }

    public class MutationProbabilities
    {
        public MutationProbability CopyError { get; set; }

        public int CopyErrorWhatToChange { get; set; }

        public MutationProbability Delta { get; set; }

        public bool EnableMutations { get; set; }
        public MutationProbability Insertion { get; set; }

        public MutationProbability MajorDeletion { get; set; }

        public MutationProbability MinorDeletion { get; set; }
        public MutationProbability PointMutation { get; set; }

        public int PointWhatToChange { get; set; }

        public MutationProbability Reversal { get; set; }

        public double GetMean(MutationType type)
        {
            return type switch
            {
                MutationType.PointMutation => PointMutation.Mean,
                MutationType.MinorDeletion => MinorDeletion.Mean,
                MutationType.Reversal => Reversal.Mean,
                MutationType.Insertion => Insertion.Mean,
                MutationType.MajorDeletion => MajorDeletion.Mean,
                MutationType.CopyError => CopyError.Mean,
                MutationType.Delta => Delta.Mean,
                _ => 0,
            };
        }

        public double GetProbability(MutationType type)
        {
            return type switch
            {
                MutationType.PointMutation => PointMutation.Probability,
                MutationType.MinorDeletion => MinorDeletion.Probability,
                MutationType.Reversal => Reversal.Probability,
                MutationType.Insertion => Insertion.Probability,
                MutationType.MajorDeletion => MajorDeletion.Probability,
                MutationType.CopyError => CopyError.Probability,
                MutationType.Delta => Delta.Probability,
                _ => 0,
            };
        }

        public double GetStandardDeviation(MutationType type)
        {
            return type switch
            {
                MutationType.PointMutation => PointMutation.StandardDeviation,
                MutationType.MinorDeletion => MinorDeletion.StandardDeviation,
                MutationType.Reversal => Reversal.StandardDeviation,
                MutationType.Insertion => Insertion.StandardDeviation,
                MutationType.MajorDeletion => MajorDeletion.StandardDeviation,
                MutationType.CopyError => CopyError.StandardDeviation,
                MutationType.Delta => Delta.StandardDeviation,
                _ => 0,
            };
        }

        public void ResetToDefault()
        {
            CopyError = new MutationProbability
            {
                Probability = 500,
                Mean = 1,
                StandardDeviation = 0
            };
            Delta = new MutationProbability
            {
                Probability = 500,
                Mean = 1,
                StandardDeviation = 0
            };
            Insertion = new MutationProbability
            {
                Probability = 500,
                Mean = 1,
                StandardDeviation = 0
            };
            MajorDeletion = new MutationProbability
            {
                Probability = 500,
                Mean = 1,
                StandardDeviation = 0
            };
            MinorDeletion = new MutationProbability
            {
                Probability = 500,
                Mean = 1,
                StandardDeviation = 0
            };
            PointMutation = new MutationProbability
            {
                Probability = 500,
                Mean = 1,
                StandardDeviation = 0
            };
            Reversal = new MutationProbability
            {
                Probability = 500,
                Mean = 1,
                StandardDeviation = 0
            };

            NeoMutations.SetDefaultLengths(this);
        }

        public void SetMean(MutationType type, double value)
        {
            switch (type)
            {
                case MutationType.PointMutation:
                    PointMutation = PointMutation with { Mean = value };
                    break;

                case MutationType.MinorDeletion:
                    MinorDeletion = MinorDeletion with { Mean = value };
                    break;

                case MutationType.Reversal:
                    Reversal = Reversal with { Mean = value };
                    break;

                case MutationType.Insertion:
                    Insertion = Insertion with { Mean = value };
                    break;

                case MutationType.MajorDeletion:
                    MajorDeletion = MajorDeletion with { Mean = value };
                    break;

                case MutationType.CopyError:
                    CopyError = CopyError with { Mean = value };
                    break;

                case MutationType.Delta:
                    Delta = Delta with { Mean = value };
                    break;
            }
        }

        public void SetProbability(MutationType type, double value)
        {
            switch (type)
            {
                case MutationType.PointMutation:
                    PointMutation = PointMutation with { Probability = value };
                    break;

                case MutationType.MinorDeletion:
                    MinorDeletion = MinorDeletion with { Probability = value };
                    break;

                case MutationType.Reversal:
                    Reversal = Reversal with { Probability = value };
                    break;

                case MutationType.Insertion:
                    Insertion = Insertion with { Probability = value };
                    break;

                case MutationType.MajorDeletion:
                    MajorDeletion = MajorDeletion with { Probability = value };
                    break;

                case MutationType.CopyError:
                    CopyError = CopyError with { Probability = value };
                    break;

                case MutationType.Delta:
                    Delta = Delta with { Probability = value };
                    break;
            }
        }

        public void SetStandardDeviation(MutationType type, double value)
        {
            switch (type)
            {
                case MutationType.PointMutation:
                    PointMutation = PointMutation with { StandardDeviation = value };
                    break;

                case MutationType.MinorDeletion:
                    MinorDeletion = MinorDeletion with { StandardDeviation = value };
                    break;

                case MutationType.Reversal:
                    Reversal = Reversal with { StandardDeviation = value };
                    break;

                case MutationType.Insertion:
                    Insertion = Insertion with { StandardDeviation = value };
                    break;

                case MutationType.MajorDeletion:
                    MajorDeletion = MajorDeletion with { StandardDeviation = value };
                    break;

                case MutationType.CopyError:
                    CopyError = CopyError with { StandardDeviation = value };
                    break;

                case MutationType.Delta:
                    Delta = Delta with { StandardDeviation = value };
                    break;
            }
        }
    }
}
