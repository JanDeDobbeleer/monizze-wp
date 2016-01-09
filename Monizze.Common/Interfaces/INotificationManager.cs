using System.Threading.Tasks;

namespace Monizze.Common.Interfaces
{
    public interface INotificationManager
    {
        Task<bool> ShowMessageBox(string message, string buttonConfirmText, string buttonCancelText);
    }
}
