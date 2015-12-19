using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace Monizze.Behaviors
{
    public class EnterKeyDownBehavior : DependencyObject, IBehavior
    {
        public void Attach(DependencyObject associatedObject)
        {
            if (!(associatedObject is UIElement))
                return;
            ((UIElement)associatedObject).GotFocus += OnGotFocus;
            ((UIElement)associatedObject).LostFocus += OnLostfocus;
            AssociatedObject = associatedObject;
        }

        private void OnLostfocus(object sender, RoutedEventArgs e)
        {
            var element = sender as UIElement;
            if (element == null)
                return;
            element.KeyDown -= OnKeyDown;
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var element = sender as UIElement;
            if (element == null)
                return;
            element.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
        {
            if (keyRoutedEventArgs.Key != VirtualKey.Enter)
                return;
            FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
        }

        public void Detach()
        {
            var uiElement = AssociatedObject as UIElement;
            if (uiElement == null)
                return;
            uiElement.GotFocus -= OnGotFocus;
            uiElement.LostFocus -= OnLostfocus;
        }

        public DependencyObject AssociatedObject { get; private set; }
    }
}
