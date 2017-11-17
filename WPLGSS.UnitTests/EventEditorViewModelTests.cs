using FakeItEasy;
using Prism.Interactivity.InteractionRequest;
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

        [Fact]
        public void StartTimeMustBeNonNegative()
        {
            var viewModel = new EventEditorViewModel(fakeConfigService)
            {
                StartTime = TimeSpan.FromSeconds(-1)
            };

            Assert.Single(viewModel.GetErrors(nameof(viewModel.StartTime)));
        }

        [Fact]
        public void ExecutingFinishCommandConfirmsNotification()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    }
                }
            });

            var notification = A.Fake<IConfirmation>();

            var finished = false;

            var viewModel = new EventEditorViewModel(configService)
            {
                Type = EventType.Abort,
                Channel = "Input",
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.FromSeconds(1),
                ThresholdMin = 10,
                ThresholdMax = 20,
                Notification = notification,
                FinishInteraction = () => finished = true
            };

            viewModel.FinishCommand.Execute();

            Assert.True(finished);
            A.CallToSet(() => notification.Content).MustHaveHappened();
            Assert.IsType<AbortCondition>(notification.Content);

            var abortCondition = (AbortCondition)notification.Content;

            Assert.Equal("Input", abortCondition.ChannelName);
            Assert.Equal(TimeSpan.Zero, abortCondition.StartTime);
            Assert.Equal(TimeSpan.FromSeconds(1), abortCondition.EndTime);
            Assert.Equal(10, abortCondition.ThresholdMin);
            Assert.Equal(20, abortCondition.ThresholdMax);
            
        }

        [Fact]
        public void ExecutingCancelCommandDoesConfirmNotification()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config
            {
                Channels =
                {
                    new InputChannel
                    {
                        Name = "Input"
                    }
                }
            });

            var notification = A.Fake<IConfirmation>();

            var finished = false;

            var viewModel = new EventEditorViewModel(configService)
            {
                Type = EventType.Abort,
                Channel = "Input",
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.FromSeconds(1),
                ThresholdMin = 10,
                ThresholdMax = 20,
                Notification = notification,
                FinishInteraction = () => finished = true
            };

            viewModel.CancelCommand.Execute(null);

            Assert.True(finished);
            A.CallToSet(() => notification.Content).MustNotHaveHappened();
            Assert.False(notification.Confirmed);
        }

        [Fact]
        public void CannotDeleteWhenCreatingNewEvent()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var viewModel = new EventEditorViewModel(configService)
            {
                Notification = new Notification()
            };

            Assert.False(viewModel.CanDeleteEvent);

            Assert.False(viewModel.DeleteCommand.CanExecute());
        }

        [Fact]
        public void SettingNotificationWithEventLoadsEditorWithEventState_Output()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var outputEvent = new OutputEvent
            {
                ChannelName = "Channel",
                StartTime = TimeSpan.FromSeconds(1),
                EndTime = TimeSpan.FromSeconds(2)
            };

            var viewModel = new EventEditorViewModel(configService)
            {
                Notification = new Confirmation
                {
                    Content = outputEvent
                }
            };

            Assert.Equal(outputEvent.ChannelName, viewModel.Channel);
            Assert.Equal(outputEvent.StartTime, viewModel.StartTime);
            Assert.Equal(outputEvent.EndTime, viewModel.EndTime);
            Assert.Equal(EventType.Output, viewModel.Type);
        }

        [Fact]
        public void SettingNotificationWithEventLoadsEditorWithEventState_Abort()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());

            var abortCondition = new AbortCondition
            {
                ChannelName = "Channel",
                StartTime = TimeSpan.FromSeconds(1),
                EndTime = TimeSpan.FromSeconds(2),
                ThresholdMin = 100,
                ThresholdMax = 200
            };

            var viewModel = new EventEditorViewModel(configService)
            {
                Notification = new Confirmation
                {
                    Content = abortCondition
                }
            };

            Assert.Equal(abortCondition.ChannelName, viewModel.Channel);
            Assert.Equal(abortCondition.StartTime, viewModel.StartTime);
            Assert.Equal(abortCondition.EndTime, viewModel.EndTime);
            Assert.Equal(abortCondition.ThresholdMin, viewModel.ThresholdMin);
            Assert.Equal(abortCondition.ThresholdMax, viewModel.ThresholdMax);
            Assert.Equal(EventType.Abort, viewModel.Type);
        }

        [Fact]
        public void SettingNotificationWithEventCanDeleteEvent()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());


            var outputEvent = new OutputEvent
            {
                ChannelName = "Channel",
                StartTime = TimeSpan.FromSeconds(1),
                EndTime = TimeSpan.FromSeconds(2)
            };

            var viewModel = new EventEditorViewModel(configService)
            {
                Notification = new Confirmation
                {
                    Content = outputEvent
                }
            };

            Assert.True(viewModel.CanDeleteEvent);

            Assert.True(viewModel.DeleteCommand.CanExecute());
        }

        [Fact]
        public void DeleteCommandConfirmsAndSetsNotificationContentToNull()
        {
            var configService = A.Fake<IConfigService>();
            A.CallTo(() => configService.Config).Returns(new Config());


            var outputEvent = new OutputEvent
            {
                ChannelName = "Channel",
                StartTime = TimeSpan.FromSeconds(1),
                EndTime = TimeSpan.FromSeconds(2)
            };

            var notification = new Confirmation
            {
                Content = outputEvent
            };

            var viewModel = new EventEditorViewModel(configService)
            {
                Notification = notification
            };

            viewModel.DeleteCommand.Execute();

            Assert.True(notification.Confirmed);
            Assert.Null(notification.Content);
        }
    }
}
