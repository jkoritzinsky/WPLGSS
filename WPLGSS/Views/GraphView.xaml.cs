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
    [ModuleExport(typeof(GraphViewModule))]
    public class GraphViewModule : IModule
    {
        [ImportingConstructor]
        public GraphViewModule(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }

        public IRegionManager RegionManager { get; }

        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.GraphRegion, typeof(GraphView));
        }
    }

    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
    [Export]
    public partial class GraphView : UserControl
    {
        public GraphView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public GraphView(GraphsViewModel model)
            :this()
        {
            DataContext = model;
        }
    }
}
