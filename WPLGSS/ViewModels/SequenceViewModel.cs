using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Interactivity;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    public class SequenceViewModel
    {
        public enum Subsequence
        {
            Primary,
            Abort
        }

        public SequenceViewModel(Sequence sequence)
        {
            Sequence = sequence;
            AddEventCommand = new DelegateCommand<string>(seq =>
                AddEvent(
                    (Subsequence)Enum.Parse(typeof(Subsequence), seq)),
                    seq => Enum.TryParse<Subsequence>(seq, out _));
        }

        private void AddEvent(Subsequence sequence)
        {
            CreateEventRequest.Raise(
                new Confirmation
                {
                    Title = "Add Event",
                },
                confirmation =>
                {
                    if (confirmation.Confirmed)
                    {
                        var evt = (Event)confirmation.Content;
                        switch (sequence)
                        {
                            case Subsequence.Primary:
                                Sequence.PrimarySequence.Add(evt);
                                break;
                            case Subsequence.Abort:
                                Sequence.AbortSequence.Add(evt);
                                break;
                            default:
                                break;
                        }
                    }
                });
        }

        private static Event CreateEvent(EventEditorViewModel content)
        {
            Event evt;
            switch (content.Type)
            {
                case EventType.Output:
                    evt = new OutputEvent();
                    break;
                case EventType.Abort:
                    evt = new AbortCondition
                    {
                        ThresholdMin = content.ThresholdMin,
                        ThresholdMax = content.ThresholdMax
                    };
                    break;
                default:
                    throw new InvalidOperationException("Invalid event type");
            }
            evt.StartTime = content.StartTime;
            evt.EndTime = content.EndTime;
            evt.ChannelName = content.Channel;
            return evt;
        }

        public Sequence Sequence { get; }
        public ICommand AddEventCommand { get; }
        public InteractionRequest<Confirmation> CreateEventRequest { get; } = new InteractionRequest<Confirmation>();
    }
}
