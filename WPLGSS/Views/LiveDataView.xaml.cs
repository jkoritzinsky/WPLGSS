using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPLGSS.ViewModels;

namespace WPLGSS.Views
{
    [ModuleExport(typeof(LiveDataViewModule))]
    public class LiveDataViewModule : IModule
    {
        [ImportingConstructor]
        public LiveDataViewModule(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }

        public IRegionManager RegionManager { get; }

        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.LiveDataRegion, typeof(LiveDataView));
        }
    }

    /// <summary>
    /// Interaction logic for LiveDataView.xaml
    /// </summary>
    [Export]
    public partial class LiveDataView : UserControl
    {
        public LiveDataView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public LiveDataView(LiveDataViewModel model)
            : this()
        {
            DataContext = model;
        }
    }
}
