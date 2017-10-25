using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPLGSS.Models;

namespace WPLGSS.Views
{
    public class LiveDataSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var liveChannel = (LiveChannel)item;
            var control = (FrameworkElement)container;
            return (DataTemplate)(liveChannel.Channel is InputChannel ?
                control.FindResource("InputChannel")
                : control.FindResource("OutputChannel"));
        }
    }
}
