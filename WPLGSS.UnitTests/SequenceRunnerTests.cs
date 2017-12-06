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
    public class SequenceRunnerTests
    {
        // Create sequence with matching configuration file with fake data
        // acquisition service

        // Run the sequence
        // Let the test wait

        // Verify that data acquisition service gets called
        [Fact]
        public async Task SampleWillRunService()
        {
            var sequence = A.Fake<Sequence>();
            var LabJack = A.Fake<ILabJackGateway>();
            var configService = A.Fake<IConfigService>();
            var service = new DataService(configService, LabJack);

            A.CallTo(() => configService.Config).Returns(config);
            using (var runSequence = new SequenceRunner(sequence, service))
            {
                runSequence.RunSequence();
                await Task.Delay(15);
            }  
           // How do I verify that the data acquisition service gets called?
        }
    }
}
