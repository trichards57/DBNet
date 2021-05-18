using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    class SavedTie
    {
        public int Port { get; internal set; }
        public int TargetBot { get; internal set; }
        public int BackTie { get; internal set; }
        public decimal Angle { get; internal set; }
        public decimal Bend { get; internal set; }
        public bool FixedAngle { get; internal set; }
        public int Length { get; internal set; }
        public int Shrink { get; internal set; }
        public bool Stat { get; internal set; }
        public int Last { get; internal set; }
        public int Mem { get; internal set; }
        public bool Back { get; internal set; }
        public bool EnergyUsed { get; internal set; }
        public bool InfoUsed { get; internal set; }
        public bool Sharing { get; internal set; }
        public byte Type { get; internal set; }
        public decimal NaturalLength { get; internal set; }
        public decimal k { get; internal set; }
        public decimal b { get; internal set; }
    }
}
