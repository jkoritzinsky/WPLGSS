using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WPLGSS.Models;
using System.IO;

namespace WPLGSS.Services
{
    [Export(typeof(IDataAquisition))]
    public class DataService : IDataAquisition
    {
        public event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        public event EventHandler<StatusChangedEventArgs> ArduinoConnectionChanged;
        public event EventHandler<ChannelValueUpdatedEventArgs> ChannelValueUpdated;

        private ILabJackGateway LJ;
        private System.Timers.Timer SampleTimer;
        private IConfigService config;

        bool recording = false;
        System.IO.StreamWriter recStream;
        DateTime startRecTime;

        private double[] dataIn;
        private double[] dataOut;

        [ImportingConstructor]
        public DataService(IConfigService config, ILabJackGateway lj)
        {
            this.config = config;
            LJ = lj;
        }

        private void Sample(object source, System.Timers.ElapsedEventArgs e)
        {
            LJ.SetAnalogData(dataOut);
            dataIn = LJ.GetAnalogData();

            DateTime SampleTime = DateTime.Now;

            if (LJ.LastError != LabJack.LJM.LJMERROR.NOERROR)
            {
                SynchronizationContext.Current.Post(_ => {
                    foreach (var chan in config.Config.Channels)
                    {
                        if (chan is InputChannel input && chan.Source == ChannelSource.LabJack)
                        {
                            (chan as InputChannel).value = input.ScalingFunction(dataIn[input.ChannelId]);
                            ChannelValueUpdated?.Invoke(
                                this,
                                new ChannelValueUpdatedEventArgs(chan, (chan as InputChannel).value, SampleTime)
                            );
                        }
                    }
                }, null);
            }

            if (recording)
            {
                recStream.Write((DateTime.Now - startRecTime).TotalMilliseconds);
                foreach (var chan in config.Config.Channels)
                {
                    if (chan is InputChannel input && chan.Source == ChannelSource.LabJack)
                        recStream.Write("\t" + (chan as InputChannel).value);
                }
                recStream.WriteLine();
            }
            else
            {
                if (recStream != null)
                {
                    recStream.Close();
                    recStream.Dispose();
                    recStream = null;
                }
            }
        }

        public void StartStopRecord(string path)
        {
            if (!recording)
            {
                recStream = new StreamWriter(File.OpenWrite(path));
                foreach (Channel ch in config.Config.Channels)
                {
                    if (ch is InputChannel input && ch.Source == ChannelSource.LabJack)
                        recStream.Write("\t" + ch.Name);
                }

                recStream.WriteLine();
                startRecTime = DateTime.Now;
            }

            recording = !recording;
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