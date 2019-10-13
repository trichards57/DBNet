using System;

namespace DBNetEngine.Physics
{
    internal class Environment
    {
        public float BouyancyScaling { get; set; }
        public float LightAvailable { get; set; }

        public void DoBorderCollisions(Robot rob)
        {
            throw new NotImplementedException();
        }
    }
}