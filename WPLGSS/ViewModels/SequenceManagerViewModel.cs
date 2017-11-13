using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Interactivity;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    [Export]
    public class SequenceManagerViewModel : BindableBase
    {
        private const string NewSequenceName = "New Sequence";
        private readonly IConfigService configService;
        private readonly ISequenceEditorService fileEditorService;

        [ImportingConstructor]
        public SequenceManagerViewModel(IConfigService configService, ISequenceEditorService fileEditorService)
        {
            NewSequenceCommand = new DelegateCommand(() =>
                fileEditorService.OpenSequenceInRegion(
                    RegionNames.SequenceEditorRegion,
                    NewSequenceName,
                    new SequenceViewModel(configService,
                        new Sequence())));
            OpenSequenceCommand = new DelegateCommand(OpenSequence);
            SaveSequenceCommand = new DelegateCommand(SaveSequence, () => CurrentSequence != null);
            this.configService = configService;
            this.fileEditorService = fileEditorService;
        }

        public string Name => "Sequence Manager";

        public ICommand NewSequenceCommand { get; }

        private SequenceFile currentSequence;
        public SequenceFile CurrentSequence
        {
            get
            {
                return currentSequence;
            }
            set
            {
                SetProperty(ref currentSequence, value, () =>
                {
                    SaveSequenceCommand.RaiseCanExecuteChanged();
                });
            }
        }

        public DelegateCommand SaveSequenceCommand { get; }

        public ICommand OpenSequenceCommand { get; }

        public InteractionRequest<FileInteractionNotification> OpenRequest { get; } = new InteractionRequest<FileInteractionNotification>();

        public InteractionRequest<FileInteractionNotification> SaveRequest { get; } = new InteractionRequest<FileInteractionNotification>();

        private FileInteractionNotification fileNotification = new FileInteractionNotification
        {
            Filter = "WPLGSS Sequence (*.seq.yaml)|*.seq.yaml",
            DefaultExtension = ".seq.yaml"
        };

        private void SaveSequence()
        {
            if (CurrentSequence.Path != NewSequenceName)
            {
                fileEditorService.SaveSequence(RegionNames.SequenceEditorRegion, CurrentSequence.Path, CurrentSequence);
            }
            SaveRequest.Raise(fileNotification, n =>
            {
                if (n.Confirmed)
                {
                    fileEditorService.SaveSequence(RegionNames.SequenceEditorRegion, n.Path, CurrentSequence);
                }
            });
        }

        private void OpenSequence()
        {
            OpenRequest.Raise(fileNotification, n =>
            {
                if (n.Confirmed)
                {
                    var sequence = fileEditorService.OpenSequenceFromFile(n.Path);
                    var viewModel = new SequenceViewModel(configService, sequence);
                    fileEditorService.OpenSequenceInRegion(RegionNames.SequenceEditorRegion, n.Path, viewModel);
                }
            });
        }
    }
}
