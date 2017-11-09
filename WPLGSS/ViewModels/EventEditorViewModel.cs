using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Models;
using WPLGSS.Services;
using System.Collections;
using System.Runtime.CompilerServices;

namespace WPLGSS.ViewModels
{
    public enum EventType
    {
        Output,
        Abort
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EventEditorViewModel : BindableBase, IInteractionRequestAware, INotifyDataErrorInfo
    {
        private EventType type;

        [ImportingConstructor]
        public EventEditorViewModel(IConfigService config)
        {
            ChannelOptions = config.Config.Channels;
            FinishCommand = new DelegateCommand(() =>
            {
                if (notification is IConfirmation confirmation)
                {
                    confirmation.Confirmed = true;
                }
                notification.Content = CreateEvent();
                FinishInteraction?.Invoke();
            }, () => !HasErrors);
            CancelCommand = new DelegateCommand(() =>
            {
                if (notification is IConfirmation confirmation)
                {
                    confirmation.Confirmed = false;
                }
                FinishInteraction?.Invoke();
            });

            Channel = ChannelOptions.FirstOrDefault()?.Name;

            Validate();
        }

        public EventType Type
        {
            get { return type; }
            set
            {
                SetProperty(ref type, value, Validate);
            }
        }

        private string channel;

        public string Channel
        {
            get { return channel; }
            set { SetProperty(ref channel, value, Validate); }
        }

        public IEnumerable<Channel> ChannelOptions { get; }

        private TimeSpan startTime;

        public TimeSpan StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value, Validate); }
        }

        private TimeSpan endTime;

        public TimeSpan EndTime
        {
            get { return endTime; }
            set { SetProperty(ref endTime, value, Validate); }
        }

        private double thresholdMin;

        public double ThresholdMin
        {
            get { return thresholdMin; }
            set { SetProperty(ref thresholdMin, value, Validate); }
        }

        private double thresholdMax;

        public double ThresholdMax
        {
            get { return thresholdMax; }
            set
            {
                SetProperty(ref thresholdMax, value, Validate);
            }
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

        public DelegateCommand FinishCommand { get; }
        public ICommand CancelCommand { get; }

        public bool HasErrors => errors.Count != 0;

        private Dictionary<string, string> errors = new Dictionary<string, string>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
            {
                yield return errors[propertyName];
            }
        }

        private void Validate()
        {
            var updatedValidations = new List<string>();

            if (!(ThresholdMin < ThresholdMax) && Type == EventType.Abort)
            {
                const string thresholdError = "Threshold minimum must be less than the threshold maximum.";
                errors[nameof(ThresholdMin)] = thresholdError;
                errors[nameof(ThresholdMax)] = thresholdError;
                updatedValidations.Add(nameof(ThresholdMin));
                updatedValidations.Add(nameof(ThresholdMax));
            }
            else
            {
                errors.Remove(nameof(ThresholdMin));
                errors.Remove(nameof(ThresholdMax));
                updatedValidations.Add(nameof(ThresholdMin));
                updatedValidations.Add(nameof(ThresholdMax));
            }


            const string timeError = "Start time must be before end time.";
            if (!(StartTime < EndTime))
            {
                errors[nameof(EndTime)] = timeError;
                updatedValidations.Add(nameof(EndTime));
            }
            else
            {
                errors.Remove(nameof(EndTime));
                updatedValidations.Add(nameof(EndTime));
            }

            if (StartTime < TimeSpan.Zero)
            {
                errors[nameof(StartTime)] = $"Start time cannot be before {TimeSpan.Zero}.";
            }
            else
            {
                errors.Remove(nameof(StartTime));
                updatedValidations.Add(nameof(StartTime));
            }
            
            if (ChannelOptions.All(channel => channel.Name != Channel))
            {
                errors[nameof(Channel)] = "Channel name is not a channel name in this current configuration";
                updatedValidations.Add(nameof(Channel));
            }
            else if (ChannelOptions.First(channel => channel.Name == Channel) is InputChannel && Type == EventType.Output)
            {
                errors[nameof(Channel)] = "Cannot set an output on an input channel.";
                updatedValidations.Add(nameof(Channel));
            }
            else if (!(ChannelOptions.First(channel => channel.Name == Channel) is InputChannel) && Type == EventType.Abort)
            {
                errors[nameof(Channel)] = "Cannot set an abort condition on an output channel.";
                updatedValidations.Add(nameof(Channel));
            }
            else
            {
                errors.Remove(nameof(Channel));
                updatedValidations.Add(nameof(Channel));
            }

            updatedValidations.ForEach(prop => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop)));
            RaisePropertyChanged(nameof(HasErrors));
            FinishCommand.RaiseCanExecuteChanged();
        }
        
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}
