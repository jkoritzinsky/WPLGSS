using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    [Export]
    public class LiveDataViewModel : ChannelPresenterViewModelBase
    {
        [ImportingConstructor]
        public LiveDataViewModel(IConfigService config, IDataAquisition dataAquisition, ISequenceRunner runner, IEventAggregator eventAggregator)
            : base(config, dataAquisition, runner)
        {
            AddToGraphCommand = new DelegateCommand<Tuple<InputChannel, int>>(param =>
            {
                eventAggregator.GetEvent<AddToGraphEvent>().Publish((param.Item1, param.Item2));
            });
            eventAggregator.GetEvent<GraphCreatedEvent>().Subscribe(id => GraphIds.Add(id));
        }


        public ICommand AddToGraphCommand { get; }

        public ObservableCollection<int> GraphIds { get; } = new ObservableCollection<int> { 0 };
    }
}
