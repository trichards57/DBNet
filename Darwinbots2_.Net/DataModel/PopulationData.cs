using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    class PopulationData
    {
        public string IName { get; internal set; }
        public IEnumerable<SpeciesPopulationData> Species { get; internal set; }
    }
}
