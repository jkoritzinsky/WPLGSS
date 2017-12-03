using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    [Export]
    public class LiveDataViewModel : BindableBase
    {
        private readonly IConfigService config;
        private readonly IDataAquisition dataAquisition;

        [ImportingConstructor]
        public LiveDataViewModel(IConfigService config, IDataAquisition dataAquisition, IEventAggregator eventAggregator, ISequenceRunner runner)
        {
            UpdateLiveChannelsFromConfig(config.Config);

            config.ConfigFileLoaded += (o, e) => UpdateLiveChannelsFromConfig(config.Config);

            dataAquisition.ChannelValueUpdated += ChannelValueUpdated;
            AddToGraphCommand = new DelegateCommand<Tuple<InputChannel, int>>(param =>
            {
                eventAggregator.GetEvent<AddToGraphEvent>().Publish((param.Item1, param.Item2));
            });

            SendOutputValueCommand = new DelegateCommand<LiveChannel>(SendOutputValue);

            eventAggregator.GetEvent<GraphCreatedEvent>().Subscribe(id => GraphIds.Add(id));
            runner.SequenceRunningStateChanged += (o, e) => EnableOutputToggling = !e.Status;
            this.config = config;
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

        private bool enableOutputToggling;

        public bool EnableOutputToggling
        {
            get { return enableOutputToggling; }
            set { SetProperty(ref enableOutputToggling, value); }
        }



        public ICommand AddToGraphCommand { get; }

        public ICommand SendOutputValueCommand { get; }

        public ObservableCollection<int> GraphIds { get; } = new ObservableCollection<int> { 0 };
    }
}
