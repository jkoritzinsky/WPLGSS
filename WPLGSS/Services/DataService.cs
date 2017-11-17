using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    [Export(typeof(IDataAquisition))]
    public class DataService : IDataAquisition
    {
        public event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        public event EventHandler<StatusChangedEventArgs> ArduinoConnectionChanged;
        public event EventHandler<ChannelValueUpdatedEventArgs> ChannelValueUpdated;

        private LabJackGateway LJ;
        private System.Timers.Timer SampleTimer;

        private byte[] dataIn;
        private byte[] dataOut;

        private void Sample(object source, System.Timers.ElapsedEventArgs e)
        {
            LJ.SetAnalogData(dataOut, 2);
            dataIn = LJ.GetAnalogData(0);
        }

        public void StartService()
        {
            LJ.OpenLabJack();

            SampleTimer = new System.Timers.Timer(10);
            SampleTimer.Elapsed += new System.Timers.ElapsedEventHandler(Sample);
            SampleTimer.Enabled = true;

            dataOut = new byte[2];

            dataOut[0] = 0;
            dataOut[1] = 0;
        }
    }
}