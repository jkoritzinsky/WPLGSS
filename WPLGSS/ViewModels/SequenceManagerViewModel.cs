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
        private readonly ISequencePersistence sequencePersisence;

        [ImportingConstructor]
        public SequenceManagerViewModel(IConfigService configService, ISequenceEditorService fileEditorService, ISequencePersistence sequencePersisence)
        {
            NewSequenceCommand = new DelegateCommand(() =>
                fileEditorService.OpenSequenceInRegion(
                    RegionNames.SequenceEditorRegion,
                    NewSequenceName,
                    new SequenceViewModel(configService,
                        new Sequence())));
            OpenSequenceCommand = new DelegateCommand(OpenSequence);
            SaveSequenceCommand = new DelegateCommand<string>(str => SaveSequence(str != null ? Convert.ToBoolean(str) : false), _ => CurrentSequence != null);
            this.configService = configService;
            this.fileEditorService = fileEditorService;
            this.sequencePersisence = sequencePersisence;
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

        public DelegateCommand<string> SaveSequenceCommand { get; }

        public ICommand OpenSequenceCommand { get; }

        public InteractionRequest<FileInteractionNotification> OpenRequest { get; } = new InteractionRequest<FileInteractionNotification>();

        public InteractionRequest<FileInteractionNotification> SaveRequest { get; } = new InteractionRequest<FileInteractionNotification>();

        private FileInteractionNotification fileNotification = new FileInteractionNotification
        {
            Filter = "WPLGSS Sequence (*.seq.yaml)|*.seq.yaml",
            DefaultExtension = ".seq.yaml"
        };

        private void SaveSequence(bool saveAs)
        {
            if (CurrentSequence.Path != NewSequenceName && !saveAs)
            {
                sequencePersisence.SaveSequence(currentSequence.Path, CurrentSequence.Sequence.Sequence);
            }
            else
            {
                SaveRequest.Raise(fileNotification, n =>
                {
                    if (n.Confirmed)
                    {
                        sequencePersisence.SaveSequence(currentSequence.Path, CurrentSequence.Sequence.Sequence);
                        fileEditorService.UpdateViewNameForSequence(RegionNames.SequenceEditorRegion, n.Path, CurrentSequence);
                    }
                }); 
            }
        }

        private void OpenSequence()
        {
            OpenRequest.Raise(fileNotification, n =>
            {
                if (n.Confirmed)
                {
                    var sequence = sequencePersisence.OpenSequence(n.Path);
                    var viewModel = new SequenceViewModel(configService, sequence);
                    fileEditorService.OpenSequenceInRegion(RegionNames.SequenceEditorRegion, n.Path, viewModel);
                }
            });
        }
    }
}
