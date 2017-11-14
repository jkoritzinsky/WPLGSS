using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using WPLGSS.Services;
using Xunit;

namespace WPLGSS.ViewModels.UnitTests
{
    public class EventEditorViewModelTests
    {
        private IConfigService fakeConfigService;

        public EventEditorViewModelTests()
        {
            fakeConfigService = A.Fake<IConfigService>();
            A.CallTo(() => fakeConfigService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    },
                    new Channel
                    {
                        Name = "Output"
                    }
                }
            });
        }

        [Fact]
        public void ThresholdMinNotLessThanThresholdMaxGivesNoErrorForOutputEvent()
        {
            var viewModel = new EventEditorViewModel(fakeConfigService)
            {
                Type = EventType.Output,
                ThresholdMin = 2,
                ThresholdMax = 1,
            };
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.ThresholdMin)));
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.ThresholdMax)));
        }

        [Fact]
        public void ThresholdMinNotLessThanThresholdMaxGivesErrorForAbortEvent()
        {
            var viewModel = new EventEditorViewModel(fakeConfigService)
            {
                Type = EventType.Abort,
                ThresholdMin = 2,
                ThresholdMax = 1,
            };
            
            Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.ThresholdMin)));
            Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.ThresholdMax)));
        }

        [Fact]
        public void FixingThresholdErrorMarksNoErrors()
        {
            var viewModel = new EventEditorViewModel(fakeConfigService)
            {
                Type = EventType.Abort,
                ThresholdMin = 2,
                ThresholdMax = 1,
            };
            
            Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.ThresholdMin)));
            Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.ThresholdMax)));

            viewModel.ThresholdMin = 0;
            
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.ThresholdMin)));
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.ThresholdMax)));
        }

        [Fact]
        public void ChannelOptionsFilledFromConfigChannels()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    },
                    new Channel
                    {
                        Name = "Output"
                    }
                }
            });

            var viewModel = new EventEditorViewModel(configService);

            Assert.Equal(configService.Config.Channels, viewModel.ChannelOptions, EqualityComparer<Channel>.Default);
        }

        [Fact]
        public void ChannelNotInChannelOptionsCausesValidationError()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    },
                    new Channel
                    {
                        Name = "Output"
                    }
                }
            });
            var viewModel = new EventEditorViewModel(configService)
            {
                Channel = "Does not exist"
            };

            Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.Channel)));
        }

        [Fact]
        public void ChannelInChannelOptionsCausesNoValidationError()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    },
                    new Channel
                    {
                        Name = "Output"
                    }
                }
            });
            var viewModel = new EventEditorViewModel(configService)
            {
                Channel = "Output"
            };

            Assert.Empty(viewModel.GetErrors(nameof(viewModel.Channel)));
        }

        [Fact]
        public void OutputEventRequiresOutputChannel()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    },
                    new Channel
                    {
                        Name = "Output"
                    }
                }
            });
            var viewModel = new EventEditorViewModel(configService)
            {
                Type = EventType.Output,
                Channel = "Input"
            };

            Assert.Single(viewModel.GetErrors(nameof(viewModel.Channel)));

            viewModel.Channel = "Output";

            Assert.Empty(viewModel.GetErrors(nameof(viewModel.Channel)));
        }

        [Fact]
        public void AbortEventRequiresInputChannel()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    },
                    new Channel
                    {
                        Name = "Output"
                    }
                }
            });
            var viewModel = new EventEditorViewModel(configService)
            {
                Type = EventType.Abort,
                Channel = "Output"
            };

            Assert.Single(viewModel.GetErrors(nameof(viewModel.Channel)));

            viewModel.Channel = "Input";

            Assert.Empty(viewModel.GetErrors(nameof(viewModel.Channel)));
        }

        [Fact]
        public void CannotSaveInvalidEvent()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    },
                    new Channel
                    {
                        Name = "Output"
                    }
                }
            });
            var viewModel = new EventEditorViewModel(configService)
            {
                Type = EventType.Abort,
                Channel = "Input",
                StartTime = TimeSpan.FromSeconds(2),
                EndTime = TimeSpan.Zero,
                ThresholdMin = 10,
                ThresholdMax = 20
            };

            Assert.False(viewModel.FinishCommand.CanExecute());
        }
    }
}
