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

        // Verify that data acquisition service gets called at all
        [Fact]
        public async Task SampleWillRunService()
        {
            var sequence = A.Fake<Sequence>();
            
            var service = A.Fake<IDataAquisition>();

            var config = A.Fake<Config>();
            var called = false;
            using (var runSequence = new SequenceRunner(service, config))
            {
                runSequence.RunSequence(sequence);
                service.ChannelValueUpdated += (s, e) =>
                {
                    called = true;
                };
                await Task.Delay(15);
            }
            Assert.True(called);
        }

        // Also need to test for abort conditions

        // Verify that the sequence runs as required
    }
}
