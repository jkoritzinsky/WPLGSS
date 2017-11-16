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

        public int OpenLabJack()
        {
            LJM.OpenS("ANY", "ANY", "ANY", ref LJhandle);

            return 0;
        }

        public byte[] GetAnalogData(int Channel)
        {
            byte[] Data = new byte[14];
            int NumBytes = 0;

            if (LJhandle == 0) return Data;

            LJM.ReadRaw(LJhandle, Data, NumBytes);

            return Data;
        }

        public void SetAnalogData(byte[] Data, int NumBytes)
        {
            LJM.WriteRaw(LJhandle, Data, NumBytes);
        }
    }
}
