using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using Xunit;

namespace WPLGSS.ViewModels.UnitTests
{
    public class SubsequenceViewModelTests
    {
        [Fact]
        public void SubsequenceWillCreateEmptyModelForEmptyEventsCollection()
        {
            var viewModel = new SubsequenceViewModel(new System.Collections.ObjectModel.ObservableCollection<Models.Event>());

            Assert.Equal(2, viewModel.Plot.Axes.Count);
            
            Assert.Single(viewModel.Plot.Axes.OfType<DateTimeAxis>());

            var dateTime = viewModel.Plot.Axes.OfType<DateTimeAxis>().First();

            Assert.Equal(AxisPosition.Top, dateTime.Position);

            Assert.Single(viewModel.Plot.Axes.OfType<CategoryAxis>());

            var category = viewModel.Plot.Axes.OfType<CategoryAxis>().First();

            Assert.Equal(AxisPosition.Left, category.Position);
        }

        [Fact]
        public void SubsequenceBuildsBarItemForEvent()
        {
            var viewModel = new SubsequenceViewModel(new System.Collections.ObjectModel.ObservableCollection<Event>
            {
                new OutputEvent
                {
                    ChannelName = "B",
                    StartTime = TimeSpan.Zero,
                    EndTime = TimeSpan.FromSeconds(1)
                }
            });

            Assert.Single(viewModel.Plot.Series.Cast<IntervalBarSeries>().SelectMany(series => series.Items));

            var item = viewModel.Plot.Series.Cast<IntervalBarSeries>().SelectMany(series => series.Items).First();

            Assert.Equal("Energize B.", item.Title);
            Assert.Equal(new DateTime(1971, 1, 1), DateTimeAxis.ToDateTime(item.Start));
            Assert.Equal(new DateTime(1971, 1, 1, 0, 0, 1), DateTimeAxis.ToDateTime(item.End));
        }

        [Fact]
        public void SubsequenceBuildsBarItemForAbort()
        {
            var viewModel = new SubsequenceViewModel(new System.Collections.ObjectModel.ObservableCollection<Event>
            {
                new AbortCondition
                {
                    ChannelName = "B",
                    StartTime = TimeSpan.Zero,
                    EndTime = TimeSpan.FromSeconds(1),
                    ThresholdMin = 100,
                    ThresholdMax = 110
                }
            });

            Assert.Single(viewModel.Plot.Series.Cast<IntervalBarSeries>().SelectMany(series => series.Items));

            var item = viewModel.Plot.Series.Cast<IntervalBarSeries>().SelectMany(series => series.Items).First();
            
            Assert.Equal("Abort if B exceeds range (100,110).", item.Title);
            Assert.Equal(new DateTime(1971, 1, 1), DateTimeAxis.ToDateTime(item.Start));
            Assert.Equal(new DateTime(1971, 1, 1, 0, 0, 1), DateTimeAxis.ToDateTime(item.End));
        }
    }
}
