using Iersera.Model;

namespace Iersera.DataModel
{
    internal class SavedTeleporter
    {
        public int BotsPerPoll { get; internal set; }
        public int Color { get; internal set; }
        public bool DriftHorizontal { get; internal set; }
        public bool DriftVertical { get; internal set; }
        public decimal Height { get; internal set; }
        public bool Highlight { get; internal set; }
        public bool In { get; internal set; }
        public int InboundPollCycles { get; internal set; }
        public bool Internet { get; internal set; }
        public bool Local { get; internal set; }
        public bool Out { get; internal set; }
        public string Path { get; internal set; }
        public int PollCountDown { get; internal set; }
        public vector Position { get; internal set; }
        public bool RespectShapes { get; internal set; }
        public bool TeleportCorpses { get; internal set; }
        public bool TeleportHeterotrophs { get; internal set; }
        public bool TeleportVeggies { get; internal set; }
        public vector Velocity { get; internal set; }
        public decimal Width { get; internal set; }
    }
}
