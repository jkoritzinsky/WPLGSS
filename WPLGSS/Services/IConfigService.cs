using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    public interface IConfigService
    {
        Config Config { get; }

        event EventHandler ConfigFileLoaded;

        void SaveConfig(string path);

        void LoadConfig(string path); 
    }
}
