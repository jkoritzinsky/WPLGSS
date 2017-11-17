using FakeItEasy;
using Prism.Events;
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
    public class LiveDataViewModelTests
    {
        [Fact]
        public void ViewModelPopulatesInstanceWithLiveChannelsForEachChannel()
        {
            var configService = A.Fake<IConfigService>();
            var channel = new Channel();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    channel
                }
            });

            var viewModel = new LiveDataViewModel(configService, A.Fake<IDataAquisition>(), A.Fake<IEventAggregator>());

            Assert.Single(viewModel.Channels);
            Assert.Equal(channel, viewModel.Channels[0].Channel);
        }

        [Fact]
        public void RaisingAddToGraphCommandWithRaisesAddToGraphEvent()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var eventAggregator = A.Fake<IEventAggregator>();
            var addToGraphEvent = A.Fake<AddToGraphEvent>();
            A.CallTo(() => eventAggregator.GetEvent<AddToGraphEvent>()).Returns(addToGraphEvent);
            A.CallTo(() => eventAggregator.GetEvent<GraphCreatedEvent>()).Returns(new GraphCreatedEvent());

            var viewModel = new LiveDataViewModel(configService, A.Fake<IDataAquisition>(), eventAggregator);

            var channelGraphTuple = Tuple.Create(new InputChannel(), 0);

            viewModel.AddToGraphCommand.Execute(channelGraphTuple);

            A.CallTo(() => addToGraphEvent.Publish(A<(InputChannel, int)>
                .That.Matches(payload =>
                    payload.Item1 == channelGraphTuple.Item1
                    && payload.Item2 == channelGraphTuple.Item2)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ChannelUpdatedByAquisitionServiceUpdatesViewModel()
        {
            var configService = A.Fake<IConfigService>();
            var channel = new Channel();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    channel
                }
            });

            var dataAquisition = A.Fake<IDataAquisition>();

            var viewModel = new LiveDataViewModel(configService, dataAquisition, A.Fake<IEventAggregator>());

            dataAquisition.ChannelValueUpdated += Raise.With(new ChannelValueUpdatedEventArgs(channel, 2, DateTime.Now));

            Assert.Equal(2, viewModel.Channels.First(live => live.Channel == channel).Value);
        }

        [Fact]
        public void SendOutputValueCommandSendsValueToDataAquisition()
        {
            var configService = A.Fake<IConfigService>();
            var channel = new Channel();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    channel
                }
            });

            var dataAquisition = A.Fake<IDataAquisition>();

            var viewModel = new LiveDataViewModel(configService, dataAquisition, A.Fake<IEventAggregator>());

            var live = viewModel.Channels.First(liveChannel => liveChannel.Channel == channel);
            live.Value = 5;

            viewModel.SendOutputValueCommand.Execute(live);

            A.CallTo(() => dataAquisition.SetChannelValue(channel, 5)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void FiringGraphCreatedEventUpdatesGraphIdsInViewModel()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var eventAggregator = A.Fake<IEventAggregator>();
            var graphCreatedEvent = new GraphCreatedEvent();
            A.CallTo(() => eventAggregator.GetEvent<GraphCreatedEvent>()).Returns(graphCreatedEvent);

            var viewModel = new LiveDataViewModel(configService, A.Fake<IDataAquisition>(), eventAggregator);
            
            graphCreatedEvent.Publish(1);

            Assert.Contains(1, viewModel.GraphIds);
        }
    }
}
