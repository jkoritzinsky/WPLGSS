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
    public class SequenceManagerViewModel : PersistViewModelBase
    {
        public const string NewSequenceName = "New Sequence";
        private readonly ISequenceEditorService fileEditorService;
        private readonly ISequencePersistence sequencePersistence;

        [ImportingConstructor]
        public SequenceManagerViewModel(ISequenceEditorService fileEditorService, ISequencePersistence sequencePersistence)
        {
            NewSequenceCommand = new DelegateCommand(() =>
                fileEditorService.OpenSequenceInRegion(
                    RegionNames.SequenceEditorRegion,
                    NewSequenceName,
                    new SequenceViewModel(new Sequence())));
            SaveSequenceCommand = new DelegateCommand<string>(str => SaveSequence(str != null ? Convert.ToBoolean(str) : false), _ => CurrentSequence != null);
            this.fileEditorService = fileEditorService;
            this.sequencePersistence = sequencePersistence;
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

        private DelegateCommand<string> SaveSequenceCommand { get; }

        public override ICommand SaveCommand => SaveSequenceCommand;

        private FileInteractionNotification fileNotification = new FileInteractionNotification
        {
            Filter = "WPLGSS Sequence (*.seq.yaml)|*.seq.yaml",
            DefaultExtension = ".seq.yaml"
        };

        protected override FileInteractionNotification Notification => fileNotification;

        private void SaveSequence(bool saveAs)
        {
            if (CurrentSequence.Path != NewSequenceName && !saveAs)
            {
                sequencePersistence.SaveSequence(currentSequence.Path, CurrentSequence.Sequence.Sequence);
            }
            else
            {
                Save();
            }
        }

        protected override void SaveCore(string path)
        {
            sequencePersistence.SaveSequence(path, CurrentSequence.Sequence.Sequence);
            fileEditorService.UpdateViewNameForSequence(RegionNames.SequenceEditorRegion, path, CurrentSequence);
        }

        protected override void OpenCore(string path)
        {
            var sequence = sequencePersistence.OpenSequence(path);
            var viewModel = new SequenceViewModel(sequence);
            fileEditorService.OpenSequenceInRegion(RegionNames.SequenceEditorRegion, path, viewModel);
        }
    }
}
