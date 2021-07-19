using DarwinBots.Modules;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DarwinBots.Model
{
    internal class Robot
    {
        private double _body;
        private double _chloroplasts;
        private Lazy<double> _radius;

        public Robot()
        {
            ResetRadiusLazy();
        }

        public static double StandardRadius => SimOpt.SimOpts.FixedBotRadii ? RobotsManager.RobSize / 2.0 : 415.475;

        public int AbsNum { get; set; }

        public DoubleVector ActualVelocity { get; set; }

        public double AddedMass { get; set; }

        public int Age { get; set; }

        public double Aim { get; set; }

        public DoubleVector AimVector => new(Math.Cos(Aim), Math.Sin(Aim));

        public double AngularMomentum { get; set; }
        public int BirthCycle { get; set; }

        public double Body
        {
            get => _body;
            set
            {
                _body = value;
                ResetRadiusLazy();
            }
        }

        public double Bouyancy { get; set; }

        public IntVector BucketPosition { get; set; }

        public bool CantReproduce { get; set; }

        public bool CantSee { get; set; }

        public double Chloroplasts
        {
            get => _chloroplasts;
            set
            {
                _chloroplasts = value;
                ResetRadiusLazy();
            }
        }

        public bool ChloroplastsDisabled { get; set; }
        public byte ChloroplastsShareDelay { get; set; }

        public Color Color { get; set; }

        public int DecayTimer { get; set; }
        public List<DnaBlock> Dna { get; set; } = new();
        public bool DnaDisabled { get; set; }
        public double Energy { get; set; }
        public int[] EpigeneticMemory { get; } = new int[14];
        public bool Exists { get; set; }
        public int Fertilized { get; set; }
        public string FName { get; set; }
        public int Generation { get; set; }
        public double GenMut { get; set; }
        public DoubleVector IndependentImpulse { get; set; }
        public bool IsCorpse { get; set; }

        public bool IsDead { get; set; }
        public bool IsFixed { get; set; }
        public bool IsMultibot { get; set; }
        public bool IsParalyzed { get; set; }
        public bool IsPoisoned { get; set; }
        public bool IsVegetable { get; set; }
        public bool IsVirusImmune { get; set; }
        public int Kills { get; set; }
        public int LastMutation { get; set; }
        public string LastMutationDetail { get; set; }
        public object LastSeenObject { get; set; }
        public DoubleVector LastSeenObjectPosition { get; set; }
        public Robot LastTouched { get; set; }
        public double Mass { get; set; }
        public int[] Memory { get; } = new int[1000];
        public bool MovementSysvarsDisabled { get; set; }
        public int MultibotTimer { get; set; }
        public MutationProbabilities MutationProbabilities { get; set; }
        public int Mutations { get; set; }
        public int NewAge { get; set; }
        public int NumberOfGenes { get; set; }
        public int[] occurr { get; } = new int[20];
        public double OldBody { get; set; }
        public double OldEnergy { get; set; }
        public int OldMutations { get; set; }
        public DoubleVector OldPosition { get; set; }
        public int ParalyzedCountdown { get; set; }
        public Robot Parent { get; set; }
        public double PermanentWaste { get; set; }
        public int PointMutationBasePair { get; set; }
        public int PointMutationCycle { get; set; }
        public double Poison { get; set; }
        public double PoisonCountdown { get; set; }
        public int PoisonLocation { get; set; }
        public int PoisonValue { get; set; }
        public DoubleVector Position { get; set; }
        public double Radius => _radius.Value;
        public DoubleVector ResistiveImpulse { get; set; }
        public double Shell { get; set; }
        public int[] Skin { get; } = new int[13];
        public double Slime { get; set; }
        public int SonNumber { get; set; }
        public List<DnaBlock> SpermDna { get; set; } = new();
        public double StaticImpulse { get; set; }
        public int SubSpecies { get; set; }

        public bool[] TieAngleOverwrite { get; } = new bool[3];

        public bool[] TieLengthOverwrite { get; } = new bool[3];

        public List<Tie> Ties { get; } = new();

        public List<Variable> Variables { get; } = new();

        public double vbody { get; set; }
        public DoubleVector Velocity { get; set; }

        public double Venom { get; set; }
        public int VirusLocation { get; set; }
        public Shot VirusShot { get; set; }
        public int VirusTimer { get; set; }

        public int VirusValue { get; set; }

        public double Waste { get; set; }

        private double GetRadius()
        {
            if (SimOpt.SimOpts.FixedBotRadii)
                return RobotsManager.RobSize / 2.0;

            var bodypoints = Math.Max(Body, 1);
            var r = Math.Pow(Math.Log(bodypoints) * bodypoints * RobotsManager.CubicTwipPerBody * 3 * 0.25 / Math.PI, 1.0 / 3);
            r += (415 - r) * Chloroplasts / 32000;

            if (r < 1)
                r = 1;

            return r;
        }

        private void ResetRadiusLazy()
        {
            _radius = new Lazy<double>(GetRadius);
        }
    }
}
