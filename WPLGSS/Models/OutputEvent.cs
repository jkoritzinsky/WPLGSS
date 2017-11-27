using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public class OutputEvent : Event
    {
        public override bool Equals(Event other)
        {
            return other is OutputEvent && base.Equals(other);
        }
    }
}
