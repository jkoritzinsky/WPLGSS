using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.ViewModels
{
    [Export]
    public class ConfigViewModel : BindableBase
    {
        public string Name => "Configuration";
    }
}
