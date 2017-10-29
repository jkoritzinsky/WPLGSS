﻿using Prism.Commands;
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

        [ImportingConstructor]
        public LiveDataViewModel(IConfigService config, IDataAquisition dataAquisition, IEventAggregator eventAggregator)
        {
            UpdateLiveChannelsFromConfig(config.Config);

            config.ConfigFileLoaded += (o, e) => UpdateLiveChannelsFromConfig(config.Config);

            dataAquisition.ChannelValueUpdated += ChannelValueUpdated;
            AddToGraphCommand = new DelegateCommand<Tuple<Channel, int>>(param =>
            {
                eventAggregator.GetEvent<AddToGraphEvent>().Publish((param.Item1, param.Item2));
            });

            eventAggregator.GetEvent<GraphCreatedEvent>().Subscribe(id => GraphIds.Add(id));
            this.config = config;
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

        private ObservableCollection<LiveChannel> channels = new ObservableCollection<LiveChannel>();

        public ObservableCollection<LiveChannel> Channels
        {
            get { return channels; }
            set { SetProperty(ref channels, value); }
        }


        public ICommand AddToGraphCommand { get; }

        public ObservableCollection<int> GraphIds { get; } = new ObservableCollection<int> { 0 };
    }
}
