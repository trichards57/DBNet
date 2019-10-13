using DBNetEngine.DNA;
using DBNetEngine.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace DBNetEngine
{
    /// <summary>
    /// Represents a robot in the simulation
    /// </summary>
    /// TODO : Update the properties to auto-update their memory addresses, or do all updates at the same time
    public class Robot : Item
    {
        /// <summary>
        /// The cubic twip per body
        /// </summary>
        /// <remarks>
        /// Gives a radius of 60 for a bot of 1000 body.
        /// </remarks>
        public const int CubicTwipPerBody = 905;

        /// <summary>
        /// The half robot size.
        /// </summary>
        /// <remarks>
        /// Was 'half'
        /// </remarks>
        public const int HalfRobotSize = RobotSize / 2;

        public const int MemoryMaximum = 1000;

        /// <summary>
        /// The robot size in fixed diameter sims.
        /// </summary>
        /// <remarks>
        /// Was 'RobSize'
        /// </remarks>
        public const int RobotSize = 120;

        private const float EnergyPerPoison = 0.25f;
        private const float EnergyPerShell = 0.1f;
        private const float EnergyPerSlime = 0.1f;
        private const float EnergyPerVenom = 1;
        private int _age;
        private float _body;
        private float _chloroplasts;
        private int _fertilizedCounter;
        private float _mass;
        private float _permanentWaste;
        private float _poison;
        private float _shell;
        private float _slime;
        private float _venom;
        private float _waste;

        /// <summary>
        /// Gets or sets this instance's absolute number.
        /// </summary>
        /// <value>
        /// The absolute number.
        /// </value>
        /// <remarks>
        /// Was 'AbsNum'
        /// </remarks>
        public int AbsoluteNumber { get; set; }

        /// <summary>
        /// Gets or sets the robot's actual velocity (considering collisions).
        /// </summary>
        /// <value>
        /// The actual velocity.
        /// </value>
        /// <remarks>
        /// Was 'actvel'
        /// </remarks>
        public Vector2 ActualVelocity { get; set; }

        /// <summary>
        /// Gets or sets the added mass from fluid displacement.
        /// </summary>
        /// <value>
        /// The added mass.
        /// </value>
        public float AddedMass { get; set; }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                Memory[SystemVariableAddresses.RobotAge] = Math.Clamp(_age, 0, (int)MathSupport.MaxValue);
            }
        }

        /// <summary>
        /// Gets or sets the current aim angle.
        /// </summary>
        /// <value>
        /// The aim angle.
        /// </value>
        public float Aim { get; set; }

        /// <summary>
        /// Gets or sets the current aim as a unit vector.
        /// </summary>
        /// <value>
        /// The aim unit vector.
        /// </value>
        public Vector2 AimVector { get; set; }

        /// <summary>
        /// Gets or sets the angular momentum.
        /// </summary>
        /// <value>
        /// The angular momentum.
        /// </value>
        /// <remarks>
        /// Was 'ma'
        /// </remarks>
        public float AngularMomentum { get; set; }

        /// <summary>
        /// Gets a value indicating whether the robot is attempting asexual reproduction.
        /// </summary>
        /// <value>
        ///   <c>true</c> if attempting asexual reproduction; otherwise, <c>false</c>.
        /// </value>
        public bool AsexualReproduction { get; private set; }

        /// <summary>
        /// Gets or sets the birth cycle.
        /// </summary>
        /// <value>
        /// The birth cycle.
        /// </value>
        public int BirthCycle { get; set; }

        /// <summary>
        /// Gets or sets the number of body points.
        /// </summary>
        /// <value>
        /// The number of body points.
        /// </value>
        public float Body
        {
            get => _body;
            set
            {
                _body = Math.Clamp(value, 0, MathSupport.MaxValue);
                Memory[SystemVariableAddresses.Body] = (int)_body;
            }
        }

        /// <summary>
        /// Gets or sets the bouyancy of this instance.
        /// </summary>
        /// <value>
        /// The bouyancy.
        /// </value>
        public float Bouyancy { get; set; }

        /// <summary>
        /// Gets or sets the bucket position of this instance.
        /// </summary>
        /// <value>
        /// The bucket position.
        /// </value>
        /// <remarks>
        /// Was 'BucketPos'
        /// </remarks>
        public Vector2 BucketPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the robot can't reproduce.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it can't reproduce; otherwise, <c>false</c>.
        /// </value>
        public bool CantReproduce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this robot can't see.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it can't see; otherwise, <c>false</c>.
        /// </value>
        public bool CantSee { get; set; }

        /// <summary>
        /// Gets or sets the number of chloroplasts.
        /// </summary>
        /// <value>
        /// The number of chloroplasts.
        /// </value>
        public float Chloroplasts
        {
            get => _chloroplasts;
            set
            {
                _chloroplasts = value;
                Memory[SystemVariableAddresses.Chloroplasts] = (int)_chloroplasts;
            }
        }

        /// <summary>
        /// Gets or sets the chloroplasts share delay.
        /// </summary>
        /// <value>
        /// The chloroplasts share delay.
        /// </value>
        /// <remarks>
        /// Was 'Chlr_Share_Delay'
        /// </remarks>
        public int ChloroplastsShareDelay { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the conditions count.  Used for cost calculations.
        /// </summary>
        /// <value>
        /// The conditions count.
        /// </value>
        /// <remarks>
        /// Was 'condnum'
        /// </remarks>
        public int ConditionsCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Robot"/> is dead.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dead; otherwise, <c>false</c>.
        /// </value>
        public bool Dead { get; set; }

        /// <summary>
        /// Gets or sets the debug string.
        /// </summary>
        /// <value>
        /// The debug string.
        /// </value>
        /// <remarks>
        /// Was 'dbgstring'
        /// </remarks>
        public string DebugString { get; set; }

        /// <summary>
        /// Gets or sets the decay timer.
        /// </summary>
        /// <value>
        /// The decay timer.
        /// </value>
        public float DecayTimer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the movement system variables are disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the movement system variables are disabled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'DisableMovementSysvars'
        /// </remarks>
        public bool DisableMovementSystemVariables { get; set; }

        /// <summary>
        /// Gets or sets the dna.
        /// </summary>
        /// <value>
        /// The dna.
        /// </value>
        public IList<Block> DNA { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dna is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dna is disabled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'DisableDNA'
        /// </remarks>
        public bool DNAIsDisabled { get; set; }

        /// <summary>
        /// Gets or sets the length of the dna.
        /// </summary>
        /// <value>
        /// The length of the dna.
        /// </value>
        /// <remarks>
        /// Was 'DnaLen'
        /// </remarks>
        public int DNALength { get; set; }

        /// <summary>
        /// Gets or sets the dq.
        /// </summary>
        /// <value>
        /// The dq.
        /// </value>
        /// TODO : Get a better name
        public int Dq { get; set; }

        /// <summary>
        /// Gets or sets the current energy.
        /// </summary>
        /// <value>
        /// The energy.
        /// </value>
        /// <remarks>
        /// Was 'nrg'
        /// </remarks>
        public float Energy { get; set; }

        /// <summary>
        /// Gets or sets the environment details around the robot.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public Physics.Environment Environment { get; set; }

        /// <summary>
        /// Gets or sets the epi memory.
        /// </summary>
        /// <value>
        /// The epi memory.
        /// </value>
        /// <remarks>
        /// Was 'epimem'
        /// </remarks>
        public int[] EpiMemory { get; set; } = new int[14];

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Robot"/> exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if exists; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'exist'
        /// </remarks>
        public bool Exists { get; set; }

        /// <summary>
        /// Gets or sets the fertilized counter.
        /// </summary>
        /// <value>
        /// The fertilized counter.
        /// </value>
        /// <remarks>
        /// Was 'fertilized'
        /// </remarks>
        public int FertilizedCounter
        {
            get => _fertilizedCounter;
            set
            {
                _fertilizedCounter = value;
                Memory[SystemVariableAddresses.Fertilized] = Math.Max(0, _fertilizedCounter);
            }
        }

        /// <summary>
        /// Gets or sets the gene activation state.
        /// </summary>
        /// <value>
        /// The gene activation.
        /// </value>
        /// <remarks>
        /// Was 'ga'
        /// </remarks>
        public bool[] GeneActivation { get; set; }

        /// <summary>
        /// Gets or sets the generation.
        /// </summary>
        /// <value>
        /// The generation.
        /// </value>
        public int Generation { get; set; }

        /// <summary>
        /// Gets or sets the number of genes.
        /// </summary>
        /// <value>
        /// The genes count.
        /// </value>
        /// <remarks>
        /// Was 'genenum'
        /// </remarks>
        public int GenesCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has no chloroplasts.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has no chloroplasts; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'NoChlr'
        /// </remarks>
        public bool HasNoChloroplasts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has attempted to see things.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has viewed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'View'
        /// </remarks>
        public bool HasViewed { get; set; }

        /// <summary>
        /// Gets or sets the independent impulse acting on this instance.
        /// </summary>
        /// <value>
        /// The independent impulse.
        /// </value>
        /// <remarks>
        /// Was 'ImpulseInd'
        /// </remarks>
        public Vector2 IndependentImpulse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a corpse.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a corpse; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'Corpse'
        /// </remarks>
        public bool IsCorpse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed or has been blocked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fixed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'Fixed'
        /// </remarks>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is part of a multi bot.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is part of a multi bot; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'Multibot'
        /// </remarks>
        public bool IsMultiBot { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is paralyzed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is paralyzed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'paralyzed'
        /// </remarks>
        public bool IsParalyzed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is poisoned.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is poisoned; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'Poisoned'
        /// </remarks>
        public bool IsPoisoned { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a vegetable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a vegetable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'Veg'
        /// </remarks>
        public bool IsVegetable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is virus immune.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is virus immune; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'VirusImmune'
        /// </remarks>
        public bool IsVirusImmune { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a wall.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a wall; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'wall'
        /// </remarks>
        public bool IsWall { get; set; }

        /// <summary>
        /// Gets or sets the number of kills performed by this robot.
        /// </summary>
        /// <value>
        /// The kills.
        /// </value>
        public int Kills { get; set; }

        /// <summary>
        /// Gets or sets the last down movement value.
        /// </summary>
        /// <value>
        /// The last down.
        /// </value>
        public int LastDown { get; set; }

        /// <summary>
        /// Gets or sets the last left movement value.
        /// </summary>
        /// <value>
        /// The last left.
        /// </value>
        public int LastLeft { get; set; }

        /// <summary>
        /// Gets or sets the last mutation.
        /// </summary>
        /// <value>
        /// The last mutation.
        /// </value>
        /// <remarks>
        /// Was 'LastMut'
        /// </remarks>
        public int LastMutation { get; set; }

        /// <summary>
        /// Gets or sets a description of the last mutation details.
        /// </summary>
        /// <value>
        /// The last mutation detail.
        /// </value>
        /// <remarks>
        /// Was 'LastMutDetail'
        /// </remarks>
        public string LastMutationDetail { get; set; }

        /// <summary>
        /// Gets or sets the last object seen in the focus eye.
        /// </summary>
        /// <value>
        /// The last object.
        /// </value>
        /// <remarks>
        /// Was 'lastopp'
        /// </remarks>
        public Item LastObject { get; set; }

        /// <summary>
        /// Gets or sets the position of <see cref="LastObject"/>.
        /// </summary>
        /// <value>
        /// The position of <see cref="LastObject"/>.
        /// </value>
        public Vector2 LastObjectPosition { get; set; }

        /// <summary>
        /// Gets or sets the type of <see cref="LastObject"/>.
        ///
        /// 0: Bot
        /// 1: Shape
        /// 2: Edge of the field
        /// </summary>
        /// <value>
        /// The type of <see cref="LastObject"/>.
        /// </value>
        /// <remarks>
        /// Was 'lastopptype'
        /// </remarks>
        /// TODO : Replace this with an enum
        public int LastObjectType { get; set; }

        /// <summary>
        /// Gets or sets the last owner's name.
        /// </summary>
        /// <value>
        /// The last owner.
        /// </value>
        public string LastOwner { get; set; }

        /// <summary>
        /// Gets or sets the last right movement value.
        /// </summary>
        /// <value>
        /// The last right.
        /// </value>
        public int LastRight { get; set; }

        /// <summary>
        /// Gets or sets the robot that last touched this instance.
        /// </summary>
        /// <value>
        /// The last touched robot.
        /// </value>
        /// <remarks>
        /// Was 'lasttch'
        /// </remarks>
        public Robot LastTouched { get; set; }

        /// <summary>
        /// Gets or sets the last up movement value.
        /// </summary>
        /// <value>
        /// The last up.
        /// </value>
        public int LastUp { get; set; }

        /// <summary>
        /// Gets or sets the mass of the robot.
        /// </summary>
        /// <value>
        /// The mass.
        /// </value>
        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                Memory[SystemVariableAddresses.Mass] = (int)Mass;
            }
        }

        /// <summary>
        /// Gets or sets the maximum used variables.
        /// </summary>
        /// <value>
        /// The maximum used variables.
        /// </value>
        /// <remarks>
        /// Was 'maxusedvars'
        /// </remarks>
        public int MaxUsedVariables { get; set; }

        /// <summary>
        /// Gets or sets the memory array.
        /// </summary>
        /// <value>
        /// The memory array.
        /// </value>
        /// <remarks>
        /// Was 'mem'
        /// </remarks>
        public int[] Memory { get; set; } = new int[MemoryMaximum];

        /// <summary>
        /// Gets or sets the mt ('torch').
        /// </summary>
        /// <value>
        /// The mt.
        /// </value>
        /// <remarks>
        /// Was 'mt'
        /// </remarks>
        /// TODO : Get a better name
        public float Mt { get; set; }

        /// <summary>
        /// Gets or sets the multi bot time.
        /// </summary>
        /// <value>
        /// The multi bot time.
        /// </value>
        /// <remarks>
        /// Was 'multibot_time'
        /// </remarks>
        public int MultiBotTime { get; set; }

        /// <summary>
        /// Gets or sets the mutation probabilities.
        /// </summary>
        /// <value>
        /// The mutation probabilities.
        /// </value>
        /// <remarks>
        /// Was 'mutables'
        /// </remarks>
        public MutationProbabilities MutationProbabilities { get; set; }

        /// <summary>
        /// Gets or sets the number of mutations before next test.
        /// </summary>
        /// <value>
        /// The mutations before next test.
        /// </value>
        /// <remarks>
        /// Was 'GenMut'
        /// </remarks>
        public float MutationsBeforeNextTest { get; set; }

        /// <summary>
        /// Gets or sets the mutations count.
        /// </summary>
        /// <value>
        /// The mutations count.
        /// </value>
        /// <remarks>
        /// Was 'Mutations'
        /// </remarks>
        public int MutationsCount { get; set; }

        /// <summary>
        /// Gets or sets the mutations until epigenetic reset.
        /// </summary>
        /// <value>
        /// The mutations until epigenetic reset.
        /// </value>
        /// <remarks>
        /// Was 'MutEpiReset'
        /// </remarks>
        public float MutationsUntilEpigeneticReset { get; set; }

        /// <summary>
        /// Gets or sets the age from this simulation
        /// </summary>
        /// <value>
        /// The new age.
        /// </value>
        public int NewAge { get; set; }

        /// <summary>
        /// Gets or sets the next point mutation cycle.
        /// </summary>
        /// <value>
        /// The next point mutation cycle.
        /// </value>
        /// <remarks>
        /// Was 'PointMutCycle'
        /// </remarks>
        public int NextPointMutationCycle { get; set; }

        /// <summary>
        /// Gets or sets the number of ties attached to this instance.
        /// </summary>
        /// <value>
        /// The number of ties.
        /// </value>
        /// <remarks>
        /// Was 'numties'
        /// </remarks>
        public float NumberOfTies { get; set; }

        /// <summary>
        /// Gets or sets the array with the ref* values
        /// </summary>
        /// TODO : Get a better name
        public int[] Occurr { get; set; } = new int[20];

        /// <summary>
        /// Gets or sets the old aim angle.
        /// </summary>
        /// <value>
        /// The old aim angle.
        /// </value>
        /// <remarks>
        /// Was 'oaim'
        /// </remarks>
        public float OldAim { get; set; }

        /// <summary>
        /// Gets or sets the previous of body points.
        /// </summary>
        /// <value>
        /// The previous of body points.
        /// </value>
        /// <remarks>
        /// Was 'obody'
        /// </remarks>
        public float OldBody { get; set; }

        /// <summary>
        /// Gets or sets the old bot number.
        /// </summary>
        /// <value>
        /// The old bot number.
        /// </value>
        /// <remarks>
        /// Was 'oldBotNum'
        /// </remarks>
        public int OldBotNumber { get; set; }

        /// <summary>
        /// Gets or sets the old energy.
        /// </summary>
        /// <value>
        /// The old energy.
        /// </value>
        /// <remarks>
        /// Was 'onrg'
        /// </remarks>
        public float OldEnergy { get; set; }

        /// <summary>
        /// Gets or sets the old genetic distance.
        /// </summary>
        /// <value>
        /// The old genetic distance.
        /// </value>
        /// <remarks>
        /// Was 'OldGD'
        /// </remarks>
        public float OldGeneticDistance { get; set; }

        /// <summary>
        /// Gets or sets the old mutations count from the DNA file.
        /// </summary>
        /// <value>
        /// The old mutations count.
        /// </value>
        /// <remarks>
        /// Was 'OldMutations'
        /// </remarks>
        public int OldMutationsCount { get; set; }

        /// <summary>
        /// Gets or sets the old skin.
        /// </summary>
        /// <value>
        /// The old skin.
        /// </value>
        /// <remarks>
        /// Was 'OSkin'
        /// </remarks>
        public int[] OldSkin { get; set; } = new int[13];

        /// <summary>
        /// Gets or sets the o position.  Is used in <see cref="ActualVelocity"/> calculation.
        /// </summary>
        /// <value>
        /// The o position.
        /// </value>
        /// <remarks>
        /// Was 'opos'
        /// </remarks>
        /// TODO : Give a better name
        public Vector2 OPos { get; set; }

        /// <summary>
        /// Gets or sets the order in the roborder array.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the paralyzed countdown.
        /// </summary>
        /// <value>
        /// The paralyzed countdown.
        /// </value>
        /// <remarks>
        /// Was 'Paracount'
        /// </remarks>
        public float ParalyzedCountdown { get; set; }

        /// <summary>
        /// Gets or sets the parent robot.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public Robot Parent { get; set; }

        /// <summary>
        /// Gets or sets the amount of permanent waste.
        /// </summary>
        /// <value>
        /// The amount of permanent waste.
        /// </value>
        /// <remarks>
        /// Was 'pwaste'
        /// </remarks>
        public float PermanentWaste
        {
            get => _permanentWaste;
            set
            {
                _permanentWaste = value;
                Memory[SystemVariableAddresses.PermamentWaste] = (int)_permanentWaste;
            }
        }

        /// <summary>
        /// Gets or sets the point2 mutation cycle.
        /// </summary>
        /// <value>
        /// The point2 mutation cycle.
        /// </value>
        /// <remarks>
        /// Was 'Point2MutCycle'
        /// </remarks>
        public long Point2MutationCycle { get; set; }

        /// <summary>
        /// Gets or sets the base pair to apply the point mutation to.
        /// </summary>
        /// <value>
        /// The point mutation base pair.
        /// </value>
        /// <remarks>
        /// Was 'PointMutBP'
        /// </remarks>
        public int PointMutationBasePair { get; set; }

        /// <summary>
        /// Gets or sets the amount of poison.
        /// </summary>
        /// <value>
        /// The amount of poison.
        /// </value>
        public float Poison
        {
            get => _poison;
            set
            {
                _poison = value;
                Memory[SystemVariableAddresses.Poison] = (int)_poison;
            }
        }

        /// <summary>
        /// Gets or sets the poison countdown.
        /// </summary>
        /// <value>
        /// The poison countdown.
        /// </value>
        /// <remarks>
        /// Was 'Poisoncount'
        /// </remarks>
        public float PoisonCountdown { get; set; }

        /// <summary>
        /// Gets or sets the location custom poison will act on.
        /// </summary>
        /// <value>
        /// The poison location.
        /// </value>
        /// <remarks>
        /// Was 'Ploc'
        /// </remarks>
        public int PoisonLocation { get; set; }

        /// <summary>
        /// Gets or sets the value custom poison will insert.
        /// </summary>
        /// <value>
        /// The poison value.
        /// </value>
        /// <remarks>
        /// Was 'Pval'
        /// </remarks>
        public int PoisonValue { get; set; }

        /// <summary>
        /// Gets or sets the position of this instance.
        /// </summary>
        /// <value>
        /// The position of the robot.
        /// </value>
        /// <remarks>
        /// Was 'pos'
        /// </remarks>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the reproduction timer.
        /// </summary>
        /// <value>
        /// The reproduction timer.
        /// </value>
        /// <remarks>
        /// Was 'reproTimer'
        /// </remarks>
        public int ReproductionTimer { get; set; }

        /// <summary>
        /// Gets or sets the resistive impulse acting on this instance.
        /// </summary>
        /// <value>
        /// The resistive impulse.
        /// </value>
        /// <remarks>
        /// Was 'ImpulseRes'
        /// </remarks>
        public Vector2 ResistiveImpulse { get; set; }

        /// <summary>
        /// Gets a value indicating whether the robot is attempting sexual reproduction.
        /// </summary>
        /// <value>
        ///   <c>true</c> if attempting sexual reproduction; otherwise, <c>false</c>.
        /// </value>
        public bool SexualReproduction { get; private set; }

        /// <summary>
        /// Gets or sets the number of shell points for the robot.
        /// </summary>
        /// <value>
        /// The number of shell points.
        /// </value>
        public float Shell
        {
            get => _shell;
            set
            {
                _shell = value;
                Memory[SystemVariableAddresses.Shell] = (int)_shell;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the sim this came from.
        /// </summary>
        /// <value>
        /// The sim ID.
        /// </value>
        /// <remarks>
        /// Was a long
        /// </remarks>
        public Guid Sim { get; set; }

        /// <summary>
        /// Gets or sets the skin.
        /// </summary>
        /// <value>
        /// The skin.
        /// </value>
        public int[] Skin { get; set; } = new int[13];

        /// <summary>
        /// Gets or sets the slime layer size.
        /// </summary>
        /// <value>
        /// The slime layer size.
        /// </value>
        public float Slime
        {
            get => _slime;
            set
            {
                _slime = value;
                Memory[SystemVariableAddresses.Slime] = (int)_slime;
            }
        }

        /// <summary>
        /// Gets or sets the son count.
        /// </summary>
        /// <value>
        /// The son count.
        /// </value>
        /// <remarks>
        /// Was 'SonNumber'
        /// </remarks>
        public int SonCount { get; set; }

        /// <summary>
        /// Gets or sets the name of the species.
        /// </summary>
        /// <value>
        /// The name of the species.
        /// </value>
        /// <remarks>
        /// Was 'FName'
        /// </remarks>
        public string SpeciesName { get; set; }

        /// <summary>
        /// Gets or sets the sperm dna.
        /// </summary>
        /// <value>
        /// The sperm dna.
        /// </value>
        public IList<DNA.Block> SpermDNA { get; set; }

        /// <summary>
        /// Gets or sets the length of the sperm dna.
        /// </summary>
        /// <value>
        /// The length of the sperm dna.
        /// </value>
        /// <remarks>
        /// Was 'spermDNAlen'
        /// </remarks>
        public int SpermDNALength { get; set; }

        /// <summary>
        /// Gets or sets the static impulse acting on this instance.  Always acts against the current force.
        /// </summary>
        /// <value>
        /// The static impulse.
        /// </value>
        /// <remarks>
        /// Was 'ImpulseStatic'
        /// </remarks>
        public float StaticImpulse { get; set; }

        /// <summary>
        /// Gets or sets the sub species.
        /// </summary>
        /// <value>
        /// The sub species.
        /// </value>
        public int SubSpecies { get; set; }

        /// <summary>
        /// Gets or sets the system variables.
        /// </summary>
        /// <value>
        /// The system variables.
        /// </value>
        /// <remarks>
        /// Was 'vars'
        /// </remarks>
        public Dictionary<string, int> SystemVariables { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets if the tie angle can be overwritten.
        /// </summary>
        /// <remarks>
        /// Was 'Tieangoverwrite'
        /// </remarks>
        /// TODO : Give a better name and check purpose
        public bool[] TieAngleOverwrite { get; set; } = new bool[3];

        /// <summary>
        /// Gets or sets if the tie length can be overwritten.
        /// </summary>
        /// <remarks>
        /// Was 'Tielenoverwrite'
        /// </remarks>
        /// TODO : Give a better name and check purpose
        public bool[] TieLengthOverwrite { get; set; } = new bool[3];

        /// <summary>
        /// Gets or sets the current ties.
        /// </summary>
        /// <value>
        /// The current ties.
        /// </value>
        public IList<Tie> Ties { get; set; }

        /// <summary>
        /// Gets or sets the used variables.
        /// </summary>
        /// <value>
        /// The used variables.
        /// </value>
        /// <remarks>
        /// Was 'usedvars'
        /// </remarks>
        public int[] UsedVariables { get; set; } = new int[1000];

        /// <summary>
        /// Gets or sets a value indicating whether this instance uses the new movement algorithm.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it uses the new movement algorithm; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Was 'NewMove'
        /// </remarks>
        public bool UsesNewMovement { get; set; }

        /// <summary>
        /// Gets or sets the velocity of this instance.
        /// </summary>
        /// <value>
        /// The velocity.
        /// </value>
        /// <remarks>
        /// Was 'vel'
        /// </remarks>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Gets or sets the amount of venom.
        /// </summary>
        /// <value>
        /// The amount of venom.
        /// </value>
        public float Venom
        {
            get => _venom;
            set
            {
                _venom = value;
                Memory[SystemVariableAddresses.Venom] = (int)_venom;
            }
        }

        /// <summary>
        /// Gets or sets the location custom venom will act on.
        /// </summary>
        /// <value>
        /// The venom location.
        /// </value>
        /// <remarks>
        /// Was 'Vloc'
        /// </remarks>
        public int VenomLocation { get; set; }

        /// <summary>
        /// Gets or sets the value custom venom will insert.
        /// </summary>
        /// <value>
        /// The venom value.
        /// </value>
        /// <remarks>
        /// Was 'Vval'
        /// </remarks>
        public int VenomValue { get; set; }

        /// <summary>
        /// Gets or sets the virtual body used to calculate body functions of MBs.
        /// </summary>
        /// <value>
        /// The virtual body.
        /// </value>
        /// <remarks>
        /// Was 'vbody'
        /// </remarks>
        public float VirtualBody { get; set; }

        /// <summary>
        /// Gets or sets the virus shot being stored.
        /// </summary>
        /// <value>
        /// The virus shot.
        /// </value>
        public int VirusShot { get; set; }

        /// <summary>
        /// Gets or sets the countdown to a virus being available.
        /// </summary>
        /// <value>
        /// The virus timer.
        /// </value>
        /// <remarks>
        /// Was 'Vtimer'
        /// </remarks>
        public int VirusTimer { get; set; }

        /// <summary>
        /// Gets or sets sometimes to do with variables.
        /// </summary>
        /// <remarks>
        /// Was 'vnum'
        /// </remarks>
        /// TODO : Get a better name
        public int VNumber { get; set; }

        /// <summary>
        /// Gets or sets the waste level.
        /// </summary>
        /// <value>
        /// The waste.
        /// </value>
        public float Waste
        {
            get => _waste;
            set
            {
                _waste = value;
                Memory[SystemVariableAddresses.Waste] = (int)_waste;
            }
        }

        private static Random Random { get; } = new Random();

        /// <summary>
        /// Gets the robot's absolute acceleration based on up-down and left-right.
        /// </summary>
        /// <returns>A <see cref="Vector2"/> for the acceleration.</returns>
        /// <remarks>
        /// Was 'absx' and 'absy'
        /// </remarks>
        public static Vector2 GetAbsoluteAcceleration(float aim, float up, float down, float left, float right)
        {
            var upTotal = up - down;
            var leftTotal = left - right;

            if (upTotal > MathSupport.MaxValue) upTotal = (int)MathSupport.MaxValue;
            if (upTotal < -MathSupport.MaxValue) upTotal = (int)-MathSupport.MaxValue;
            if (leftTotal > MathSupport.MaxValue) leftTotal = (int)MathSupport.MaxValue;
            if (leftTotal < -MathSupport.MaxValue) leftTotal = (int)-MathSupport.MaxValue;

            var x = Math.Cos(aim) * upTotal + Math.Sin(aim) * leftTotal;
            var y = -Math.Sin(aim) * upTotal + Math.Cos(aim) * leftTotal;

            return new Vector2((float)x, (float)y);
        }

        public void CalculateAddedMass()
        {
            throw new NotImplementedException();
        }

        public void CalculateMass()
        {
            throw new NotImplementedException();
        }

        public void Decay()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs the DNA Manipulation routines (viruses, gene deletion).
        /// </summary>
        public void DNAManipulation()
        {
            if (VirusTimer > 1)
                VirusTimer -= 1;
            Memory[SystemVariableAddresses.VirusTimer] = VirusTimer;

            if (Memory[SystemVariableAddresses.MakeVirus] > 0 && VirusTimer == 0)
            {
                if (Chloroplasts == 0)
                {
                    if (MakeVirus(Memory[SystemVariableAddresses.MakeVirus]))
                    {
                        var length = DNAHelper.GeneLength(this, Memory[SystemVariableAddresses.MakeVirus]) * 2;
                        Energy -= (length / 2) * SimulationOptions.Instance.DnaCopyCost * SimulationOptions.Instance.DnaCost;

                        VirusTimer = (int)Math.Min(MathSupport.MaxValue, length);
                    }
                    else
                    {
                        VirusTimer = 0;
                        VirusShot = 0;
                    }
                }
                else
                {
                    Chloroplasts = 0;
                }
            }

            if (Memory[SystemVariableAddresses.VirusShoot] != 0 && VirusTimer == 1)
            {
                if (VirusShot > 0)
                    Shot.VirusShoot(this);

                Memory[SystemVariableAddresses.VirusShoot] = 0;
                Memory[SystemVariableAddresses.VirusTimer] = 0;
                Memory[SystemVariableAddresses.MakeVirus] = 0;
                VirusTimer = 0;
                VirusShot = 0;
            }

            if (Memory[SystemVariableAddresses.DeleteGene] > 0)
            {
                DNAHelper.DeleteGene(this, Memory[SystemVariableAddresses.DeleteGene]);
                Memory[SystemVariableAddresses.DeleteGene] = 0;
            }

            Memory[SystemVariableAddresses.DNALength] = DNALength;
            Memory[SystemVariableAddresses.Genes] = GenesCount;
        }

        /// <summary>
        /// Handles the robot requesting a tie
        /// </summary>
        public void FireTies()
        {
            var resetLastOpp = false;

            if (LastObject == null && Age < 2 && Parent?.Exists == true)
            {
                LastObject = Parent;
                resetLastOpp = true;
            }

            if (LastObject == null && LastTouched?.Exists == true)
            {
                LastObject = LastTouched;
                resetLastOpp = true;
            }

            if (Memory[SystemVariableAddresses.MakeTie] != 0)
            {
                if (LastObject is Robot otherRobot && !SimulationOptions.Instance.DisableTies)
                {
                    var length = (otherRobot.Position - Position).Length();

                    if (length < RobotSize * 4 + GetRadius() + otherRobot.GetRadius())
                        Tie.CreateTie(this, LastObject as Robot, length, -20, Memory[SystemVariableAddresses.MakeTie]);
                }

                Memory[SystemVariableAddresses.MakeTie] = 0;
            }

            if (resetLastOpp)
                LastObject = null;

            // TODO : In some circumstances, making ties causes a disqualification.
        }

        /// <summary>
        /// Gets the robot's radius.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns>Radius based on body and cholorplasts</returns>
        /// <remarks>
        /// Was 'FindRadius'
        /// </remarks>
        public float GetRadius(float multiplier = 1)
        {
            if (SimulationOptions.Instance.FixedBotRadii)
                return HalfRobotSize;

            float bodyPoints;
            float chloroplasts;

            if (multiplier == -1)
            {
                bodyPoints = MathSupport.MaxValue;
                chloroplasts = 0;
            }
            else
            {
                bodyPoints = Body * multiplier;
                chloroplasts = Chloroplasts * multiplier;
            }

            if (bodyPoints < 1) bodyPoints = 1;

            var radius = Math.Pow((Math.Log(bodyPoints) * bodyPoints * CubicTwipPerBody * 3 * 0.25 / Math.PI), 1 / 3);
            radius += (415 - radius) * chloroplasts / MathSupport.MaxValue;
            if (radius < 1) radius = 1;

            return (float)radius;
        }

        /// <summary>
        /// Handles the ageing process in this instance.
        /// </summary>
        public void HandleAgeing()
        {
            Age++;
            NewAge++;

            Memory[SystemVariableAddresses.Timer]++;
            if (Memory[SystemVariableAddresses.Timer] > MathSupport.MaxValue)
                Memory[SystemVariableAddresses.Timer] = -(int)MathSupport.MaxValue;
        }

        public void HandleChloroplasts()
        {
            if (Memory[SystemVariableAddresses.MakeChloroplasts] > 0 || Memory[SystemVariableAddresses.RemoveChloroplasts] > 0)
                MakeChloroplasts();

            Chloroplasts -= Math.Clamp((float)(0.5 / (Math.Pow(100, Chloroplasts / 16000))), 0, MathSupport.MaxValue);

            Memory[SystemVariableAddresses.Light] = (int)(MathSupport.MaxValue - (Environment.LightAvailable * MathSupport.MaxValue));
        }

        /// <summary>
        /// Handles the robot being poisoned.
        /// </summary>
        public void HandlePoisons()
        {
            if (IsParalyzed)
            {
                Memory[VenomLocation] = VenomValue;
                ParalyzedCountdown -= 1;
                if (ParalyzedCountdown < 1)
                {
                    IsParalyzed = false;
                    VenomLocation = 0;
                    VenomValue = 0;
                }
            }

            Memory[SystemVariableAddresses.Poison] = (int)PoisonCountdown;
        }

        /// <summary>
        /// Handles a robot experiencing shock if body is changed too fast.
        /// </summary>
        public void HandleShock()
        {
            var change = OldEnergy - Energy;
            if (change > (OldEnergy / 2))
            {
                Energy = 0;
                // This is a translation of the old code
                // TODO : Try and work out what this was supposed to do and replace it
                Body += Energy / 10;
            }
        }

        /// <summary>
        /// Handles the robot shooting.
        /// </summary>
        public void HandleShooting()
        {
            if (Memory[SystemVariableAddresses.Shoot] > 0)
                Shoot();

            Memory[SystemVariableAddresses.Shoot] = 0;
        }

        /// <summary>
        /// Handles waste defecation and high waste effects.
        /// </summary>
        public void HandleWaste()
        {
            if (Waste > 0 && Chloroplasts > 0) FeedVegetable2();
            if (SimulationOptions.Instance.BadWasteLevel == 0)
                SimulationOptions.Instance.BadWasteLevel = 400;
            if (SimulationOptions.Instance.BadWasteLevel > 0 && (PermanentWaste + Waste) > SimulationOptions.Instance.BadWasteLevel)
                ApplyAlzheimers();
            if (Waste > MathSupport.MaxValue)
                Defacate();
            if (PermanentWaste > MathSupport.MaxValue)
                PermanentWaste = MathSupport.MaxValue;
            if (Waste < 0)
                Waste = 0;
        }

        public void Kill()
        {
            throw new NotImplementedException();
        }

        public void MakeStuff()
        {
            if (Memory[SystemVariableAddresses.MakeVenom] != 0) StoreVenom();
            if (Memory[SystemVariableAddresses.MakePoison] != 0) StorePoison();
            if (Memory[SystemVariableAddresses.MakeShell] != 0) MakeShell();
            if (Memory[SystemVariableAddresses.MakeSlime] != 0) MakeSlime();
        }

        /// <summary>
        /// Handles body management (creating body, feeding it, etc.)
        /// </summary>
        public void ManageBody()
        {
            if (Memory[SystemVariableAddresses.StoreBody] > 0)
                StoreBody();
            if (Memory[SystemVariableAddresses.FeedBody] > 0)
                FeedBody();
        }

        /// <summary>
        /// Manages changes to the robot's bouyancy.
        /// </summary>
        public void ManageBouyancy()
        {
            if (Memory[SystemVariableAddresses.SetBouyancy] != 0)
            {
                Bouyancy += Math.Clamp(Memory[SystemVariableAddresses.SetBouyancy] / MathSupport.MaxValue, 0, 1);
                Memory[SystemVariableAddresses.ReadBouyancy] = (int)(Bouyancy * MathSupport.MaxValue);
                Memory[SystemVariableAddresses.SetBouyancy] = 0;
            }
        }

        /// <summary>
        /// Manages the death of the robot.
        /// </summary>
        /// <remarks>
        /// The previous version added the robot to a list
        /// of robots to kill, which I don't think is needed
        /// but might be added later.
        /// </remarks>
        public void ManageDeath()
        {
            if (SimulationOptions.Instance.CorpsesEnabled)
            {
                if (!IsCorpse && Energy < 15 && Age > 0)
                {
                    IsCorpse = true;
                    SpeciesName = "Corpse"; // TODO: Replace with a constant from somewhere
                    Occurr = null;
                    Color = Color.White; // TODO : Replace with a constant from somewhere
                    IsVegetable = false;
                    IsFixed = false;
                    Energy = 0;
                    DNAIsDisabled = true;
                    DisableMovementSystemVariables = true;
                    CantSee = true;
                    IsVirusImmune = true;
                    Chloroplasts = 0;

                    for (var i = SystemVariableAddresses.EyeStart + 1; i > SystemVariableAddresses.EyeEnd; i++)
                    {
                        Memory[i] = 0;
                    }

                    Bouyancy = 0;
                }
                if (Body < 0.5)
                    Dead = true;
            }
            else if (Energy < 0.5 || Body < 0.5)
                Dead = true;
        }

        public void ManageFixed()
        {
            IsFixed = Memory[SystemVariableAddresses.FixedPosition] > 0;
        }

        /// <summary>
        /// Handles reproduction (both sexual and asexual).
        /// </summary>
        /// <remarks>
        /// This used to add to a list of robots ready to reproduce.  Instead, it sets a flag
        /// that can be filtered on.
        /// </remarks>
        public void ManageReproduction()
        {
            if (FertilizedCounter >= 0)
            {
                FertilizedCounter--;
            }
            else
            {
                if (FertilizedCounter < -10)
                    FertilizedCounter++;
                else
                {
                    if (FertilizedCounter == -1)
                    {
                        SpermDNA = null;
                        SpermDNALength = 0;
                    }
                    FertilizedCounter = -2;
                }
            }

            if ((Memory[SystemVariableAddresses.Reproduce] > 0 || Memory[SystemVariableAddresses.MutatingReproduce] > 0) && !CantReproduce)
                AsexualReproduction = true;

            if (Memory[SystemVariableAddresses.SexualReproduce] > 0 && FertilizedCounter >= 0 && !CantReproduce)
                SexualReproduction = true;
        }

        /// <summary>
        /// Handles upkeep costs.
        /// </summary>
        public void ProcessUpkeep()
        {
            var ageDelta = Age - SimulationOptions.Instance.AgeCostStart;

            // Age cost
            if (Age > 0 && ageDelta > 0)
            {
                float ageCost;
                if (SimulationOptions.Instance.AgeCostIsLog)
                    ageCost = (float)(SimulationOptions.Instance.AgeCost * Math.Log(ageDelta));
                else if (SimulationOptions.Instance.AgeCostIsLinear)
                    ageCost = SimulationOptions.Instance.AgeCost + ageDelta * SimulationOptions.Instance.AgeCostScalar;
                else
                    ageCost = SimulationOptions.Instance.AgeCost;

                Energy -= ageCost * SimulationOptions.Instance.CostMultiplier;
            }

            var bodyCost = Body * SimulationOptions.Instance.BodyUpkeep * SimulationOptions.Instance.CostMultiplier;
            Energy -= bodyCost;

            var dnaCost = (DNALength - 1) * SimulationOptions.Instance.DnaCost * SimulationOptions.Instance.CostMultiplier;
            Energy -= dnaCost;

            Slime *= 0.98f;
            if (Slime < 0.5) Slime = 0;

            Poison *= 0.98f;
            if (Poison < 0.5) Poison = 0;
        }

        /// <summary>
        /// Updates the robot's angle
        /// </summary>
        /// <returns>The final robot angle</returns>
        public float SetAim()
        {
            var aimLeftInt = Memory[SystemVariableAddresses.AimLeft];
            var aimRightInt = Memory[SystemVariableAddresses.AimRight];
            var setAimInt = Memory[SystemVariableAddresses.SetAim];
            var aimInt = MathSupport.AngleToInteger(Aim);
            var setAimAngle = MathSupport.IntegerToAngle(setAimInt);
            var aimLeftAngle = MathSupport.IntegerToAngle(aimLeftInt);
            var aimRightAngle = MathSupport.IntegerToAngle(aimRightInt);

            float resultAngle;
            var diffAngle = aimLeftAngle - aimRightAngle;
            float angularMomentumDiff = 0;

            if (setAimInt == aimInt)
                resultAngle = Aim + diffAngle;
            else
            {
                // .setaim overrides .aimsx and .aimdx
                resultAngle = setAimAngle;
                diffAngle = -MathSupport.AngleDifference(Aim, setAimAngle);
                // TODO : Check that angular momentum is working right, as this doens't make much sense...
                // Set angular momentum difference to the nearest 2*Pi
                angularMomentumDiff = (float)(Math.Sign(diffAngle) * (Math.Round(Math.Abs(diffAngle) / (2 * Math.PI)) * (2 * Math.PI)));
            }

            Energy -= (Math.Abs(diffAngle + angularMomentumDiff) * SimulationOptions.Instance.TurnCost * SimulationOptions.Instance.CostMultiplier);

            resultAngle = MathSupport.AngleNormalise(resultAngle + AngularMomentum);

            if (AngularMomentum > 0 && diffAngle < 0)
            {
                AngularMomentum += (diffAngle + angularMomentumDiff);
                if (AngularMomentum < 0) AngularMomentum = 0;
            }
            if (AngularMomentum < 0 && diffAngle > 0)
            {
                AngularMomentum += (diffAngle + angularMomentumDiff);
                if (AngularMomentum > 0) AngularMomentum = 0;
            }

            AimVector = new Vector2((float)Math.Cos(Aim), (float)Math.Sin(Aim));

            Memory[SystemVariableAddresses.AimLeft] = 0;
            Memory[SystemVariableAddresses.AimRight] = 0;
            Memory[SystemVariableAddresses.Aim] = MathSupport.AngleToInteger(Aim);
            Memory[SystemVariableAddresses.SetAim] = MathSupport.AngleToInteger(Aim);

            return resultAngle;
        }

        /// <summary>
        /// Makes a robot share it's chloroplasts with a tied robot
        /// </summary>
        public void ShareChloroplasts(Tie tie)
        {
            if (DoGeneticDistance(this, tie.TargetRobot) > 0.25)
            {
                ChloroplastsShareDelay = 8;
            }
            else
            {
                Memory[SystemVariableAddresses.ShareChloroplasts] = Math.Clamp(Memory[SystemVariableAddresses.ShareChloroplasts], 0, 99);
                var totalChloroplasts = Chloroplasts + tie.TargetRobot.Chloroplasts;

                Chloroplasts = Math.Min(totalChloroplasts * (Memory[SystemVariableAddresses.ShareChloroplasts] / 100.0f), MathSupport.MaxValue);
                tie.TargetRobot.Chloroplasts = Math.Min(totalChloroplasts * (1 - Memory[SystemVariableAddresses.ShareChloroplasts] / 100.0f), MathSupport.MaxValue);
            }
        }

        /// <summary>
        /// Makes a robot share it's energy with a tied robot
        /// </summary>
        public void ShareEnergy(Tie tie)
        {
            if (Energy < 0 || tie.TargetRobot.Energy < 0)
                return;

            if (Memory[SystemVariableAddresses.ShareEnergy] <= 0)
                Memory[SystemVariableAddresses.ShareEnergy] = 0;
            else
            {
                Memory[SystemVariableAddresses.ShareEnergy] = Math.Max(0, Memory[SystemVariableAddresses.ShareEnergy] % 100);
                if (Memory[SystemVariableAddresses.ShareEnergy] == 0)
                    Memory[SystemVariableAddresses.ShareEnergy] = 100;
            }

            var totalEnergy = Energy + tie.TargetRobot.Energy;

            var myPortion = Math.Min(totalEnergy * (Memory[SystemVariableAddresses.ShareEnergy] / 100.0f), MathSupport.MaxValue);
            var myChange = myPortion - Energy;

            if (Math.Abs(myChange) > Body)
                myChange = Math.Sign(myChange) * Body;

            if (Energy + myChange > MathSupport.MaxValue)
                myChange = MathSupport.MaxValue - Energy;
            if (Energy + myChange < 0)
                myChange = -Energy;

            if (tie.TargetRobot.Energy - myChange > MathSupport.MaxValue)
                myChange = MathSupport.MaxValue - tie.TargetRobot.Energy;
            if (tie.TargetRobot.Energy - myChange < 0)
                myChange = -tie.TargetRobot.Energy;

            Energy = Math.Min(Energy + myChange, MathSupport.MaxValue);
            tie.TargetRobot.Energy = Math.Min(tie.TargetRobot.Energy + myChange, MathSupport.MaxValue);
        }

        /// <summary>
        /// Makes a robot share it's shell with a tied robot
        /// </summary>
        public void ShareShell(Tie tie)
        {
            Memory[SystemVariableAddresses.ShareShell] = Math.Clamp(Memory[SystemVariableAddresses.ShareShell], 0, 99);
            var totalShell = Shell + tie.TargetRobot.Shell;

            Shell = Math.Min(totalShell * (Memory[SystemVariableAddresses.ShareShell] / 100.0f), MathSupport.MaxValue);
            tie.TargetRobot.Shell = Math.Min(totalShell * (1 - Memory[SystemVariableAddresses.ShareShell] / 100.0f), MathSupport.MaxValue);

            Memory[SystemVariableAddresses.Shell] = (int)Shell;
            tie.TargetRobot.Memory[SystemVariableAddresses.Shell] = (int)tie.TargetRobot.Shell;
        }

        /// <summary>
        /// Makes a robot share it's slime with a tied robot
        /// </summary>
        public void ShareSlime(Tie tie)
        {
            Memory[SystemVariableAddresses.ShareSlime] = Math.Clamp(Memory[SystemVariableAddresses.ShareSlime], 0, 99);
            var totalSlime = Slime + tie.TargetRobot.Slime;

            Slime = Math.Min(totalSlime * (Memory[SystemVariableAddresses.ShareSlime] / 100.0f), MathSupport.MaxValue);
            tie.TargetRobot.Slime = Math.Min(totalSlime * (1 - Memory[SystemVariableAddresses.ShareSlime] / 100.0f), MathSupport.MaxValue);
        }

        /// <summary>
        /// Makes a robot share it's waste with a tied robot
        /// </summary>
        public void ShareWaste(Tie tie)
        {
            Memory[SystemVariableAddresses.ShareWaste] = Math.Clamp(Memory[SystemVariableAddresses.ShareWaste], 0, 99);
            var totalWaste = Waste + tie.TargetRobot.Waste;

            Waste = Math.Min(totalWaste * (Memory[SystemVariableAddresses.ShareWaste] / 100.0f), MathSupport.MaxValue);
            tie.TargetRobot.Waste = Math.Min(totalWaste * (1 - Memory[SystemVariableAddresses.ShareWaste] / 100.0f), MathSupport.MaxValue);
        }

        public void TransferGeneticMemory()
        {
            throw new NotImplementedException();
        }

        public void UpdateBotBucket()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the robot's current position and velocity
        /// </summary>
        public void UpdatePosition()
        {
            if ((Mass + AddedMass) < 0.25) Mass = 0.25f - AddedMass;

            if (IsFixed)
                Velocity = new Vector2(0, 0);
            else
            {
                Velocity += Vector2.Clamp(Vector2.Divide(IndependentImpulse, Mass + AddedMass), MathSupport.MinVector, MathSupport.MaxVector);

                var v = Velocity.LengthSquared();

                if (v > (SimulationOptions.Instance.MaximumVelocity * SimulationOptions.Instance.MaximumVelocity))
                {
                    Velocity = Vector2.Normalize(Velocity) * SimulationOptions.Instance.MaximumVelocity;
                }

                Position += Velocity;

                UpdateBotBucket();
            }

            IndependentImpulse = new Vector2(0, 0);
            ResistiveImpulse = new Vector2(0, 0);
            StaticImpulse = 0;

            if (SimulationOptions.Instance.ZeroMomentum)
                Velocity = new Vector2(0, 0);

            LastUp = Memory[SystemVariableAddresses.DirUp];
            LastDown = Memory[SystemVariableAddresses.DirDown];
            LastLeft = Memory[SystemVariableAddresses.DirSx];
            LastRight = Memory[SystemVariableAddresses.DirDx];
            Memory[SystemVariableAddresses.DirUp] = 0;
            Memory[SystemVariableAddresses.DirDown] = 0;
            Memory[SystemVariableAddresses.DirSx] = 0;
            Memory[SystemVariableAddresses.DirDx] = 0;
            Memory[SystemVariableAddresses.VelocityScalar] = (int)Math.Clamp(Velocity.Length(), -MathSupport.MaxValue, MathSupport.MaxValue);
            Memory[SystemVariableAddresses.VelocityUp] = (int)Math.Clamp(Math.Cos(Aim) * Velocity.X - Math.Sin(Aim) * Velocity.Y, -MathSupport.MaxValue, MathSupport.MaxValue);
            Memory[SystemVariableAddresses.VelocityDown] = -Memory[SystemVariableAddresses.VelocityUp];
            Memory[SystemVariableAddresses.VelocityRight] = (int)Math.Clamp(Math.Sin(Aim) * Velocity.X + Math.Cos(Aim) * Velocity.Y, -MathSupport.MaxValue, MathSupport.MaxValue);
            Memory[SystemVariableAddresses.VelocityLeft] = -Memory[SystemVariableAddresses.VelocityRight];
            Memory[SystemVariableAddresses.MaxVelocity] = (int)SimulationOptions.Instance.MaximumVelocity;
        }

        internal void KillRobot()
        {
            throw new NotImplementedException();
        }

        internal void ReproduceSexually()
        {
            throw new NotImplementedException();
        }

        internal void WriteSenses()
        {
            throw new NotImplementedException();
        }

        private static float DoGeneticDistance(Robot robot, Robot targetRobot)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes the robot act in a bizarre way when waste builds up.
        /// </summary>
        private void ApplyAlzheimers()
        {
            var loops = (PermanentWaste + Waste - SimulationOptions.Instance.BadWasteLevel) / 4;

            for (var i = 0; i < loops; i++)
            {
                int location;

                do
                {
                    location = Random.Next(1, 1001);
                } while (location != SystemVariableAddresses.MakeChloroplasts && location != SystemVariableAddresses.RemoveChloroplasts);

                var val = Random.Next(-(int)MathSupport.MaxValue, (int)MathSupport.MaxValue + 1);
                Memory[location] = val;
            }
        }

        private Robot CreateRobot()
        {
            throw new NotImplementedException();
        }

        private void Defacate()
        {
            throw new NotImplementedException();
        }

        private void FeedBody()
        {
            if (Memory[SystemVariableAddresses.FeedBody] > 100)
                Memory[SystemVariableAddresses.FeedBody] = 100;

            Energy += Memory[SystemVariableAddresses.FeedBody];
            Body -= (float)Memory[SystemVariableAddresses.FeedBody] / 10;

            if (Energy > MathSupport.MaxValue)
                Energy = MathSupport.MaxValue;

            Memory[SystemVariableAddresses.FeedBody] = 0;
        }

        private void FeedVegetable2()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles creating and destroying chloroplasts.
        /// </summary>
        /// <remarks>
        /// Was ChangeChlr
        ///
        /// The previous version limited the total chloroplasts to be less than the maximum population setting.
        /// This might need re-introducing.
        /// </remarks>
        private void MakeChloroplasts()
        {
            var oldChloroplasts = Chloroplasts;

            Chloroplasts += Memory[SystemVariableAddresses.MakeChloroplasts];
            Chloroplasts -= Memory[SystemVariableAddresses.RemoveChloroplasts];

            if (oldChloroplasts < Chloroplasts)
            {
                var newEnergy = Energy - (Chloroplasts - oldChloroplasts) * SimulationOptions.Instance.ChloroplastCost * SimulationOptions.Instance.CostMultiplier;

                if (newEnergy < 100)
                    Chloroplasts = oldChloroplasts;
                else
                    Energy = newEnergy;
            }

            Memory[SystemVariableAddresses.MakeChloroplasts] = 0;
            Memory[SystemVariableAddresses.RemoveChloroplasts] = 0;
        }

        /// <summary>
        /// Processes requests to update shell for the robot.
        /// </summary>
        private void MakeShell()
        {
            if (Energy <= 0)
                return;

            Memory[SystemVariableAddresses.MakeShell] = Math.Clamp(Memory[SystemVariableAddresses.MakeShell], -(int)MathSupport.MaxValue, (int)MathSupport.MaxValue);

            float change = Memory[SystemVariableAddresses.MakeShell];

            if (Math.Abs(change) * EnergyPerShell > Energy)
                change = Math.Sign(change) * Energy / EnergyPerShell;

            change = Math.Clamp(change, -100, 100);

            if (Shell + change > MathSupport.MaxValue)
                change = MathSupport.MaxValue - Shell;
            if (Shell + change < 0)
                change = -Shell;

            Shell += change;

            Energy -= Math.Abs(change) * EnergyPerShell;

            var cost = Math.Abs(change) * SimulationOptions.Instance.ShellCost * SimulationOptions.Instance.CostMultiplier;

            if (IsMultiBot)
                Energy -= cost / Math.Max(0, NumberOfTies) + 1;
            else
                Energy -= cost;

            Waste += cost;

            Memory[SystemVariableAddresses.MakeShell] = 0;

            // TODO : In some circumstances, making shell causes a disqualification.
        }

        /// <summary>
        /// Processes requests to update slime for the robot.
        /// </summary>
        private void MakeSlime()
        {
            if (Energy <= 0)
                return;

            Memory[SystemVariableAddresses.MakeSlime] = Math.Clamp(Memory[SystemVariableAddresses.MakeSlime], -(int)MathSupport.MaxValue, (int)MathSupport.MaxValue);

            float change = Memory[SystemVariableAddresses.MakeSlime];

            if (Math.Abs(change) * EnergyPerSlime > Energy)
                change = Math.Sign(change) * Energy / EnergyPerSlime;

            change = Math.Clamp(change, -100, 100);

            if (Slime + change > MathSupport.MaxValue)
                change = MathSupport.MaxValue - Slime;
            if (Slime + change < 0)
                change = -Slime;

            Slime += change;

            Energy -= Math.Abs(change) * EnergyPerSlime;

            var cost = Math.Abs(change) * SimulationOptions.Instance.SlimeCost * SimulationOptions.Instance.CostMultiplier;

            if (IsMultiBot)
                Energy -= cost / Math.Max(0, NumberOfTies) + 1;
            else
                Energy -= cost;

            Waste += cost;

            Memory[SystemVariableAddresses.MakeSlime] = 0;
        }

        private bool MakeVirus(int v)
        {
            throw new NotImplementedException();
        }

        private void Shoot()
        {
            if (Energy > 0)
            {
                var type = Memory[SystemVariableAddresses.Shoot];
                var value = (float)Memory[SystemVariableAddresses.ShootValue];
                var cost = 0.0f;
                var valMode = value > 0;
                float multiplier = 1;
                float rangeMultiplier = 1;

                if (type == -1 || type == -6)
                {
                    multiplier = value <= 0 ? 1.0f : value;
                    rangeMultiplier = value >= 0 ? 1.0f : value;

                    if (rangeMultiplier > 4)
                    {
                        cost = rangeMultiplier * SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier;
                        rangeMultiplier = (float)Math.Log(rangeMultiplier / 2, 2);
                    }
                    else if (!valMode)
                    {
                        rangeMultiplier = 1;
                        cost = (SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier) / (Math.Max(NumberOfTies, 0) + 1);
                    }

                    if (multiplier > 4)
                    {
                        cost = multiplier * SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier;
                        multiplier = (float)Math.Log(multiplier / 2, 2);
                    }
                    else if (valMode)
                    {
                        multiplier = 1;
                        cost = (SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier) / (Math.Max(NumberOfTies, 0) + 1);
                    }

                    if (cost > Energy && cost > 2 && Energy > 2 && valMode)
                    {
                        cost = Energy;
                        multiplier = (float)Math.Log(Energy / (SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier), 2);
                    }

                    if (cost > Energy && cost > 2 && Energy > 2 && !valMode)
                    {
                        cost = Energy;
                        rangeMultiplier = (float)Math.Log(Energy / (SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier), 2);
                    }
                }

                if (type >= 0) // Memory short
                {
                    type %= MemoryMaximum;
                    cost = SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier;

                    if (Energy < cost)
                        cost = Energy;

                    Energy -= cost;

                    Shot.CreateShot(this, type, value, 1, true);

                    // This has some disqualification bits here
                }
                else if (type == -1) // Energy request feeding shot
                {
                    if (IsMultiBot)
                        value = 20 + (Body / 5) * (Math.Max(NumberOfTies, 0) + 1);
                    else
                        value = 20 + (Body / 5);

                    value *= multiplier;

                    if (Energy < cost)
                        cost = Energy;

                    Energy -= cost;

                    Shot.CreateShot(this, type, value, rangeMultiplier, true);
                }
                else if (type == -2) // Energy shot
                {
                    value = Math.Abs(value);

                    if (Energy < value)
                        value = Energy;

                    if (value == 0) // Use the default value
                        value = Energy / 100;

                    cost = value + SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier / (Math.Max(NumberOfTies, 0) + 1);

                    if (Energy < cost)
                        cost = Energy;

                    Energy -= cost;

                    Shot.CreateShot(this, type, value, 1, true);
                }
                else if (type == -3) // Shoot Venom
                {
                    value = Math.Abs(value);
                    if (value > Venom)
                        value = Venom;
                    if (value == 0)
                        value = Venom / 20; // Use the default value

                    Venom -= value;

                    cost = SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier / (Math.Max(NumberOfTies, 0) + 1);

                    if (Energy < cost)
                        cost = Energy;

                    Energy -= cost;

                    Shot.CreateShot(this, type, value, 1, true);
                }
                else if (type == -4)
                {
                    value = Math.Abs(value);

                    if (value == 0)
                        value = Waste / 20;
                    if (value > Waste)
                        value = Waste;

                    Waste -= value * 0.99f;
                    PermanentWaste += value / 100;

                    cost = SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier / (Math.Max(NumberOfTies, 0) + 1);

                    if (Energy < cost)
                        cost = Energy;

                    Energy -= cost;

                    Shot.CreateShot(this, type, value, 1, true);
                }
                // -5 is skipped, as poison is only shot automatically in response to an attack
                else if (type == -6) // Body Shot
                {
                    if (IsMultiBot)
                        value = 10 + Body / 2 * (Math.Max(NumberOfTies, 0) + 1);
                    else
                        value = 10 + Math.Abs(Body) / 2;

                    if (Energy < cost)
                        cost = Energy;

                    Energy -= cost;

                    value *= multiplier;

                    Shot.CreateShot(this, type, value, rangeMultiplier, true);
                }
                else if (type == -8) // Sperm shot
                {
                    cost = SimulationOptions.Instance.ShotCost * SimulationOptions.Instance.CostMultiplier;

                    if (Energy < cost)
                        cost = Energy;

                    Energy -= cost;

                    Shot.CreateShot(this, type, value, 1, true);
                }
            }
            else
            {
                Memory[SystemVariableAddresses.Shoot] = 0;
                Memory[SystemVariableAddresses.ShootValue] = 0;
            }
        }

        private void StoreBody()
        {
            if (Memory[SystemVariableAddresses.StoreBody] > 100)
                Memory[SystemVariableAddresses.StoreBody] = 100;

            Energy -= Memory[SystemVariableAddresses.StoreBody];
            Body += (float)Memory[SystemVariableAddresses.StoreBody] / 10;

            if (Body > MathSupport.MaxValue)
                Body = MathSupport.MaxValue;

            Memory[SystemVariableAddresses.StoreBody] = 0;
        }

        private void StorePoison()
        {
            if (Energy < 0) return;

            Memory[SystemVariableAddresses.StorePoison] = Math.Clamp(Memory[SystemVariableAddresses.StorePoison], -(int)MathSupport.MaxValue, (int)MathSupport.MaxValue);

            float change = Memory[SystemVariableAddresses.StorePoison];

            if (Math.Abs(change) > Math.Min(Energy / EnergyPerPoison, 100))
                change = Math.Sign(change) * Math.Min(Energy / EnergyPerPoison, 100);

            if (Poison + change > MathSupport.MaxValue)
                change = MathSupport.MaxValue - Poison;
            if (Poison + change < 0)
                change = -Poison;

            Poison += change;
            Energy -= Math.Abs(change) * EnergyPerPoison;

            var cost = Math.Abs(change) * SimulationOptions.Instance.PoisonCost * SimulationOptions.Instance.CostMultiplier;
            Energy -= cost;
            Waste += cost;

            Memory[SystemVariableAddresses.StorePoison] = 0;
        }

        private void StoreVenom()
        {
            if (Energy < 0) return;

            Memory[SystemVariableAddresses.StoreVenom] = Math.Clamp(Memory[SystemVariableAddresses.StoreVenom], -(int)MathSupport.MaxValue, (int)MathSupport.MaxValue);

            float change = Memory[SystemVariableAddresses.StoreVenom];

            if (Math.Abs(change) > Math.Min(Energy / EnergyPerVenom, 100))
                change = Math.Sign(change) * Math.Min(Energy / EnergyPerVenom, 100);

            if (Venom + change > MathSupport.MaxValue)
                change = MathSupport.MaxValue - Venom;
            if (Venom + change < 0)
                change = -Venom;

            Venom += change;
            Energy -= Math.Abs(change) * EnergyPerVenom;

            var cost = Math.Abs(change) * SimulationOptions.Instance.VenomCost * SimulationOptions.Instance.CostMultiplier;
            Energy -= cost;
            Waste += cost;

            Memory[SystemVariableAddresses.StoreVenom] = 0;
        }
    }
}