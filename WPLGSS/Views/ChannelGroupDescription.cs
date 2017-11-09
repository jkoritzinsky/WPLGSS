using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Views
{
    public class ChannelGroupDescription : GroupDescription
    {
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            switch (item)
            {
                case InputChannel _:
                    return "Input Channels ";
                case Channel _:
                    return "Output Channels ";
                default:
                    return "";
            }
        }

        public override bool NamesMatch(object groupName, object itemName)
        {
            return (string)groupName == (string)itemName;
        }
    }
}
