namespace Monizze.Common.Interfaces
{
    public interface IDeviceInfo
    {
        string AppVersion { get; }
        string Uuid { get; }
        string DeviceName { get; }
        string GetEmailBody();
        bool IsRoaming();
        bool IsConnected();
        bool IsMobileVikingsNetwork();
    }
}
