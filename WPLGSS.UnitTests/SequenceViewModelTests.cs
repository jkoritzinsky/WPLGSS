using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using Xunit;
using static WPLGSS.ViewModels.SequenceViewModel;

namespace WPLGSS.ViewModels.UnitTests
{
    public class SequenceViewModelTests
    {
        [Theory]
        [InlineData(Subsequence.Primary)]
        [InlineData(Subsequence.Abort)]
        public void CanAddEventToSubsequence(Subsequence subsequence)
        {
            var evt = new OutputEvent
            {
                ChannelName = "Channel",
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.FromSeconds(1)
            };
            var viewModel = new SequenceViewModel(new Sequence());

            viewModel.CreateEventRequest.Raised += (o, e) =>
            {
                var confirmation = (IConfirmation)e.Context;
                confirmation.Confirmed = true;
                confirmation.Content = evt;
                e.Callback();
            };

            viewModel.AddEventCommand.Execute(subsequence.ToString());

            var seq = (subsequence == Subsequence.Primary ? viewModel.Sequence.PrimarySequence : viewModel.Sequence.AbortSequence);

            Assert.Single(seq);
            Assert.Same(evt, seq.First());
        }
    }
}
