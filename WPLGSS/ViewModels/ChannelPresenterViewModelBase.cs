using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    public class ChannelPresenterViewModelBase : BindableBase
    {
        private readonly IDataAquisition dataAquisition;

        [ImportingConstructor]
        public ChannelPresenterViewModelBase(IConfigService config, IDataAquisition dataAquisition, ISequenceRunner runner)
        {
            UpdateLiveChannelsFromConfig(config.Config);

            config.ConfigFileLoaded += (o, e) => UpdateLiveChannelsFromConfig(config.Config);

            dataAquisition.ChannelValueUpdated += ChannelValueUpdated;
            SendOutputValueCommand = new DelegateCommand<LiveChannel>(SendOutputValue, channel => channel != null && EnableOutputToggling);

            runner.SequenceRunningStateChanged += (o, e) => EnableOutputToggling = !e.Status;
            this.dataAquisition = dataAquisition;
        }

        private void UpdateLiveChannelsFromConfig(Config config)
        {
            UpdateLiveChannels(config.Channels);
            config.Channels.CollectionChanged += (o, e) => UpdateLiveChannels((IEnumerable<Channel>)o);
        }

        private void UpdateLiveChannels(IEnumerable<Channel> channels)
        {
            Channels.Clear();
            Channels.AddRange(channels.Select(channel => new LiveChannel(channel)));
        }

        private void ChannelValueUpdated(object sender, ChannelValueUpdatedEventArgs e)
        {
            var channelToUpdate = Channels.FirstOrDefault(channel => channel.Channel == e.Channel);
            if (channelToUpdate != null)
            {
                channelToUpdate.Value = e.Value;
            }
        }

        private void SendOutputValue(LiveChannel channel)
        {
            dataAquisition.SetChannelValue(channel.Channel, channel.Value);
        }

        private ObservableCollection<LiveChannel> channels = new ObservableCollection<LiveChannel>();

        public ObservableCollection<LiveChannel> Channels
        {
            get { return channels; }
            set { SetProperty(ref channels, value); }
        }

        private bool enableOutputToggling = true;

        public bool EnableOutputToggling
        {
            get { return enableOutputToggling; }
            set
            {
                SetProperty(ref enableOutputToggling, value);
                SendOutputValueCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand<LiveChannel> SendOutputValueCommand { get; }
    }
}
