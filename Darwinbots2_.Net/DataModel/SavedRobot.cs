using Iersera.Model;
using System.Collections.Generic;
using static Globals;

namespace Iersera.DataModel
{
    internal class SavedRobot
    {
        public int AbsoluteNumber { get; internal set; }
        public vector ActualVelocity { get; internal set; }
        public int Age { get; internal set; }
        public double Aim { get; internal set; }
        public decimal AngularMomentum { get; internal set; }
        public int BirthCycle { get; internal set; }
        public double Body { get; internal set; }
        public decimal Bouyancy { get; internal set; }
        public bool CantReproduce { get; internal set; }
        public bool CantSee { get; internal set; }
        public byte ChloroplastShareDelay { get; internal set; }
        public int CopyErrorWhatToChange { get; internal set; }
        public bool Corpse { get; internal set; }
        public bool Dead { get; internal set; }
        public bool DisableDna { get; internal set; }
        public bool DisableMovementSysvars { get; internal set; }
        public List<DNABlock> Dna { get; internal set; }
        public int Dq { get; internal set; }
        public double Energy { get; internal set; }
        public int[] EpigeneticMemory { get; internal set; }
        public bool Exist { get; internal set; }
        public int Fertilized { get; internal set; }
        public bool Fixed { get; internal set; }
        public int GeneNumber { get; internal set; }
        public int Generation { get; internal set; }
        public int LastMutation { get; internal set; }
        public string LastMutationDetail { get; internal set; }
        public string LastOwner { get; internal set; }
        public int[] Memory { get; internal set; }
        public int MultiBotTime { get; internal set; }
        public double[] MutationArray { get; internal set; }
        public int Mutations { get; internal set; }
        public double[] MutationsMeans { get; internal set; }
        public bool MutationsProbs { get; internal set; }
        public double[] MutationsStdDevs { get; internal set; }
        public bool NewMove { get; internal set; }
        public bool NoChloroplasts { get; internal set; }
        public int OldBotNumber { get; internal set; }
        public float OldGeneticDistance { get; internal set; }
        public int OldMutations { get; internal set; }
        public int Parent { get; internal set; }
        public decimal PermanentWaste { get; internal set; }
        public int PointWhatToChange { get; internal set; }
        public decimal Poison { get; internal set; }
        public vector Position { get; internal set; }
        public decimal Shell { get; internal set; }
        public int Sim { get; internal set; }
        public int[] Skin { get; internal set; }
        public decimal Slime { get; internal set; }
        public int SonNumber { get; internal set; }
        public string SpeciesName { get; internal set; }
        public DNABlock[] SpermDNA { get; internal set; }
        public int SubSpecies { get; internal set; }
        public bool SunBelt { get; internal set; }
        public string Tag { get; internal set; }
        public IEnumerable<SavedTie> Ties { get; internal set; }
        public decimal Torque { get; internal set; }
        public int VariableNumber { get; internal set; }
        public Variable[] Variables { get; internal set; }
        public bool Veg { get; internal set; }
        public object Velocity { get; internal set; }
        public decimal Venom { get; internal set; }
        public bool View { get; internal set; }
        public bool VirusImmune { get; internal set; }
        public bool Wall { get; internal set; }
        public decimal Waste { get; internal set; }
    }
}
