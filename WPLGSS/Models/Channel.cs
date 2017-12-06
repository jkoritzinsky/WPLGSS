using Prism.Mvvm;
using System;
using System.Collections;
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

    public class Channel : BindableBase, INotifyDataErrorInfo
    {
        public Channel()
        {
            Validate();
        }

        private int channelId;

        public int ChannelId
        {
            get { return channelId; }
            set
            {
                SetProperty(ref channelId, value, Validate);
            }
        }

        private ChannelSource source;

        public ChannelSource Source
        {
            get { return source; }
            set
            {
                SetProperty(ref source, value, Validate);
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value, Validate);
            }
        }
        
        private string description;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public string Description
        {
            get { return description; }
            set
            {
                SetProperty(ref description, value);
            }
        }
        [YamlIgnore]
        public bool HasErrors => errors.Any();

        [YamlIgnore]
        private readonly Dictionary<string, string> errors = new Dictionary<string, string>();

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                errors[nameof(Name)] = "Channel name cannot be empty";
            }
            else
            {
                errors.Remove(nameof(Name));
            }
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));

            if (source == ChannelSource.LabJack)
            {
                var validChannel = true;
                if (this is InputChannel)
                {
                    validChannel = ChannelId >= 0 && ChannelId <= 13;
                }
                else if (!(ChannelId >= 0 && ChannelId <= 24))
                {
                    validChannel = false;
                }
                if (!validChannel)
                {
                    errors[nameof(ChannelId)] = "Channel Id out of range";
                }
                else
                {
                    errors.Remove(nameof(ChannelId));
                }
            }
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(ChannelId)));

        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (errors.TryGetValue(propertyName, out var error))
            {
                yield return error;
            }
        }
    }
}
