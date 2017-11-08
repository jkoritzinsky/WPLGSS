using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    [Export]
    public class SequenceManagerViewModel
    {
        private readonly ISequenceEditorService fileEditorService;

        [ImportingConstructor]
        public SequenceManagerViewModel(IConfigService configService, ISequenceEditorService fileEditorService)
        {
            NewSequenceCommand = new DelegateCommand(() =>
                fileEditorService.OpenSequence(
                    RegionNames.SequenceEditorRegion,
                    "*",
                    new SequenceViewModel(configService,
                        new Sequence()
                        {
                            PrimarySequence =
                            {
                                new OutputEvent
                                {
                                    ChannelName = "Test",
                                    StartTime = TimeSpan.Zero,
                                    EndTime = TimeSpan.FromSeconds(1)
                                }
                            }
                        })));
            this.fileEditorService = fileEditorService;
        }

        public string Name => "Sequence Manager";

        public ICommand NewSequenceCommand { get; }

        public ObservableCollection<Sequence> Sequences { get; }
    }
}
