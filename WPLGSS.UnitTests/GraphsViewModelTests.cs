using FakeItEasy;
using OxyPlot.Axes;
using OxyPlot.Series;
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
    public class GraphsViewModelTests
    {
        [Fact]
        public void SubscribesToAddToGraphEvent()
        {
            var eventAggregator = A.Fake<IEventAggregator>();

            var evt = A.Fake<AddToGraphEvent>();

            A.CallTo(() => eventAggregator.GetEvent<AddToGraphEvent>()).Returns(evt);

            var viewModel = new GraphsViewModel(eventAggregator, A.Fake<IDataAquisition>());

            A.CallTo(() => evt.Subscribe(A<Action<(InputChannel, int)>>.Ignored, A<ThreadOption>.Ignored, A<bool>.Ignored, A<Predicate<(InputChannel channel, int graphId)>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void PublishingAddToGraphEventWithId0CreatesNewGraph()
        {
            var eventAggregator = A.Fake<IEventAggregator>();

            var evt = new AddToGraphEvent();

            A.CallTo(() => eventAggregator.GetEvent<AddToGraphEvent>()).Returns(evt);

            var viewModel = new GraphsViewModel(eventAggregator, A.Fake<IDataAquisition>());
            A.CallTo(() => eventAggregator.GetEvent<GraphCreatedEvent>()).Returns(new GraphCreatedEvent());

            evt.Publish((new InputChannel(), 0));

            Assert.Single(viewModel.Graphs);
        }

        [Fact]
        public void PublishingAddToGraphEventWithExistingGraphIdUsesExistingGraph()
        {
            var eventAggregator = A.Fake<IEventAggregator>();

            var evt = new AddToGraphEvent();

            A.CallTo(() => eventAggregator.GetEvent<AddToGraphEvent>()).Returns(evt);
            A.CallTo(() => eventAggregator.GetEvent<GraphCreatedEvent>()).Returns(new GraphCreatedEvent());

            var viewModel = new GraphsViewModel(eventAggregator, A.Fake<IDataAquisition>());

            evt.Publish((new InputChannel(), 0));

            evt.Publish((new InputChannel(), 1));

            Assert.Single(viewModel.Graphs);
        }

        [Fact]
        public void PublishingAddToGraphEventWithId0RaisesGraphCreatedEvent()
        {
            var eventAggregator = A.Fake<IEventAggregator>();

            var evt = new AddToGraphEvent();

            var graphCreated = A.Fake<GraphCreatedEvent>();

            A.CallTo(() => eventAggregator.GetEvent<AddToGraphEvent>()).Returns(evt);
            A.CallTo(() => eventAggregator.GetEvent<GraphCreatedEvent>()).Returns(graphCreated);

            var viewModel = new GraphsViewModel(eventAggregator, A.Fake<IDataAquisition>());

            evt.Publish((new InputChannel(), 0));

            A.CallTo(() => graphCreated.Publish(1)).MustHaveHappened();
        }

        [Fact]
        public void RaisingChannelValueUpdatedEventForGraphedChannelAddsPoint()
        {
            var eventAggregator = A.Fake<IEventAggregator>();

            var evt = new AddToGraphEvent();

            A.CallTo(() => eventAggregator.GetEvent<AddToGraphEvent>()).Returns(evt);
            A.CallTo(() => eventAggregator.GetEvent<GraphCreatedEvent>()).Returns(new GraphCreatedEvent());

            var dataAquisition = A.Fake<IDataAquisition>();

            var channel = new InputChannel();

            var viewModel = new GraphsViewModel(eventAggregator, dataAquisition);
            evt.Publish((channel, 0));

            var time = DateTime.Now;

            dataAquisition.ChannelValueUpdated += Raise.With(new ChannelValueUpdatedEventArgs(channel, 3.0, time));

            Assert.IsType<LineSeries>(viewModel.Graphs[0].Series[0]);
            var lineSeries = (LineSeries)viewModel.Graphs[0].Series[0];
            var point = lineSeries.Points[0];

            Assert.Equal(DateTimeAxis.ToDouble(time), point.X);
            Assert.Equal(3.0, point.Y);
        }
    }
}
