using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace WPLGSS.Interactivity
{
    public class SaveFileAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (!(parameter is InteractionRequestedEventArgs args))
            {
                return;
            }

            var notification = (FileInteractionNotification)args.Context;

            var openFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = notification.DefaultExtension,
                Filter = notification.Filter,
                FileName = notification.Path
            };

            notification.Confirmed = openFileDialog.ShowDialog(Window.GetWindow(AssociatedObject)) ?? false;
            notification.Path = openFileDialog.FileName;

            args.Callback();
        }
    }
}
