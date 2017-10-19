using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Interactivity
{
    public class FileInteractionNotification : Confirmation
    {
        public string Path { get; set; }
        public string DefaultExtension { get; set; }
        public string Filter { get; set; }
    }
}
