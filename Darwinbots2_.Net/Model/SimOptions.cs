using DarwinBots.Modules;
using System;
using System.Collections.Generic;

namespace DarwinBots.Model
{
    public class SimOptions
    {
        public SimOptions()
        {
            SetToDefaults();
        }

        public int BadWasteLevel { get; set; }

        public double CoefficientElasticity { get; set; }

        public double CoefficientKinetic { get; set; }

        public double CoefficientStatic { get; set; }

        public bool CorpseEnabled { get; set; }

        public Costs Costs { get; set; } = new();

        public int CycleLength { get; set; }

        public bool DayNight { get; set; }

        public int DayNightCycleCounter { get; set; }

        public bool Daytime { get; set; }

        public bool DeadRobotSnp { get; set; }

        public double Decay { get; set; }

        public int DecayDelay { get; set; }

        public DecayType DecayType { get; set; }

        public double Density { get; set; }

        public bool DisableFixing { get; set; }

        public bool DisableMutations { get; set; }

        public bool DisableTies { get; set; }

        public bool DisableTypArepro { get; set; }

        public bool DxSxConnected { get; set; }

        public bool EnableAutoSpeciation { get; set; }

        public ShotMode EnergyExType { get; set; }

        public int EnergyFix { get; set; }

        public double EnergyProp { get; set; }

        public int FieldHeight { get; set; }

        public int FieldWidth { get; set; }

        public bool FixedBotRadii { get; set; }

        public FieldMode FluidSolidCustom { get; set; }

        public double Gradient { get; set; }

        public int LightIntensity { get; set; }

        public int MaxAbsNum { get; set; }

        public int MaxEnergy { get; set; }

        public int MaxPopulation { get; set; }

        public double MaxVelocity { get; set; }

        public int MinVegs { get; set; }

        public double MutCurrMult { get; set; }

        public int MutCycMax { get; set; }

        public int MutCycMin { get; set; }

        public bool MutOscill { get; set; }

        public bool MutOscillSine { get; set; }

        public bool NoShotDecay { get; set; }

        public bool NoWShotDecay { get; set; }

        public double OldCostX { get; set; }

        public double PhysBrown { get; set; }

        public double PhysMoving { get; set; }

        public bool PlanetEaters { get; set; }

        public double PlanetEatersG { get; set; }

        public bool PondMode { get; set; }

        public int RepopAmount { get; set; }

        public int RepopCooldown { get; set; }

        public Guid SimGuid { get; set; }

        public bool SnpExcludeVegs { get; set; }

        public int SpeciationForkInterval { get; set; }

        public int SpeciationGeneticDistance { get; set; }

        public List<Species> Specie { get; } = new();

        public int StartChlr => 32000;

        public bool SunDown { get; set; }

        public int SunDownThreshold { get; set; }

        public bool SunOnRnd { get; set; }

        public SunThresholdMode SunThresholdMode { get; set; }

        public bool SunUp { get; set; }

        public int SunUpThreshold { get; set; }

        public int Tides { get; set; }

        public int TidesOf { get; set; }

        public int TotBorn { get; set; }

        public int TotRunCycle { get; set; }

        public bool UpDnConnected { get; set; }

        public double VegFeedingToBody { get; set; }

        public double Viscosity { get; set; }

        public double YGravity { get; set; }

        public bool ZeroMomentum { get; set; }

        public double ZGravity { get; set; }

        public void SetToDefaults()
        {
            BadWasteLevel = -1;
            CoefficientElasticity = 0;
            CoefficientKinetic = 0;
            CoefficientStatic = 0;
            CorpseEnabled = false;
            Costs = Costs.ZeroCosts with { CostMultiplier = 1, ZeroCostPopulationLimit = -1 };
            CycleLength = 0;
            DayNight = false;
            DayNightCycleCounter = 0;
            Daytime = true;
            DeadRobotSnp = false;
            Decay = 0;
            DecayType = DecayType.None;
            Density = 0;
            DisableFixing = false;
            DisableMutations = false;
            DisableTies = false;
            DisableTypArepro = false;
            DxSxConnected = false;
            EnableAutoSpeciation = false;
            EnergyExType = ShotMode.Proportional;
            EnergyFix = 200;
            FieldHeight = 12000;
            FieldWidth = 16000;
            FixedBotRadii = false;
            FluidSolidCustom = FieldMode.Custom;
            Gradient = 1.02;
            LightIntensity = 0;
            MaxAbsNum = 0;
            MaxEnergy = 100;
            MaxPopulation = 100;
            MaxVelocity = 60;
            MinVegs = 50;
            MutCurrMult = 1;
            MutCycMax = 0;
            MutCycMin = 0;
            MutOscill = false;
            MutOscillSine = false;
            NoShotDecay = false;
            NoWShotDecay = false;
            OldCostX = 1;
            PhysBrown = 0.5;
            PhysMoving = 0.66;
            PlanetEaters = false;
            PlanetEatersG = 0;
            PondMode = false;
            RepopAmount = 10;
            RepopCooldown = 10;
            SimGuid = Guid.Empty;
            SnpExcludeVegs = false;
            SpeciationForkInterval = 0;
            SpeciationGeneticDistance = 0;
            Specie.Clear();
            SunDown = false;
            SunDownThreshold = 1000000;
            SunOnRnd = false;
            SunThresholdMode = 0;
            SunUp = false;
            SunUpThreshold = 500000;
            Tides = 0;
            TidesOf = 0;
            TotBorn = 0;
            TotRunCycle = 0;
            UpDnConnected = false;
            VegFeedingToBody = 0.75;
            Viscosity = 0;
            YGravity = 0;
            ZeroMomentum = false;
            ZGravity = 0;
        }
    }
}
