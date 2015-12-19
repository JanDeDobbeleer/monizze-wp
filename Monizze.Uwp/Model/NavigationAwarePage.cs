using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.ServiceLocation;
using Monizze.Interfaces;

namespace Monizze.Model
{
    public class NavigationAwarePage : Page
    {
        public NavigationAwarePage()
        {
            Loaded += BackNavigationAwarePage_Loaded;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var navigableViewModel = DataContext as INavigable;
            var activate = navigableViewModel?.Activate();
            if (activate != null)
                await activate;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            var navigableViewModel = DataContext as INavigable;
            navigableViewModel?.Deactivate();
            //remove the handler before you leave!
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
            UnRegisterKeyboardNotifications();
        }

        void BackNavigationAwarePage_Loaded(object sender, RoutedEventArgs e)
        {
            //This should be written here rather than the contructor
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            RegisterKeyboardNotifications();
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            var navigableViewModel = DataContext as INavigable;
            navigableViewModel?.OnBackKeyPress();
        }

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
            var keyboardAwareViewModel = DataContext as IKeyboardAwareViewModel;
            keyboardAwareViewModel?.OnKeyboardShowing(sender, args);
        }

        private void OnKeyboardHiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            var keyboardAwareViewModel = DataContext as IKeyboardAwareViewModel;
            keyboardAwareViewModel?.OnKeyboardHiding(sender, args);
        }
    }
}
