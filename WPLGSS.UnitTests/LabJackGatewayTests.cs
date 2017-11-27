using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LabJack;

namespace WPLGSS.Services.UnitTests
{
    public class LabJackGatewayTests
    {
        [Fact]
        public void OpenLabJack()
        {
            var LJDevice = new LabJackGateway();

            LJDevice.OpenLabJack();

            Assert.Equal(LJM.LJMERROR.NOERROR, LJDevice.LastError);
        }

        [Fact]
        public void AnalogLoopback()
        {
            double[] DataOut = new double[2];
            double[] DataIn;
            var LJDevice = new LabJackGateway();

            LJDevice.OpenLabJack();

            DataOut[0] = 15;
            DataOut[1] = 30;
            LJDevice.SetAnalogData(DataOut);

            DataIn = LJDevice.GetAnalogData();

            Assert.Equal(DataIn[0], DataOut[0], 3);
            Assert.Equal(DataIn[1], DataOut[1], 3);
        }
    }
}
