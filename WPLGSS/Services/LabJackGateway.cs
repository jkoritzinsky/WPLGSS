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
        bool opened;
        int LJhandle = 0;
        String[] InputNames = new String[14];
        String[] OutputNames = new String[25];

        public LJM.LJMERROR LastError { get; private set; }

        public void OpenLabJack()
        {
            if (opened)
            {
                return;
            }

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

            OutputNames[0] = "DIO0";
            OutputNames[1] = "DIO1";
            OutputNames[2] = "DIO2";
            OutputNames[3] = "DIO3";
            OutputNames[4] = "DIO4";
            OutputNames[5] = "DIO5";
            OutputNames[6] = "DIO6";
            OutputNames[7] = "DIO7";
            OutputNames[8] = "DIO8";
            OutputNames[9] = "DIO9";
            OutputNames[10] = "DIO10";
            OutputNames[11] = "DIO11";
            OutputNames[12] = "DIO12";
            OutputNames[13] = "DIO13";
            OutputNames[14] = "DIO14";
            OutputNames[15] = "DIO15";
            OutputNames[16] = "DIO16";
            OutputNames[17] = "DIO17";
            OutputNames[18] = "DIO18";
            OutputNames[19] = "DIO19";
            OutputNames[20] = "DIO20";
            OutputNames[21] = "DIO21";
            OutputNames[22] = "DIO22";
            OutputNames[23] = "DAC0";
            OutputNames[24] = "DAC1";

            opened = true;
        }

        public double[] GetAnalogData()
        {
            double[] Data = new double[14];
            int ErrorAddress = 0;

            if (LJhandle == 0) return Data;

            LastError = LJM.eReadNames(LJhandle, 14, InputNames, Data, ref ErrorAddress);

            return Data;
        }

        public void SetAnalogData(double[] Data)
        {
            int ErrorAddress = 0;
            int numFrames;

            if (Data.Length >= 25)
            {
                numFrames = 24;
            } else
            {
                numFrames = Data.Length;
            }

            LastError = LJM.eWriteNames(LJhandle, numFrames, OutputNames, Data, ref ErrorAddress);
        }
    }
}
