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
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WPLGSS.ViewModels
{
    [Export]
    public class SchematicViewModel : ChannelPresenterViewModelBase
    {
        [ImportingConstructor]
        public SchematicViewModel(IConfigService config, IDataAquisition dataAquisition, ISequenceRunner runner)
            :base(config, dataAquisition, runner)
        {
            Channels.CollectionChanged += (o, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (LiveChannel item in e.NewItems)
                    {
                        item.Channel.PropertyChanged += NotifyUpdated;
                    } 
                }
                if (e.OldItems != null)
                {
                    foreach (LiveChannel item in e.OldItems)
                    {
                        item.Channel.PropertyChanged -= NotifyUpdated;
                    } 
                }
                RaiseUpdates();
            };
        }

        private void NotifyUpdated(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Channel.Name))
            {
                RaiseUpdates();
            }
        }

        private void RaiseUpdates()
        {
            RaisePropertyChanged(Binding.IndexerName);
            SendOutputValueCommand.RaiseCanExecuteChanged();
        }

        public LiveChannel this[string name] => Channels.FirstOrDefault(channel => channel.Channel.Name == name);
    }
}
