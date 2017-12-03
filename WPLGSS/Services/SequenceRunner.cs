using System;
using System.Collections.Concurrent;
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
        private System.Timers.Timer timer;
        ObservableCollection<Event> primarySequence;
        ObservableCollection<Event> abortSequence;
        ConcurrentDictionary<InputChannel, double> ChannelValues;
        public Queue<Event> cache;
        private WPLGSS.Models.AbortCondition AbortCondition;
        

        // Run the abort sequence
        public void OnThresholdReached(object source, System.Timers.ElapsedEventArgs e)
        {
            foreach(Event curr in abortSequence)
            {
                SequenceRunningStateChanged(this, new StatusChangedEventArgs());
                cache.Enqueue(curr);
            }
        }

        // Raises StatusChangedEvent every millisecond and runs abort once abort condition met
        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            foreach (Event curr in primarySequence)
            {
                if (timer.Equals(curr.StartTime))
                {
                    SequenceRunningStateChanged(this, new StatusChangedEventArgs());
                    cache.Enqueue(curr);
                    if(AbortCondition.Equals(curr))
                    {
                        timer.Start();
                        timer.Elapsed += OnThresholdReached;
                        OnThresholdReached(this, e);
                    }
                }
            }
        }

        // Raises Timed event every millisecond
        public void RunSequence(Sequence sequence)
        {
            primarySequence = sequence.PrimarySequence;
            abortSequence = sequence.AbortSequence;
            timer = new System.Timers.Timer();
            timer.Interval = 1;
            timer.Elapsed += OnTimedEvent;       
            timer.Enabled = true;
        }
    }
}
