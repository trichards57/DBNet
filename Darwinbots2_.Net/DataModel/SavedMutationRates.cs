using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    class SavedMutationRates
    {
        public double[] MutationStdDevs { get; internal set; }
        public double[] MutationMeans { get; internal set; }
        public double[] MutationProbabilities { get; internal set; }
        public int PointWhatToChange { get; internal set; }
        public int CopyErrorWhatToChange { get; internal set; }
    }
}
