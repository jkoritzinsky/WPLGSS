using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPLGSS.Interactivity;

namespace WPLGSS.ViewModels
{
    public abstract class PersistViewModelBase : BindableBase
    {
        protected PersistViewModelBase()
        {
            _saveCommand = new DelegateCommand(Save);
            _openCommand = new DelegateCommand(Open);
        }

        private ICommand _openCommand;
        private ICommand _saveCommand;

        public virtual ICommand SaveCommand => _saveCommand;

        public virtual ICommand OpenCommand => _openCommand;

        public InteractionRequest<FileInteractionNotification> SaveRequest { get; } = new InteractionRequest<FileInteractionNotification>();

        public InteractionRequest<FileInteractionNotification> OpenRequest { get; } = new InteractionRequest<FileInteractionNotification>();

        protected abstract FileInteractionNotification Notification { get; }

        protected void Save()
        {
            Notification.Confirmed = false;
            SaveRequest.Raise(Notification, n =>
            {
                if (n.Confirmed)
                {
                    SaveCore(n.Path);
                }
            });
        }

        protected abstract void SaveCore(string path);

        protected void Open()
        {
            Notification.Confirmed = false;
            OpenRequest.Raise(Notification, n =>
            {
                if (n.Confirmed)
                {
                    OpenCore(n.Path);
                }
            });
        }

        protected abstract void OpenCore(string path);
    }
}
