using Windows.ApplicationModel.Background;
using Monizze.Api.Client;
using Monizze.Common.Implementations;
using Monizze.Common.Model;

namespace Monizze.LiveTile
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var info = new DeviceInfo();
            if (!info.IsConnected())
                return;
            var deferral = taskInstance.GetDeferral();
            var api = new MonizzeClient(new Logger(), new CredentialManager());
            var response = await api.GetAccount();
            if (string.IsNullOrWhiteSpace(response.Balance))
                return;
            var updater = new TileUpdater();
            updater.UpdateTile(response.Balance);
            deferral.Complete();
        }
    }
}
