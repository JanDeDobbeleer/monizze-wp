using System.Threading.Tasks;

namespace Monizze.Common.Interfaces
{
    public interface IDeviceInfo
    {
        string AppVersion { get; set; }
        string Uuid { get; set; }
        string DeviceName { get; set; }
        Task<string> GetPushChannel();
        string GetEmailBody();
        bool IsRoaming();
        bool IsConnected();
        bool HasInternet();
    }
}
