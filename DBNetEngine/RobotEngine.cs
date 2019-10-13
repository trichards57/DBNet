using DBNetEngine.DNA;
using DBNetEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DBNetEngine
{
    internal class RobotEngine
    {
        /// <summary>
        /// The genetic sensitivity used for DNA comparison.
        /// </summary>
        public const int GeneticSensitivity = 75;

        /// <summary>
        /// The robot array maximum size
        /// </summary>
        /// <remarks>
        /// Was 'ROBARRAYMAX'
        /// </remarks>
        public const int RobotArrayMaxSize = 32000;

        public const int ShotArrayMaxSize = 32000;

        public int CurrentEnergyCycle { get; set; }

        /// <summary>
        /// Gets or sets the current reproducing robot, I think.
        /// </summary>
        /// <value>
        /// The current reproducing robot.
        /// </value>
        /// <remarks>
        /// Was 'rp'
        /// </remarks>
        public int CurrentReproducingRobot { get; set; }

        public Physics.Environment Environment { get; private set; }

        /// <summary>
        /// Gets or sets the current robot to kill.
        /// </summary>
        /// <value>
        /// The robot to  kill.
        /// </value>
        /// <remarks>
        /// Was 'kl'
        /// </remarks>
        public int Kill { get; set; }

        /// <summary>
        /// Gets or sets the maximum memory.
        /// </summary>
        /// <value>
        /// The maximum memory.
        /// </value>
        /// <remarks>
        /// Was 'MaxMem'
        /// </remarks>
        public int MaxMemory { get; set; }

        /// <summary>
        /// Gets or sets the maximum robots.
        /// </summary>
        /// <value>
        /// The maximum robots.
        /// </value>
        /// <remarks>
        /// Was 'MaxRobs'
        /// </remarks>
        public int MaxRobots { get; set; }

        public List<Obstacle> Obstacles { get; set; }

        public int Restarts { get; private set; }

        /// <summary>
        /// Gets or sets the robot reproduction array.
        /// </summary>
        /// <value>
        /// The robot reproduction array.
        /// </value>
        /// <remarks>
        /// Was 'rep'
        /// </remarks>
        public int[] RobotReproductionArray { get; set; } = new int[RobotArrayMaxSize];

        /// <summary>
        /// Gets or sets the robot array.
        /// </summary>
        /// <value>
        /// The robots.
        /// </value>
        /// <remarks>
        /// Was 'rob'
        /// </remarks>
        public List<Robot> Robots { get; set; }

        /// <summary>
        /// Gets or sets the robots to kill.
        /// </summary>
        /// <value>
        /// The robots to kill.
        /// </value>
        public int[] RobotsToKill { get; set; } = new int[RobotArrayMaxSize];

        /// <summary>
        /// Gets or sets the shots array.
        /// </summary>
        /// <value>
        /// The shots.
        /// </value>
        public List<Shot> Shots { get; set; }

        public List<Species> Species { get; set; }

        public bool StartAnotherRound { get; private set; }
        public List<Teleporter> Teleporters { get; set; }

        public int TotalCorpses { get; private set; }

        public float[] TotalEnergy { get; set; }
        public int TotalNotVegetables { get; private set; }

        /// <summary>
        /// Gets or sets the total robots.
        /// </summary>
        /// <value>
        /// The total robots.
        /// </value>
        public int TotalRobots { get; set; }

        public int TotalVegetables { get; private set; }

        public void RemoveExtinctSpecies()
        {
            Species = Species.Where(s => s.Population > 0 || s.Native).ToList();
        }

        public void UpdateCounters()
        {
            TotalRobots = Robots.Count(r => r.Exists);

            var speciesCounts = Robots.GroupBy(r => r.SpeciesName, (key, robots) => new KeyValuePair<string, int>(key, robots.Count()));

            foreach (var speciesCount in speciesCounts)
            {
                var species = Species.FirstOrDefault(s => s.Name == speciesCount.Key);
                if (species == null)
                {
                    species = new Species { Name = speciesCount.Key };
                    Species.Add(species);
                }
                species.Population = Math.Clamp(speciesCount.Value, 0, (int)MathSupport.MaxValue);
            }

            TotalVegetables = Robots.Count(r => r.Exists && r.IsVegetable);
            TotalNotVegetables = Robots.Count(r => r.Exists && !r.IsVegetable);
            TotalCorpses = Robots.Count(r => r.Exists && r.IsCorpse);

            foreach (var r in Robots.Where(r => r.Exists))
            {
                if (r.Body > 0)
                    r.Decay();
                else
                    r.Kill();
            }
        }

        public void UpdateRobots()
        {
            // TODO : Do some contest bits

            if (Teleporters.Any())
            {
                foreach (var rob in Robots.Where(r => r.Exists))
                    Teleporter.Check(Teleporters, rob);
            }

            if (SimulationOptions.Instance.Density != 0)
            {
                foreach (var rob in Robots.Where(r => r.Exists))
                    rob.CalculateAddedMass();
            }

            if (SimulationOptions.Instance.Tides == 0)
            {
                Environment.BouyancyScaling = 1;
            }
            else
            {
                Environment.BouyancyScaling = (float)Math.Sqrt(1 + Math.Sin((SimulationOptions.Instance.TotalRunCycle + SimulationOptions.Instance.TidesCounter) % SimulationOptions.Instance.Tides));
                SimulationOptions.Instance.YGravity = (1 - Environment.BouyancyScaling) * 4;
                SimulationOptions.Instance.BrownianMotion = Environment.BouyancyScaling > 0.8 ? 10 : 0;
            }

            foreach (var rob in Robots.Where(r => r.Exists))
            {
                if (!rob.IsCorpse)
                    rob.ProcessUpkeep();

                if (!rob.IsCorpse && !rob.DNAIsDisabled)
                    rob.HandlePoisons();

                if (!SimulationOptions.Instance.DisableFixing)
                    rob.ManageFixed();

                rob.CalculateMass();
                if (Obstacles.Any())
                    Obstacle.DoCollisions(Obstacles, rob);

                Environment.DoBorderCollisions(rob);

                if (rob.StaticImpulse > 0 && rob.IndependentImpulse != Vector2.Zero)
                {
                    float staticForce;

                    if (rob.Velocity == Vector2.Zero)
                        staticForce = rob.StaticImpulse;
                    else
                        staticForce = rob.StaticImpulse * Math.Abs(Vector2.Normalize(rob.Velocity).CrossProduct(Vector2.Normalize(rob.IndependentImpulse)));

                    if (staticForce > rob.IndependentImpulse.Length())
                        rob.IndependentImpulse = Vector2.Zero;
                }

                rob.IndependentImpulse -= rob.ResistiveImpulse;

                if (rob.IsCorpse && !rob.DNAIsDisabled)
                {
                    Tie.TransferTiesData(rob);
                    Tie.ReadTies(rob);
                }
            }

            foreach (var s in Species)
                s.Population = 0;

            UpdateCounters();

            foreach (var rob in Robots.Where(r => r.Exists))
            {
                Tie.UpdateTies(rob);

                if (rob.Age < 15)
                    rob.TransferGeneticMemory();

                if (!rob.IsCorpse && !rob.DNAIsDisabled)
                {
                    rob.SetAim();
                    rob.DNAManipulation();
                }

                rob.UpdatePosition();

                rob.Energy = Math.Clamp(rob.Energy, -MathSupport.MaxValue, MathSupport.MaxValue);
                rob.Poison = Math.Clamp(rob.Poison, 0, MathSupport.MaxValue);
                rob.Venom = Math.Clamp(rob.Venom, 0, MathSupport.MaxValue);
                rob.Waste = Math.Clamp(rob.Waste, 0, MathSupport.MaxValue);
            }

            foreach (var rob in Robots.Where(r => r.Exists && ((r.Chloroplasts < r.Body / 2) || r.Kills > 5) && r.Body > SimulationOptions.Instance.BodyFix))
                rob.Kill();

            foreach (var rob in Robots)
            {
                Tie.UpdateTieAngles(rob);

                if (!rob.IsCorpse && !rob.DNAIsDisabled && rob.Exists)
                {
                    Mutations.Mutate(rob);
                    rob.MakeStuff();
                    rob.HandleWaste();
                    rob.HandleShooting();
                    if (!rob.HasNoChloroplasts)
                        rob.HandleChloroplasts();
                    rob.ManageBody();
                    rob.ManageBouyancy();
                    rob.ManageReproduction();
                    rob.HandleShock();
                    rob.WriteSenses();
                    rob.FireTies();
                }
                if (!rob.IsCorpse && rob.Exists)
                {
                    rob.HandleAgeing();
                    rob.ManageDeath();
                }
                if (rob.Exists)
                    TotalEnergy[CurrentEnergyCycle] += rob.Energy + rob.Body * 10;
            }

            ReproduceAndKill();
            RemoveExtinctSpecies();

            if (TotalNotVegetables == 0 && SimulationOptions.Instance.Restart)
            {
                Restarts++;
                StartAnotherRound = true;
            }
        }

        internal void Reproduce(Robot rob, int energyPercent)
        {
            energyPercent %= 100;

            if ((SimulationOptions.Instance.DisableAsexualReproduction && !rob.IsVegetable)
                || !rob.Exists || rob.Body < 5 || rob.CantReproduce || energyPercent <= 0
                || rob.Energy <= 0)
                return;

            // There is are limits here to prevent reproduction of vegs if there are too many
            // There is a limit here to kill robots that set energyPercent to less than 3

            var childDistance = rob.GetRadius(energyPercent / 100.0f) + rob.GetRadius(1.0f - energyPercent / 100.0f);
            var tempEnergy = rob.Energy;
            var newPosition = rob.Position + Robot.GetAbsoluteAcceleration(rob.Aim, childDistance, 0, 0, 0);

            if (!SimpleCollision(rob, newPosition))
            {
                var child = CreateRobot();

                SimulationOptions.Instance.TotalBorn++;

                child.DNA = rob.DNA.Select(d => new Block(d.Type, d.Value)).ToList();
                child.DNALength = rob.DNALength;
                child.GenesCount = rob.GenesCount;
                child.MutationProbabilities = rob.MutationProbabilities.Clone();
                child.MutationsCount = rob.MutationsCount;
                child.OldMutationsCount = rob.OldMutationsCount;
                child.LastMutation = 0;
                child.LastMutationDetail = rob.LastMutationDetail;
                child.UsedVariables = rob.UsedVariables.Clone() as int[];
                child.Skin = rob.Skin.Clone() as int[];
                child.MaxUsedVariables = rob.MaxUsedVariables;

                // Skipped erasing memory and ties, this is done by the runtime.

                child.Position = newPosition;
                child.Exists = true;
                child.BucketPosition = new Vector2(-2, -2);
                child.UpdateBotBucket();
                child.Velocity = rob.Velocity;
                child.ActualVelocity = rob.ActualVelocity;
                child.Color = rob.Color;
                child.Aim = MathSupport.AngleNormalise(rob.Aim + (float)Math.PI);
                child.AimVector = new Vector2((float)Math.Cos(child.Aim), (float)Math.Sin(child.Aim));
                child.Memory[SystemVariableAddresses.SetAim] = MathSupport.AngleToInteger(child.Aim);
                child.Memory[SystemVariableAddresses.FixedAngle] = (int)MathSupport.MaxValue;
                child.IsCorpse = false;
                child.Dead = false;
                child.UsesNewMovement = rob.UsesNewMovement;
                child.Generation = Math.Min(rob.Generation + 1, (int)MathSupport.MaxValue);
                child.BirthCycle = SimulationOptions.Instance.TotalRunCycle;
                child.VNumber = 1;

                var proportion = energyPercent / 100.0f;

                var newEnergy = rob.Energy * proportion;
                var newBody = rob.Body * proportion;
                var newWaste = rob.Waste * proportion;
                var newPermamentWaste = rob.PermanentWaste * proportion;
                var newChloroplasts = rob.Chloroplasts * proportion;

                rob.Energy -= 1.001f * newEnergy;
                rob.Waste -= newWaste;
                rob.PermanentWaste -= newPermamentWaste;
                rob.Body -= newBody;
                rob.Chloroplasts -= newChloroplasts;
                rob.Memory[SystemVariableAddresses.Energy] = (int)rob.Energy;
                rob.Memory[SystemVariableAddresses.Body] = (int)rob.Body;

                child.Chloroplasts = newChloroplasts;
                child.Body = newBody;
                child.Waste = newWaste;
                child.PermanentWaste = newPermamentWaste;
                child.Energy = newEnergy * 0.999f;
                child.OldEnergy = child.Energy;
                child.Memory[SystemVariableAddresses.Energy] = (int)child.Energy;
                child.Memory[SystemVariableAddresses.Body] = (int)child.Body;

                child.IsPoisoned = false;
                child.Parent = rob;
                child.SpeciesName = rob.SpeciesName;
                child.LastOwner = rob.LastOwner;
                child.IsVegetable = rob.IsVegetable;
                child.HasNoChloroplasts = rob.HasNoChloroplasts;
                child.IsFixed = rob.IsFixed;
                child.CantSee = rob.CantSee;
                child.DNAIsDisabled = rob.DNAIsDisabled;
                child.DisableMovementSystemVariables = rob.DisableMovementSystemVariables;
                child.CantReproduce = rob.CantReproduce;
                child.IsVirusImmune = rob.IsVirusImmune;
                if (child.IsFixed)
                    child.Memory[SystemVariableAddresses.FixedPosition] = 1;
                child.SubSpecies = rob.SubSpecies;

                child.OldGeneticDistance = rob.OldGeneticDistance;
                child.MutationsBeforeNextTest = rob.MutationsBeforeNextTest;
                child.Tag = rob.Tag;
                child.Bouyancy = rob.Bouyancy;

                if (rob.MultiBotTime > 0)
                    child.MultiBotTime = rob.MultiBotTime / 2 + 2;

                child.Dq = rob.Dq;

                child.VirusTimer = 0;
                child.VirusShot = 0;

                for (var i = 0; i < 5; i++)
                    child.Memory[971 + i] = rob.Memory[971 + i];

                for (var i = 0; i < 15; i++)
                {
                    child.EpiMemory[i] = rob.Memory[976 + i];
                    rob.EpiMemory[i] = 0;
                }

                if (SimulationOptions.Instance.Delta2)
                {
                    var mRatesMax = SimulationOptions.Instance.NormaliseDNALength ? child.DNALength * SimulationOptions.Instance.MaximumNormalisedMutation : 2000000000;
                    var dynamicMutationOverloadCorrection = Math.Max(0.001, 1 + (child.DNALength - SimulationOptions.Instance.CurrentDNASize) / 500);
                    if (!SimulationOptions.Instance.YNormSize)
                        dynamicMutationOverloadCorrection = 1;

                    if (SimulationOptions.Instance.XRestartMode == 7 || SimulationOptions.Instance.XRestartMode == 8)
                    {
                        if (child.SpeciesName == "Mutate.txt")
                        {
                        }
                    }
                }
            }
        }

        private Robot CreateRobot()
        {
            throw new NotImplementedException();
        }

        private void ReproduceAndKill()
        {
            var random = new Random();

            foreach (var rob in Robots.Where(r => r.AsexualReproduction))
            {
                int energyPercentage;

                if (rob.Memory[SystemVariableAddresses.MutatingReproduce] > 0 && rob.Memory[SystemVariableAddresses.Reproduce] > 0)
                    energyPercentage = (random.NextDouble() > 0.5) ? rob.Memory[SystemVariableAddresses.Reproduce] : rob.Memory[SystemVariableAddresses.MutatingReproduce];
                else if (rob.Memory[SystemVariableAddresses.MutatingReproduce] > 0)
                    energyPercentage = rob.Memory[SystemVariableAddresses.MutatingReproduce];
                else
                    energyPercentage = rob.Memory[SystemVariableAddresses.Reproduce];

                Reproduce(rob, energyPercentage);
            }

            foreach (var rob in Robots.Where(r => r.SexualReproduction))
                rob.ReproduceSexually();

            foreach (var rob in Robots.Where(r => r.Dead))
                rob.KillRobot();
        }

        private bool SimpleCollision(Robot rob, Vector2 newPosition)
        {
            throw new NotImplementedException();
        }
    }
}