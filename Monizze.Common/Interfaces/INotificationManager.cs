using System;
using System.Threading.Tasks;

namespace Monizze.Common.Interfaces
{
    public interface INotificationManager
    {
        Task<bool> ShowMessageBox(string message, string buttonConfirmText, string buttonCancelText);
        Task<Tuple<bool, string>> ShowInteractionBox(string title, string info, string boxContent, string placeHolderText, string actionButton, string cancelButton);
    }
}
