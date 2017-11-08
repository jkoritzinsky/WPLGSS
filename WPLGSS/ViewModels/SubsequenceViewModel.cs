using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.ViewModels
{
    class SubsequenceViewModel : BindableBase
    {
        private static readonly DateTime BaseDate = new DateTime(1971, 1, 1); // Use this instead of 1970-1-1 so we don't get an argument out of range when the user is zooming out.

        private readonly CategoryAxis categoryAxis;

        private readonly IntervalBarSeries series;

        public SubsequenceViewModel(ObservableCollection<Event> primaryEvents)
        {
            Plot = new PlotModel
            {
                Axes =
                {
                    new DateTimeAxis
                    {
                        Position = AxisPosition.Top,
                        StringFormat = "HH:mm:ss.FFFFF"
                    },
                    (categoryAxis = new CategoryAxis
                    {
                        Position = AxisPosition.Left
                    })
                },
                Series =
                {
                    (series = new IntervalBarSeries())
                }
            };

            Controller = new PlotController();
            Controller.UnbindMouseDown(OxyMouseButton.Left);
            Controller.BindMouseDown(OxyMouseButton.Left,
                new DelegateViewCommand<OxyMouseDownEventArgs>(OnPlotEdit));

            Events = primaryEvents;
            Events.CollectionChanged += (o, e) =>
            {
                RebuildPlotData();
            };

            RebuildPlotData();
        }

        private void RebuildPlotData()
        {
            var categories = GetCategoriesFromEvents(Events);
            categoryAxis.ItemsSource = categories;
            RebuildItems(Events, categories);
            Plot.InvalidatePlot(true);
        }

        private void RebuildItems(ObservableCollection<Event> events, List<string> categories)
        {
            series.Items.Clear();
            foreach (var evt in events)
            {
                if (evt is OutputEvent)
                {
                    series.Items.Add(new EventBasedIntervalBarItem
                    {
                        CategoryIndex = categories.IndexOf(evt.ChannelName),
                        Start = DateTimeAxis.ToDouble(BaseDate + evt.StartTime),
                        End = DateTimeAxis.ToDouble(BaseDate + evt.EndTime),
                        Color = OxyColors.Green,
                        Title = $"Open {evt.ChannelName}.",
                        Event = evt
                    });
                }
                else if (evt is AbortCondition condition)
                {
                    series.Items.Add(new EventBasedIntervalBarItem
                    {
                        CategoryIndex = categories.IndexOf($"{evt.ChannelName} Abort Condition"),
                        Start = DateTimeAxis.ToDouble(BaseDate + evt.StartTime),
                        End = DateTimeAxis.ToDouble(BaseDate + evt.EndTime),
                        Color = OxyColors.LightSalmon,
                        Title = $"Abort if {evt.ChannelName} exceeds range ({condition.ThresholdMin},{condition.ThresholdMax}).",
                        Event = evt
                    });
                }
            }
        }

        public InteractionRequest<Confirmation> EditEventRequest { get; } = new InteractionRequest<Confirmation>();

        private void OnPlotEdit(IView view, IController controller, OxyMouseDownEventArgs mouseDownArgs)
        {
            var result = series.GetNearestPoint(mouseDownArgs.Position, false);
            if (result == null || result.Position.DistanceTo(mouseDownArgs.Position) > 20)
            {
                return;
            }

            var evt = ((EventBasedIntervalBarItem)result.Item).Event;

            EditEventRequest.Raise(new Confirmation { Title = "Edit Event", Content = evt }, confirmation =>
            {
                if (confirmation.Confirmed)
                {
                    Events.Remove(evt);
                    Events.Add((Event)confirmation.Content);
                    RebuildPlotData();
                }
            });
        }

        private static List<string> GetCategoriesFromEvents(ObservableCollection<Event> events)
        {
            return events.SelectMany(evt => new[] { evt.ChannelName, $"{evt.ChannelName} Abort Condition" }).Distinct().ToList();
        }

        private PlotModel plot;

        public PlotModel Plot
        {
            get { return plot; }
            set { SetProperty(ref plot, value); }
        }

        private PlotController controller;

        public PlotController Controller
        {
            get { return controller; }
            set { SetProperty(ref controller, value); }
        }

        private ObservableCollection<Event> Events { get; }

        private class EventBasedIntervalBarItem : IntervalBarItem
        {
            public Event Event { get; set; }
        }
    }
}
