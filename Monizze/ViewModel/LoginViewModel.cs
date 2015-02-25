using System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Monizze.Api.Client;
using Monizze.Model;
using Monizze.View;

namespace Monizze.ViewModel
{
    public class LoginViewModel: ViewModelBase, INavigable
    {
        private readonly IMonizzeClient _client;
        private readonly INavigationService _navigationService;

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand ForgotCommand { get; set; }

        private double _rectangleHeight;
        public double RectangleHeight
        {
            get { return _rectangleHeight; }
            set
            {
                if (value == _rectangleHeight)
                    return;
                _rectangleHeight = value;
                RaisePropertyChanged(() => RectangleHeight);
            }
        }
        
        private string _username;
        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public LoginViewModel(IMonizzeClient client, INavigationService navigationService)
        {
            _client = client;
            _navigationService = navigationService;
            LoginCommand = new RelayCommand(async () =>
            {
                //Verify if you filled in the necessary credentials
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
                {
                    var dialog = new MessageDialog((string.IsNullOrWhiteSpace(UserName)) 
                        ? "Please provide a username"
                        : "Please provide a password");
                    await dialog.ShowAsync();
                    return;
                }
                Loading = true;
                //Get the token
                var success = await _client.Login(UserName, Password);
                //Verify the response
                if (!success)
                {
                    var dialog = new MessageDialog("We were unable to log you in, please try again");
                    await dialog.ShowAsync();
                    Loading = false;
                    return;
                }
                Loading = false;
                Password = string.Empty;
                _navigationService.NavigateTo(typeof(MainView));
            });
            ForgotCommand = new RelayCommand(async () =>
            {
                var dialog = new MessageDialog("Check back soon :-)");
                await dialog.ShowAsync();
            });
        }

        #region Inavigable
        public void Activate(object parameter)
        {
            RegisterKeyboardNotifications();
        }

        public void Deactivate(object parameter)
        {
            UnRegisterKeyboardNotifications();
        }

        public void OnBackKeyPress()
        {
            _navigationService.Exit();
        }
        #endregion

        #region keyboard Notifications

        private void RegisterKeyboardNotifications()
        {
            InputPane.GetForCurrentView().Showing += OnKeyboardShowing;
            InputPane.GetForCurrentView().Hiding += OnKeyboardHiding;
        }

        private void UnRegisterKeyboardNotifications()
        {
            InputPane.GetForCurrentView().Showing -= OnKeyboardShowing;
            InputPane.GetForCurrentView().Hiding -= OnKeyboardHiding;
        }

        private void OnKeyboardShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            RectangleHeight = args.OccludedRect.Height;
        }

        private void OnKeyboardHiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            RectangleHeight = 0;
        }

        #endregion
    }
}
