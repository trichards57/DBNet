using System;
using System.Collections.Generic;
using System.Text;

namespace DBNetModel.Physics
{
    public class Environment
    {
        public float LightAvailable { get; set; }
        public float BouyancyScaling { get; set; }

        public void DoBorderCollisions(Robot rob)
        {
            throw new NotImplementedException();
        }
    }
}