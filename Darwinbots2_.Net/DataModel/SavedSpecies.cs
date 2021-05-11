using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    class SavedSpecies
    {
        public int Colind { get; internal set; }
        public int Color { get; internal set; }
        public bool Fixed { get; internal set; }
        public double[] MutArray { get; internal set; }
        public bool Mutations { get; internal set; }
        public string Name { get; internal set; }
        public string Path { get; internal set; }
        public int Qty { get; internal set; }
        public int[] Skin { get; internal set; }
        public int Stnrg { get; internal set; }
        public bool Veg { get; internal set; }
        public int CopyErrorWhatToChange { get; internal set; }
        public int PointWhatToChange { get; internal set; }
        public double[] MutationsMeans { get; internal set; }
        public double[] MutationsStdDevs { get; internal set; }
        public decimal Poslf { get; internal set; }
        public decimal Posrg { get; internal set; }
        public decimal Postp { get; internal set; }
        public decimal Posdn { get; internal set; }
        public bool CantSee { get; internal set; }
        public bool DisableDNA { get; internal set; }
        public bool DisableMovementSysvars { get; internal set; }
        public bool CantReproduce { get; internal set; }
        public bool VirusImmune { get; internal set; }
        public int Population { get; internal set; }
        public int SubSpeciesCounter { get; internal set; }
        public bool Native { get; internal set; }
    }
}
