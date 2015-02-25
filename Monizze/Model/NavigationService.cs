using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Monizze.Common.Interfaces;

namespace Monizze.Model
{
    public struct BackNavigationContent
    {
        public object Content { get; set; }
    }

    public class NavigationService : INavigationService
    {
        private readonly ILogger _logger;
        private List<PageStackEntry> _backStack = new List<PageStackEntry>();

        public NavigationService(ILogger logger)
        {
            _logger = logger;
        }

        public void NavigateTo(Type sourcePageType)
        {
            NavigateTo(sourcePageType, string.Empty);
        }

        public void NavigateTo(Type sourcePageType, object parameter)
        {
            _logger.Info("Navigating to " + sourcePageType + " with element " + parameter);
            _backStack = ((Frame)Window.Current.Content).BackStack.ToList();
            ((Frame)Window.Current.Content).Navigate(sourcePageType, parameter, new SlideNavigationTransitionInfo());
        }

        public void GoBack()
        {
            _logger.Info("Navigating back");
            ((Frame)Window.Current.Content).GoBack();
        }

        public void GoBack(object parameter)
        {
            _logger.Info("Navigating back");
            var previous = ((Frame)Window.Current.Content).BackStack.Last();
            ((Frame)Window.Current.Content).Navigate(previous.SourcePageType, new BackNavigationContent { Content = parameter }, new ContinuumNavigationTransitionInfo());
        }

        public void Exit()
        {
            Application.Current.Exit();
        }

        public void RemoveLastEntry()
        {
            ((Frame)Window.Current.Content).BackStack.Clear();
            foreach (var pageStackEntry in _backStack)
            {
                ((Frame)Window.Current.Content).BackStack.Add(pageStackEntry);
            }
        }

        public void ClearBackStack()
        {
            _logger.Info("Clearing backstack");
            ((Frame)Window.Current.Content).BackStack.Clear();
        }

        public void ResetPageCache()
        {
            var cacheSize = ((Frame)Window.Current.Content).CacheSize;
            ((Frame)Window.Current.Content).CacheSize = 0;
            ((Frame)Window.Current.Content).CacheSize = cacheSize;
        }
    }
}
