using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.Storage.Streams;
using GalaSoft.MvvmLight.Command;
using Monizze.Common.Interfaces;
using Monizze.Interfaces;
using Monizze.View;

namespace Monizze.ViewModel
{
    public class AccountViewModel: SuperViewModelBase, INavigable
    {
        private readonly INavigationService _navigationService;
        private readonly ICredentialManager _credentialManager;
        private readonly ILogger _logger;
        private readonly IDeviceInfo _deviceInfo;

        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand MailCommand { get; set; }
        public RelayCommand PrivacyCommand { get; set; }
        public string VersionNumber { get; set; }

        public AccountViewModel(INavigationService navigationService, ICredentialManager credentialManager, ILogger logger, IDeviceInfo deviceInfo, INotificationManager notificationManager)
        {
            _navigationService = navigationService;
            _credentialManager = credentialManager;
            _logger = logger;
            _deviceInfo = deviceInfo;
            _notificationManager = notificationManager;
            LogoutCommand = new RelayCommand(async () =>
            {
                var confirmed  = await _notificationManager.ShowMessageBox("Are you sure you want to log out?", "Yes", "No");
                if (!confirmed)
                    return;
                await _credentialManager.Logout();
                _navigationService.NavigateTo(typeof(LoginView));
            });
            MailCommand = new RelayCommand(async () =>
            {
                try
                {
                    _logger.Debug(GetType() + " Preparing email with logs");
                    var sendTo = new EmailRecipient
                    {
                        Address = "monizze@outlook.be"
                    };
                    //generate mail object
                    var mail = new EmailMessage { Subject = "Crash report from the My Monizze Windows Phone app" };
                    mail.To.Add(sendTo);
                    var folders = ApplicationData.Current.LocalFolder;
                    var files = await folders.GetFilesAsync();
                    foreach (var file in files)
                    {
                        if (!file.Name.EndsWith(".log"))
                            continue;
                        var stream = RandomAccessStreamReference.CreateFromFile(file);
                        mail.Attachments.Add(new EmailAttachment(file.Name, stream));
                    }
                    mail.Body = _deviceInfo.GetEmailBody();
                    //open the share contract with Mail only:
                    await EmailManager.ShowComposeNewEmailAsync(mail);
                }
                catch (Exception e)
                {
                    _logger.Error(GetType() + " trouble sending email, ship is not sailing son", e);
                }
            });
            PrivacyCommand = new RelayCommand(async () =>
            {
                var page = new Uri("http://jan-joris.be/privacy.html");
                await Windows.System.Launcher.LaunchUriAsync(page);
            });
        }


        #region Inavigable
        public async Task Activate()
        {
            VersionNumber = _deviceInfo.AppVersion;
            await Task.Factory.StartNew(() =>
            {
            });
        }

        public void Deactivate()
        {
            //TODO: handle this when needed
        }

        public void OnBackKeyPress()
        {
            _navigationService.GoBack();
        }
        #endregion
    }
}
