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

        public Sequence Sequence { get; }
        public ICommand AddEventCommand { get; }
        public InteractionRequest<Confirmation> CreateEventRequest { get; } = new InteractionRequest<Confirmation>();
    }
}
