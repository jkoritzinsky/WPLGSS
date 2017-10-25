using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public class LiveDataModel : BindableBase
    {
        public ObservableCollection<LiveChannel> Channels { get; } = new ObservableCollection<LiveChannel>();
    }
}
