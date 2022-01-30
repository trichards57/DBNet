namespace DarwinBots.Model
{
    internal record Costs
    {
        public double AdvancedCommandCost { get; init; }
        public double AgeCost { get; init; }
        public int AgeCostBeginAge { get; init; }
        public double AgeCostIncreasePerCycle { get; init; }
        public double BasicCommandCost { get; init; }
        public double BitwiseCommandCost { get; init; }
        public double BodyUpkeepCost { get; init; }
        public double CholorplastCost { get; init; }
        public double ConditionCost { get; init; }
        public double CostMultiplier { get; init; }
        public double DnaCopyCost { get; init; }
        public double DnaUpkeepCost { get; init; }
        public double DotNumberCost { get; init; }
        public bool EnableAgeCostIncreaseLog { get; init; }
        public bool EnableAgeCostIncreasePerCycle { get; init; }
        public double FlowCommandCost { get; init; }
        public double LogicCost { get; init; }
        public double NumberCost { get; init; }
        public double PoisonCost { get; init; }
        public double RotationCost { get; init; }
        public double ShellCost { get; init; }
        public double ShotFormationCost { get; init; }
        public double SlimeCost { get; init; }
        public double StoresCost { get; init; }
        public double TieFormationCost { get; init; }
        public double VenomCost { get; init; }
        public double VoluntaryMovementCost { get; init; }

        public static Costs ZeroCosts { get; } = new()
        {
            AdvancedCommandCost = 0,
            AgeCost = 0,
            AgeCostBeginAge = 0,
            AgeCostIncreasePerCycle = 0,
            BasicCommandCost = 0,
            BitwiseCommandCost = 0,
            BodyUpkeepCost = 0,
            CholorplastCost = 0,
            ConditionCost = 0,
            CostMultiplier = 0,
            DnaCopyCost = 0,
            DnaUpkeepCost = 0,
            DotNumberCost = 0,
            EnableAgeCostIncreaseLog = false,
            EnableAgeCostIncreasePerCycle = false,
            FlowCommandCost = 0,
            LogicCost = 0,
            NumberCost = 0,
            PoisonCost = 0,
            RotationCost = 0,
            ShellCost = 0,
            ShotFormationCost = 0,
            SlimeCost = 0,
            StoresCost = 0,
            TieFormationCost = 0,
            VenomCost = 0,
            VoluntaryMovementCost = 0,
        };

        public static Costs DefaultCosts { get; } = new()
        {
            AdvancedCommandCost = 0,
            AgeCost = 0.01,
            AgeCostBeginAge = 32,
            AgeCostIncreasePerCycle = 0,
            BasicCommandCost = 0,
            BitwiseCommandCost = 0,
            BodyUpkeepCost = 0.00001,
            CholorplastCost = 0.2,
            ConditionCost = 0.004,
            CostMultiplier = 1,
            DnaCopyCost = 0,
            DnaUpkeepCost = 0,
            DotNumberCost = 0,
            EnableAgeCostIncreaseLog = false,
            EnableAgeCostIncreasePerCycle = false,
            FlowCommandCost = 0,
            LogicCost = 0,
            NumberCost = 0,
            PoisonCost = 0.01,
            RotationCost = 0,
            ShellCost = 0.1,
            ShotFormationCost = 2,
            SlimeCost = 0.1,
            StoresCost = 0.04,
            TieFormationCost = 2,
            VenomCost = 0.01,
            VoluntaryMovementCost = 0.05,
        };
    }
}
