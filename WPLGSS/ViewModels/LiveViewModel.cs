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
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    [Export]
    public class LiveViewModel : BindableBase
    {
        private readonly IDataAquisition dataAquisition;
        private readonly ISequenceEditorService fileEditorService;
        private readonly IConfigService configService;
        private readonly ISequencePersistence sequencePersistence;

        [ImportingConstructor]
        public LiveViewModel(IConfigService configService, ISequencePersistence sequencePersistence, ISequenceEditorService fileEditorService, IDataAquisition dataAquisition)
        {
            this.sequencePersistence = sequencePersistence;
            this.configService = configService;
            this.fileEditorService = fileEditorService;

            OpenSequenceCommand = new DelegateCommand(OpenSequence);
            StartServiceCommand = new DelegateCommand(StartService);
            this.dataAquisition = dataAquisition;
        }

        public string Name => "Live View";

        public ICommand StartServiceCommand { get; }

        public ICommand RunSequenceCommand { get; }

        public ICommand OpenSequenceCommand { get; }

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
        
        public InteractionRequest<FileInteractionNotification> OpenRequest { get; } = new InteractionRequest<FileInteractionNotification>();
        
        private FileInteractionNotification fileNotification = new FileInteractionNotification
        {
            Filter = "WPLGSS Sequence (*.seq.yaml)|*.seq.yaml",
            DefaultExtension = ".seq.yaml"
        };

    }
}
