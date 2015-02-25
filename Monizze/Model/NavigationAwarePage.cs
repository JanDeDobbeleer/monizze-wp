using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.ServiceLocation;

namespace Monizze.Model
{
    public class NavigationAwarePage : Page
    {
        private readonly INavigationService _navigationService;

        public NavigationAwarePage()
        {
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            Loaded += BackNavigationAwarePage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navigableViewModel = DataContext as INavigable;
            //Handle back naviagtion with parameter, remove last entry in that case
            if (e.Parameter is BackNavigationContent)
            {
                _navigationService.RemoveLastEntry();
                if (navigableViewModel != null)
                    navigableViewModel.Activate(((BackNavigationContent)e.Parameter).Content);
                return;
            }
            //navigate with parameter
            if (navigableViewModel != null)
                navigableViewModel.Activate(e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var navigableViewModel = DataContext as INavigable;
            if (navigableViewModel != null)
                navigableViewModel.Deactivate(e.Parameter);
            //remove the handler before you leave!
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        void BackNavigationAwarePage_Loaded(object sender, RoutedEventArgs e)
        {
            //This should be written here rather than the contructor
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            var navigableViewModel = DataContext as INavigable;
            if (navigableViewModel != null)
                navigableViewModel.OnBackKeyPress();
        }
    }
}
