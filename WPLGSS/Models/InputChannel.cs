using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public class InputChannel : Channel
    {
        private string scaling;

        public string Scaling
        {
            get { return scaling; }
            set
            {
                SetProperty(ref scaling, value);
            }
        }

        private string unit;

        public string Unit
        {
            get { return unit; }
            set
            {
                SetProperty(ref unit, value);
            }
        }

    }
}
