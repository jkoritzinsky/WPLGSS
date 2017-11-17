using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    [Export(typeof(IDataAquisition))] public class DummyDataService : IDataAquisition
    {
        public event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        public event EventHandler<StatusChangedEventArgs> ArduinoConnectionChanged;
        public event EventHandler<ChannelValueUpdatedEventArgs> ChannelValueUpdated;

        public void SetChannelValue(Channel channel, double value)
        {
        }

        public void StartService()
        {
            throw new NotImplementedException();
        }
    }
}
