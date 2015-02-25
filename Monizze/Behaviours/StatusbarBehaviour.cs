using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Monizze.Behaviours
{
    public class StatusBarBehavior : DependencyObject, IBehavior
    {
        #region dependancyProperty
        public DependencyObject AssociatedObject { get; private set; }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground",
            typeof(Color),
            typeof(StatusBarBehavior),
            new PropertyMetadata(Colors.White, OnForeGroundChanged));
        #endregion

        #region Properties
        public Color ForeGround
        {
            get { return (Color)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        #endregion

        #region Interface
        public void Attach(DependencyObject associatedObject)
        {
        }

        public void Detach()
        {
        }
        #endregion

        #region Events
        private static void OnForeGroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (StatusBarBehavior)d;
            StatusBar.GetForCurrentView().ForegroundColor = behavior.ForeGround;
        }
        #endregion
    }
}
