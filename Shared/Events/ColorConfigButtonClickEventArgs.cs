using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTemplate.Shared.Events
{
    public class ColorConfigButtonClickEventArgs : EventArgs
    {
        public int Red;
        public int Green;
        public int Blue;
    }
}
