using DarwinBots.Model;
using DarwinBots.Modules;
using System.Collections.Generic;

namespace DarwinBots.DataModel
{
    internal record SavedSimulation
    {
        public int BadWastelevel { get; set; }
        public double CoefficientElasticity { get; set; }
        public double CoefficientKinetic { get; set; }
        public double CoefficientStatic { get; set; }
        public bool CorpseEnabled { get; set; }
        public Costs Costs { get; set; }
        public double Decay { get; set; }
        public int DecayDelay { get; set; }
        public DecayType DecayType { get; set; }
        public double Density { get; set; }
        public bool DisableFixing { get; set; }
        public bool DisableMutations { get; set; }
        public bool DisableTies { get; set; }
        public bool DisableTypArepro { get; set; }
        public bool EnableAutoSpeciation { get; set; }
        public ShotMode EnergyExType { get; set; }
        public int EnergyFix { get; set; }
        public double EnergyProp { get; set; }
        public int FieldHeight { get; set; }
        public int FieldWidth { get; set; }
        public bool FixedBotRadii { get; set; }
        public FieldMode FluidSolidCustom { get; set; }
        public int MaxEnergy { get; set; }
        public int MaxPopulation { get; set; }
        public double MaxVelocity { get; set; }
        public int MinVegs { get; set; }
        public double MutCurrMult { get; set; }
        public bool NoShotDecay { get; set; }
        public bool NoWShotDecay { get; set; }
        public double PhysBrown { get; set; }
        public double PhysMoving { get; set; }
        public int RepopAmount { get; set; }
        public int RepopCooldown { get; set; }
        public IEnumerable<Robot> Robots { get; set; }
        public IEnumerable<Shot> Shots { get; set; }
        public int SpeciationForkInterval { get; set; }
        public int SpeciationGeneticDistance { get; set; }
        public IEnumerable<Species> Species { get; set; }
        public int TotBorn { get; set; }
        public int TotRunCycle { get; set; }
        public double VegFeedingToBody { get; set; }
        public double Viscosity { get; set; }
        public double Ygravity { get; set; }
        public bool ZeroMomentum { get; set; }
        public double Zgravity { get; set; }
    }
}
