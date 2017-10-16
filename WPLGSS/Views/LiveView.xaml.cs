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
    [ModuleExport(typeof(LiveViewModule))]
    public class LiveViewModule : IModule
    {
        [ImportingConstructor]
        public LiveViewModule(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }

        public IRegionManager RegionManager { get; }

        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(LiveView));
        }
    }

    /// <summary>
    /// Interaction logic for LiveView.xaml
    /// </summary>
    [Export]
    [ViewSortHint("3")]
    public partial class LiveView : UserControl
    {
        public LiveView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public LiveView(LiveViewModel model)
            :this()
        {
            DataContext = model;
        }
    }
}
