using System;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    public class ChannelValueUpdatedEventArgs
    {
        public ChannelValueUpdatedEventArgs(Channel channel, double value, DateTime time)
        {
            Channel = channel;
            Value = value;
            Time = time;
        }

        public Channel Channel { get; }
        public double Value { get; }

        public DateTime Time { get; }
    }
}