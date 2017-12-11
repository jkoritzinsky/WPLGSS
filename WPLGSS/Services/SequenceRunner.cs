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

        public List<AbortCondition> abortConditions;
        private IConfigService config;
        private IDataAquisition Service;
        private TimeSpan sequenceStartTime;
        bool abortnow = false;

        public SequenceRunner(IDataAquisition dataService, IConfigService configService)
        {
            this.Service = dataService;
            this.config = configService;
        }

        // Raises StatusChangedEvent every millisecond and runs abort once abort condition met
        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            ObservableCollection<Channel> Channels = config.Config.Channels;
            ObservableCollection<Event> currSequence;
            if (abortnow == true) currSequence = abortSequence;
            else currSequence = primarySequence;

            foreach (Event curr in primarySequence)
            {
                if ((curr.StartTime + sequenceStartTime).TotalMilliseconds - DateTime.Now.TimeOfDay.TotalMilliseconds < 1)
                {
                    var channelName = curr.ChannelName;
                    Channel channel = null;
                    foreach(Channel chan in Channels)
                    {
                        if (chan.Name.Equals(channelName))
                        {
                            channel = chan;
                            break;
                        }
                            
                    }
                    if (channel == null)
                    {
                        continue;
                    }
                    switch(curr)
                    {
                        case OutputEvent output:
                            Service.SetChannelValue(channel, 1);
                            break;
                        case AbortCondition abort:
                            sequenceStartTime = DateTime.Now.TimeOfDay;    
                            abortnow = true;
                            abortConditions.Add(abort);
                            break;
                    }
     
                }

                if ((curr.EndTime + sequenceStartTime).TotalMilliseconds - DateTime.Now.TimeOfDay.TotalMilliseconds < 1)
                {
                    var channelName = curr.ChannelName;
                    Channel channel = null;
                    foreach (Channel chan in Channels)
                    {
                        if (chan.Name.Equals(channelName))
                        {
                            channel = chan;
                            break;
                        }

                    }
                    if (channel == null)
                    {
                        continue;
                    }
                    switch (curr)
                    {
                        case OutputEvent output:
                            Service.SetChannelValue(channel, 0);
                            break;
                        case AbortCondition abort:
                            abortnow = false;
                            abortConditions.Remove(abort);
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
            SequenceRunningStateChanged?.Invoke(this, new StatusChangedEventArgs());
            timer = new System.Timers.Timer();
            timer.Interval = 1;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            sequenceStartTime = DateTime.Now.TimeOfDay;
            Service.ChannelValueUpdated += Service_ChannelValueUpdated;
        }

        private void Service_ChannelValueUpdated(object sender, ChannelValueUpdatedEventArgs e)
        {
            foreach (AbortCondition curr in abortConditions)
            {
                if (e.Channel.Name.Equals(curr.ChannelName))
                {
                    if (curr.ThresholdMax <= e.Value || curr.ThresholdMin >= e.Value)
                    {
                        abortnow = true;
                        sequenceStartTime = e.Time.TimeOfDay;
                    }
                }
            }
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
