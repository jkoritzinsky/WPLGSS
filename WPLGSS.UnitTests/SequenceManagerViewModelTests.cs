using FakeItEasy;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Interactivity;
using WPLGSS.Services;
using Xunit;

namespace WPLGSS.ViewModels.UnitTests
{
    public class SequenceManagerViewModelTests
    {
        [Fact]
        public void SavingNonNewSequenceDoesNotRaiseInteractionRequest()
        {
            var sequencePersistence = A.Fake<ISequencePersistence>();
            var path = "Test Path";

            var viewModel = new SequenceManagerViewModel(A.Fake<ISequenceEditorService>(), sequencePersistence)
            {
                CurrentSequence = new SequenceFile(path, new SequenceViewModel(new Models.Sequence()))
            };

            var saveRequestRaised = false;
            viewModel.SaveRequest.Raised += (o, e) => saveRequestRaised = true;

            Assert.True(viewModel.SaveCommand.CanExecute(false.ToString()));
            viewModel.SaveCommand.Execute(false.ToString());

            Assert.False(saveRequestRaised);
            A.CallTo(() => sequencePersistence.SaveSequence(path, A<Models.Sequence>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void SaveAsRaisesInteractionRequest()
        {
            var sequencePersistence = A.Fake<ISequencePersistence>();
            var path = "Test Path";

            var viewModel = new SequenceManagerViewModel(A.Fake<ISequenceEditorService>(), sequencePersistence)
            {
                CurrentSequence = new SequenceFile(path, new SequenceViewModel(new Models.Sequence()))
            };

            var saveRequestRaised = false;
            viewModel.SaveRequest.Raised += (o, e) =>
            {
                saveRequestRaised = true;
                ((IConfirmation)e.Context).Confirmed = true;
                e.Callback();
            };

            viewModel.SaveCommand.Execute(true.ToString());

            Assert.True(saveRequestRaised);
            A.CallTo(() => sequencePersistence.SaveSequence(null, A<Models.Sequence>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void SaveNewSequenceRaisesInteractionRequest()
        {
            var sequencePersistence = A.Fake<ISequencePersistence>();
            var path = "Test Path";

            var viewModel = new SequenceManagerViewModel(A.Fake<ISequenceEditorService>(), sequencePersistence)
            {
                CurrentSequence = new SequenceFile(SequenceManagerViewModel.NewSequenceName, new SequenceViewModel(new Models.Sequence()))
            };

            var saveRequestRaised = false;
            viewModel.SaveRequest.Raised += (o, e) =>
            {
                saveRequestRaised = true;
                var notification = (FileInteractionNotification)e.Context;
                notification.Confirmed = true;
                notification.Path = path;
                e.Callback();
            };

            viewModel.SaveCommand.Execute(false.ToString());

            Assert.True(saveRequestRaised);
            A.CallTo(() => sequencePersistence.SaveSequence(path, A<Models.Sequence>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void SaveAsUpdatesViewName()
        {
            var sequenceEditor = A.Fake<ISequenceEditorService>();
            var path = "Test Path";
            var file = new SequenceFile(path, new SequenceViewModel(new Models.Sequence()));

            var viewModel = new SequenceManagerViewModel(sequenceEditor, A.Fake<ISequencePersistence>())
            {
                CurrentSequence = file
            };

            var saveRequestRaised = false;
            viewModel.SaveRequest.Raised += (o, e) =>
            {
                saveRequestRaised = true;
                ((IConfirmation)e.Context).Confirmed = true;
                e.Callback();
            };

            viewModel.SaveCommand.Execute(true.ToString());

            Assert.True(saveRequestRaised);
            A.CallTo(() => sequenceEditor.UpdateViewNameForSequence(A<string>.Ignored, null, file)).MustHaveHappened();
        }

        [Fact]
        public void OpenCommandOpensSequence()
        {
            var sequenceEditor = A.Fake<ISequenceEditorService>();
            var sequencePersistence = A.Fake<ISequencePersistence>();
            var path = "Test Path";
            var file = new SequenceFile(path, new SequenceViewModel(new Models.Sequence()));

            var viewModel = new SequenceManagerViewModel(sequenceEditor, sequencePersistence);

            var openRequestRaised = false;
            viewModel.OpenRequest.Raised += (o, e) =>
            {
                openRequestRaised = true;
                var notification = (FileInteractionNotification)e.Context;
                notification.Confirmed = true;
                notification.Path = path;
                e.Callback();
            };

            viewModel.OpenCommand.Execute(null);

            Assert.True(openRequestRaised);
            A.CallTo(() => sequencePersistence.OpenSequence(path)).MustHaveHappened();
            A.CallTo(() => sequenceEditor.OpenSequenceInRegion(A<string>.Ignored, path, A<SequenceViewModel>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void NewSequenceCommandOpensSequenceInEditor()
        {
            var sequenceEditor = A.Fake<ISequenceEditorService>();
            var sequencePersistence = A.Fake<ISequencePersistence>();

            var viewModel = new SequenceManagerViewModel(sequenceEditor, sequencePersistence);

            viewModel.NewSequenceCommand.Execute(null);
            
            A.CallTo(() => sequencePersistence.OpenSequence(SequenceManagerViewModel.NewSequenceName)).MustNotHaveHappened();
            A.CallTo(() => sequenceEditor.OpenSequenceInRegion(A<string>.Ignored, SequenceManagerViewModel.NewSequenceName, A<SequenceViewModel>.Ignored)).MustHaveHappened();
        }
    }
}
