using System.Drawing;

namespace DarwinBots.Model
{
    public class Teleporter
    {
        public int BotsPerPoll { get; set; }
        public vector Center { get; set; }
        public Color Color { get; set; }
        public bool DriftHorizontal { get; set; }
        public bool DriftVertical { get; set; }
        public bool Exist { get; set; }
        public double Height { get; set; }
        public bool Highlight { get; set; }
        public bool In { get; set; }
        public int InboundPollCycles { get; set; }
        public string IntInPath { get; set; }
        public string IntOutPath { get; set; }
        public bool Local { get; set; }
        public int NumTeleported { get; set; }
        public int NumTeleportedIn { get; set; }
        public bool Out { get; set; }
        public string path { get; set; }
        public int PollCountDown { get; set; }
        public vector Pos { get; set; }
        public bool RespectShapes { get; set; }
        public bool TeleportCorpses { get; set; }
        public bool TeleportHeterotrophs { get; set; }
        public bool TeleportVeggies { get; set; }
        public vector Vel { get; set; }
        public double Width { get; set; }
    }
}
