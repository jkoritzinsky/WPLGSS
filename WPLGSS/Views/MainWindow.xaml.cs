using System.ComponentModel.Composition;
using System.Windows;
using WPLGSS.ViewModels;

namespace WPLGSS.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public MainWindow(MainWindowViewModel model)
            :this()
        {
            DataContext = model;
        }
    }
}
