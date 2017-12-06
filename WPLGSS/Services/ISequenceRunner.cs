using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Services
{
    public interface ISequenceRunner
    {
        void RunSequence();
        event EventHandler<StatusChangedEventArgs> SequenceRunningStateChanged;
        void OnThresholdReached(object Threshold, System.Timers.ElapsedEventArgs e);
    }
}
