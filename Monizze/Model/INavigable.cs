namespace Monizze.Model
{
    public interface INavigable
    {
        void Activate(object parameter);
        void Deactivate(object parameter);
        void OnBackKeyPress();
    }
}
