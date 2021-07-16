using DarwinBots.Modules;
using System;
using System.Collections.Generic;

namespace DarwinBots.Model
{
    public class SimOptions
    {
        public bool AllowHorizontalShapeDrift { get; set; }
        public bool AllowVerticalShapeDrift { get; set; }
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
        public bool MakeAllShapesBlack { get; set; }
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
        public int ShapeDriftRate { get; set; }
        public bool ShapesAbsorbShots { get; set; }
        public bool ShapesAreSeeThrough { get; set; }
        public bool ShapesAreVisable { get; set; }
        public Guid SimGuid { get; set; }
        public bool SnpExcludeVegs { get; set; }
        public int SpeciationForkInterval { get; set; }
        public int SpeciationGeneticDistance { get; set; }
        public List<Species> Specie { get; } = new();
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
    }
}
