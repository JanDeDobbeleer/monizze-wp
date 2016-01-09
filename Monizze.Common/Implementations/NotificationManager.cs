using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Monizze.Common.Interfaces;

namespace Monizze.Common.Implementations
{
    public class NotificationManager: INotificationManager
    {
        public async Task<bool> ShowMessageBox(string message, string buttonConfirmText, string buttonCancelText)
        {
            var dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand(buttonConfirmText) { Id = 0 });
            dialog.Commands.Add(new UICommand(buttonCancelText) { Id = 1 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var result = await dialog.ShowAsync();
            return result.Id.Equals(0);
        }
    }
}
