using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using WPLGSS.Services;

namespace WPLGSS.ViewModels
{
    [Export]
    public class LiveDataViewModel
    {
        [ImportingConstructor]
        public LiveDataViewModel(IDataAquisition dataAquisition)
        {
            DataModel = dataAquisition.DataModel;
        }

        public LiveDataModel DataModel { get; }
    }
}
