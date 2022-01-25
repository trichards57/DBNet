using DarwinBots.Modules;
using System.Collections.Generic;

namespace DarwinBots.Model
{
    internal class SimOptions
    {
        public const int StartChlr = 32000;

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

        public int SpeciationForkInterval { get; set; }

        public int SpeciationGeneticDistance { get; set; }

        public List<Species> Specie { get; } = new();
        public int TotBorn { get; set; }

        public int TotRunCycle { get; set; }

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
            Costs = Costs.ZeroCosts with { CostMultiplier = 1 };
            Decay = 0;
            DecayType = DecayType.None;
            Density = 0;
            DisableFixing = false;
            DisableMutations = false;
            DisableTies = false;
            DisableTypArepro = false;
            EnableAutoSpeciation = false;
            EnergyExType = ShotMode.Proportional;
            EnergyFix = 200;
            FieldHeight = 12000;
            FieldWidth = 16000;
            FixedBotRadii = false;
            FluidSolidCustom = FieldMode.Custom;
            MaxEnergy = 100;
            MaxPopulation = 100;
            MaxVelocity = 60;
            MinVegs = 50;
            MutCurrMult = 1;
            NoShotDecay = false;
            NoWShotDecay = false;
            PhysBrown = 0.5;
            PhysMoving = 0.66;
            RepopAmount = 10;
            RepopCooldown = 10;
            SpeciationForkInterval = 0;
            SpeciationGeneticDistance = 0;
            Specie.Clear();
            TotBorn = 0;
            TotRunCycle = 0;
            VegFeedingToBody = 0.75;
            Viscosity = 0;
            YGravity = 0;
            ZeroMomentum = false;
            ZGravity = 0;
        }
    }
}
