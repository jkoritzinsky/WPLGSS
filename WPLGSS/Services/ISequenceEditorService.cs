﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using WPLGSS.ViewModels;

namespace WPLGSS.Services
{
    public interface ISequenceEditorService
    {
        void OpenSequenceInRegion(string editorRegion, string path, SequenceViewModel sequence);

        Sequence OpenSequenceFromFile(string path);

        void SaveSequence(string editorRegionName, string path, SequenceFile sequence);
    }
}
