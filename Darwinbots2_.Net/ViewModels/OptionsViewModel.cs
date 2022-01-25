using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Modules;
using DarwinBots.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PostSharp.Patterns.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DarwinBots.ViewModels
{
    internal enum BrownianMotion
    {
        Molecular,
        Bacterial,
        Animal
    }

    internal enum DragPresets
    {
        ThickFluid,
        Transitory,
        ThinFluid,
        None,
        Custom
    }

    internal enum FrictionPresets
    {
        Sandpaper,
        Metal,
        Teflon,
        None,
        Custom
    }

    internal enum MovementEfficiency
    {
        Ideal,
        Biological,
        Mechanical
    }

    internal enum VerticalGravity
    {
        None,
        Moon,
        Earth,
        Jupiter,
        Star
    }

    [NotifyPropertyChanged(ExcludeExplicitProperties = true)]
    internal class OptionsViewModel : ObservableObject
    {
        // TODO : Implement saving and loading settings

        private readonly IClipboardService _clipboardService;
        private readonly IDialogService _dialogService;
        private bool _costsCustom;
        private bool _costsNoCosts;
        private int _cyclesHigh;
        private int _cyclesLow;
        private bool _decayTypeEnergy;
        private bool _decayTypeNone = true;
        private bool _decayTypeWaste;
        private bool _enableCorpseMode;
        private bool _fieldModeCustom;
        private bool _fieldModeFluid;
        private bool _fieldModeSolid;
        private int _fieldSize;
        private int _initialLightEnergy;
        private int _maximumChloroplasts;
        private int _minimumChloroplastsThreshold;
        private double _mutationMultiplier;
        private double _physBrown;
        private double _physMoving;
        private int _repopulationCooldownPeriod;
        private int _robotsPerRepopulationEvent;
        private SpeciesViewModel _selectedSpecies;
        private int _shotEnergy;
        private bool _shotModeFixedEnergy;
        private bool _shotModeProportional;
        private double yGravity;

        public OptionsViewModel() : this(null, null)
        {
        }

        public OptionsViewModel(IClipboardService clipboardService = null, IDialogService dialogService = null)
        {
            _clipboardService = clipboardService ?? new ClipboardService();
            _dialogService = dialogService ?? new DialogService(Application.Current?.MainWindow);
            ShowGlobalSettingsCommand = new RelayCommand(ShowGlobalSettings);
            ShowCustomPhysicsCommand = new RelayCommand(ShowCustomPhysics);
            ListNonNativeSpeciesCommand = new RelayCommand(ListNonNativeSpecies);
            ShowCustomCostsCommand = new RelayCommand(ShowCustomCosts);
            AddSpeciesCommand = new AsyncRelayCommand(AddSpecies);
            DuplicateSpeciesCommand = new RelayCommand(DuplicateSpecies);
            DeleteSpeciesCommand = new RelayCommand(DeleteSpecies);
        }

        public ICommand AddSpeciesCommand { get; }

        public BrownianMotion BrownianMotion
        {
            get
            {
                return PhysBrown switch
                {
                    <= 0.5 => BrownianMotion.Animal,
                    > 0.5 and <= 7 => BrownianMotion.Bacterial,
                    _ => BrownianMotion.Molecular
                };
            }
            set
            {
                PhysBrown = value switch
                {
                    BrownianMotion.Bacterial => 7,
                    BrownianMotion.Molecular => 0.5,
                    _ => 0,
                };
            }
        }

        public double CoefficientKinetic { get; set; }
        public double CoefficientStatic { get; set; }
        public double CollisionElasticity { get; set; }

        public Costs Costs { get; set; }

        public bool CostsCustom
        {
            get => _costsCustom;
            set
            {
                if (SetProperty(ref _costsCustom, value) && _costsCustom)
                {
                    CostsNoCosts = false;
                    Costs = Costs.DefaultCosts;
                }
            }
        }

        public bool CostsNoCosts
        {
            get => _costsNoCosts;
            set
            {
                if (SetProperty(ref _costsNoCosts, value) && _costsNoCosts)
                {
                    CostsCustom = false;
                    Costs = Costs.ZeroCosts;
                }
            }
        }

        public int CyclesHigh { get => _cyclesHigh; set => SetProperty(ref _cyclesHigh, Math.Clamp(value, 0, 500000)); }
        public int CyclesLow { get => _cyclesLow; set => SetProperty(ref _cyclesLow, Math.Clamp(value, 0, 500000)); }
        public int DecayPeriod { get; set; }
        public double DecayRate { get; set; }

        public bool DecayTypeEnergy
        {
            get => _decayTypeEnergy;
            set
            {
                if (SetProperty(ref _decayTypeEnergy, value) && _decayTypeEnergy)
                {
                    DecayTypeNone = false;
                    DecayTypeWaste = false;
                }
            }
        }

        public bool DecayTypeNone
        {
            get => _decayTypeNone;
            set
            {
                if (SetProperty(ref _decayTypeNone, value) && _decayTypeNone)
                {
                    DecayTypeEnergy = false;
                    DecayTypeWaste = false;
                }
            }
        }

        public bool DecayTypeWaste
        {
            get => _decayTypeWaste;
            set
            {
                if (SetProperty(ref _decayTypeWaste, value) && _decayTypeWaste)
                {
                    DecayTypeNone = false;
                    DecayTypeEnergy = false;
                }
            }
        }

        public ICommand DeleteSpeciesCommand { get; }
        public double Density { get; set; }
        public bool DisableMutations { get; set; }
        public ICommand DuplicateSpeciesCommand { get; }

        public bool EnableCorpseMode
        {
            get => _enableCorpseMode;
            set
            {
                if (SetProperty(ref _enableCorpseMode, value) && _enableCorpseMode)
                {
                    DecayTypeEnergy = true;
                    DecayRate = 75;
                    DecayPeriod = 3;
                }
            }
        }

        public int FieldHeight
        {
            get
            {
                if (FieldSize == 1)
                {
                    return 6928;
                }

                var height = 6000;

                if (FieldSize <= 12)
                {
                    return height * FieldSize;
                }

                return height * 12 * (FieldSize - 12) * 2;
            }
            private set
            {
                if (value <= 72000)
                    FieldSize = value / 6000;
                else
                    FieldSize = (value / (6000 * 12 * 2)) + 12;
            }
        }

        public bool FieldModeCustom
        {
            get => _fieldModeCustom;
            set
            {
                if (SetProperty(ref _fieldModeCustom, value) && _fieldModeCustom)
                {
                    FieldModeFluid = false;
                    FieldModeSolid = false;
                }
            }
        }

        public bool FieldModeFluid
        {
            get => _fieldModeFluid;
            set
            {
                if (SetProperty(ref _fieldModeFluid, value) && _fieldModeFluid)
                {
                    FieldModeCustom = false;
                    FieldModeSolid = false;
                    MovementFriction = FrictionPresets.None;
                }
            }
        }

        public bool FieldModeSolid
        {
            get => _fieldModeSolid;
            set
            {
                if (SetProperty(ref _fieldModeSolid, value) && _fieldModeSolid)
                {
                    FieldModeFluid = false;
                    FieldModeCustom = false;
                }
            }
        }

        public int FieldSize
        {
            get => _fieldSize;
            set
            {
                if (SetProperty(ref _fieldSize, value))
                {
                    OnPropertyChanged(nameof(FieldWidth));
                    OnPropertyChanged(nameof(FieldHeight));
                }
            }
        }

        public int FieldWidth
        {
            get
            {
                if (FieldSize == 1)
                {
                    return 9237;
                }

                var width = 8000;

                if (FieldSize <= 12)
                {
                    return width * FieldSize;
                }

                return width * 12 * (FieldSize - 12) * 2;
            }
            private set
            {
                if (value <= 96000)
                    FieldSize = value / 8000;
                else
                    FieldSize = (value / (8000 * 12 * 2)) + 12;
            }
        }

        public bool FixBotRadii { get; set; }

        public VerticalGravity Gravity
        {
            get
            {
                return YGravity switch
                {
                    <= 0.1 => VerticalGravity.None,
                    > 0.1 and <= 0.3 => VerticalGravity.Moon,
                    > 0.3 and <= 0.9 => VerticalGravity.Earth,
                    > 0.9 and <= 9 => VerticalGravity.Jupiter,
                    _ => VerticalGravity.Star
                };
            }
            set
            {
                YGravity = value switch
                {
                    VerticalGravity.None => 0,
                    VerticalGravity.Moon => 0.1,
                    VerticalGravity.Earth => 0.3,
                    VerticalGravity.Jupiter => 0.9,
                    _ => 6,
                };
            }
        }

        public int InitialLightEnergy { get => _initialLightEnergy; set => SetProperty(ref _initialLightEnergy, Math.Clamp(value, 0, 32000)); }
        public bool IsSpeciesSelected => SelectedSpecies != null;
        public ICommand ListNonNativeSpeciesCommand { get; }
        public int MaximumChloroplasts { get => _maximumChloroplasts; set => SetProperty(ref _maximumChloroplasts, Math.Clamp(value, 0, 32000)); }
        public double MaxVelocity { get; set; }
        public int MinimumChloroplastsThreshold { get => _minimumChloroplastsThreshold; set => SetProperty(ref _minimumChloroplastsThreshold, Math.Clamp(value, 0, 32000)); }
        public DragPresets MovementDrag { get; set; }

        public MovementEfficiency MovementEfficiency
        {
            get
            {
                return PhysMoving switch
                {
                    <= 0.33 => MovementEfficiency.Mechanical,
                    > 0.33 and <= 0.66 => MovementEfficiency.Biological,
                    _ => MovementEfficiency.Ideal,
                };
            }
            set
            {
                PhysMoving = value switch
                {
                    MovementEfficiency.Mechanical => 0.33,
                    MovementEfficiency.Biological => 0.66,
                    _ => 1,
                };
            }
        }

        public FrictionPresets MovementFriction { get; set; }

        public string MutationDisplay
        {
            get
            {
                if (MutationMultiplier >= 0)
                {
                    return $"{(int)Math.Pow(2, MutationMultiplier)} X";
                }
                return $"1/{(int)Math.Pow(2, -MutationMultiplier)} X";
            }
        }

        public double MutationMultiplier
        {
            get => _mutationMultiplier;
            set
            {
                if (SetProperty(ref _mutationMultiplier, value))
                {
                    OnPropertyChanged(nameof(MutationDisplay));
                }
            }
        }

        public OptionsForm ParentForm { get; set; }

        public double PhysBrown
        {
            get => _physBrown;
            set
            {
                if (SetProperty(ref _physBrown, value))
                {
                    OnPropertyChanged(nameof(BrownianMotion));
                }
            }
        }

        public double PhysMoving
        {
            get => _physMoving;
            set
            {
                if (SetProperty(ref _physMoving, value))
                    OnPropertyChanged(nameof(MovementEfficiency));
            }
        }

        public ICommand RenameSpeciesCommand { get; }
        public int RepopulationCooldownPeriod { get => _repopulationCooldownPeriod; set => SetProperty(ref _repopulationCooldownPeriod, Math.Clamp(value, 0, 32000)); }
        public int RobotsPerRepopulationEvent { get => _robotsPerRepopulationEvent; set => SetProperty(ref _robotsPerRepopulationEvent, Math.Clamp(value, 0, 32000)); }
        public ICommand SaveSettingsCommand { get; }

        public SpeciesViewModel SelectedSpecies
        {
            get => _selectedSpecies;
            set
            {
                SetProperty(ref _selectedSpecies, value);
                OnPropertyChanged(nameof(IsSpeciesSelected));
            }
        }

        public int ShotEnergy { get => _shotEnergy; set => SetProperty(ref _shotEnergy, Math.Clamp(value, 0, 10000)); }

        public bool ShotModeFixedEnergy
        {
            get => _shotModeFixedEnergy; set
            {
                if (SetProperty(ref _shotModeFixedEnergy, value) && _shotModeFixedEnergy)
                {
                    ShotModeProportional = false;
                }
            }
        }

        public bool ShotModeProportional
        {
            get => _shotModeProportional; set
            {
                if (SetProperty(ref _shotModeProportional, value) && _shotModeProportional)
                {
                    ShotModeFixedEnergy = false;
                }
            }
        }

        public double ShotProportion { get; set; }
        public ICommand ShowCustomCostsCommand { get; }
        public ICommand ShowCustomPhysicsCommand { get; }
        public ICommand ShowGlobalSettingsCommand { get; }
        public ObservableCollection<SpeciesViewModel> SpeciesList { get; } = new();
        public double VegEnergyBodyDistribution { get; set; }
        public double Viscosity { get; internal set; }
        public int WasteThreshold { get; set; }

        public double YGravity
        {
            get => yGravity;
            set
            {
                if (SetProperty(ref yGravity, value))
                    OnPropertyChanged(nameof(Gravity));
            }
        }

        public bool ZeroMomentum { get; set; }
        public double ZGravity { get; set; }

        public async Task LoadFromOptions(SimOptions options)
        {
            MaximumChloroplasts = options.MaxPopulation;
            MinimumChloroplastsThreshold = options.MinVegs;
            RobotsPerRepopulationEvent = options.RepopAmount;
            RepopulationCooldownPeriod = options.RepopCooldown;
            EnableCorpseMode = options.CorpseEnabled;

            switch (options.DecayType)
            {
                case DecayType.Waste:
                    DecayTypeWaste = true;
                    break;

                case DecayType.Energy:
                    DecayTypeEnergy = true;
                    break;

                case DecayType.None:
                default:
                    DecayTypeNone = true;
                    break;
            }

            DecayRate = options.Decay;
            DecayPeriod = options.DecayDelay;
            ShotProportion = options.EnergyProp * 100;
            ShotEnergy = options.EnergyFix;
            ShotModeProportional = options.EnergyExType == ShotMode.Proportional;
            ShotModeFixedEnergy = options.EnergyExType == ShotMode.Fixed;
            MutationMultiplier = Math.Log(Math.Max(options.MutCurrMult, 0), 2);
            DisableMutations = options.DisableMutations;
            InitialLightEnergy = options.MaxEnergy;

            switch (options.FluidSolidCustom)
            {
                case FieldMode.Solid:
                    FieldModeSolid = true;
                    break;

                case FieldMode.Custom:
                    FieldModeCustom = true;
                    break;

                default:
                case FieldMode.Fluid:
                    FieldModeFluid = true;
                    break;
            }

            if (options.Costs == Costs.ZeroCosts)
                CostsNoCosts = true;
            else
                CostsCustom = true;

            switch (options.CoefficientKinetic)
            {
                case 0.75 when options.CoefficientStatic == 0.9 && options.ZGravity == 4:
                    MovementFriction = FrictionPresets.Sandpaper;
                    break;

                case 0.4 when options.CoefficientStatic == 0.6 && options.ZGravity == 2:
                    MovementFriction = FrictionPresets.Metal;
                    break;

                default:
                    if (options.CoefficientStatic == 0.05 && options.CoefficientKinetic == 0.05 && options.ZGravity == 1)
                    {
                        MovementFriction = FrictionPresets.Teflon;
                    }
                    else if (options.CoefficientStatic == 0 & options.CoefficientKinetic == 0 & options.ZGravity == 0)
                    {
                        MovementFriction = FrictionPresets.None;
                    }
                    else
                    {
                        MovementFriction = FrictionPresets.Custom;
                    }

                    break;
            }

            switch (options.Viscosity)
            {
                case 0.01 when options.Density == 0.0000001:
                    MovementDrag = DragPresets.ThickFluid;
                    break;

                case 0.0005 when options.Density == 0.0000001:
                    MovementDrag = DragPresets.Transitory;
                    break;

                case 0.000025 when options.Density == 0.0000001:
                    MovementDrag = DragPresets.ThinFluid;
                    break;

                default:
                    if (options.Viscosity == 0 & options.Density == 0)
                    {
                        MovementDrag = DragPresets.None;
                    }
                    else
                    {
                        MovementDrag = DragPresets.Custom;
                    }

                    break;
            }

            Viscosity = options.Viscosity;
            MaxVelocity = options.MaxVelocity;
            VegEnergyBodyDistribution = options.VegFeedingToBody * 100;
            WasteThreshold = options.BadWasteLevel == -1 ? 0 : options.BadWasteLevel;
            CollisionElasticity = options.CoefficientElasticity * 10;
            FixBotRadii = options.FixedBotRadii;
            SelectedSpecies = null;

            PhysBrown = options.PhysBrown;

            SpeciesList.Clear();

            foreach (var s in options.Specie)
            {
                var vm = new SpeciesViewModel(s)
                {
                    ParentForm = ParentForm
                };
                await vm.LoadComment();
                SpeciesList.Add(vm);
            }

            FieldWidth = options.FieldWidth;
            FieldHeight = options.FieldHeight;
            ZeroMomentum = options.ZeroMomentum;
            PhysMoving = options.PhysMoving;
            ZGravity = options.ZGravity;
            CoefficientStatic = options.CoefficientStatic;
            CoefficientKinetic = options.CoefficientKinetic;
            YGravity = options.YGravity;
            Density = options.Density;
        }

        public void SaveToOptions(SimOptions options)
        {
            options.MaxPopulation = MaximumChloroplasts;
            options.MinVegs = MinimumChloroplastsThreshold;
            options.RepopAmount = RobotsPerRepopulationEvent;
            options.RepopCooldown = RepopulationCooldownPeriod;
            options.CorpseEnabled = EnableCorpseMode;

            if (DecayTypeEnergy)
            {
                options.DecayType = DecayType.Energy;
            }
            else if (DecayTypeNone)
            {
                options.DecayType = DecayType.None;
            }
            else
            {
                options.DecayType = DecayType.Waste;
            }

            options.Decay = DecayRate;
            options.DecayDelay = DecayPeriod;
            options.EnergyProp = ShotProportion / 100;
            options.EnergyFix = ShotEnergy;
            options.EnergyExType = ShotModeProportional ? ShotMode.Proportional : ShotMode.Fixed;
            options.MutCurrMult = Math.Pow(MutationMultiplier, 2);
            options.DisableMutations = DisableMutations;
            options.MaxEnergy = InitialLightEnergy;

            if (FieldModeFluid)
            {
                options.FluidSolidCustom = FieldMode.Fluid;
            }
            else if (FieldModeSolid)
            {
                options.FluidSolidCustom = FieldMode.Solid;
            }
            else
            {
                options.FluidSolidCustom = FieldMode.Custom;
            }

            switch (MovementFriction)
            {
                case FrictionPresets.Sandpaper:
                    options.CoefficientKinetic = 0.75;
                    options.CoefficientStatic = 0.9;
                    options.ZGravity = 4;
                    break;

                case FrictionPresets.Metal:
                    options.CoefficientKinetic = 0.4;
                    options.CoefficientStatic = 0.6;
                    options.ZGravity = 2;
                    break;

                case FrictionPresets.Teflon:
                    options.CoefficientKinetic = 0.05;
                    options.CoefficientStatic = 0.05;
                    options.ZGravity = 1;
                    break;

                case FrictionPresets.None:
                    options.CoefficientKinetic = 0;
                    options.CoefficientStatic = 0;
                    options.ZGravity = 0;
                    break;

                case FrictionPresets.Custom:
                    // TODO : Work out how this works
                    break;
            }

            switch (MovementDrag)
            {
                case DragPresets.ThickFluid:
                    options.Viscosity = 0.01;
                    options.Density = 0.0000001;
                    break;

                case DragPresets.Transitory:
                    options.Viscosity = 0.0005;
                    options.Density = 0.0000001;
                    break;

                case DragPresets.ThinFluid:
                    options.Viscosity = 0.000025;
                    options.Density = 0.0000001;
                    break;

                case DragPresets.None:
                    options.Viscosity = 0;
                    options.Density = 0;
                    break;

                case DragPresets.Custom:
                    // TODO : Work out how this works
                    break;
            }

            options.MaxVelocity = MaxVelocity;
            options.VegFeedingToBody = VegEnergyBodyDistribution / 100;
            options.BadWasteLevel = WasteThreshold == 0 ? -1 : WasteThreshold;
            options.CoefficientElasticity = CollisionElasticity / 10;
            options.FixedBotRadii = FixBotRadii;

            options.PhysBrown = PhysBrown;

            foreach (var s in SpeciesList)
            {
                s.Save();
                options.Specie.Add(s.Species);
            }

            options.Costs = Costs;

            options.FieldWidth = FieldWidth;
            options.FieldHeight = FieldHeight;
            options.ZeroMomentum = ZeroMomentum;
            options.PhysMoving = PhysMoving;
            options.ZGravity = ZGravity;
            options.CoefficientStatic = CoefficientStatic;
            options.CoefficientKinetic = CoefficientKinetic;
            options.YGravity = YGravity;
            options.Density = Density;
        }

        private async Task AddSpecies()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "DNA File (*.txt)|*.txt|All Files (*.*)|*.*",
                InitialDirectory = "robots",
                Title = "Choose a DNA File",
                CheckFileExists = true,
                ShowReadOnly = false
            };

            var result = dialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            var species = new Species(dialog.FileName);
            species.Mutables.ResetToDefault();
            species.AssignSkin();

            var vm = new SpeciesViewModel(species)
            {
                ParentForm = ParentForm
            };
            await vm.LoadComment();

            SpeciesList.Add(vm);
        }

        private void DeleteSpecies()
        {
            if (SelectedSpecies == null)
            {
                return;
            }

            SpeciesList.Remove(SelectedSpecies);

            SelectedSpecies = null;
        }

        private void DuplicateSpecies()
        {
            if (SelectedSpecies == null)
            {
                return;
            }

            if (SelectedSpecies.Native)
            {
                SpeciesList.Add(SelectedSpecies.Duplicate());
            }
            else
            {
                MessageBox.Show("You cannot duplicate a bot that did not originate in this simulation.", "Cannot Duplicate Non-Native Species", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ListNonNativeSpecies()
        {
            var nonNative = SpeciesList.Where(s => !s.Native).ToList();

            if (!nonNative.Any())
            {
                _dialogService.ShowInfoMessageBox("Non-Native Species Summary", "There are no non-native species.");
            }
            else
            {
                var names = string.Join(", ", nonNative.Select(s => $"\"{s.Name}\""));

                _clipboardService.CopyCsvToClipboard(names);
                _dialogService.ShowInfoMessageBox("Non-Native Species Summary", $"The non-native species are:\n\n{names}\n\nThese have been copied to the clipboard.");
            }
        }

        private void ShowCustomCosts()
        {
            _dialogService.ShowOptionsSubDialog<CostsForm>(this, ParentForm);
        }

        private void ShowCustomPhysics()
        {
            _dialogService.ShowOptionsSubDialog<PhysicsOptions>(this, ParentForm);
        }

        private void ShowGlobalSettings()
        {
            // TODO : Implement this
        }
    }
}
