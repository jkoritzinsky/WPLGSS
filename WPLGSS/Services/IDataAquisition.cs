using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    public interface IDataAquisition
    {
        void StartService();

        void StartStopRecord(string path);

        void SetChannelValue(Channel channel, double value);

        event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        event EventHandler<StatusChangedEventArgs> ArduinoConnectionChanged;
        event EventHandler<ChannelValueUpdatedEventArgs> ChannelValueUpdated;

        ConcurrentDictionary<InputChannel, double> ChannelValues { get; }
    }
}
