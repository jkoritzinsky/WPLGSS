using System;

namespace WPLGSS.Services
{
    public class StatusChangedEventArgs : EventArgs
    {
        public bool Status { get; set; }
    }
}