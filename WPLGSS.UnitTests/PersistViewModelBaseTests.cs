using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Interactivity;
using Xunit;

namespace WPLGSS.ViewModels.UnitTests
{
    public class PersistViewModelBaseTests
    {
        class Test : PersistViewModelBase
        {
            public bool SaveCoreCalled;
            public bool OpenCoreCalled;

            protected override FileInteractionNotification Notification { get; } = new FileInteractionNotification();

            public FileInteractionNotification GetNotification() => Notification;

            protected override void OpenCore(string path)
            {
                OpenCoreCalled = true;
            }

            protected override void SaveCore(string path)
            {
                SaveCoreCalled = true;
            }
        }


        [Fact]
        public void SaveCommandRaisesInteractionRequest()
        {
            var viewModel = new Test();

            var saveRequestRaised = false;

            viewModel.SaveRequest.Raised += (o, e) => saveRequestRaised = true;

            viewModel.SaveCommand.Execute(null);

            Assert.True(saveRequestRaised);
        }

        [Fact]
        public void SaveRequestCallbackSavesConfig()
        {
            var viewModel = new Test();

            const string path = "FakePath.config";

            viewModel.SaveRequest.Raised += (o, e) =>
            {
                Assert.Equal(viewModel.GetNotification(), e.Context);
                var notification = (FileInteractionNotification)e.Context;
                notification.Confirmed = true;
                notification.Path = path;
                e.Callback();
            };

            viewModel.SaveCommand.Execute(null);

            Assert.True(viewModel.SaveCoreCalled);
        }

        [Fact]
        public void OpenCommandRaisesInteractionRequest()
        {
            var viewModel = new Test();

            var openRequestRaised = false;

            viewModel.OpenRequest.Raised += (o, e) => openRequestRaised = true;

            viewModel.OpenCommand.Execute(null);

            Assert.True(openRequestRaised);
        }

        [Fact]
        public void OpenRequestCallbackOpensConfig()
        {
            var viewModel = new Test();

            const string path = "FakePath.config";

            viewModel.OpenRequest.Raised += (o, e) =>
            {
                Assert.Equal(viewModel.GetNotification(), e.Context);
                var notification = (FileInteractionNotification)e.Context;
                notification.Confirmed = true;
                notification.Path = path;
                e.Callback();
            };

            viewModel.OpenCommand.Execute(null);

            Assert.True(viewModel.OpenCoreCalled);
        }
    }
}
