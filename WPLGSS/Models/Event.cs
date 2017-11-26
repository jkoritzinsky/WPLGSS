using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public class Event : BindableBase, IEquatable<Event>
    {
        private TimeSpan startTime;

        public TimeSpan StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value); }
        }

        private TimeSpan endTime;

        public TimeSpan EndTime
        {
            get { return endTime; }
            set { SetProperty(ref endTime, value); }
        }

        private string channelName;

        public string ChannelName
        {
            get { return channelName; }
            set { SetProperty(ref channelName, value); }
        }

        public override bool Equals(object obj)
        {
            return obj is Event evt && Equals(evt);
        }

        public virtual bool Equals(Event other)
        {
            return StartTime == other.StartTime && EndTime == other.EndTime && ChannelName == other.ChannelName;
        }
    }
}
