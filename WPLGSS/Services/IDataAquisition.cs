using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    public interface IDataAquisition
    {
        void StartService();

        void SetChannelValue(Channel channel, double value);

        event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        event EventHandler<StatusChangedEventArgs> ArduinoConnectionChanged;
        event EventHandler<ChannelValueUpdatedEventArgs> ChannelValueUpdated;
    }
}
