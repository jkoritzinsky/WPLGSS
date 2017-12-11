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
            var sequence = new Sequence
            {
                PrimarySequence =
                {
                    new OutputEvent
                    {
                        StartTime = TimeSpan.FromMilliseconds(0),
                        EndTime = TimeSpan.FromMilliseconds(10),
                        ChannelName = "Output"
                    }
                }
            };
            
            var service = A.Fake<IDataAquisition>();

            var config = A.Fake<IConfigService>();
            A.CallTo(() => config.Config).Returns(new Config
            {
                Channels =
                {
                    new Channel
                    {
                        Name = "Output"
                    }
                }
            });

            using (var runSequence = new SequenceRunner(service, config))
            {
                runSequence.RunSequence(sequence);
                await Task.Delay(15);
            }
            A.CallTo(() => service.SetChannelValue(A<Channel>.Ignored, A<double>.Ignored)).MustHaveHappened(Repeated.AtLeast.Twice);
        }

        // Also need to test for abort conditions

        // Verify that the sequence runs as required
    }
}
