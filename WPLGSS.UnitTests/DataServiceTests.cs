using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LabJack;

namespace WPLGSS.Services.UnitTests
{
    public class DataServiceTests
    {
        [Fact]
        public void OpenLabJack()
        {
            LabJackGateway LJDevice = new LabJackGateway();

            LJDevice.OpenLabJack();

            Assert.Equal(LJM.LJMERROR.NOERROR, LJDevice.ret);
        }

        [Fact]
        public void AnalogLoopback()
        {
            double[] DataOut = new double[2];
            double[] DataIn;
            LabJackGateway LJDevice = new LabJackGateway();

            LJDevice.OpenLabJack();

            DataOut[0] = 1;
            DataOut[1] = 2;
            LJDevice.SetAnalogData(DataOut);

            DataIn = LJDevice.GetAnalogData();

            Assert.Equal(DataOut[0], DataIn[0], 2);
            Assert.Equal(DataOut[1], DataIn[1], 2);
        }
    }
}
