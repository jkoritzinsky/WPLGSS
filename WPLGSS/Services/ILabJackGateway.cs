using LabJack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Services
{
    public interface ILabJackGateway
    {
        void OpenLabJack();

        double[] GetAnalogData();

        void SetAnalogData(double[] Data);

        LJM.LJMERROR LastError { get; }
    }
}
