using System.Windows.Media;

namespace DarwinBots.Model
{
    internal enum TieType
    {
        DampedSpring = 0,
        String = 1,
        Bone = 2,
        AntiRope = 3
    }

    internal class Tie
    {
        public double Angle { get; set; }
        public double b { get; set; }
        public bool BackTie { get; set; }
        public double Bend { get; set; }

        public Color? Color
        {
            get
            {
                if (Sharing)
                    return Colors.Yellow;
                if (EnergyUsed)
                    return Colors.Red;
                if (InfoUsed)
                    return Colors.White;
                return null;
            }
        }

        public bool EnergyUsed { get; set; }
        public bool FixedAngle { get; set; }
        public bool InfoUsed { get; set; }
        public double k { get; set; }
        public int Last { get; set; }
        public int Mem { get; set; }
        public double NaturalLength { get; set; }
        public Robot OtherBot { get; set; }
        public int Port { get; set; }
        public Tie ReverseTie { get; set; }
        public bool Sharing { get; set; }
        public TieType Type { get; set; }

        public void CleanUp()
        {
            OtherBot = null;
            ReverseTie = null;
        }
    }
}
