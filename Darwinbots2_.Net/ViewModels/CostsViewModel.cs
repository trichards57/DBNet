using DarwinBots.Modules;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PostSharp.Patterns.Model;
using System.Windows.Input;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged]
    internal class CostsViewModel : ViewModelBase
    {
        private bool _enableAgeCostIncreaseLog;
        private bool _enableAgeCostIncreasePerCycle;

        public CostsViewModel()
        {
            RestoreDefaultsCommand = new RelayCommand(RestoreDefaults);
        }

        public double AdvancedCommandCost { get; set; }
        public double AgeCost { get; set; }
        public int AgeCostBeginAge { get; set; }
        public double AgeCostIncreasePerCycle { get; set; }
        public bool AllowMultiplerToGoNegative { get; set; }
        public double BasicCommandCost { get; set; }
        public double BitwiseCommandCost { get; set; }
        public double BodyUpkeepCost { get; set; }
        public double CholorplastCost { get; set; }
        public double ConditionCost { get; set; }
        public double CostMultiplier { get; set; }
        public double DnaCopyCost { get; set; }
        public double DnaUpkeepCost { get; set; }
        public int DynamicCostsLowerRangeTarget { get; set; }
        public double DynamicCostsSensitivity { get; set; }
        public int DynamicCostsTargetPopulation { get; set; }
        public int DynamicCostsUpperRangeTarget { get; set; }

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

        public bool EnableDynamicCosts { get; set; }
        public double FlowCommandCost { get; set; }
        public bool IncludeAllRobotsInPopulation { get; set; }
        public double LogicCost { get; set; }
        public double NumberCost { get; set; }
        public double PoisonCost { get; set; }
        public int ReinstateCostPopulationLimit { get; set; }
        public ICommand RestoreDefaultsCommand { get; }
        public double RotationCost { get; set; }
        public double ShellCost { get; set; }
        public double ShotFormationCost { get; set; }
        public double SlimeCost { get; set; }
        public double StarNumberCost { get; set; }
        public double StoresCost { get; set; }
        public double TieFormationCost { get; set; }
        public double VenomCost { get; set; }
        public double VoluntaryMovementCost { get; set; }
        public int ZeroCostPopulationLimit { get; set; }

        public void LoadFromOptions(Costs costs)
        {
            if (costs.EnableAgeCostIncreaseLog && costs.EnableAgeCostIncreasePerCycle)
            {
                EnableAgeCostIncreaseLog = false;
                EnableAgeCostIncreasePerCycle = false;
            }
            else
            {
                EnableAgeCostIncreaseLog = costs.EnableAgeCostIncreaseLog;
                EnableAgeCostIncreasePerCycle = costs.EnableAgeCostIncreasePerCycle;
            }
            AdvancedCommandCost = costs.AdvancedCommandCost;
            AgeCost = costs.AgeCost;
            AgeCostBeginAge = costs.AgeCostBeginAge;
            AgeCostIncreasePerCycle = costs.AgeCostIncreasePerCycle;
            AllowMultiplerToGoNegative = costs.AllowMultiplerToGoNegative;
            BasicCommandCost = costs.BasicCommandCost;
            BitwiseCommandCost = costs.BitwiseCommandCost;
            BodyUpkeepCost = costs.BodyUpkeepCost;
            CholorplastCost = costs.CholorplastCost;
            ConditionCost = costs.ConditionCost;
            CostMultiplier = costs.CostMultiplier;
            DnaCopyCost = costs.DnaCopyCost;
            DnaUpkeepCost = costs.DnaUpkeepCost;
            EnableDynamicCosts = costs.EnableDynamicCosts;
            FlowCommandCost = costs.FlowCommandCost;
            IncludeAllRobotsInPopulation = costs.DynamicCostsIncludePlants;
            LogicCost = costs.LogicCost;
            NumberCost = costs.NumberCost;
            PoisonCost = costs.PoisonCost;
            ReinstateCostPopulationLimit = costs.ReinstateCostPopulationLimit;
            RotationCost = costs.RotationCost;
            ShellCost = costs.ShellCost;
            ShotFormationCost = costs.ShotFormationCost;
            SlimeCost = costs.SlimeCost;
            StarNumberCost = costs.DotNumberCost;
            StoresCost = costs.StoresCost;
            TieFormationCost = costs.TieFormationCost;
            VenomCost = costs.VenomCost;
            VoluntaryMovementCost = costs.VoluntaryMovementCost;
            ZeroCostPopulationLimit = costs.ZeroCostPopulationLimit;
            DynamicCostsTargetPopulation = costs.DynamicCostsTargetPopulation;
            DynamicCostsSensitivity = costs.DynamicCostsSensitivity;
            DynamicCostsUpperRangeTarget = costs.DynamicCostsUpperRangeTarget;
            DynamicCostsLowerRangeTarget = (int)costs.DynamicCostsLowerRangeTarget;
        }

        public Costs SaveOptions()
        {
            return new Costs
            {
                AdvancedCommandCost = AdvancedCommandCost,
                AgeCost = AgeCost,
                AgeCostIncreasePerCycle = AgeCostIncreasePerCycle,
                EnableAgeCostIncreasePerCycle = EnableAgeCostIncreasePerCycle,
                EnableAgeCostIncreaseLog = EnableAgeCostIncreaseLog,
                AgeCostBeginAge = AgeCostBeginAge,
                AllowMultiplerToGoNegative = AllowMultiplerToGoNegative,
                BasicCommandCost = BasicCommandCost,
                BodyUpkeepCost = BodyUpkeepCost,
                ZeroCostPopulationLimit = ZeroCostPopulationLimit,
                BitwiseCommandCost = BitwiseCommandCost,
                CholorplastCost = CholorplastCost,
                ConditionCost = ConditionCost,
                CostMultiplier = CostMultiplier,
                StoresCost = StoresCost,
                ReinstateCostPopulationLimit = ReinstateCostPopulationLimit,
                DnaCopyCost = DnaCopyCost,
                DnaUpkeepCost = DnaUpkeepCost,
                DotNumberCost = StarNumberCost,
                DynamicCostsIncludePlants = IncludeAllRobotsInPopulation,
                FlowCommandCost = FlowCommandCost,
                LogicCost = LogicCost,
                VoluntaryMovementCost = VoluntaryMovementCost,
                NumberCost = NumberCost,
                PoisonCost = PoisonCost,
                ShellCost = ShellCost,
                ShotFormationCost = ShotFormationCost,
                SlimeCost = SlimeCost,
                TieFormationCost = TieFormationCost,
                RotationCost = RotationCost,
                EnableDynamicCosts = EnableDynamicCosts,
                VenomCost = VenomCost,
                DynamicCostsTargetPopulation = DynamicCostsTargetPopulation,
                DynamicCostsSensitivity = DynamicCostsSensitivity,
                DynamicCostsUpperRangeTarget = DynamicCostsUpperRangeTarget,
                DynamicCostsLowerRangeTarget = DynamicCostsLowerRangeTarget,
            };
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
