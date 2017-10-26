using Prism.Commands;
using Prism.Events;
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
    public class LiveDataViewModel
    {
        [ImportingConstructor]
        public LiveDataViewModel(IDataAquisition dataAquisition, IEventAggregator eventAggregator)
        {
            DataModel = dataAquisition.DataModel;
            AddToGraphCommand = new DelegateCommand<Tuple<Channel, int>>(param =>
            {
                eventAggregator.GetEvent<AddToGraphEvent>().Publish((param.Item1, param.Item2));
            });

            eventAggregator.GetEvent<GraphCreatedEvent>().Subscribe(id => GraphIds.Add(id));
        }

        public LiveDataModel DataModel { get; }

        public ICommand AddToGraphCommand { get; }

        public ObservableCollection<int> GraphIds { get; } = new ObservableCollection<int> { 0 };
    }
}
