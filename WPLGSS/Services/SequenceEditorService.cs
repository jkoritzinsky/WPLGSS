using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.ViewModels;
using WPLGSS.Models;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

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

        public void OpenSequenceInRegion(string editorRegionName, string path, SequenceViewModel sequence)
        {
            var editorRegion = regionManager.Regions[editorRegionName];
            if (editorRegion.GetView(path) != null)
            {
                editorRegion.Activate(editorRegion.GetView(path));
                return;
            }

            var view = new SequenceFile(path, sequence);

            editorRegion.Add(view, path);
            editorRegion.Activate(view);
        }

        public void UpdateViewNameForSequence(string editorRegionName, string path, SequenceFile sequenceFile)
        {
            if (path != sequenceFile.Path)
            {
                var newView = new SequenceFile(path, sequenceFile.Sequence);
                var editorRegion = regionManager.Regions[editorRegionName];
                editorRegion.Remove(sequenceFile);
                editorRegion.Add(newView, path);
                editorRegion.Activate(newView);
            }
        }
    }
}
