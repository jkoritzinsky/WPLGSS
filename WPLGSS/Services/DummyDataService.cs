using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    [Export(typeof(IDataAquisition))] public class DummyDataService : IDataAquisition
    {
        public DummyDataService()
        {
            DataModel = new LiveDataModel
            {
                Channels =
                {
                    new LiveChannel(new InputChannel
                    {
                        Name = "Input Channel Test",
                        Unit = "atm"
                    })
                    {
                        Value = 2.7
                    },
                    new LiveChannel(new Channel
                    {
                        Name= "Output Channel Test"
                    })
                }
            };
        }

        public LiveDataModel DataModel { get; }

        public event EventHandler<StatusChangedEventArgs> LabJackConnectionChanged;
        public event EventHandler<StatusChangedEventArgs> ArduinoConnectionChanged;


        public void StartService()
        {
            throw new NotImplementedException();
        }
    }
}
