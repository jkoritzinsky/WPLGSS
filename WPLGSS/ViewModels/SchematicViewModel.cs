using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Interactivity;
using WPLGSS.Services;
using WPLGSS.Models;
using System.Windows.Controls;

namespace WPLGSS.ViewModels
{
    [Export]
    public class SchematicViewModel : BindableBase
    {
        private readonly IDataAquisition dataAquisition;
        private readonly IConfigService configService;

        [ImportingConstructor]
        public SchematicViewModel(IDataAquisition dataAquisition, IConfigService configService)
        {
            this.dataAquisition = dataAquisition;
            this.configService = configService;
            dataAquisition.ChannelValueUpdated += (o, e) => PushValueToIndicator(e.Channel, e.Value, e.Time);
        }

        private void PushValueToIndicator(Channel channel, double value, DateTime time)
        {
            foreach (ChannelIndicator chi in chIndicators)
            {
                if (chi.ch == channel)
                {
                    chi.block.Text = value;
                    break;
                }
            }
        }
        public string Name => "Schematic View";

        struct ChannelIndicator
        {
            public Channel ch;
            public TextBlock block;
        }
        List<ChannelIndicator> chIndicators = new List<ChannelIndicator>();

        public void SetBindings(UIElementCollection elements)
        {
            string devName;
            foreach (System.Windows.Controls.UIElement ele in elements)
            {
                if (ele is TextBlock)
                {
                    // Indicators

                    // Element names are the same as the facility number, but with
                    // a leading 't' and '-' replaced with '_' to meet XAML naming reqs
                    devName = (ele as TextBlock).Name.Substring(1).Replace('_', '-');

                    foreach (Channel ch in configService.Config.Channels)
                    {
                        if (ch.Name.Equals(devName))
                        {
                            ChannelIndicator chi = new ChannelIndicator();
                            chi.ch = ch;
                            chi.block = (ele as TextBlock);
                            chIndicators.Add(chi);
                            break;
                        }
                    }
                }
            }
        }

        public void ToggleOutput(object sender, RoutedEventArgs e)
        {
            string name = (sender as System.Windows.Controls.Button).Name.Substring(1).Replace('_', '-');
            string txt = (sender as System.Windows.Controls.Button).Content;
            foreach (Channel ch in configService.Config.Channels)
            {
                if (!(ch is InputChannel))
                {
                    // Output channels only
                    if (ch.Name.Equals(name))
                    {
                        // Match
                        if (txt.Equals("Open"))
                        {
                            dataAquisition.SetChannelValue(ch, 0);
                            (sender as System.Windows.Controls.Button).Content = "Closed";
                        }
                        else if (txt.Equals("Closed"))
                        {
                            dataAquisition.SetChannelValue(ch, 1);
                            (sender as System.Windows.Controls.Button).Content = "Open";
                        }
                    }
                }
            }
        }


        public InteractionRequest<FileInteractionNotification> OpenRequest { get; } = new InteractionRequest<FileInteractionNotification>();
        

    }
}
