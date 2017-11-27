using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using Xunit;

namespace WPLGSS.Services.UnitTests
{
    public class DataServiceTests
    {
        [Fact]
        public async Task StartingServiceWillTriggerSampleIn15Milliseconds()
        {
            var labJack = A.Fake<ILabJackGateway>();
            A.CallTo(() => labJack.GetAnalogData()).Returns(new double[1]);

            using (var service = new DataService(A.Fake<IConfigService>(), labJack))
            {
                service.StartService();
                await Task.Delay(15);
            }

            A.CallTo(() => labJack.GetAnalogData()).MustHaveHappened(Repeated.AtLeast.Once);
        }

        [Fact]
        public async Task SampleWillOnlyReportConfiguredChannels()
        {
            var labJack = A.Fake<ILabJackGateway>();
            A.CallTo(() => labJack.LastError).Returns(LabJack.LJM.LJMERROR.NOERROR);
            A.CallTo(() => labJack.GetAnalogData()).Returns(new double[] { 0, 1, 2, 3, 4, 5 });

            InputChannel channel;
            var config = new Config
            {
                Channels =
                {
                    (channel = new InputChannel
                    {
                        Source = ChannelSource.LabJack,
                        ChannelId = 2,
                        Scaling = "x"
                    })
                }
            };

            var configService = A.Fake<IConfigService>();

            A.CallTo(() => configService.Config).Returns(config);

            var called = false;
            using (var service = new DataService(configService, labJack))
            {
                service.ChannelValueUpdated += (o, e) =>
                {
                    Assert.Equal(channel, e.Channel);
                    Assert.Equal(2, e.Value);
                    called = true;
                };
                service.StartService();

                await Task.Delay(50);
            }

            Assert.True(called);
        }
    }
}
