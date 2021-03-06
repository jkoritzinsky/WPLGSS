﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WPLGSS.Models;
using System.IO;
using System.Collections.Concurrent;

namespace WPLGSS.Services
{
    [Export(typeof(IDataAquisition))]
    public class DataService : IDataAquisition, IDisposable
    {
        public event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        public event EventHandler<ChannelValueUpdatedEventArgs> ChannelValueUpdated;

        public ConcurrentDictionary<InputChannel, double> ChannelValues { get; } = new ConcurrentDictionary<InputChannel, double>();

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

            dataOut = new double[25];
            for (int i=0; i<25; i++)
            {
                dataOut[i] = 0;
            }

            LJ = lj;
        }

        private void RunOnMainThread(SendOrPostCallback callback)
        {
            if (SynchronizationContext.Current != null)
            {
                SynchronizationContext.Current.Post(callback, null);
            }
            else
            {
                callback(null);
            }
        }

        private void Sample(object source, System.Timers.ElapsedEventArgs e)
        {
            LJ.SetAnalogData(dataOut);
            dataIn = LJ.GetAnalogData();

            DateTime SampleTime = DateTime.Now;

            if (LJ.LastError == LabJack.LJM.LJMERROR.NOERROR)
            {
                RunOnMainThread(_ =>
                {
                    ChannelValues.Clear();
                    foreach (var chan in config.Config.Channels)
                    {
                        if (chan is InputChannel input && chan.Source == ChannelSource.LabJack)
                        {
                            double value;
                            if (input.ScalingFunction != null)
                            {
                                value = input.ScalingFunction(dataIn[input.ChannelId]);
                            }
                            else
                            {
                                value = dataIn[input.ChannelId];
                            }
                            ChannelValues[input] = value;
                            ChannelValueUpdated?.Invoke(
                                this,
                                new ChannelValueUpdatedEventArgs(chan, value, SampleTime)
                            );
                        }
                        else
                        {
                            ChannelValueUpdated?.Invoke(
                                this,
                                new ChannelValueUpdatedEventArgs(chan, dataOut[chan.ChannelId] < 0.5 ? 0 : 1, SampleTime)
                            );
                        }
                    }
                });
            }

            if (recording)
            {
                recStream.Write((DateTime.Now - startRecTime).TotalMilliseconds);
                foreach (var chan in config.Config.Channels)
                {
                    if (chan is InputChannel input && chan.Source == ChannelSource.LabJack)
                        recStream.Write("\t" + ChannelValues[input]);
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

            if (SampleTimer == null)
            {
                SampleTimer = new System.Timers.Timer(10);
                SampleTimer.Elapsed += new System.Timers.ElapsedEventHandler(Sample); 
            }
            SampleTimer.Enabled = true;
        }

        public void PauseService()
        {
            if (SampleTimer != null)
            {
                SampleTimer.Enabled = false; 
            }
        }

        public void SetChannelValue(Channel channel, double value)
        {
            if (0 <= channel.ChannelId && channel.ChannelId <= dataOut.Length)
            {
                dataOut[channel.ChannelId] = value;
            }

            RunOnMainThread(_ => ChannelValueUpdated?.Invoke(this, new ChannelValueUpdatedEventArgs(channel, value, DateTime.Now)));
        }

        public void Dispose()
        {
            SampleTimer.Stop();
            SampleTimer.Dispose();
        }
    }
}