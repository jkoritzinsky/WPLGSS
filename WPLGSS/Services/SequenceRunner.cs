using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    [Export(typeof(ISequenceRunner))]
    public class SequenceRunner: ISequenceRunner
    {
        public event EventHandler<StatusChangedEventArgs> SequenceRunningStateChanged;
        

        public void OnThresholdReached(object threshold, ChannelValueUpdatedEventArgs e)
        {
            // Run Abort Sequence
        }

        public void RunSequence(Sequence sequence)
        {
            //ObservableCollection<Event> primarysequence = sequence.PrimarySequence;
            //WaitHandle waitObject = null;
            //WaitOrTimerCallback callBack = null;
            //object state = null;
            //TimeSpan timeout;
            //bool executeOnlyOnce = true;
            //foreach (Event curr in primarysequence) {
                
            //    timeout = curr.EndTime - curr.StartTime;
            //    RegisteredWaitHandle waitHandle = ThreadPool.RegisterWaitForSingleObject(waitObject, callBack, state, timeout, executeOnlyOnce);
            //}

        }
    }
}
