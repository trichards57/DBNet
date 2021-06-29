using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Modules;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PostSharp.Patterns.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DarwinBots.ViewModels
{
    public enum BrownianMotion
    {
        Molecular,
        Bacterial,
        Animal
    }

    public enum DragPresets
    {
        ThickFluid,
        Transitory,
        ThinFluid,
        None,
        Custom
    }

    public enum FrictionPresets
    {
        Sandpaper,
        Metal,
        Teflon,
        None,
        Custom
    }

    public enum MovementEfficiency
    {
        Ideal,
        Biological,
        Mechanical
    }

    public enum VerticalGravity
    {
        None,
        Moon,
        Earth,
        Jupiter,
        Star
    }

    [NotifyPropertyChanged]
    internal class OptionsViewModel : ViewModelBase
    {
        // TODO : Show custom physics dialog
        // TODO : Implement saving and loading settings

        private readonly Timer _lightTimer;
        private Costs _costs;
        private int _cyclesHigh;
        private int _cyclesLow;
        private int _fieldSize;
        private bool _lightCurrentlyIncreasing = false;
        private bool costsCustom;
        private bool costsNoCosts;
        private bool decayTypeEnergy;
        private bool decayTypeNone;
        private bool decayTypeWaste;
        private bool enabelMutationSineWave;
        private bool enableCorpseMode;
        private float energyScalingFactor;
        private bool fieldModeCustom;
        private bool fieldModeFluid;
        private bool fieldModeSolid;
        private int initialLightEnergy;
        private int lightLevel;
        private int maximumChloroplasts;
        private int minimumChloroplastsThreshold;
        private double mutationMultiplier;
        private int repopulationCooldownPeriod;
        private int robotsPerRepopulationEvent;
        private double sedimentLevel;
        private int shotEnergy;
        private bool shotModeFixedEnergy;
        private bool shotModeProportional;

        public OptionsViewModel()
        {
            _lightTimer = new Timer(LightTimerTick, null, Timeout.Infinite, Timeout.Infinite);
            ShowEnergyManagementCommand = new RelayCommand(ShowEnergyManagement);
            ShowGlobalSettingsCommand = new RelayCommand(ShowGlobalSettings);
            ListNonNativeSpeciesCommand = new RelayCommand(ListNonNativeSpecies);
            ShowCustomCostsCommand = new RelayCommand(ShowCustomCosts);
            AddSpeciesCommand = new RelayCommand(AddSpecies);
            DuplicateSpeciesCommand = new RelayCommand(DuplicateSpecies);
            DeleteSpeciesCommand = new RelayCommand(DeleteSpecies);
        }

        public ICommand AddSpeciesCommand { get; }
        public BrownianMotion BrownianMotion { get; set; }
        public ICommand ChangeCommand { get; }
        public double CollisionElasticity { get; set; }

        public bool CostsCustom
        {
            get => costsCustom;
            set
            {
                costsCustom = value;
                RaisePropertyChanged();
                if (costsCustom)
                {
                    CostsNoCosts = false;
                    _costs = Costs.ZeroCosts() with
                    {
                        ConditionCost = 0.004,
                        StoresCost = 0.04,
                        VoluntaryMovementCost = 0.05,
                        TieFormationCost = 2,
                        ShotFormationCost = 2,
                        VenomCost = 0.01,
                        PoisonCost = 0.01,
                        SlimeCost = 0.1,
                        ShellCost = 0.1,
                        BodyUpkeepCost = 0.00001,
                        AgeCost = 0.01,
                        CostMultiplier = 1
                    };
                }
            }
        }

        public bool CostsNoCosts
        {
            get => costsNoCosts;
            set
            {
                costsNoCosts = value;
                RaisePropertyChanged();
                if (costsNoCosts)
                {
                    CostsCustom = false;
                    _costs = Costs.ZeroCosts();
                }
            }
        }

        public int CyclesHigh { get => _cyclesHigh; set { _cyclesHigh = Math.Clamp(value, 0, 500000); RaisePropertyChanged(); } }
        public int CyclesLow { get => _cyclesLow; set { _cyclesLow = Math.Clamp(value, 0, 500000); RaisePropertyChanged(); } }
        public int DecayPeriod { get; set; }
        public double DecayRate { get; set; }

        public bool DecayTypeEnergy
        {
            get => decayTypeEnergy;
            set
            {
                decayTypeEnergy = value;
                RaisePropertyChanged();
                if (decayTypeEnergy)
                {
                    DecayTypeNone = false;
                    DecayTypeWaste = false;
                }
            }
        }

        public bool DecayTypeNone
        {
            get => decayTypeNone;
            set
            {
                decayTypeNone = value;
                RaisePropertyChanged();
                if (decayTypeNone)
                {
                    DecayTypeEnergy = false;
                    DecayTypeWaste = false;
                }
            }
        }

        public bool DecayTypeWaste
        {
            get => decayTypeWaste; set
            {
                decayTypeWaste = value;
                RaisePropertyChanged();
                if (decayTypeWaste)
                {
                    DecayTypeNone = false;
                    DecayTypeEnergy = false;
                }
            }
        }

        public ICommand DeleteSpeciesCommand { get; }
        public bool DisableMutations { get; set; }
        public ICommand DuplicateSpeciesCommand { get; }

        public bool EnabelMutationSineWave
        {
            get => enabelMutationSineWave;
            set
            {
                enabelMutationSineWave = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MaxCyclesLabel));
                RaisePropertyChanged(nameof(MinCyclesLabel));
            }
        }

        public bool EnableCorpseMode
        {
            get => enableCorpseMode;
            set
            {
                enableCorpseMode = value;
                RaisePropertyChanged();

                if (enableCorpseMode)
                {
                    DecayTypeEnergy = true;
                    DecayRate = 75;
                    DecayPeriod = 3;
                }
            }
        }

        public bool EnableLeftRightWrap { get; set; }
        public bool EnableMutationCycling { get; set; }
        public bool EnablePondMode { get; set; }
        public bool EnableTides { get; set; }
        public bool EnableTopDownWrap { get; set; }
        public float EnergyScalingFactor { get => energyScalingFactor; set { energyScalingFactor = value == 0 ? 1 : value; RaisePropertyChanged(); } }

        public int FieldHeight
        {
            get
            {
                if (FieldSize == 1)
                    return 6928;

                var height = 6000;

                if (FieldSize <= 12)
                    return height * FieldSize;

                return height * 12 * (FieldSize - 12) * 2;
            }
        }

        public bool FieldModeCustom
        {
            get => fieldModeCustom;
            set
            {
                fieldModeCustom = value;
                RaisePropertyChanged();
                if (fieldModeCustom)
                {
                    FieldModeFluid = false;
                    FieldModeSolid = false;
                }
            }
        }

        public bool FieldModeFluid
        {
            get => fieldModeFluid;
            set
            {
                fieldModeFluid = value;
                RaisePropertyChanged();
                if (fieldModeFluid)
                {
                    FieldModeCustom = false;
                    FieldModeSolid = false;
                    MovementFriction = FrictionPresets.None;
                }
            }
        }

        public bool FieldModeSolid
        {
            get => fieldModeSolid;
            set
            {
                fieldModeSolid = value;
                RaisePropertyChanged();
                if (fieldModeSolid)
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
                _fieldSize = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FieldWidth));
                RaisePropertyChanged(nameof(FieldHeight));
            }
        }

        public int FieldWidth
        {
            get
            {
                if (FieldSize == 1)
                    return 9237;

                var width = 8000;

                if (FieldSize <= 12)
                    return width * FieldSize;

                return width * 12 * (FieldSize - 12) * 2;
            }
        }

        public bool FixBotRadii { get; set; }
        public bool ForceCholoplastsSustain { get; set; }
        public int GraphUpdateInterval { get; set; }
        public VerticalGravity Gravity { get; set; }
        public int InitialLightEnergy { get => initialLightEnergy; set { initialLightEnergy = Math.Clamp(value, 0, 32000); RaisePropertyChanged(); } }
        public int LightLevel { get => lightLevel; set { lightLevel = Math.Clamp(value, 0, 1000); RaisePropertyChanged(); } }
        public ICommand ListNonNativeSpeciesCommand { get; }
        public ICommand LoadSettingsCommand { get; set; }
        public string MaxCyclesLabel => EnabelMutationSineWave ? "Max at 20x" : "Cycles at 16x";
        public int MaximumChloroplasts { get => maximumChloroplasts; set { maximumChloroplasts = Math.Clamp(value, 0, 32000); RaisePropertyChanged(); } }
        public double MaxVelocity { get; set; }
        public string MinCyclesLabel => EnabelMutationSineWave ? "Max at 1/20x" : "Cycles at 1/16x";
        public int MinimumChloroplastsThreshold { get => minimumChloroplastsThreshold; set { minimumChloroplastsThreshold = Math.Clamp(value, 0, 32000); RaisePropertyChanged(); } }
        public DragPresets MovementDrag { get; set; }
        public MovementEfficiency MovementEfficiency { get; set; }
        public FrictionPresets MovementFriction { get; set; }

        public string MutationDisplay
        {
            get
            {
                if (MutationMultiplier > 1)
                    return $"{(int)Math.Pow(2, MutationMultiplier)} X";
                return $"1/{(int)Math.Pow(2, -MutationMultiplier)} X";
            }
        }

        public double MutationMultiplier
        {
            get => mutationMultiplier;
            set
            {
                mutationMultiplier = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MutationDisplay));
            }
        }

        public ICommand RenameSpeciesCommand { get; }
        public int RepopulationCooldownPeriod { get => repopulationCooldownPeriod; set { repopulationCooldownPeriod = Math.Clamp(value, 0, 32000); RaisePropertyChanged(); } }
        public int RobotsPerRepopulationEvent { get => robotsPerRepopulationEvent; set { robotsPerRepopulationEvent = Math.Clamp(value, 0, 32000); RaisePropertyChanged(); } }
        public ICommand SaveSettingsCommand { get; }
        public double SedimentLevel { get => sedimentLevel; set { sedimentLevel = Math.Clamp(value, 0.0, 200.0); RaisePropertyChanged(); } }
        public SpeciesViewModel SelectedSpecies { get; set; }
        public int ShotEnergy { get => shotEnergy; set { shotEnergy = Math.Clamp(value, 0, 10000); RaisePropertyChanged(); } }

        public bool ShotModeFixedEnergy
        {
            get => shotModeFixedEnergy; set
            {
                shotModeFixedEnergy = value;
                RaisePropertyChanged();
                if (shotModeFixedEnergy)
                    ShotModeProportional = false;
            }
        }

        public bool ShotModeProportional
        {
            get => shotModeProportional; set
            {
                shotModeProportional = value;
                RaisePropertyChanged();
                if (shotModeProportional)
                    ShotModeFixedEnergy = false;
            }
        }

        public double ShotProportion { get; set; }
        public ICommand ShowCustomCostsCommand { get; }
        public ICommand ShowCustomPhysicsCommand { get; }
        public ICommand ShowEnergyManagementCommand { get; }
        public ICommand ShowGlobalSettingsCommand { get; }
        public ObservableCollection<SpeciesViewModel> SpeciesList { get; } = new ObservableCollection<SpeciesViewModel>();
        public ICommand StartNewCommand { get; }
        public double VegEnergyBodyDistribution { get; set; }
        public int WasteThreshold { get; set; }
        public object YGravity { get; private set; }

        public async Task LoadFromOptions(SimOptions options)
        {
            FieldSize = options.FieldSize;
            MaximumChloroplasts = options.MaxPopulation;
            MinimumChloroplastsThreshold = options.MinVegs;
            RobotsPerRepopulationEvent = options.RepopAmount;
            RepopulationCooldownPeriod = options.RepopCooldown;
            EnableCorpseMode = options.CorpseEnabled;

            switch (options.DecayType)
            {
                case DecayType.None:
                    DecayTypeNone = true;
                    break;

                case DecayType.Waste:
                    DecayTypeWaste = true;
                    break;

                case DecayType.Energy:
                    DecayTypeEnergy = true;
                    break;
            }

            DecayRate = options.Decay;
            DecayPeriod = options.Decaydelay;
            LightLevel = options.LightIntensity;
            SedimentLevel = (options.Gradient - 1) * 10;
            EnableTopDownWrap = options.Updnconnected;
            EnableLeftRightWrap = options.Dxsxconnected;
            EnablePondMode = options.Pondmode;
            ShotProportion = options.EnergyProp * 100;
            ShotEnergy = options.EnergyFix;
            ShotModeProportional = options.EnergyExType == ShotMode.Proportional;
            ShotModeFixedEnergy = options.EnergyExType == ShotMode.Fixed;
            MutationMultiplier = Math.Log(Math.Max(options.MutCurrMult, 0), 2);
            EnableMutationCycling = options.MutOscill;
            EnabelMutationSineWave = options.MutOscillSine;
            DisableMutations = options.DisableMutations;
            CyclesHigh = options.MutCycMax;
            CyclesLow = options.MutCycMin;
            InitialLightEnergy = options.MaxEnergy;

            switch (options.FluidSolidCustom)
            {
                case FieldMode.Fluid:
                    FieldModeFluid = true;
                    break;

                case FieldMode.Solid:
                    FieldModeSolid = true;
                    break;

                case FieldMode.Custom:
                    FieldModeCustom = true;
                    break;
            }

            switch (options.CostRadioSetting)
            {
                case CostSetting.None:
                    CostsNoCosts = true;
                    break;

                case CostSetting.Custom:
                    CostsCustom = true;
                    break;
            }

            if (options.CoefficientKinetic == 0.75 && options.CoefficientStatic == 0.9 && options.Zgravity == 4)
                MovementFriction = FrictionPresets.Sandpaper;
            else if (options.CoefficientKinetic == 0.4 && options.CoefficientStatic == 0.6 && options.Zgravity == 2)
                MovementFriction = FrictionPresets.Metal;
            else if (options.CoefficientStatic == 0.05 && options.CoefficientKinetic == 0.05 && options.Zgravity == 1)
                MovementFriction = FrictionPresets.Teflon;
            else if (options.CoefficientStatic == 0 & options.CoefficientKinetic == 0 & options.Zgravity == 0)
                MovementFriction = FrictionPresets.None;
            else
                MovementFriction = FrictionPresets.Custom;

            if (options.Viscosity == 0.01 && options.Density == 0.0000001)
                MovementDrag = DragPresets.ThickFluid;
            else if (options.Viscosity == 0.0005 && options.Density == 0.0000001)
                MovementDrag = DragPresets.Transitory;
            else if (options.Viscosity == 0.000025 && options.Density == 0.0000001)
                MovementDrag = DragPresets.ThinFluid;
            else if (options.Viscosity == 0 & options.Density == 0)
                MovementDrag = DragPresets.None;
            else
                MovementDrag = DragPresets.Custom;

            MaxVelocity = options.MaxVelocity;
            VegEnergyBodyDistribution = options.VegFeedingToBody * 100;
            GraphUpdateInterval = options.ChartingInterval;
            WasteThreshold = options.BadWastelevel == -1 ? 0 : options.BadWastelevel;
            CollisionElasticity = options.CoefficientElasticity * 10;
            FixBotRadii = options.FixedBotRadii;
            SelectedSpecies = null;
            EnableTides = options.Tides > 0;

            MovementEfficiency = options.PhysMoving switch
            {
                <= 0.33 => MovementEfficiency.Mechanical,
                > 0.33 and <= 0.66 => MovementEfficiency.Biological,
                _ => MovementEfficiency.Ideal,
            };

            BrownianMotion = options.PhysBrown switch
            {
                <= 0.5 => BrownianMotion.Animal,
                > 0.5 and <= 7 => BrownianMotion.Bacterial,
                _ => BrownianMotion.Molecular
            };

            Gravity = options.Ygravity switch
            {
                <= 0.1 => VerticalGravity.None,
                > 0.1 and <= 0.3 => VerticalGravity.Moon,
                > 0.3 and <= 0.9 => VerticalGravity.Earth,
                > 0.9 and <= 9 => VerticalGravity.Jupiter,
                _ => VerticalGravity.Star
            };

            SpeciesList.Clear();

            foreach (var s in options.Specie)
            {
                var vm = new SpeciesViewModel(s);
                await vm.LoadComment();
                SpeciesList.Add(vm);
            }
        }

        public void SaveToOptions(SimOptions options)
        {
            options.FieldSize = FieldSize;
            options.MaxPopulation = MaximumChloroplasts;
            options.MinVegs = MinimumChloroplastsThreshold;
            options.RepopAmount = RobotsPerRepopulationEvent;
            options.RepopCooldown = RepopulationCooldownPeriod;
            options.CorpseEnabled = EnableCorpseMode;

            if (DecayTypeEnergy)
                options.DecayType = DecayType.Energy;
            else if (DecayTypeNone)
                options.DecayType = DecayType.None;
            else
                options.DecayType = DecayType.Waste;

            options.Decay = DecayRate;
            options.Decaydelay = DecayPeriod;
            options.LightIntensity = LightLevel;
            options.Gradient = SedimentLevel / 10 + 1;
            options.Updnconnected = EnableTopDownWrap;
            options.Dxsxconnected = EnableLeftRightWrap;
            options.Toroidal = EnableTopDownWrap && EnableLeftRightWrap;
            options.Pondmode = EnablePondMode;
            options.EnergyProp = ShotProportion / 100;
            options.EnergyFix = ShotEnergy;
            options.EnergyExType = ShotModeProportional ? ShotMode.Proportional : ShotMode.Fixed;
            options.MutCurrMult = Math.Pow(MutationMultiplier, 2);
            options.MutOscill = EnableMutationCycling;
            options.MutOscillSine = EnabelMutationSineWave;
            options.DisableMutations = DisableMutations;
            options.MutCycMax = CyclesHigh;
            options.MutCycMin = CyclesLow;
            options.MaxEnergy = InitialLightEnergy;

            if (FieldModeFluid)
                options.FluidSolidCustom = FieldMode.Fluid;
            else if (FieldModeSolid)
                options.FluidSolidCustom = FieldMode.Solid;
            else
                options.FluidSolidCustom = FieldMode.Custom;

            options.CostRadioSetting = CostsNoCosts ? CostSetting.None : CostSetting.Custom;

            switch (MovementFriction)
            {
                case FrictionPresets.Sandpaper:
                    options.CoefficientKinetic = 0.75;
                    options.CoefficientStatic = 0.9;
                    options.Zgravity = 4;
                    break;

                case FrictionPresets.Metal:
                    options.CoefficientKinetic = 0.4;
                    options.CoefficientStatic = 0.6;
                    options.Zgravity = 2;
                    break;

                case FrictionPresets.Teflon:
                    options.CoefficientKinetic = 0.05;
                    options.CoefficientStatic = 0.05;
                    options.Zgravity = 1;
                    break;

                case FrictionPresets.None:
                    options.CoefficientKinetic = 0;
                    options.CoefficientStatic = 0;
                    options.Zgravity = 0;
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
            options.ChartingInterval = GraphUpdateInterval;
            options.BadWastelevel = WasteThreshold == 0 ? -1 : WasteThreshold;
            options.CoefficientElasticity = CollisionElasticity / 10;
            options.FixedBotRadii = FixBotRadii;
            // TODO : Work out how this works
            // EnableTides = options.Tides > 0;

            options.PhysMoving = MovementEfficiency switch
            {
                MovementEfficiency.Ideal => 0.66,
                MovementEfficiency.Biological => 0.33,
                _ => 1,
            };

            options.PhysBrown = BrownianMotion switch
            {
                BrownianMotion.Bacterial => 7,
                BrownianMotion.Molecular => 0.5,
                _ => 0,
            };

            options.Ygravity = Gravity switch
            {
                VerticalGravity.None => 0,
                VerticalGravity.Moon => 0.1,
                VerticalGravity.Earth => 0.3,
                VerticalGravity.Jupiter => 0.9,
                _ => 6,
            };

            foreach (var s in SpeciesList)
                s.Save();

            options.Costs = _costs;
        }

        public void SetPondMode(bool enable)
        {
            if (enable)
            {
                var res = MessageBox.Show("Turning on Pond Mode will greatly alter the physics of the simulation.  Chloroplasts will recieve less energy towards the bottom of the field.  Gravity will be turned on and top/down wrapping switched off.  Do you want to continue?", "Enable Pond Mode", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (res == MessageBoxResult.Yes)
                {
                    EnablePondMode = true;
                    EnableTopDownWrap = false;
                    YGravity = 6.2;
                    LightLevel = 100;
                    SedimentLevel = 2;
                }
            }
            EnablePondMode = false;
        }

        public void StartLightTimer()
        {
            _lightTimer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
        }

        public void StopLightTimer()
        {
            _lightTimer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
        }

        private void AddSpecies()
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
                return;

            var species = new Species(dialog.FileName);
            species.Mutables.ResetToDefault();
            species.AssignSkin();

            var vm = new SpeciesViewModel(species);

            SpeciesList.Add(vm);
        }

        private void DeleteSpecies()
        {
            if (SelectedSpecies == null)
                return;

            SpeciesList.Remove(SelectedSpecies);

            SelectedSpecies = null;
        }

        private void DuplicateSpecies()
        {
            if (SelectedSpecies == null)
                return;

            if (SelectedSpecies.Native)
                SpeciesList.Add(SelectedSpecies.Duplicate());
            else
                MessageBox.Show("You cannot duplicate a bot that did not originate in this simulation.", "Cannot Duplicate Non-Native Species", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void LightTimerTick(object state)
        {
            if (_lightCurrentlyIncreasing)
            {
                var n = EnergyScalingFactor + 5;

                if (n > 100)
                {
                    _lightCurrentlyIncreasing = false;
                    EnergyScalingFactor = 100;
                }
                else
                    EnergyScalingFactor = n;
            }
            else
            {
                var n = EnergyScalingFactor - 5;

                if (n < 2)
                {
                    _lightCurrentlyIncreasing = true;
                    EnergyScalingFactor = 2;
                }
                else
                    EnergyScalingFactor = n;
            }
        }

        private void ListNonNativeSpecies()
        {
            var nativeSpecies = SpeciesList.Where(s => s.Native);

            if (!nativeSpecies.Any())
                MessageBox.Show("There are no non-native species.", "Non-Native Species Summary", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                var names = string.Join(", ", nativeSpecies.Select(s => $"\"{s.Name}\""));

                Clipboard.SetText(names, TextDataFormat.CommaSeparatedValue);

                MessageBox.Show($"The non-native species are:\n\n{names}\n\nThese have been copied to the clipboard.", "Non-Native Species Summary", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowCustomCosts()
        {
            var vm = new CostsViewModel();
            vm.LoadFromOptions(_costs);

            var form = new CostsForm();
            var res = form.ShowDialog();

            if (res == true)
                _costs = vm.SaveOptions();
        }

        private void ShowEnergyManagement()
        {
            // TODO : Implement this
        }

        private void ShowGlobalSettings()
        {
            // TODO : Implement this
        }
    }
}
