using DarwinBots.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PostSharp.Patterns.Model;
using System.Windows.Input;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged(ExcludeExplicitProperties = true)]
    internal class CostsViewModel : ObservableObject
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
        public double BasicCommandCost { get; set; }
        public double BitwiseCommandCost { get; set; }
        public double BodyUpkeepCost { get; set; }
        public double CholorplastCost { get; set; }
        public double ConditionCost { get; set; }
        public double CostMultiplier { get; set; }
        public double DnaCopyCost { get; set; }
        public double DnaUpkeepCost { get; set; }

        public bool EnableAgeCostIncreaseLog
        {
            get => _enableAgeCostIncreaseLog;
            set
            {
                SetProperty(ref _enableAgeCostIncreaseLog, value);
                if (EnableAgeCostIncreasePerCycle && value)
                    EnableAgeCostIncreasePerCycle = false;
            }
        }

        public bool EnableAgeCostIncreasePerCycle
        {
            get => _enableAgeCostIncreasePerCycle;
            set
            {
                SetProperty(ref _enableAgeCostIncreasePerCycle, value);
                if (EnableAgeCostIncreaseLog && value)
                    EnableAgeCostIncreaseLog = false;
            }
        }

        public double FlowCommandCost { get; set; }
        public double LogicCost { get; set; }
        public double NumberCost { get; set; }
        public double PoisonCost { get; set; }
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
            BasicCommandCost = costs.BasicCommandCost;
            BitwiseCommandCost = costs.BitwiseCommandCost;
            BodyUpkeepCost = costs.BodyUpkeepCost;
            CholorplastCost = costs.CholorplastCost;
            ConditionCost = costs.ConditionCost;
            CostMultiplier = costs.CostMultiplier;
            DnaCopyCost = costs.DnaCopyCost;
            DnaUpkeepCost = costs.DnaUpkeepCost;
            FlowCommandCost = costs.FlowCommandCost;
            LogicCost = costs.LogicCost;
            NumberCost = costs.NumberCost;
            PoisonCost = costs.PoisonCost;
            RotationCost = costs.RotationCost;
            ShellCost = costs.ShellCost;
            ShotFormationCost = costs.ShotFormationCost;
            SlimeCost = costs.SlimeCost;
            StarNumberCost = costs.DotNumberCost;
            StoresCost = costs.StoresCost;
            TieFormationCost = costs.TieFormationCost;
            VenomCost = costs.VenomCost;
            VoluntaryMovementCost = costs.VoluntaryMovementCost;
        }

        public Costs SaveToOptions()
        {
            return new()
            {
                AdvancedCommandCost = AdvancedCommandCost,
                AgeCost = AgeCost,
                AgeCostIncreasePerCycle = AgeCostIncreasePerCycle,
                EnableAgeCostIncreasePerCycle = EnableAgeCostIncreasePerCycle,
                EnableAgeCostIncreaseLog = EnableAgeCostIncreaseLog,
                AgeCostBeginAge = AgeCostBeginAge,
                BasicCommandCost = BasicCommandCost,
                BodyUpkeepCost = BodyUpkeepCost,
                BitwiseCommandCost = BitwiseCommandCost,
                CholorplastCost = CholorplastCost,
                ConditionCost = ConditionCost,
                CostMultiplier = CostMultiplier,
                StoresCost = StoresCost,
                DnaCopyCost = DnaCopyCost,
                DnaUpkeepCost = DnaUpkeepCost,
                DotNumberCost = StarNumberCost,
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
                VenomCost = VenomCost,
            };
        }

        private void RestoreDefaults()
        {
            LoadFromOptions(Costs.DefaultCosts);
        }
    }
}
