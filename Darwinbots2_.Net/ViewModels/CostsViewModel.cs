using GalaSoft.MvvmLight;

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
        private double _dynamicCostsLowerRangeTarget;
        private double _dynamicCostsSensitivity;
        private int _dynamicCostsTargetPopulation;
        private double _dynamicCostsUpperRangeTarget;
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
        public double DynamicCostsLowerRangeTarget { get => _dynamicCostsLowerRangeTarget; set { _dynamicCostsLowerRangeTarget = value; RaisePropertyChanged(); } }
        public double DynamicCostsSensitivity { get => _dynamicCostsSensitivity; set { _dynamicCostsSensitivity = value; RaisePropertyChanged(); } }
        public int DynamicCostsTargetPopulation { get => _dynamicCostsTargetPopulation; set { _dynamicCostsTargetPopulation = value; RaisePropertyChanged(); } }
        public double DynamicCostsUpperRangeTarget { get => _dynamicCostsUpperRangeTarget; set { _dynamicCostsUpperRangeTarget = value; RaisePropertyChanged(); } }
        public bool EnableAgeCostIncreaseLog { get => _enableAgeCostIncreaseLog; set { _enableAgeCostIncreaseLog = value; RaisePropertyChanged(); } }
        public bool EnableAgeCostIncreasePerCycle { get => _enableAgeCostIncreasePerCycle; set { _enableAgeCostIncreasePerCycle = value; RaisePropertyChanged(); } }
        public bool EnableDynamicCosts { get => _enableDynamicCosts; set { _enableDynamicCosts = value; RaisePropertyChanged(); } }
        public double FlowCommandCost { get => _flowCommandCost; set { _flowCommandCost = value; RaisePropertyChanged(); } }
        public bool IncludeAllRobotsInPopulation { get => _includeAllRobotsInPopulation; set { _includeAllRobotsInPopulation = value; RaisePropertyChanged(); } }
        public double LogicCost { get => _logicCost; set { _logicCost = value; RaisePropertyChanged(); } }
        public double NumberCost { get => _numberCost; set { _numberCost = value; RaisePropertyChanged(); } }
        public double PoisonCost { get => _poisonCost; set { _poisonCost = value; RaisePropertyChanged(); } }
        public int ReinstateCostPopulationLimit { get => _reinstateCostPopulationLimit; set { _reinstateCostPopulationLimit = value; RaisePropertyChanged(); } }
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
    }
}
