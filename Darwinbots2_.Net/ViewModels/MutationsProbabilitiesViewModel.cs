using DarwinBots.Model;
using DarwinBots.Modules;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PostSharp.Patterns.Model;
using System;
using System.Windows.Input;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged(ExcludeExplicitProperties = true)]
    public class MutationsProbabilitiesViewModel : ObservableObject
    {
        // TODO : Save mutations to a mrates file.
        // TODO : Work out a more efficient way of changing the mutation values.  Probably need a mutable version for this use.

        private bool _amplificationSelected;
        private bool _copyError2Selected;
        private bool _copyErrorSelected;
        private bool _deltaMutationSelected;
        private bool _insertionSelected;
        private bool _majorDeletionSelected;
        private bool _minorDeletionSelected;
        private MutationProbabilities _mutationProbabilities;
        private bool _point2Selected;
        private bool _pointMutationSelected;
        private bool _reversalSelected;
        private bool _translocationSelected;

        public MutationsProbabilitiesViewModel()
        {
            SetDefaultRatesCommand = new RelayCommand(SetDefaultRates);
            PointMutationSelected = true;
        }

        public bool AmplificationSelected
        {
            get => _amplificationSelected;
            set
            {
                if (SetProperty(ref _amplificationSelected, value) && _amplificationSelected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    PointMutationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = false;
                    Explanation = "A series of bp are replicated and inserted in another place in the genome.";
                    Unit = "per bp per copy";
                    GaussLabel = "Length";

                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool ApplyChangesGlobally { get; set; }

        public double ChancePerBasePair
        {
            get
            {
                var pNone = AntiProb(_mutationProbabilities.CopyError.Probability)
                           * AntiProb(_mutationProbabilities.Delta.Probability)
                           * AntiProb(_mutationProbabilities.Insertion.Probability)
                           * AntiProb(_mutationProbabilities.MajorDeletion.Probability)
                           * AntiProb(_mutationProbabilities.MinorDeletion.Probability)
                           * AntiProb(_mutationProbabilities.PointMutation.Probability)
                           * AntiProb(_mutationProbabilities.Reversal.Probability);

                var pSome = 1 - pNone;

                return pSome == 0 ? double.PositiveInfinity : 1 / pSome;
            }
            set { }
        }

        public double ChancePerUnit
        {
            get => _mutationProbabilities.GetProbability(GetCurrentMutation());
            set
            {
                _mutationProbabilities.SetProbability(GetCurrentMutation(), value);
                OnPropertyChanged(string.Empty);
            }
        }

        public bool CopyError2Selected
        {
            get => _copyError2Selected;
            set
            {
                if (SetProperty(ref _copyError2Selected, value) && _copyError2Selected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    PointMutationSelected = false;

                    EnableGauss = false;
                    EnableTypeSlider = false;
                    Explanation = "Similar to copy error, but always changes to an existing sysvar, *sysvar, or special values if followed by .shoot store or .focuseye store.";
                    Unit = "per bp per copy";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool CopyErrorSelected
        {
            get => _copyErrorSelected;
            set
            {
                if (SetProperty(ref _copyErrorSelected, value) && _copyErrorSelected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    PointMutationSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = true;
                    Explanation = "Similar to point mutations, but these occur during DNA replication for reproduction or viruses.  A small series (usually 1 bp) is changed in either parent or child.";
                    Unit = "per bp per copy";
                    GaussLabel = "Length";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public double CustomGaussLower
        {
            get => CustomGaussMean - CustomGaussStdDev * 2;
            set
            {
                if (Math.Abs(CustomGaussLower - value) < 0.01) return;

                var lower = value;
                var upper = CustomGaussUpper;

                CustomGaussMean = (lower + upper) / 2;
                CustomGaussStdDev = (upper - lower) / 4;

                OnPropertyChanged(string.Empty);
            }
        }

        public double CustomGaussMean
        {
            get => _mutationProbabilities.GetMean(GetCurrentMutation());
            set
            {
                if (Math.Abs(_mutationProbabilities.GetMean(GetCurrentMutation()) - value) < 0.01) return;

                var upper = CustomGaussUpper;
                var lower = CustomGaussLower;
                var temp = (upper - lower) / 2;

                _mutationProbabilities.SetMean(GetCurrentMutation(), value);

                OnPropertyChanged(string.Empty);
            }
        }

        public double CustomGaussStdDev
        {
            get => _mutationProbabilities.GetStandardDeviation(GetCurrentMutation());
            set
            {
                if (Math.Abs(_mutationProbabilities.GetStandardDeviation(GetCurrentMutation()) - value) < 0.01)
                    return;

                _mutationProbabilities.SetStandardDeviation(GetCurrentMutation(), value);

                OnPropertyChanged(string.Empty);
            }
        }

        public double CustomGaussUpper
        {
            get => CustomGaussMean + CustomGaussStdDev * 2;
            set
            {
                if (Math.Abs(CustomGaussUpper - value) < 0.01) return;

                var lower = CustomGaussLower;
                var upper = value;

                CustomGaussMean = (lower + upper) / 2;
                CustomGaussStdDev = (upper - lower) / 4;

                OnPropertyChanged(string.Empty);
            }
        }

        public bool DeltaMutationSelected
        {
            get => _deltaMutationSelected;
            set
            {
                if (SetProperty(ref _deltaMutationSelected, value) && _deltaMutationSelected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    PointMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = false;
                    Explanation = "The mutation rates of a bot are allowed to change slowly over time.  This change in mutation rates can include Delta EnableMutations as well.  Theoretically, it may be possible for a bot to figure its own optimal mutation rate.";
                    Unit = "00 per cycle";
                    GaussLabel = "Standard Deviation";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool EnableGauss { get; set; }
        public bool EnableTypeSlider { get; set; }
        public string Explanation { get; set; }
        public string GaussLabel { get; private set; }

        public bool InsertionSelected
        {
            get => _insertionSelected;
            set
            {
                if (SetProperty(ref _insertionSelected, value) && _insertionSelected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    PointMutationSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = false;
                    Explanation = "A run of random bp are inserted into the genome.  The size of this run should be fairly small.";
                    Unit = "per bp per copy";
                    GaussLabel = "Length";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool IsEnabled
        {
            get => _mutationProbabilities.GetProbability(GetCurrentMutation()) > 0;
            set
            {
                _mutationProbabilities.SetProbability(GetCurrentMutation(), value
                    ? Math.Abs(_mutationProbabilities.GetProbability(GetCurrentMutation()))
                    : -Math.Abs(_mutationProbabilities.GetProbability(GetCurrentMutation())));
                OnPropertyChanged();
            }
        }

        public bool MajorDeletionSelected
        {
            get => _majorDeletionSelected;
            set
            {
                if (SetProperty(ref _majorDeletionSelected, value) && _majorDeletionSelected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    PointMutationSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = false;
                    Explanation = "A relatively long series of bp are deleted from the genome.  This can be quite disasterous, so set probabilities wisely.";
                    Unit = "per bp per copy";
                    GaussLabel = "Length";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool MinorDeletionSelected
        {
            get => _minorDeletionSelected;
            set
            {
                if (SetProperty(ref _minorDeletionSelected, value) && _minorDeletionSelected)
                {
                    PointMutationSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = false;
                    Explanation = "A small series of bp are deleted from the genome.";
                    Unit = "per bp per copy";
                    GaussLabel = "Length";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool Point2Selected
        {
            get => _point2Selected;
            set
            {
                if (SetProperty(ref _point2Selected, value) && _point2Selected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    PointMutationSelected = false;
                    CopyError2Selected = false;

                    EnableGauss = false;
                    EnableTypeSlider = false;
                    Explanation = "Note: The length of this mutation is always 1, but the rate is multiplied by the Gaussen Length of Point Mutation.  Similar to point mutations, but always changes to an existing sysvar, *sysvar, or special values if followed by .shoot store or .focuseye store.  The algorithm is also designed to introduce more stores. Should allow for evolving a zero-bot the same as a random-bot.";
                    Unit = "per bp per cycle";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool PointMutationSelected
        {
            get => _pointMutationSelected;
            set
            {
                if (SetProperty(ref _pointMutationSelected, value) && _pointMutationSelected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = true;
                    Explanation = "A small scale mutation that causes a small series of commands to change.  It may occur at any time in a bots life.  Represents environmental mutations such as UV light or an error in DNA maintenance.  Length should be kept relatively small to mirror real life (~1 bp).  Unlike other mutations, point mutation chances are given as 1 in X per bp per kilocycles, so they occur quite independantly of reproduction rate.  To find the liklihood of at least one mutation over any length of time: 1/(1 - (1-1/X)^(how many cycles)) = Y, as in 1 chance in Y per that many cycles.  Finding the probable number of mutations in that range is more difficult.  (Lookup Negative Binomial Distribution).";
                    Unit = "per bp per cycle";
                    GaussLabel = "Length";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool ReversalSelected
        {
            get => _reversalSelected;
            set
            {
                if (SetProperty(ref _reversalSelected, value) && _reversalSelected)
                {
                    MinorDeletionSelected = false;
                    PointMutationSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    TranslocationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = false;
                    Explanation = "A series of bp are reversed in the genome.  For example, '2 3 > or' becomes 'or > 3 2'.  Length of reversal should be >= 2.";
                    Unit = "per bp per copy";
                    GaussLabel = "Length";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public ICommand SetDefaultRatesCommand { get; }
        public bool ShowDeltaMutation { get; set; }

        public bool TranslocationSelected
        {
            get => _translocationSelected;
            set
            {
                if (SetProperty(ref _translocationSelected, value) && _translocationSelected)
                {
                    MinorDeletionSelected = false;
                    ReversalSelected = false;
                    InsertionSelected = false;
                    AmplificationSelected = false;
                    MajorDeletionSelected = false;
                    CopyErrorSelected = false;
                    DeltaMutationSelected = false;
                    PointMutationSelected = false;
                    Point2Selected = false;
                    CopyError2Selected = false;

                    EnableGauss = true;
                    EnableTypeSlider = false;
                    Explanation = "Tranlocation moves a segment of DNA from one location to another in the genome.";
                    Unit = "per bp per copy";
                    GaussLabel = "Length";
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public int TypeValueRatio
        {
            get
            {
                if (CopyErrorSelected)
                    return _mutationProbabilities.CopyErrorWhatToChange;
                if (PointMutationSelected)
                    return _mutationProbabilities.PointWhatToChange;

                return 0;
            }
            set
            {
                if (CopyErrorSelected)
                    _mutationProbabilities.CopyErrorWhatToChange = value;
                if (PointMutationSelected)
                    _mutationProbabilities.PointWhatToChange = value;
            }
        }

        public string Unit { get; private set; }

        public void LoadFromProbabilities(MutationProbabilities probs)
        {
            _mutationProbabilities = new MutationProbabilities
            {
                CopyErrorWhatToChange = probs.CopyErrorWhatToChange,
                EnableMutations = probs.EnableMutations,
                PointWhatToChange = probs.PointWhatToChange,
                CopyError = probs.CopyError,
                Insertion = probs.Insertion,
                MajorDeletion = probs.MajorDeletion,
                Delta = probs.Delta,
                PointMutation = probs.PointMutation,
                MinorDeletion = probs.MinorDeletion,
                Reversal = probs.Reversal
            };

            ShowDeltaMutation = false;
        }

        public void SaveToProbabilities(MutationProbabilities probs)
        {
            probs.CopyErrorWhatToChange = _mutationProbabilities.CopyErrorWhatToChange;
            probs.EnableMutations = _mutationProbabilities.EnableMutations;
            probs.PointWhatToChange = _mutationProbabilities.PointWhatToChange;
            probs.CopyError = _mutationProbabilities.CopyError;
            probs.Insertion = _mutationProbabilities.Insertion;
            probs.MajorDeletion = _mutationProbabilities.MajorDeletion;
            probs.Delta = _mutationProbabilities.Delta;
            probs.PointMutation = _mutationProbabilities.PointMutation;
            probs.MinorDeletion = _mutationProbabilities.MinorDeletion;
            probs.Reversal = _mutationProbabilities.Reversal;
        }

        private static double AntiProb(double a)
        {
            return a <= 0 ? 1 : 1 - 1 / a;
        }

        private MutationType GetCurrentMutation()
        {
            if (CopyErrorSelected)
                return MutationType.CopyError;
            if (DeltaMutationSelected)
                return MutationType.Delta;
            if (InsertionSelected)
                return MutationType.Insertion;
            if (MajorDeletionSelected)
                return MutationType.MajorDeletion;
            if (MinorDeletionSelected)
                return MutationType.MinorDeletion;
            if (PointMutationSelected)
                return MutationType.PointMutation;
            return MutationType.Reversal;
        }

        private void SetDefaultRates()
        {
            _mutationProbabilities.ResetToDefault();
            OnPropertyChanged(string.Empty);
        }
    }
}
