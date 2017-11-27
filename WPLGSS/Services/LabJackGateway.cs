using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LabJack;

namespace WPLGSS.Services
{
    [Export(typeof(ILabJackGateway))]
    public class LabJackGateway : ILabJackGateway
    {
        int LJhandle = 0;
        String[] InputNames = new String[14];
        String[] OutputNames = new String[2];

        public LJM.LJMERROR LastError { get; private set; }

        public void OpenLabJack()
        {

            LastError = LJM.OpenS("ANY", "ANY", "ANY", ref LJhandle);

            InputNames[0] = "AIN0";
            InputNames[1] = "AIN1";
            InputNames[2] = "AIN2";
            InputNames[3] = "AIN3";
            InputNames[4] = "AIN4";
            InputNames[5] = "AIN5";
            InputNames[6] = "AIN6";
            InputNames[7] = "AIN7";
            InputNames[8] = "AIN8";
            InputNames[9] = "AIN9";
            InputNames[10] = "AIN10";
            InputNames[11] = "AIN11";
            InputNames[12] = "AIN12";
            InputNames[13] = "AIN13";

            OutputNames[0] = "DAC0";
            OutputNames[1] = "DAC1";
        }

        public double[] GetAnalogData()
        {
            double[] Data = new double[14];
            int ErrorAddress = 0;

            if (LJhandle == 0) return Data;

            LastError = LJM.eReadNames(LJhandle, 1, InputNames, Data, ref ErrorAddress);

            return Data;
        }

        public void SetAnalogData(double[] Data)
        {
            int ErrorAddress = 0;

            LastError = LJM.eWriteNames(LJhandle, 1, OutputNames, Data, ref ErrorAddress);
        }
    }
}
