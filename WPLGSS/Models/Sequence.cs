using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace WPLGSS.Models
{
    public class Sequence : BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public ObservableCollection<Event> PrimarySequence { get; set; } = new ObservableCollection<Event>();

        public ObservableCollection<Event> AbortSequence { get; set; } = new ObservableCollection<Event>();
    }
}
