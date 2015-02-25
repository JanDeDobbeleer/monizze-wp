using Windows.UI.Xaml.Navigation;
using Monizze.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Monizze.View
{
    public sealed partial class MainView : NavigationAwarePage
    {
        public MainView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }
    }
}
