using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
