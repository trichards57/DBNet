using System;

namespace DBNetEngine
{
    public class SimulationOptions
    {
        private static Lazy<SimulationOptions> _instance = new Lazy<SimulationOptions>(() => new SimulationOptions());

        private SimulationOptions()
        {
        }

        public static SimulationOptions Instance => _instance.Value;

        public float AgeCost { get; set; }
        public bool AgeCostIsLinear { get; set; }
        public bool AgeCostIsLog { get; set; }
        public int AgeCostScalar { get; set; }
        public int AgeCostStart { get; set; }
        public float BadWasteLevel { get; set; }
        public float BodyFix { get; internal set; }
        public float BodyUpkeep { get; set; }
        public int BrownianMotion { get; set; }
        public float ChloroplastCost { get; set; }
        public bool CorpsesEnabled { get; set; }
        public float CostMultiplier { get; set; }
        public int Density { get; set; }
        public bool DisableFixing { get; set; }
        public bool DisableTies { get; set; }
        public float DnaCopyCost { get; set; }
        public int DnaCost { get; set; }
        public bool FixedBotRadii { get; set; }
        public float MaximumVelocity { get; set; }
        public bool Restart { get; internal set; }
        public float ShellCost { get; set; }
        public float SlimeCost { get; set; }
        public int Tides { get; set; }
        public int TidesCounter { get; set; }
        public int TotalRunCycle { get; set; }
        public float TurnCost { get; set; }
        public float YGravity { get; set; }
        public bool ZeroMomentum { get; set; }
        public int ShotCost { get; internal set; }
        public int VenomCost { get; internal set; }
        public float PoisonCost { get; internal set; }
        public bool DisableAsexualReproduction { get; internal set; }
        public int TotalBorn { get; internal set; }
        public bool Delta2 { get; internal set; }
        public int MaximumNormalisedMutation { get; internal set; }
        public bool NormaliseDNALength { get; internal set; }
        public int CurrentDNASize { get; internal set; }
        public bool YNormSize { get; internal set; }
        public int XRestartMode { get; internal set; }
    }
}