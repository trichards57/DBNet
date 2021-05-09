using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    public class EvoData
    {
        public double LFOR { get; internal set; }
        public bool LFORdir { get; internal set; }
        public double LFORcorr { get; internal set; }
        public int hidePredCycl { get; internal set; }
        public int curr_dna_size { get; internal set; }
        public int target_dna_size { get; internal set; }
        public int Init_hidePredCycl { get; internal set; }
        public int y_Stgwins { get; internal set; }


    }
}
