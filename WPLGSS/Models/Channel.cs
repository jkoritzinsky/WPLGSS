using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public enum ChannelSource
    {
        LabJack,
        Arduino
    }

    public class Channel : BindableBase
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
    }
}
