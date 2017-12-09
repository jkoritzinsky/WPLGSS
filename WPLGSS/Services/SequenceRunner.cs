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

        private WPLGSS.Models.AbortCondition AbortCondition;
        private Config config;
        private IDataAquisition Service;
        private Sequence sequence;
        private TimeSpan sequenceStartTime;
        bool abortnow = false;

        public SequenceRunner(IDataAquisition dataService, Config config)
        {
            this.Service = dataService;
            this.config = config;
        }

        // Raises StatusChangedEvent every millisecond and runs abort once abort condition met
        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            ObservableCollection<Channel> Channels = config.Channels;
            ObservableCollection<Event> currSequence;
            if (abortnow == true) currSequence = abortSequence;
            else currSequence = primarySequence;

            foreach (Event curr in primarySequence)
            {
                if ((curr.StartTime + sequenceStartTime).TotalMilliseconds - DateTime.Now.TimeOfDay.TotalMilliseconds < 1)
                {
                    String channelName = curr.ChannelName;
                    Channel channel = null;
                    foreach(Channel chan in Channels)
                    {
                        if (chan.Name.Equals(channelName))
                        {
                            channel = chan;
                            break;
                        }
                            
                    }
                    switch(curr)
                    {
                        case OutputEvent output:
                            Service.SetChannelValue(channel, 1);
                            break;
                        case AbortCondition abort:
                            timer.Start();      // Restart Timer ?
                            abortnow = true;
                            break;
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
                            abortnow = false;
                            break;
                    }
                }
            }
        }

        // Raises Timed event every millisecond
        public void RunSequence(Sequence sequence)
        {
            this.primarySequence = sequence.PrimarySequence;
            this.abortSequence = sequence.AbortSequence;
            SequenceRunningStateChanged(this, new StatusChangedEventArgs());
            timer = new System.Timers.Timer();
            timer.Interval = 1;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            sequenceStartTime = DateTime.Now.TimeOfDay;
            Service.ChannelValueUpdated += Service_ChannelValueUpdated;
        }

        private void Service_ChannelValueUpdated(object sender, ChannelValueUpdatedEventArgs e)
        {
            
            if(AbortCondition.ThresholdMax <= e.Value || AbortCondition.ThresholdMin >= e.Value)
            {
                abortnow = true;
                sequenceStartTime = e.Time.TimeOfDay;
                // Run OnTimedEvent
            }
        }

        public void Dispose()
        {
            ((IDisposable)timer).Dispose();
        }
    }
}
