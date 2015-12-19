using Windows.UI.ViewManagement;

namespace Monizze.Interfaces
{
    public interface IKeyboardAwareViewModel
    {
        void OnKeyboardShowing(InputPane sender, InputPaneVisibilityEventArgs args);
        void OnKeyboardHiding(InputPane sender, InputPaneVisibilityEventArgs args);
    }
}
