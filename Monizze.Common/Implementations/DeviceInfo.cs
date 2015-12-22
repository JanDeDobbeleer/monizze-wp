using System;
using System.Linq;
using System.Text;
using Windows.ApplicationModel;
using Windows.Networking.Connectivity;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Monizze.Common.Extensions;
using Monizze.Common.Interfaces;

namespace Monizze.Common.Implementations
{
    public class DeviceInfo : IDeviceInfo
    {
        public string AppVersion => GetAppVersion();
        public string Uuid => GetDeviceId();
        public string DeviceName => GetDeviceName();

        private string _appVersion = string.Empty;
        private string _deviceId = string.Empty;
        private string _deviceName = string.Empty;

        private string GetDeviceId()
        {
            if (!string.IsNullOrWhiteSpace(_deviceId))
                return _deviceId;
            try
            {
                var token = HardwareIdentification.GetPackageSpecificToken(null);
                _deviceId = token.Id.ToMd5Hash();
            }
            catch (Exception)
            {
                _deviceId = string.Empty;
            }
            return _deviceId;
        }

        private string GetAppVersion()
        {
            if (!string.IsNullOrWhiteSpace(_appVersion))
                return _appVersion;
            try
            {
                var package = Package.Current;
                _appVersion = package.Id.Version.Major + "." + package.Id.Version.Minor + "." + package.Id.Version.Build + "." + package.Id.Version.Revision;
            }
            catch (Exception)
            {
                _appVersion = string.Empty;
            }
            return _appVersion;
        }

        private string GetDeviceName()
        {
            if (!string.IsNullOrWhiteSpace(_deviceName))
                return _deviceName;
            var info = new EasClientDeviceInformation();
            _deviceName = info.SystemSku;
            return _deviceName;
        }

        public string GetEmailBody(string msisdn)
        {
            var builder = new StringBuilder();
            builder.Append("User Primary SIM: ");
            builder.Append(msisdn);
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
                var profile = NetworkInformation.GetInternetConnectionProfile();
                return profile != null && profile.GetConnectionCost().Roaming;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsConnected()
        {
            try
            {
                var connection = NetworkInformation.GetInternetConnectionProfile();
                return (connection != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsMobileVikingsNetwork()
        {
            var profiles = NetworkInformation.GetConnectionProfiles();
            var mobileDataProfile = profiles.FirstOrDefault(x => x.IsWwanConnectionProfile && x.WwanConnectionProfileDetails.AccessPointName.Equals("web.be"));
            return mobileDataProfile != null && !mobileDataProfile.GetConnectionCost().Roaming;
        }
    }
}
