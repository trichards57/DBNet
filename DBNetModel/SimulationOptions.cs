using System;
using System.Collections.Generic;
using System.Text;

namespace DBNetModel
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
        public float BodyUpkeep { get; set; }
        public float ChloroplastCost { get; set; }
        public float CostMultiplier { get; set; }
        public float DnaCopyCost { get; set; }
        public int DnaCost { get; set; }
        public bool FixedBotRadii { get; set; }
        public float MaximumVelocity { get; set; }
        public float ShellCost { get; set; }
        public float SlimeCost { get; set; }
        public float TurnCost { get; set; }
        public bool ZeroMomentum { get; set; }
        public bool CorpsesEnabled { get; set; }
        public bool DisableTies { get; set; }
        public int Density { get; set; }
        public int Tides { get; set; }
        public int TotalRunCycle { get; set; }
        public int TidesCounter { get; set; }
        public float YGravity { get; set; }
        public int BrownianMotion { get; set; }
        public bool DisableFixing { get; set; }
        public float BodyFix { get; set; }
    }
}