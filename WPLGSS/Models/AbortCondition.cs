using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public class AbortCondition : Event
    {
        private double thresholdMin;

        public double ThresholdMin
        {
            get { return thresholdMin; }
            set { SetProperty(ref thresholdMin, value); }
        }

        private double thresholdMax;

        public double ThresholdMax
        {
            get { return thresholdMax; }
            set { SetProperty(ref thresholdMax, value); }
        }

        public override bool Equals(Event other)
        {
            return other is AbortCondition cond && ThresholdMin == cond.ThresholdMin && ThresholdMax == cond.ThresholdMax && base.Equals(other);
        }
    }
}
