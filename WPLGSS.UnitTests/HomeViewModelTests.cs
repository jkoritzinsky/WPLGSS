using FakeItEasy;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WPLGSS.ViewModels.UnitTests
{
    public class HomeViewModelTests
    {
        [Fact]
        public void NavigateCommandRequestsNavigation()
        {
            var regionManager = A.Fake<IRegionManager>();

            var viewModel = new HomeViewModel(regionManager);

            viewModel.NavigateToViewCommand.Execute("View");

            A.CallTo(() => regionManager.RequestNavigate(A<string>.Ignored, "View")).MustHaveHappened();
        }
    }
}
