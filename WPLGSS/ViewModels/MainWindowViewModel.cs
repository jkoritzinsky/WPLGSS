using Prism.Mvvm;
using System.ComponentModel.Composition;

namespace WPLGSS.ViewModels
{
    [Export]
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "WPL Ground Support System";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
