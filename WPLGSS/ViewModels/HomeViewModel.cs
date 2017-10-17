using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPLGSS.ViewModels
{
    [Export]
    public class HomeViewModel : BindableBase
    {
        [ImportingConstructor]
        public HomeViewModel(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            NavigateToViewCommand = new DelegateCommand<string>(NavigateToView);
        }

        public string Name => "Home";

        public IRegionManager RegionManager { get; }

        public ICommand NavigateToViewCommand { get; }

        private void NavigateToView(string viewName)
        {
            RegionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
        }
    }
}
