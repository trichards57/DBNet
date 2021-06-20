﻿using System;

namespace DarwinBots.Modules
{
    public record Costs
    {
        public double AdvancedCommandCost { get; init; }
        public double AgeCost { get; init; }
        public int AgeCostBeginAge { get; init; }
        public double AgeCostIncreasePerCycle { get; init; }
        public bool AllowMultiplerToGoNegative { get; init; }
        public double BasicCommandCost { get; init; }
        public double BitwiseCommandCost { get; init; }
        public double BodyUpkeepCost { get; init; }
        public double CholorplastCost { get; init; }
        public double ConditionCost { get; init; }
        public double CostMultiplier { get; init; }
        public double DnaCopyCost { get; init; }
        public double DnaUpkeepCost { get; init; }
        public double DotNumberCost { get; init; }
        public bool DynamicCostsIncludePlants { get; init; }
        public int DynamicCostsLowerRangeTarget { get; init; }
        public double DynamicCostsSensitivity { get; init; }
        public int DynamicCostsTargetPopulation { get; init; }
        public int DynamicCostsUpperRangeTarget { get; init; }
        public bool EnableAgeCostIncreaseLog { get; init; }
        public bool EnableAgeCostIncreasePerCycle { get; init; }
        public bool EnableDynamicCosts { get; init; }
        public double FlowCommandCost { get; init; }
        public double LogicCost { get; init; }
        public double NumberCost { get; init; }
        public double PoisonCost { get; init; }
        public int ReinstateCostPopulationLimit { get; init; }
        public double RotationCost { get; init; }
        public double ShellCost { get; init; }
        public double ShotFormationCost { get; init; }
        public double SlimeCost { get; init; }
        public double StoresCost { get; init; }
        public double TieFormationCost { get; init; }
        public double VenomCost { get; init; }
        public double VoluntaryMovementCost { get; init; }
        public int ZeroCostPopulationLimit { get; init; }

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
