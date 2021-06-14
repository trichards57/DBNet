using System;

namespace Iersera.Modules
{
    public struct Costs
    {
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
        public double DotNumberCost { get; set; }
        public bool DynamicCostsIncludePlants { get; set; }
        public int DynamicCostsLowerRangeTarget { get; set; }
        public double DynamicCostsSensitivity { get; set; }
        public int DynamicCostsTargetPopulation { get; set; }
        public int DynamicCostsUpperRangeTarget { get; set; }
        public bool EnableAgeCostIncreaseLog { get; set; }
        public bool EnableAgeCostIncreasePerCycle { get; set; }
        public bool EnableDynamicCosts { get; set; }
        public double FlowCommandCost { get; set; }
        public double LogicCost { get; set; }
        public double NumberCost { get; set; }
        public double PoisonCost { get; set; }
        public int ReinstateCostPopulationLimit { get; set; }
        public double RotationCost { get; set; }
        public double ShellCost { get; set; }
        public double ShotFormationCost { get; set; }
        public double SlimeCost { get; set; }
        public double StoresCost { get; set; }
        public double TieFormationCost { get; set; }
        public double VenomCost { get; set; }
        public double VoluntaryMovementCost { get; set; }
        public int ZeroCostPopulationLimit { get; set; }

        public static Costs ZeroCosts()
        {
            return new Costs
            {
                AdvancedCommandCost = 0,
                AgeCost = 0,
                AgeCostIncreasePerCycle = 0,
                EnableAgeCostIncreasePerCycle = false,
                EnableAgeCostIncreaseLog = false,
                AgeCostBeginAge = 0,
                AllowMultiplerToGoNegative = false,
                BasicCommandCost = 0,
                BodyUpkeepCost = 0,
                ZeroCostPopulationLimit = 0,
                BitwiseCommandCost = 0,
                CholorplastCost = 0,
                ConditionCost = 0,
                CostMultiplier = 0,
                StoresCost = 0,
                ReinstateCostPopulationLimit = 0,
                DnaCopyCost = 0,
                DnaUpkeepCost = 0,
                DotNumberCost = 0,
                DynamicCostsIncludePlants = false,
                FlowCommandCost = 0,
                LogicCost = 0,
                VoluntaryMovementCost = 0,
                NumberCost = 0,
                PoisonCost = 0,
                ShellCost = 0,
                ShotFormationCost = 0,
                SlimeCost = 0,
                TieFormationCost = 0,
                RotationCost = 0,
                EnableDynamicCosts = false,
                VenomCost = 0,
                DynamicCostsTargetPopulation = 0,
                DynamicCostsSensitivity = 0,
                DynamicCostsUpperRangeTarget = 0,
                DynamicCostsLowerRangeTarget = 0,
            };
        }

        [Obsolete("Use direct property access")]
        public double this[int index] => index switch
        {
            SimOpt.ADCMDCOST => AdvancedCommandCost,
            SimOpt.AGECOST => AgeCost,
            SimOpt.AGECOSTLINEARFRACTION => AgeCostIncreasePerCycle,
            SimOpt.AGECOSTMAKELINEAR => EnableAgeCostIncreasePerCycle ? 1 : 0,
            SimOpt.AGECOSTMAKELOG => EnableAgeCostIncreaseLog ? 1 : 0,
            SimOpt.AGECOSTSTART => AgeCostBeginAge,
            SimOpt.ALLOWNEGATIVECOSTX => AllowMultiplerToGoNegative ? 1 : 0,
            SimOpt.BCCMDCOST => BasicCommandCost,
            SimOpt.BODYUPKEEP => BodyUpkeepCost,
            SimOpt.BOTNOCOSTLEVEL => ZeroCostPopulationLimit,
            SimOpt.BTCMDCOST => BitwiseCommandCost,
            SimOpt.CHLRCOST => CholorplastCost,
            SimOpt.CONDCOST => ConditionCost,
            SimOpt.COSTMULTIPLIER => CostMultiplier,
            SimOpt.COSTSTORE => StoresCost,
            SimOpt.COSTXREINSTATEMENTLEVEL => ReinstateCostPopulationLimit,
            SimOpt.DNACOPYCOST => DnaCopyCost,
            SimOpt.DNACYCCOST => DnaUpkeepCost,
            SimOpt.DOTNUMCOST => DotNumberCost,
            SimOpt.DYNAMICCOSTINCLUDEPLANTS => DynamicCostsIncludePlants ? 1 : 0,
            SimOpt.FLOWCOST => FlowCommandCost,
            SimOpt.LOGICCOST => LogicCost,
            SimOpt.MOVECOST => VoluntaryMovementCost,
            SimOpt.NUMCOST => NumberCost,
            SimOpt.POISONCOST => PoisonCost,
            SimOpt.SHELLCOST => ShellCost,
            SimOpt.SHOTCOST => ShotFormationCost,
            SimOpt.SLIMECOST => SlimeCost,
            SimOpt.TIECOST => TieFormationCost,
            SimOpt.TURNCOST => RotationCost,
            SimOpt.USEDYNAMICCOSTS => EnableDynamicCosts ? 1 : 0,
            SimOpt.VENOMCOST => VenomCost,
            SimOpt.DYNAMICCOSTTARGET => DynamicCostsTargetPopulation,
            SimOpt.DYNAMICCOSTSENSITIVITY => DynamicCostsSensitivity,
            SimOpt.DYNAMICCOSTTARGETUPPERRANGE => DynamicCostsUpperRangeTarget,
            SimOpt.DYNAMICCOSTTARGETLOWERRANGE => DynamicCostsLowerRangeTarget,
            _ => throw new InvalidOperationException(),
        };
    }
}
