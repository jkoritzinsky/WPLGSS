using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    public enum EventType
    {
        Output,
        Abort
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EventEditorViewModel : BindableBase, IInteractionRequestAware
    {
        private EventType type;

        [ImportingConstructor]
        public EventEditorViewModel(IConfigService config)
        {
            ChannelOptions = config.Config.Channels.Select(channel => channel.Name);
            FinishCommand = new DelegateCommand(() =>
            {
                if (notification is IConfirmation confirmation)
                {
                    confirmation.Confirmed = true;
                }
                notification.Content = CreateEvent();
                FinishInteraction?.Invoke();
            });
            CancelCommand = new DelegateCommand(() =>
            {
                if (notification is IConfirmation confirmation)
                {
                    confirmation.Confirmed = false;
                }
                FinishInteraction?.Invoke();
            });
        }

        public EventType Type
        {
            get { return type; }
            set
            {
                SetProperty(ref type, value);
            }
        }

        private string channel;

        public string Channel
        {
            get { return channel; }
            set { SetProperty(ref channel, value); }
        }

        public IEnumerable<string> ChannelOptions { get; }

        private TimeSpan startTime;

        public TimeSpan StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value); }
        }

        private TimeSpan endTime;

        public TimeSpan EndTime
        {
            get { return endTime; }
            set { SetProperty(ref endTime, value); }
        }

        private double thresholdMin;

        public double ThresholdMin
        {
            get { return thresholdMin; }
            set { SetProperty(ref thresholdMin, value); }
        }

        private double thresholdMax;

        public double ThresholdMax
        {
            get { return thresholdMax; }
            set { SetProperty(ref thresholdMax, value); }
        }

        private INotification notification;

        public INotification Notification
        {
            get { return notification; }
            set
            {
                SetProperty(ref notification, value);
                if (value.Content is Event evt)
                {
                    SetInitialContent(evt);
                }
            }
        }

        private void SetInitialContent(Event evt)
        {
            StartTime = evt.StartTime;
            EndTime = evt.EndTime;
            Channel = evt.ChannelName;
            if (evt is AbortCondition abort)
            {
                Type = EventType.Abort;
                ThresholdMin = abort.ThresholdMin;
                ThresholdMax = abort.ThresholdMax;
            }
            else if (evt is OutputEvent)
            {
                Type = EventType.Output;
            }
        }

        private Event CreateEvent()
        {
            Event evt;
            switch (Type)
            {
                case EventType.Output:
                    evt = new OutputEvent();
                    break;
                case EventType.Abort:
                    evt = new AbortCondition
                    {
                        ThresholdMin = ThresholdMin,
                        ThresholdMax = ThresholdMax
                    };
                    break;
                default:
                    throw new InvalidOperationException("Invalid event type");
            }
            evt.StartTime = StartTime;
            evt.EndTime = EndTime;
            evt.ChannelName = Channel;
            return evt;
        }

        public Action FinishInteraction { get; set; }

        public ICommand FinishCommand { get; }
        public ICommand CancelCommand { get; }
    }
}
