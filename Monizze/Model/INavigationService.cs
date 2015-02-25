using System;

namespace Monizze.Model
{
    public interface INavigationService
    {
        void NavigateTo(Type sourcePageType);
        void NavigateTo(Type sourcePageType, object parameter);
        void GoBack();
        void GoBack(object parameter);
        void Exit();
        void RemoveLastEntry();
        void ClearBackStack();
        void ResetPageCache();
    }
}
