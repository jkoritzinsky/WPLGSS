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
    public class SchematicViewModel : BindableBase
    {
        private readonly IDataAquisition dataAquisition;

        [ImportingConstructor]
        public SchematicViewModel(ISequencePersistence sequencePersistence, ISequenceEditorService fileEditorService, IDataAquisition dataAquisition)
        {

            //OpenSequenceCommand = new DelegateCommand(OpenSequence);

            this.dataAquisition = dataAquisition;
        }

        public string Name => "Schematic View";

        //public ICommand StartServiceCommand { get; }


        

        public InteractionRequest<FileInteractionNotification> OpenRequest { get; } = new InteractionRequest<FileInteractionNotification>();
        

    }
}
