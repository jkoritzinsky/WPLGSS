using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
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
    public class LiveViewModel : BindableBase
    {
        private readonly IDataAquisition dataAquisition;
        private readonly ISequenceEditorService fileEditorService;
        private readonly ISequencePersistence sequencePersistence;

        [ImportingConstructor]
        public LiveViewModel(ISequenceRunner runner, ISequencePersistence sequencePersistence, ISequenceEditorService fileEditorService, IDataAquisition dataAquisition)
        {
            this.sequencePersistence = sequencePersistence;
            this.fileEditorService = fileEditorService;

            OpenSequenceCommand = new DelegateCommand(OpenSequence);
            StartServiceCommand = new DelegateCommand(StartService);
            StartStopRecCommand = new DelegateCommand(StartStopRecord);
            RunSequenceCommand = new DelegateCommand<Sequence>(runner.RunSequence);
            this.dataAquisition = dataAquisition;
        }

        public string Name => "Live View";

        public ICommand StartServiceCommand { get; }

        public ICommand RunSequenceCommand { get; }

        public ICommand OpenSequenceCommand { get; }
        public ICommand StartStopRecCommand { get; }

        private bool recording;

        public bool Recording
        {
            get { return recording; }
            set { SetProperty(ref recording, value); }
        }


        private SequenceFile currentSequence;
        public SequenceFile CurrentSequence
        {
            get
            {
                return currentSequence;
            }
            set
            {
                SetProperty(ref currentSequence, value);
            }
        }

        private bool started;
        private void StartService()
        {
            if (!started)
            {
                dataAquisition.StartService();
                started = true;
            }
        }
        
        private void OpenSequence()
        {
            OpenRequest.Raise(fileNotification, n =>
            {
                if (n.Confirmed)
                {
                    var sequence = sequencePersistence.OpenSequence(n.Path);
                    var viewModel = new SequenceViewModel(sequence);
                    fileEditorService.OpenSequenceInRegion(RegionNames.SequenceRunnerRegion, n.Path, viewModel);
                }
            });
        }

        public InteractionRequest<FileInteractionNotification> RecordingFileRequest { get; } = new InteractionRequest<FileInteractionNotification>();
        
        private void StartStopRecord()
        {
            if (Recording)
            {
                RecordingFileRequest.Raise(new FileInteractionNotification
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
                }, file =>
                {
                    if (file.Confirmed)
                    {
                        dataAquisition.StartStopRecord(file.Path);
                    }
                });
            }
            else
            {
                dataAquisition.StartStopRecord(null); 
            }
        }

        public InteractionRequest<FileInteractionNotification> OpenRequest { get; } = new InteractionRequest<FileInteractionNotification>();
        
        private FileInteractionNotification fileNotification = new FileInteractionNotification
        {
            Filter = "WPLGSS Sequence (*.seq.yaml)|*.seq.yaml",
            DefaultExtension = ".seq.yaml"
        };

    }
}
