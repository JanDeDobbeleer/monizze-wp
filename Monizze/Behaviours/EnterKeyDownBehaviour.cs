using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace Monizze.Behaviours
{
    public class EnterKeyDownBehaviour : DependencyObject, IBehavior
    {
        public void Attach(DependencyObject associatedObject)
        {
            if (!(associatedObject is UIElement))
                return;
            ((UIElement)associatedObject).KeyDown += OnKeyDown;
            AssociatedObject = associatedObject;
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
            if (uiElement != null) uiElement.KeyDown -= OnKeyDown;
        }

        public DependencyObject AssociatedObject { get; private set; }
    }
}
