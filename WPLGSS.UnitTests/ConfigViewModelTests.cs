using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Interactivity;
using WPLGSS.Models;
using WPLGSS.Services;
using Xunit;

namespace WPLGSS.ViewModels.UnitTests
{
    public class ConfigViewModelTests
    {
        [Fact]
        public void CanCreateConfigViewModel()
        {
            var configService = A.Fake<IConfigService>();
            Assert.NotNull(new ConfigViewModel(configService));
        }

        [Fact]
        public void InputChannelsIncludedInInputChannelsCollection()
        {
            InputChannel testChannel;
            var config = new Config
            {
                Channels =
                {
                    (testChannel = new InputChannel())
                }
            };

            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(config);

            var viewModel = new ConfigViewModel(configService);

            Assert.Contains(testChannel, viewModel.InputChannels.OfType<object>());
        }

        [Fact]
        public void InputChannelsNotIncludedInOutputChannelsCollection()
        {
            InputChannel testChannel;
            var config = new Config
            {
                Channels =
                {
                    (testChannel = new InputChannel())
                }
            };

            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(config);

            var viewModel = new ConfigViewModel(configService);

            Assert.DoesNotContain(testChannel, viewModel.OutputChannels.OfType<object>());
        }

        [Fact]
        public void OutputChannelsIncludedInOutputChannelsCollection()
        {
            Channel testChannel;
            var config = new Config
            {
                Channels =
                {
                    (testChannel = new Channel())
                }
            };

            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(config);

            var viewModel = new ConfigViewModel(configService);

            Assert.Contains(testChannel, viewModel.OutputChannels.OfType<object>());
        }

        [Fact]
        public void InputChannelsAddedAfterConstructionIncludedInInputChannelsCollection()
        {
            var config = new Config
            {
                Channels = { }
            };

            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(config);

            var viewModel = new ConfigViewModel(configService);

            var testChannel = new InputChannel();
            config.Channels.Add(testChannel);
            Assert.Contains(testChannel, viewModel.InputChannels.OfType<object>());
        }

        [Fact]
        public void OutputChannelsAddedAfterConstructionIncludedInOutputChannelsCollection()
        {
            var config = new Config
            {
                Channels = { }
            };

            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(config);

            var viewModel = new ConfigViewModel(configService);

            var testChannel = new Channel();
            config.Channels.Add(testChannel);
            Assert.Contains(testChannel, viewModel.OutputChannels.OfType<object>());
        }

        [Fact]
        public void AddInputCommandAddsNewInputChannel()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var viewModel = new ConfigViewModel(configService);
            viewModel.AddInputChannelCommand.Execute(null);

            Assert.Single(viewModel.InputChannels);
        }

        [Fact]
        public void AddOutputCommandAddsNewOutputChannel()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var viewModel = new ConfigViewModel(configService);
            viewModel.AddOutputChannelCommand.Execute(null);

            Assert.Single(viewModel.OutputChannels);
        }

        [Fact]
        public void LoadingNewConfigFileUpdatesConfigViewModel()
        {
            var configService = A.Fake<IConfigService>();
            var config = new Config();
            A.CallTo(() => configService.Config).Returns(config);

            var viewModel = new ConfigViewModel(configService);

            Assert.Empty(viewModel.OutputChannels);
            Assert.Empty(viewModel.InputChannels);

            config.Channels = new System.Collections.ObjectModel.ObservableCollection<Channel>
            {
                new InputChannel()
            };

            configService.ConfigFileLoaded += Raise.WithEmpty();

            Assert.Single(viewModel.InputChannels);
        }

        [Fact]
        public void SaveCommandRaisesInteractionRequest()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var viewModel = new ConfigViewModel(configService);

            var saveRequestRaised = false;

            viewModel.SaveRequest.Raised += (o, e) => saveRequestRaised = true;

            viewModel.SaveConfigCommand.Execute(null);

            Assert.True(saveRequestRaised);
        }

        [Fact]
        public void SaveRequestCallbackSavesConfig()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var viewModel = new ConfigViewModel(configService);

            const string path = "FakePath.config";

            viewModel.SaveRequest.Raised += (o, e) =>
            {
                Assert.IsType<FileInteractionNotification>(e.Context);
                var notification = (FileInteractionNotification)e.Context;
                notification.Confirmed = true;
                notification.Path = path;
                e.Callback();
            };

            viewModel.SaveConfigCommand.Execute(null);

            A.CallTo(() => configService.SaveConfig(path)).MustHaveHappened();
        }

        [Fact]
        public void OpenCommandRaisesInteractionRequest()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var viewModel = new ConfigViewModel(configService);

            var openRequestRaised = false;

            viewModel.OpenRequest.Raised += (o, e) => openRequestRaised = true;

            viewModel.OpenConfigCommand.Execute(null);

            Assert.True(openRequestRaised);
        }

        [Fact]
        public void OpenRequestCallbackOpensConfig()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var viewModel = new ConfigViewModel(configService);

            const string path = "FakePath.config";

            viewModel.OpenRequest.Raised += (o, e) =>
            {
                Assert.IsType<FileInteractionNotification>(e.Context);
                var notification = (FileInteractionNotification)e.Context;
                notification.Confirmed = true;
                notification.Path = path;
                e.Callback();
            };

            viewModel.OpenConfigCommand.Execute(null);

            A.CallTo(() => configService.LoadConfig(path)).MustHaveHappened();
        }
    }
}
