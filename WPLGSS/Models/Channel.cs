using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace WPLGSS.Models
{
    public enum ChannelSource
    {
        LabJack
    }

    public class Channel : BindableBase, IDataErrorInfo
    {
        private int channelId;

        public int ChannelId
        {
            get { return channelId; }
            set
            {
                SetProperty(ref channelId, value);
            }
        }

        private ChannelSource source;

        public ChannelSource Source
        {
            get { return source; }
            set
            {
                SetProperty(ref source, value);
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value);
            }
        }
        
        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                SetProperty(ref description, value);
            }
        }

        [YamlIgnore]
        public string Error => null;

        [YamlIgnore]
        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Name))
                {
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        return "Channel name cannot be empty";
                    }
                }
                return null;
            }
        }
    }
}
