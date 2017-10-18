﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    [Export]
    public class ConfigViewModel : BindableBase
    {
        private readonly IConfigService configService;

        [ImportingConstructor]
        public ConfigViewModel(IConfigService configService)
        {
            this.configService = configService;
            configService.ConfigFileLoaded += (o, e) => UpdateFromUnderlyingConfig(configService.Config);
            UpdateFromUnderlyingConfig(configService.Config);
            AddInputChannelCommand = new DelegateCommand(AddInputChannel);
            AddOutputChannelCommand = new DelegateCommand(AddOutputChannel);
        }

        public string Name => "Configuration";

        private ICollectionView inputChannels;

        public ICollectionView InputChannels
        {
            get
            {
                return inputChannels;
            }
            set
            {
                SetProperty(ref inputChannels, value);
            }
        }

        private ICollectionView outputChannels;

        public ICollectionView OutputChannels
        {
            get
            {
                return outputChannels;
            }
            set
            {
                SetProperty(ref outputChannels, value);
            }
        }

        public ICommand AddInputChannelCommand { get; }
        public ICommand AddOutputChannelCommand { get; }

        private void AddInputChannel()
        {
            configService.Config.Channels.Add(new InputChannel());
        }

        private void AddOutputChannel()
        {
            configService.Config.Channels.Add(new Channel());
        }

        private void UpdateFromUnderlyingConfig(Config config)
        {
            InputChannels = new ListCollectionView(config.Channels)
            {
                Filter = obj => obj is InputChannel
            };
            
            OutputChannels = new ListCollectionView(config.Channels)
            {
                Filter = obj => !(obj is InputChannel)
            };
        }
    }
}
