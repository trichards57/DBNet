using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace Iersera.ViewModels
{
    internal class CostsViewModel : ViewModelBase
    {
        private double _advancedCommandCost;
        private double _ageCost;
        private int _ageCostBeginAge;
        private double _ageCostIncreasePerCycle;
        private bool _allowMultiplerToGoNegative;
        private double _basicCommandCost;
        private double _bitwiseCommandCost;
        private double _bodyUpkeepCost;
        private double _cholorplastCost;
        private double _conditionCost;
        private double _costMultiplier;
        private double _dnaCopyCost;
        private double _dnaUpkeepCost;
        private int _dynamicCostsLowerRangeTarget;
        private double _dynamicCostsSensitivity;
        private int _dynamicCostsTargetPopulation;
        private int _dynamicCostsUpperRangeTarget;
        private bool _enableAgeCostIncreaseLog;
        private bool _enableAgeCostIncreasePerCycle;
        private bool _enableDynamicCosts;
        private double _flowCommandCost;
        private bool _includeAllRobotsInPopulation;
        private double _logicCost;
        private double _numberCost;
        private double _poisonCost;
        private int _reinstateCostPopulationLimit;
        private double _rotationCost;
        private double _shellCost;
        private double _shotFormationCost;
        private double _slimeCost;
        private double _starNumberCost;
        private double _storesCost;
        private double _tieFormationCost;
        private double _venomCost;
        private double _voluntaryMovementCost;
        private int _zeroCostPopulationLimit;

        public CostsViewModel()
        {
            RestoreDefaultsCommand = new RelayCommand(RestoreDefaults);
        }

        public double AdvancedCommandCost { get => _advancedCommandCost; set { _advancedCommandCost = value; RaisePropertyChanged(); } }
        public double AgeCost { get => _ageCost; set { _ageCost = value; RaisePropertyChanged(); } }
        public int AgeCostBeginAge { get => _ageCostBeginAge; set { _ageCostBeginAge = value; RaisePropertyChanged(); } }
        public double AgeCostIncreasePerCycle { get => _ageCostIncreasePerCycle; set { _ageCostIncreasePerCycle = value; RaisePropertyChanged(); } }
        public bool AllowMultiplerToGoNegative { get => _allowMultiplerToGoNegative; set { _allowMultiplerToGoNegative = value; RaisePropertyChanged(); } }
        public double BasicCommandCost { get => _basicCommandCost; set { _basicCommandCost = value; RaisePropertyChanged(); } }
        public double BitwiseCommandCost { get => _bitwiseCommandCost; set { _bitwiseCommandCost = value; RaisePropertyChanged(); } }
        public double BodyUpkeepCost { get => _bodyUpkeepCost; set { _bodyUpkeepCost = value; RaisePropertyChanged(); } }
        public double CholorplastCost { get => _cholorplastCost; set { _cholorplastCost = value; RaisePropertyChanged(); } }
        public double ConditionCost { get => _conditionCost; set { _conditionCost = value; RaisePropertyChanged(); } }
        public double CostMultiplier { get => _costMultiplier; set { _costMultiplier = value; RaisePropertyChanged(); } }
        public double DnaCopyCost { get => _dnaCopyCost; set { _dnaCopyCost = value; RaisePropertyChanged(); } }
        public double DnaUpkeepCost { get => _dnaUpkeepCost; set { _dnaUpkeepCost = value; RaisePropertyChanged(); } }
        public int DynamicCostsLowerRangeTarget { get => _dynamicCostsLowerRangeTarget; set { _dynamicCostsLowerRangeTarget = value; RaisePropertyChanged(); } }
        public double DynamicCostsSensitivity { get => _dynamicCostsSensitivity; set { _dynamicCostsSensitivity = value; RaisePropertyChanged(); } }
        public int DynamicCostsTargetPopulation { get => _dynamicCostsTargetPopulation; set { _dynamicCostsTargetPopulation = value; RaisePropertyChanged(); } }
        public int DynamicCostsUpperRangeTarget { get => _dynamicCostsUpperRangeTarget; set { _dynamicCostsUpperRangeTarget = value; RaisePropertyChanged(); } }

        public bool EnableAgeCostIncreaseLog
        {
            get => _enableAgeCostIncreaseLog;
            set
            {
                _enableAgeCostIncreaseLog = value;
                RaisePropertyChanged();
                if (EnableAgeCostIncreasePerCycle && value)
                    EnableAgeCostIncreasePerCycle = false;
            }
        }

        public bool EnableAgeCostIncreasePerCycle
        {
            get => _enableAgeCostIncreasePerCycle;
            set
            {
                _enableAgeCostIncreasePerCycle = value;
                RaisePropertyChanged();
                if (EnableAgeCostIncreaseLog && value)
                    EnableAgeCostIncreaseLog = false;
            }
        }

        public bool EnableDynamicCosts { get => _enableDynamicCosts; set { _enableDynamicCosts = value; RaisePropertyChanged(); } }
        public double FlowCommandCost { get => _flowCommandCost; set { _flowCommandCost = value; RaisePropertyChanged(); } }
        public bool IncludeAllRobotsInPopulation { get => _includeAllRobotsInPopulation; set { _includeAllRobotsInPopulation = value; RaisePropertyChanged(); } }
        public double LogicCost { get => _logicCost; set { _logicCost = value; RaisePropertyChanged(); } }
        public double NumberCost { get => _numberCost; set { _numberCost = value; RaisePropertyChanged(); } }
        public double PoisonCost { get => _poisonCost; set { _poisonCost = value; RaisePropertyChanged(); } }
        public int ReinstateCostPopulationLimit { get => _reinstateCostPopulationLimit; set { _reinstateCostPopulationLimit = value; RaisePropertyChanged(); } }
        public ICommand RestoreDefaultsCommand { get; }
        public double RotationCost { get => _rotationCost; set { _rotationCost = value; RaisePropertyChanged(); } }
        public double ShellCost { get => _shellCost; set { _shellCost = value; RaisePropertyChanged(); } }
        public double ShotFormationCost { get => _shotFormationCost; set { _shotFormationCost = value; RaisePropertyChanged(); } }
        public double SlimeCost { get => _slimeCost; set { _slimeCost = value; RaisePropertyChanged(); } }
        public double StarNumberCost { get => _starNumberCost; set { _starNumberCost = value; RaisePropertyChanged(); } }
        public double StoresCost { get => _storesCost; set { _storesCost = value; RaisePropertyChanged(); } }
        public double TieFormationCost { get => _tieFormationCost; set { _tieFormationCost = value; RaisePropertyChanged(); } }
        public double VenomCost { get => _venomCost; set { _venomCost = value; RaisePropertyChanged(); } }
        public double VoluntaryMovementCost { get => _voluntaryMovementCost; set { _voluntaryMovementCost = value; RaisePropertyChanged(); } }
        public int ZeroCostPopulationLimit { get => _zeroCostPopulationLimit; set { _zeroCostPopulationLimit = value; RaisePropertyChanged(); } }

        public void LoadFromOptions()
        {
            if (SimOpt.SimOpts.Costs.EnableAgeCostIncreaseLog && SimOpt.SimOpts.Costs.EnableAgeCostIncreasePerCycle)
            {
                EnableAgeCostIncreaseLog = false;
                EnableAgeCostIncreasePerCycle = false;
            }
            else
            {
                EnableAgeCostIncreaseLog = SimOpt.SimOpts.Costs.EnableAgeCostIncreaseLog;
                EnableAgeCostIncreasePerCycle = SimOpt.SimOpts.Costs.EnableAgeCostIncreasePerCycle;
            }
            AdvancedCommandCost = SimOpt.SimOpts.Costs.AdvancedCommandCost;
            AgeCost = SimOpt.SimOpts.Costs.AgeCost;
            AgeCostBeginAge = SimOpt.SimOpts.Costs.AgeCostBeginAge;
            AgeCostIncreasePerCycle = SimOpt.SimOpts.Costs.AgeCostIncreasePerCycle;
            AllowMultiplerToGoNegative = SimOpt.SimOpts.Costs.AllowMultiplerToGoNegative;
            BasicCommandCost = SimOpt.SimOpts.Costs.BasicCommandCost;
            BitwiseCommandCost = SimOpt.SimOpts.Costs.BitwiseCommandCost;
            BodyUpkeepCost = SimOpt.SimOpts.Costs.BodyUpkeepCost;
            CholorplastCost = SimOpt.SimOpts.Costs.CholorplastCost;
            ConditionCost = SimOpt.SimOpts.Costs.ConditionCost;
            CostMultiplier = SimOpt.SimOpts.Costs.CostMultiplier;
            DnaCopyCost = SimOpt.SimOpts.Costs.DnaCopyCost;
            DnaUpkeepCost = SimOpt.SimOpts.Costs.DnaUpkeepCost;
            EnableDynamicCosts = SimOpt.SimOpts.Costs.EnableDynamicCosts;
            FlowCommandCost = SimOpt.SimOpts.Costs.FlowCommandCost;
            IncludeAllRobotsInPopulation = SimOpt.SimOpts.Costs.DynamicCostsIncludePlants;
            LogicCost = SimOpt.SimOpts.Costs.LogicCost;
            NumberCost = SimOpt.SimOpts.Costs.NumberCost;
            PoisonCost = SimOpt.SimOpts.Costs.PoisonCost;
            ReinstateCostPopulationLimit = SimOpt.SimOpts.Costs.ReinstateCostPopulationLimit;
            RotationCost = SimOpt.SimOpts.Costs.RotationCost;
            ShellCost = SimOpt.SimOpts.Costs.ShellCost;
            ShotFormationCost = SimOpt.SimOpts.Costs.ShotFormationCost;
            SlimeCost = SimOpt.SimOpts.Costs.SlimeCost;
            StarNumberCost = SimOpt.SimOpts.Costs.DotNumberCost;
            StoresCost = SimOpt.SimOpts.Costs.StoresCost;
            TieFormationCost = SimOpt.SimOpts.Costs.TieFormationCost;
            VenomCost = SimOpt.SimOpts.Costs.VenomCost;
            VoluntaryMovementCost = SimOpt.SimOpts.Costs.VoluntaryMovementCost;
            ZeroCostPopulationLimit = SimOpt.SimOpts.Costs.ZeroCostPopulationLimit;
            DynamicCostsTargetPopulation = SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;
            DynamicCostsSensitivity = SimOpt.SimOpts.Costs.DynamicCostsSensitivity;
            DynamicCostsUpperRangeTarget = SimOpt.SimOpts.Costs.DynamicCostsUpperRangeTarget;
            DynamicCostsLowerRangeTarget = (int)SimOpt.SimOpts.Costs.DynamicCostsLowerRangeTarget;
        }

        public void UpdateOptions()
        {
            SimOpt.SimOpts.Costs.AdvancedCommandCost = AdvancedCommandCost;
            SimOpt.SimOpts.Costs.AgeCost = AgeCost;
            SimOpt.SimOpts.Costs.AgeCostIncreasePerCycle = AgeCostIncreasePerCycle;
            SimOpt.SimOpts.Costs.EnableAgeCostIncreasePerCycle = EnableAgeCostIncreasePerCycle;
            SimOpt.SimOpts.Costs.EnableAgeCostIncreaseLog = EnableAgeCostIncreaseLog;
            SimOpt.SimOpts.Costs.AgeCostBeginAge = AgeCostBeginAge;
            SimOpt.SimOpts.Costs.AllowMultiplerToGoNegative = AllowMultiplerToGoNegative;
            SimOpt.SimOpts.Costs.BasicCommandCost = BasicCommandCost;
            SimOpt.SimOpts.Costs.BodyUpkeepCost = BodyUpkeepCost;
            SimOpt.SimOpts.Costs.ZeroCostPopulationLimit = ZeroCostPopulationLimit;
            SimOpt.SimOpts.Costs.BitwiseCommandCost = BitwiseCommandCost;
            SimOpt.SimOpts.Costs.CholorplastCost = CholorplastCost;
            SimOpt.SimOpts.Costs.ConditionCost = ConditionCost;
            SimOpt.SimOpts.Costs.CostMultiplier = CostMultiplier;
            SimOpt.SimOpts.Costs.StoresCost = StoresCost;
            SimOpt.SimOpts.Costs.ReinstateCostPopulationLimit = ReinstateCostPopulationLimit;
            SimOpt.SimOpts.Costs.DnaCopyCost = DnaCopyCost;
            SimOpt.SimOpts.Costs.DnaUpkeepCost = DnaUpkeepCost;
            SimOpt.SimOpts.Costs.DotNumberCost = StarNumberCost;
            SimOpt.SimOpts.Costs.DynamicCostsIncludePlants = IncludeAllRobotsInPopulation;
            SimOpt.SimOpts.Costs.FlowCommandCost = FlowCommandCost;
            SimOpt.SimOpts.Costs.LogicCost = LogicCost;
            SimOpt.SimOpts.Costs.VoluntaryMovementCost = VoluntaryMovementCost;
            SimOpt.SimOpts.Costs.NumberCost = NumberCost;
            SimOpt.SimOpts.Costs.PoisonCost = PoisonCost;
            SimOpt.SimOpts.Costs.ShellCost = ShellCost;
            SimOpt.SimOpts.Costs.ShotFormationCost = ShotFormationCost;
            SimOpt.SimOpts.Costs.SlimeCost = SlimeCost;
            SimOpt.SimOpts.Costs.TieFormationCost = TieFormationCost;
            SimOpt.SimOpts.Costs.RotationCost = RotationCost;
            SimOpt.SimOpts.Costs.EnableDynamicCosts = EnableDynamicCosts;
            SimOpt.SimOpts.Costs.VenomCost = VenomCost;

            SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation = DynamicCostsTargetPopulation;
            SimOpt.SimOpts.Costs.DynamicCostsSensitivity = DynamicCostsSensitivity;
            SimOpt.SimOpts.Costs.DynamicCostsUpperRangeTarget = DynamicCostsUpperRangeTarget;
            SimOpt.SimOpts.Costs.DynamicCostsLowerRangeTarget = DynamicCostsLowerRangeTarget;
        }

        private void RestoreDefaults()
        {
            // TODO : Check these are the defaults
            // TODO : Set up the defaults for the rest of the properties

            NumberCost = 0;
            StarNumberCost = 0;
            BasicCommandCost = 0;
            AdvancedCommandCost = 0;
            BitwiseCommandCost = 0;
            ConditionCost = 0.004;
            LogicCost = 0;
            StoresCost = 0.04;
            CholorplastCost = 0.2;
            FlowCommandCost = 0;

            VoluntaryMovementCost = 0.05;
            RotationCost = 0;
            TieFormationCost = 2;
            ShotFormationCost = 2;
            DnaUpkeepCost = 0;
            DnaCopyCost = 0;
            VenomCost = 0.01;
            PoisonCost = 0.01;
            SlimeCost = 0.1;
            ShellCost = 0.1;
            BodyUpkeepCost = 0.00001;
            AgeCost = 0.01;

            AgeCostBeginAge = 32;
            EnableAgeCostIncreaseLog = false;
            ZeroCostPopulationLimit = 0;
            ReinstateCostPopulationLimit = 0;
            EnableDynamicCosts = false;
            CostMultiplier = 1;
        }
    }
}
