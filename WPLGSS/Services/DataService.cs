using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    [Export(typeof(IDataAquisition))]
    public class DataService : IDataAquisition
    {
        public event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        public event EventHandler<StatusChangedEventArgs> ArduinoConnectionChanged;
        public event EventHandler<ChannelValueUpdatedEventArgs> ChannelValueUpdated;

        private LabJackGateway LJ = new LabJackGateway();
        private System.Timers.Timer SampleTimer;
        private IConfigService config;

        private double[] dataIn;
        private double[] dataOut;

        [ImportingConstructor]
        public DataService(IConfigService config)
        {
            this.config = config;
        }

        private void Sample(object source, System.Timers.ElapsedEventArgs e)
        {
            LJ.SetAnalogData(dataOut);
            dataIn = LJ.GetAnalogData();

            if (LJ.ret != LabJack.LJM.LJMERROR.NOERROR)
            {
                SynchronizationContext.Current.Post(_ => {

                }, null);
            }

            DateTime SampleTime = DateTime.Now;

            SynchronizationContext.Current.Post( _ => {
                foreach (var chan in config.Config.Channels)
                {
                    if (chan is InputChannel input && chan.Source == ChannelSource.LabJack)
                    {
                        ChannelValueUpdated?.Invoke(
                            this,
                            new ChannelValueUpdatedEventArgs(chan, input.ScalingFunction(dataIn[input.ChannelId]), SampleTime)
                        );
                    }
                }
            }, null);

        }

        public void StartService()
        {
            LJ.OpenLabJack();

            SampleTimer = new System.Timers.Timer(10);
            SampleTimer.Elapsed += new System.Timers.ElapsedEventHandler(Sample);
            SampleTimer.Enabled = true;

            dataOut = new double[2];

            dataOut[0] = 0;
            dataOut[1] = 0;
        }

        public void SetChannelValue(Channel channel, double value)
        {
            if (value == 0) dataOut[channel.ChannelId] = 0;
            if (value == 1) dataOut[channel.ChannelId] = 12;
        }
    }
}