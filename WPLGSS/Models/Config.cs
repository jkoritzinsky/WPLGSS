﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLGSS.Models
{
    public class Config
    {
        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>();
    }
}
