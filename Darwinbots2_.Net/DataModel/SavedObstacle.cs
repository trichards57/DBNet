namespace Iersera.DataModel
{
    internal class SavedObstacle
    {
        public int Color { get; internal set; }
        public bool Exist { get; internal set; }
        public double Height { get; internal set; }
        public vector Position { get; internal set; }
        public vector Velocity { get; internal set; }
        public double Width { get; internal set; }
    }
}
