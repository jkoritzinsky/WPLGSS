using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.ViewModels;

namespace WPLGSS.Services
{
    public interface ISequenceEditorService
    {
        void OpenSequence(string editorRegion, string path, SequenceViewModel sequence);
    }
}
