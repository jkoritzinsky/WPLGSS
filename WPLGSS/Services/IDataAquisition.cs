using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    public interface IDataAquisition
    {
        void StartService();

        LiveDataModel DataModel { get; }
        
        event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        event EventHandler<StatusChangedEventArgs> ArduinoConnectionChanged;
    }
}
