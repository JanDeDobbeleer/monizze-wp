using System;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.Storage.Streams;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Monizze.Common.Interfaces;
using Monizze.Model;
using Monizze.View;

namespace Monizze.ViewModel
{
    public class AccountViewModel: ViewModelBase, INavigable
    {
        private readonly INavigationService _navigationService;
        private readonly ICredentialManager _credentialManager;
        private readonly ILogger _logger;
        private readonly IDeviceInfo _deviceInfo;

        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand MailCommand { get; set; }

        public AccountViewModel(INavigationService navigationService, ICredentialManager credentialManager, ILogger logger, IDeviceInfo deviceInfo)
        {
            _navigationService = navigationService;
            _credentialManager = credentialManager;
            _logger = logger;
            _deviceInfo = deviceInfo;
            LogoutCommand = new RelayCommand(() =>
            {
                _credentialManager.Logout();
                _navigationService.NavigateTo(typeof(LoginView));
            });
            MailCommand = new RelayCommand(async () =>
            {
                try
                {
                    _logger.Debug(GetType() + " Preparing email with logs");
                    var sendTo = new EmailRecipient
                    {
                        Address = "jan.de.dobbeleer@mobilevikings.com"
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
        }


        #region Inavigable
        public void Activate(object parameter)
        {
        }

        public void Deactivate(object parameter)
        {
        }

        public void OnBackKeyPress()
        {
            _navigationService.GoBack();
        }
        #endregion
    }
}
