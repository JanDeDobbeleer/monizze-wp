using System.Threading.Tasks;

namespace Monizze.Interfaces
{
    public interface INavigable
    {
        Task Activate();
        void Deactivate();
        void OnBackKeyPress();
    }
}
