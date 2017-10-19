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
    [Export(typeof(IConfigService))]
    class ConfigService : IConfigService
    {
        public ConfigService()
        {
            config = new Config
            {
                Channels = new System.Collections.ObjectModel.ObservableCollection<Channel>()
            };

            var deserializerBuilder = new DeserializerBuilder();
            deserializerBuilder
                .WithTagMapping("tag:yaml.org,2002:input", typeof(InputChannel))
                .WithNamingConvention(new CamelCaseNamingConvention());
            deserializer = deserializerBuilder.Build();

            var serializerBuilder = new SerializerBuilder();
            serializerBuilder
                .WithTagMapping("tag:yaml.org,2002:input", typeof(InputChannel))
                .WithNamingConvention(new CamelCaseNamingConvention());
            serializer = serializerBuilder.Build();
        }

        private Config config;
        private Deserializer deserializer;
        private Serializer serializer;

        public Config Config
        {
            get => config;
            set
            {
                config = value;
                ConfigFileLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler ConfigFileLoaded;

        public void LoadConfig(string path)
        {
            using (var fileStream = File.OpenRead(path))
            using (var reader = new StreamReader(fileStream))
            {
                Config = deserializer.Deserialize<Config>(reader);
            }
        }

        public void SaveConfig(string path)
        {
            using (var fileStream = File.OpenWrite(path))
            using (var writer = new StreamWriter(fileStream))
            {
                serializer.Serialize(writer, config);
            }
        }
    }
}
