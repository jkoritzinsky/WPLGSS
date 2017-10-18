using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    [Export(typeof(IConfigService))]
    class ConfigService : IConfigService
    {
        private Config config = new Config
        {
            Channels = new System.Collections.ObjectModel.ObservableCollection<Channel>()
        };

        public Config Config
        {
            get => config;
            set
            {
                config = value;
                ConfigFileLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler ConfigFileLoaded;

        public void LoadConfig(string path)
        {
            throw new NotImplementedException();
        }

        public void SaveConfig(string path)
        {
            throw new NotImplementedException();
        }
    }
}
