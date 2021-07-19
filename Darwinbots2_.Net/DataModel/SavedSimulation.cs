using DarwinBots.Model;
using DarwinBots.Modules;
using System;
using System.Collections.Generic;

namespace DarwinBots.DataModel
{
    internal class SavedSimulation
    {
        public bool AllowHorizontalShapeDrift { get; internal set; }
        public bool AllowVerticalShapeDrift { get; internal set; }
        public int BadWastelevel { get; internal set; }
        public double CoefficientElasticity { get; internal set; }
        public double CoefficientKinetic { get; internal set; }
        public double CoefficientStatic { get; internal set; }
        public bool CorpseEnabled { get; internal set; }
        public Costs Costs { get; internal set; }
        public int CycleLength { get; internal set; }
        public bool DayNight { get; internal set; }
        public int DayNightCycleCounter { get; internal set; }
        public bool Daytime { get; internal set; }
        public bool DeadRobotSnp { get; internal set; }
        public double Decay { get; internal set; }
        public int DecayDelay { get; internal set; }
        public DecayType DecayType { get; internal set; }
        public double Density { get; internal set; }
        public bool DisableFixing { get; internal set; }
        public bool DisableMutations { get; internal set; }
        public bool DisableTies { get; internal set; }
        public bool DisableTypArepro { get; internal set; }
        public bool DxSxConnected { get; internal set; }
        public bool EnableAutoSpeciation { get; internal set; }
        public ShotMode EnergyExType { get; internal set; }
        public int EnergyFix { get; internal set; }
        public double EnergyProp { get; internal set; }
        public int FieldHeight { get; internal set; }
        public int FieldWidth { get; internal set; }
        public bool FixedBotRadii { get; internal set; }
        public FieldMode FluidSolidCustom { get; internal set; }
        public double Gradient { get; internal set; }
        public int LightIntensity { get; internal set; }
        public bool MakeAllShapesBlack { get; internal set; }
        public int MaxAbsNum { get; internal set; }
        public int MaxEnergy { get; internal set; }
        public int MaxPopulation { get; internal set; }
        public double MaxVelocity { get; internal set; }
        public int MinVegs { get; internal set; }
        public double MutCurrMult { get; internal set; }
        public int MutCycMax { get; internal set; }
        public int MutCycMin { get; internal set; }
        public bool MutOscill { get; internal set; }
        public bool MutOscillSine { get; internal set; }
        public bool NoShotDecay { get; internal set; }
        public bool NoWShotDecay { get; internal set; }
        public IEnumerable<Obstacle> Obstacles { get; internal set; }
        public double OldCostX { get; internal set; }
        public double PhysBrown { get; internal set; }
        public double PhysMoving { get; internal set; }
        public bool PlanetEaters { get; internal set; }
        public double PlanetEatersG { get; internal set; }
        public bool Pondmode { get; internal set; }
        public int RepopAmount { get; internal set; }
        public int RepopCooldown { get; internal set; }
        public IEnumerable<Robot> Robots { get; set; }
        public int ShapeDriftRate { get; internal set; }
        public bool ShapesAbsorbShots { get; internal set; }
        public bool ShapesAreSeeThrough { get; internal set; }
        public bool ShapesAreVisable { get; internal set; }
        public IEnumerable<Shot> Shots { get; set; }
        public Guid SimGuid { get; internal set; }
        public bool SnpExcludeVegs { get; internal set; }
        public int SpeciationForkInterval { get; internal set; }
        public int SpeciationGeneticDistance { get; internal set; }
        public IEnumerable<Species> Species { get; internal set; }
        public double SunChange { get; internal set; }
        public bool SunDown { get; internal set; }
        public int SunDownThreshold { get; internal set; }
        public bool SunOnRnd { get; internal set; }
        public double SunPosition { get; internal set; }
        public double SunRange { get; internal set; }
        public SunThresholdMode SunThresholdMode { get; internal set; }
        public bool SunUp { get; internal set; }
        public int SunUpThreshold { get; internal set; }
        public int Tides { get; internal set; }
        public int TidesOf { get; internal set; }
        public int TotBorn { get; internal set; }
        public int TotRunCycle { get; internal set; }
        public bool UpDnConnected { get; internal set; }
        public double VegFeedingToBody { get; internal set; }
        public double Viscosity { get; internal set; }
        public double Ygravity { get; internal set; }
        public bool ZeroMomentum { get; internal set; }
        public double Zgravity { get; internal set; }
    }
}
