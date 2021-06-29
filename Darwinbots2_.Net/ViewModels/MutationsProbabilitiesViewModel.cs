using DarwinBots.Model;
using DarwinBots.Modules;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PostSharp.Patterns.Model;
using System;
using System.Windows.Input;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged(ExcludeExplicitProperties = true)]
    public class MutationsProbabilitiesViewModel : ViewModelBase
    {
        // TODO : Save mutations to a mrates file.

        private bool _amplificationSelected;
        private bool _copyError2Selected;
        private bool _copyErrorSelected;
        private double _customGaussLower;
        private double _customGaussUpper;
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
        }

        public bool AmplificationSelected
        {
            get => _amplificationSelected;
            set
            {
                if (value == _amplificationSelected)
                    return;

                _amplificationSelected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool ApplyChangesGlobally { get; set; }
        public double ChancePerBasePair { get; set; }

        public double ChancePerUnit
        {
            get => _mutationProbabilities.mutarray[GetCurrentMutation()];
            set
            {
                _mutationProbabilities.mutarray[GetCurrentMutation()] = value;
                RaisePropertyChanged();

                var pNone = AntiProb(_mutationProbabilities.mutarray[NeoMutations.AmplificationUP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.CE2UP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.CopyErrorUP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.DeltaUP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.InsertionUP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.MajorDeletionUP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.MinorDeletionUP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.P2UP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.PointUP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.ReversalUP])
                            * AntiProb(_mutationProbabilities.mutarray[NeoMutations.TranslocationUP]);

                var pSome = 1 - pNone;

                ChancePerBasePair = pSome == 0 ? double.PositiveInfinity : 1 / pSome;
            }
        }

        public bool CopyError2Selected
        {
            get => _copyError2Selected;
            set
            {
                if (value == _copyError2Selected)
                    return;

                _copyError2Selected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool CopyErrorSelected
        {
            get => _copyErrorSelected;
            set
            {
                if (value == _copyErrorSelected)
                    return;

                _copyErrorSelected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public double CustomGaussLower
        {
            get => _customGaussLower;
            set
            {
                if (_customGaussLower == value) return;

                _customGaussLower = value;

                CustomGaussMean = (CustomGaussLower + CustomGaussUpper) / 2;
                CustomGaussStdDev = (CustomGaussUpper - CustomGaussLower) / 4;

                RaisePropertyChanged(string.Empty);
            }
        }

        public double CustomGaussMean
        {
            get => _mutationProbabilities.Mean[GetCurrentMutation()];
            set
            {
                if (_mutationProbabilities.Mean[GetCurrentMutation()] == value) return;

                _mutationProbabilities.Mean[GetCurrentMutation()] = value;

                if (CustomGaussMean == (CustomGaussLower + CustomGaussUpper) / 2)
                    return;

                var temp = (CustomGaussUpper - CustomGaussLower) / 2;
                _customGaussLower = CustomGaussMean - temp;
                _customGaussUpper = CustomGaussMean + temp;

                CustomGaussStdDev = (CustomGaussLower + CustomGaussUpper) / 4;

                RaisePropertyChanged(string.Empty);
            }
        }

        public double CustomGaussStdDev
        {
            get => _mutationProbabilities.StdDev[GetCurrentMutation()];
            set
            {
                if (_mutationProbabilities.StdDev[GetCurrentMutation()] == value)
                    return;

                _mutationProbabilities.StdDev[GetCurrentMutation()] = value;
                _customGaussLower = CustomGaussMean - value * 2;
                _customGaussUpper = CustomGaussMean + value * 2;

                RaisePropertyChanged(string.Empty);
            }
        }

        public double CustomGaussUpper
        {
            get => _customGaussUpper;
            set
            {
                if (_customGaussUpper == value) return;

                _customGaussUpper = value;

                CustomGaussMean = (CustomGaussLower + CustomGaussUpper) / 2;
                CustomGaussStdDev = (CustomGaussUpper - CustomGaussLower) / 4;

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool DeltaMutationSelected
        {
            get => _deltaMutationSelected;
            set
            {
                if (value == _deltaMutationSelected)
                    return;

                _deltaMutationSelected = value;

                if (value)
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
                    Explanation = "The mutation rates of a bot are allowed to change slowly over time.  This change in mutation rates can include Delta Mutations as well.  Theoretically, it may be possible for a bot to figure its own optimal mutation rate.";
                    Unit = "00 per cycle";
                    GaussLabel = "Standard Deviation";
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool EnableGauss { get; set; }
        public bool EnableTypeSlider { get; set; }
        public string Explanation { get; set; }
        public string GaussLabel { get; set; }

        public bool InsertionSelected
        {
            get => _insertionSelected;
            set
            {
                if (value == _insertionSelected)
                    return;

                _insertionSelected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool IsEnabled
        {
            get => _mutationProbabilities.mutarray[GetCurrentMutation()] > 0;
            set
            {
                _mutationProbabilities.mutarray[GetCurrentMutation()] = value ? 1 : 0;
                RaisePropertyChanged();
            }
        }

        public bool MajorDeletionSelected
        {
            get => _majorDeletionSelected;
            set
            {
                if (value == _majorDeletionSelected)
                    return;

                _majorDeletionSelected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool MinorDeletionSelected
        {
            get => _minorDeletionSelected;
            set
            {
                if (value == _minorDeletionSelected)
                    return;

                _minorDeletionSelected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool Point2Selected
        {
            get => _point2Selected;
            set
            {
                if (value == _point2Selected)
                    return;

                _point2Selected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool PointMutationSelected
        {
            get => _pointMutationSelected;
            set
            {
                if (_pointMutationSelected == value)
                    return;

                _pointMutationSelected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public bool ReversalSelected
        {
            get => _reversalSelected;
            set
            {
                if (value == _reversalSelected)
                    return;

                _reversalSelected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        public ICommand SetDefaultRatesCommand { get; }
        public bool ShowDeltaMutation { get; set; }

        public bool TranslocationSelected
        {
            get => _translocationSelected;
            set
            {
                if (value == _translocationSelected)
                    return;

                _translocationSelected = value;

                if (value)
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
                }

                RaisePropertyChanged(string.Empty);
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

        public string Unit { get; set; }

        public void LoadFromProbabilities(MutationProbabilities probs)
        {
            _mutationProbabilities = new MutationProbabilities
            {
                CopyErrorWhatToChange = probs.CopyErrorWhatToChange,
                Mutations = probs.Mutations,
                PointWhatToChange = probs.PointWhatToChange
            };

            Array.Copy(probs.Mean, _mutationProbabilities.Mean, _mutationProbabilities.Mean.Length);
            Array.Copy(probs.mutarray, _mutationProbabilities.mutarray, _mutationProbabilities.mutarray.Length);
            Array.Copy(probs.StdDev, _mutationProbabilities.StdDev, _mutationProbabilities.StdDev.Length);

            ShowDeltaMutation = Globals.Delta2;
        }

        private double AntiProb(double a)
        {
            return a <= 0 ? 1 : 1 - 1 / a;
        }

        private int GetCurrentMutation()
        {
            if (AmplificationSelected)
                return NeoMutations.AmplificationUP;
            if (CopyError2Selected)
                return NeoMutations.CE2UP;
            if (CopyErrorSelected)
                return NeoMutations.CopyErrorUP;
            if (DeltaMutationSelected)
                return NeoMutations.DeltaUP;
            if (InsertionSelected)
                return NeoMutations.InsertionUP;
            if (MajorDeletionSelected)
                return NeoMutations.MajorDeletionUP;
            if (MinorDeletionSelected)
                return NeoMutations.MinorDeletionUP;
            if (Point2Selected)
                return NeoMutations.P2UP;
            if (PointMutationSelected)
                return NeoMutations.PointUP;
            if (ReversalSelected)
                return NeoMutations.ReversalUP;

            return NeoMutations.TranslocationUP;
        }

        private void SetDefaultRates()
        {
            _mutationProbabilities.ResetToDefault();
        }
    }
}
