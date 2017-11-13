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
        private Deserializer deserializer;
        private Serializer serializer;

        [ImportingConstructor]
        public SequenceEditorService(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            var deserializerBuilder = new DeserializerBuilder();
            deserializerBuilder
                .WithTagMapping("tag:yaml.org,2002:abort", typeof(AbortCondition))
                .WithTagMapping("tag:yaml.org,2002:output", typeof(OutputEvent))
                .WithNamingConvention(new CamelCaseNamingConvention());
            deserializer = deserializerBuilder.Build();

            var serializerBuilder = new SerializerBuilder();
            serializerBuilder
                .WithTagMapping("tag:yaml.org,2002:abort", typeof(AbortCondition))
                .WithTagMapping("tag:yaml.org,2002:output", typeof(OutputEvent))
                .WithNamingConvention(new CamelCaseNamingConvention());
            serializer = serializerBuilder.Build();
        }

        public Sequence OpenSequenceFromFile(string path)
        {
            using (var fileStream = File.OpenRead(path))
            using (var reader = new StreamReader(fileStream))
            {
                return deserializer.Deserialize<Sequence>(reader);
            }
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

        public void SaveSequence(string editorRegionName, string path, SequenceFile sequenceFile)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var fileStream = File.OpenWrite(path))
            using (var writer = new StreamWriter(fileStream))
            {
                serializer.Serialize(writer, sequenceFile.Sequence.Sequence);
            }

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
