using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    public interface ISequenceRunner
    {
        void RunSequence(Models.Sequence sequence);
        event EventHandler<StatusChangedEventArgs> SequenceRunningStateChanged;
    }
}
