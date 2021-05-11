using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    class SavedShot
    {
        public int FileCounter { get; internal set; }
        public bool Visible { get; internal set; }
        public int Left { get; internal set; }
        public int Top { get; internal set; }
        public bool Save { get; internal set; }
    }
}
