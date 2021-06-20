using System.Collections.Generic;

namespace DarwinBots.DataModel
{
    internal class PopulationData
    {
        public IEnumerable<SpeciesPopulationData> Species { get; internal set; }
    }
}
