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
    [ModuleExport(typeof(SequenceManagerModule))]
    public class SequenceManagerModule : IModule
    {
        [ImportingConstructor]
        public SequenceManagerModule(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }

        public IRegionManager RegionManager { get; }

        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SequenceManagerView));
        }
    }
    /// <summary>
    /// Interaction logic for SequenceManagerView.xaml
    /// </summary>
    [Export]
    [ViewSortHint("2")]
    public partial class SequenceManagerView : UserControl
    {
        public SequenceManagerView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public SequenceManagerView(SequenceManagerViewModel model)
            :this()
        {
            DataContext = model;
        }
    }
}
