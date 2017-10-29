using WPLGSS.Models;

namespace WPLGSS.Services
{
    public class ChannelValueUpdatedEventArgs
    {
        public ChannelValueUpdatedEventArgs(Channel channel, double value)
        {
            Channel = channel;
            Value = value;
        }

        public Channel Channel { get; }
        public double Value { get; }
    }
}