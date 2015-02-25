using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.PushNotifications;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Monizze.Common.Interfaces;

namespace Monizze.Common.Implementations
{
    public class DeviceInfo : IDeviceInfo
    {
        public string AppVersion { get; set; }
        public string Uuid { get; set; }
        public string DeviceName { get; set; }

        public DeviceInfo()
        {
            Uuid = GetDeviceId();
            GetDeviceInfo();
            AppVersion = GetAppVersion();
        }

        private string GetDeviceId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;

            var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var hashed = hasher.HashData(hardwareId);

            return CryptographicBuffer.EncodeToHexString(hashed);
        }

        public async Task<string> GetPushChannel()
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            return channel.Uri;
        }

        private string GetAppVersion()
        {
            string version;
            try
            {
                var type = Application.Current.GetType().AssemblyQualifiedName;
                version = type.Split(',')[2].Split('=')[1];
            }
            catch (Exception)
            {
                version = "unknown";
            }
            return version;
        }

        private void GetDeviceInfo()
        {
            var info = new EasClientDeviceInformation();
            DeviceName = info.SystemSku;
        }

        public string GetEmailBody()
        {
            var builder = new StringBuilder();
            builder.Append(Environment.NewLine);
            builder.Append("Device ID: ");
            builder.Append(GetDeviceId());
            builder.Append(Environment.NewLine);
            builder.Append("Application version: ");
            builder.Append(AppVersion);
            builder.Append(Environment.NewLine);
            builder.Append("Device model: ");
            builder.Append(DeviceName);
            var info = NetworkInformation.GetInternetConnectionProfile().NetworkAdapter.IanaInterfaceType;
            builder.Append(Environment.NewLine);
            builder.Append("Network: ");
            builder.Append((info.Equals(71) ? "wifi" : "mobile"));
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            return builder.ToString();
        }

        public bool IsRoaming()
        {
#if DEBUG
            if (DeviceName.Equals("Microsoft Virtual"))
                return false;
#endif
            try
            {
                return NetworkInformation.GetInternetConnectionProfile().GetConnectionCost().Roaming;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsConnected()
        {
#if DEBUG
            if (DeviceName.Equals("Microsoft Virtual"))
                return true;
#endif
            try
            {
                var connection = NetworkInformation.GetInternetConnectionProfile();
                if (connection == null)
                    return false;
                return connection.GetSignalBars() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasInternet()
        {
#if DEBUG
            if (DeviceName.Equals("Microsoft Virtual"))
                return true;

#endif
            try
            {
                return NetworkInformation.GetInternetConnectionProfile().GetSignalBars() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
