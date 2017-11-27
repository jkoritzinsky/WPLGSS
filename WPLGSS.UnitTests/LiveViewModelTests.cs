using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Interactivity;
using WPLGSS.Models;
using WPLGSS.Services;
using Xunit;

namespace WPLGSS.ViewModels.UnitTests
{
    public class LiveViewModelTests
    {
        [Fact]
        public void OpenCommandOpensSequence()
        {
            var sequenceEditor = A.Fake<ISequenceEditorService>();
            var sequencePersistence = A.Fake<ISequencePersistence>();
            var path = "Test Path";
            var file = new SequenceFile(path, new SequenceViewModel(new Models.Sequence()));

            var viewModel = new LiveViewModel(A.Fake<ISequenceRunner>(), sequencePersistence, sequenceEditor, A.Fake<IDataAquisition>());

            var openRequestRaised = false;
            viewModel.OpenRequest.Raised += (o, e) =>
            {
                openRequestRaised = true;
                var notification = (FileInteractionNotification)e.Context;
                notification.Confirmed = true;
                notification.Path = path;
                e.Callback();
            };

            viewModel.OpenSequenceCommand.Execute(null);

            Assert.True(openRequestRaised);
            A.CallTo(() => sequencePersistence.OpenSequence(path)).MustHaveHappened();
            A.CallTo(() => sequenceEditor.OpenSequenceInRegion(A<string>.Ignored, path, A<SequenceViewModel>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void StartServiceCommandStartsDataAquisition()
        {
            var dataAquisition = A.Fake<IDataAquisition>();

            var viewModel = new LiveViewModel(A.Fake<ISequenceRunner>(), A.Fake<ISequencePersistence>(), A.Fake<ISequenceEditorService>(), dataAquisition);

            viewModel.StartServiceCommand.Execute(null);

            A.CallTo(() => dataAquisition.StartService()).MustHaveHappened();
        }

        [Fact]
        public void RunSequenceCommandRunsSequence()
        {
            var runner = A.Fake<ISequenceRunner>();
            var sequence = new Sequence();

            var viewModel = new LiveViewModel(runner, A.Fake<ISequencePersistence>(), A.Fake<ISequenceEditorService>(), A.Fake<IDataAquisition>());

            viewModel.RunSequenceCommand.Execute(sequence);

            A.CallTo(() => runner.RunSequence(sequence)).MustHaveHappened();
        }
    }
}
