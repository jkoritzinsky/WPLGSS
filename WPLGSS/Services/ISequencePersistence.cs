using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;

namespace WPLGSS.Services
{
    public interface ISequencePersistence
    {
        void SaveSequence(string path, Sequence sequence);

        Sequence OpenSequence(string path);
    }
}
