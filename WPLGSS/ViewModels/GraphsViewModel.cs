using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Events;
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
    [Export]
    public class GraphsViewModel : BindableBase
    {
        private const int NumPoints = 10000;
        private const string XAxisKey = "Time";
        private readonly IEventAggregator eventAggregator;
        private Dictionary<Channel, List<LineSeries>> channelSeriesMap = new Dictionary<Channel, List<LineSeries>>();

        [ImportingConstructor]
        public GraphsViewModel(IEventAggregator eventAggregator, IDataAquisition dataAquisition)
        {
            eventAggregator.GetEvent<AddToGraphEvent>().Subscribe((param) => AddToGraph(param.channel, param.graphId));
            this.eventAggregator = eventAggregator;
            dataAquisition.ChannelValueUpdated += (o, e) => RecordValue(e.Channel, e.Value, e.Time);
        }

        public ObservableCollection<PlotModel> Graphs { get; } = new ObservableCollection<PlotModel>();

        private void AddToGraph(Channel channel, int graphId)
        {
            var inputChannel = (InputChannel)channel;


            List<LineSeries> channelSeriesList;
            if (channelSeriesMap.ContainsKey(channel))
            {
                channelSeriesList = channelSeriesMap[channel];
            }
            else
            {
                channelSeriesList = new List<LineSeries>();
                channelSeriesMap.Add(channel, channelSeriesList); 
            }

            var channelSeries = new LineSeries
            {
                XAxisKey = XAxisKey,
                YAxisKey = inputChannel.Unit,
                Title = channel.Name
            };

            channelSeriesList.Add(channelSeries);

            if (graphId == 0)
            {
                Graphs.Add(new PlotModel
                {
                    Axes =
                    {
                        new LinearAxis
                        {
                            Key = inputChannel.Unit,
                            Title = inputChannel.Unit
                        },
                        new DateTimeAxis
                        {
                            Title = "Time",
                            Key = XAxisKey,
                            StringFormat = "HH:mm:ss.FFFFF",
                            Position = AxisPosition.Bottom
                        }
                    },
                    Series =
                    {
                        channelSeries
                    },
                    IsLegendVisible = true,
                    LegendPosition = LegendPosition.BottomCenter,
                    LegendPlacement = LegendPlacement.Outside
                });
                eventAggregator.GetEvent<GraphCreatedEvent>().Publish(Graphs.Count);
            }
            else
            {
                var graph = Graphs[graphId - 1];
                graph.Series.Add(channelSeries);
                if (!graph.Axes.Any(axis => axis.Key == inputChannel.Unit))
                {
                    graph.Axes.Add(
                        new LinearAxis
                        {
                            Key = inputChannel.Unit,
                            Title = inputChannel.Unit,
                            Position = graph.Axes.Count == 2 ? AxisPosition.Right : AxisPosition.None
                        });
                    graph.InvalidatePlot(false);
                }
            }
        }

        private void RecordValue(Channel channel, double value, DateTime time)
        {
            var seriesList = channelSeriesMap[channel];
            seriesList.ForEach(series =>
            {
                if (series.Points.Count >= NumPoints)
                    series.Points.RemoveAt(0);
                series.Points.Add(DateTimeAxis.CreateDataPoint(time, value));
                series.PlotModel.InvalidatePlot(true);
            });
        }
    }
}
