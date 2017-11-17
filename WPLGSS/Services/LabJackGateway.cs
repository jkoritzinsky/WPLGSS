using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LabJack;

namespace WPLGSS.Services
{
    public class LabJackGateway
    {
        int LJhandle = 0;
        String[] InputNames;
        String[] OutputNames;

        public int OpenLabJack()
        {
            LJM.OpenS("ANY", "ANY", "ANY", ref LJhandle);

            InputNames[0] = "AIN0";
            InputNames[0] = "AIN1";
            InputNames[0] = "AIN2";
            InputNames[0] = "AIN3";
            InputNames[0] = "AIN4";
            InputNames[0] = "AIN5";
            InputNames[0] = "AIN6";
            InputNames[0] = "AIN7";
            InputNames[0] = "AIN8";
            InputNames[0] = "AIN9";
            InputNames[0] = "AIN10";
            InputNames[0] = "AIN11";
            InputNames[0] = "AIN12";
            InputNames[0] = "AIN13";

            OutputNames[0] = "DAC0";
            OutputNames[1] = "DAC1";

            return 0;
        }

        public double[] GetAnalogData()
        {
            double[] Data = new double[14];
            int ErrorAddress = 0;

            if (LJhandle == 0) return Data;

            LJM.eReadNames(LJhandle, 1, InputNames, Data, ref ErrorAddress);

            return Data;
        }

        public void SetAnalogData(double[] Data)
        {
            int ErrorAddress = 0;

            LJM.eWriteNames(LJhandle, 1, OutputNames, Data, ref ErrorAddress);
        }
    }
}
