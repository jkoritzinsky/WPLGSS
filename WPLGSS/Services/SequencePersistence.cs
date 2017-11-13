using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WPLGSS.Services
{
    [Export(typeof(ISequencePersistence))]
    public class SequencePersistence : ISequencePersistence
    {
        private Deserializer deserializer;
        private Serializer serializer;

        public SequencePersistence()
        {


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

        public Sequence OpenSequence(string path)
        {
            using (var fileStream = File.OpenRead(path))
            using (var reader = new StreamReader(fileStream))
            {
                return deserializer.Deserialize<Sequence>(reader);
            }
        }

        public void SaveSequence(string path, Sequence sequence)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var fileStream = File.OpenWrite(path))
            using (var writer = new StreamWriter(fileStream))
            {
                serializer.Serialize(writer, sequence);
            }
        }
    }
}
