using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.ViewModels;

namespace WPLGSS.Services
{
    [Export(typeof(ISequenceEditorService))]
    public class SequenceEditorService : ISequenceEditorService
    {
        private readonly IRegionManager regionManager;

        [ImportingConstructor]
        public SequenceEditorService(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OpenSequence(string editorRegionName, string path, SequenceViewModel sequence)
        {
            var editorRegion = regionManager.Regions[editorRegionName];
            if (editorRegion.GetView(path) != null)
            {
                editorRegion.Activate(editorRegion.GetView(path));
                return;
            }

            var view = new { Path = path, Sequence = sequence };

            editorRegion.Add(view, path);
            editorRegion.Activate(view);
        }
    }
}
