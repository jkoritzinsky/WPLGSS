using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public class LiveChannel : BindableBase
    {
        public LiveChannel(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; }

        private double value;

        public double Value
        {
            get { return value; }
            set { SetProperty(ref this.value, value); }
        }
    }
}
