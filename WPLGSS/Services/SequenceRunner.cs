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
    public class SequenceRunner : ISequenceRunner, IDisposable
    {
        public event EventHandler<StatusChangedEventArgs> SequenceRunningStateChanged;
        private System.Timers.Timer timer;

        ObservableCollection<Event> primarySequence;
        ObservableCollection<Event> abortSequence;
        public Queue<Event> cache;

        private WPLGSS.Models.AbortCondition AbortCondition;
        private IDataAquisition Service;
        private Sequence sequence;
        private TimeSpan sequenceStartTime;

        public SequenceRunner(Sequence sequence, IDataAquisition dataService)
        {
            this.primarySequence = sequence.PrimarySequence;
            this.abortSequence = sequence.AbortSequence;
            this.Service = dataService;
        }

        // Run the abort sequence
        public void OnThresholdReached(object source, System.Timers.ElapsedEventArgs e)
        {
            foreach (Event curr in abortSequence)
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
                if ((curr.StartTime + sequenceStartTime).TotalMilliseconds - DateTime.Now.TimeOfDay.TotalMilliseconds < 1)
                {
                    Channel channel = null;
                    switch(curr)
                    {
                        case OutputEvent output:
                            Service.SetChannelValue(channel, 1);
                            break;
                        case AbortCondition abort:
                            break;
                    }
                    SequenceRunningStateChanged(this, new StatusChangedEventArgs());
                    cache.Enqueue(curr);
                    if (AbortCondition.Equals(curr))
                    {
                        timer.Start();
                        timer.Elapsed += OnThresholdReached;
                        OnThresholdReached(this, e);
                    }
                }

                if ((curr.EndTime + sequenceStartTime).TotalMilliseconds - DateTime.Now.TimeOfDay.TotalMilliseconds < 1)
                {
                    Channel channel = null;
                    switch (curr)
                    {
                        case OutputEvent output:
                            Service.SetChannelValue(channel, 0);
                            break;
                        case AbortCondition abort:
                            break;
                    }
                }
            }
        }

        // Raises Timed event every millisecond
        public void RunSequence()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            sequenceStartTime = DateTime.Now.TimeOfDay;
            Service.ChannelValueUpdated += Service_ChannelValueUpdated;
        }

        private void Service_ChannelValueUpdated(object sender, ChannelValueUpdatedEventArgs e)
        {
            //Checking
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            ((IDisposable)timer).Dispose();
        }
    }
}
