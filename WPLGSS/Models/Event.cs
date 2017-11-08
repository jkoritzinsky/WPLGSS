using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public class Event : BindableBase
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
    }
}
