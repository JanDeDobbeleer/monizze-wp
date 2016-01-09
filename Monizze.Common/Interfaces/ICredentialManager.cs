using System.Threading.Tasks;

namespace Monizze.Common.Interfaces
{
    public interface ICredentialManager
    {
        Task<bool> IsLoggedIn();
        Task<string> GetToken();
        Task SaveToken(string token);
        Task Logout();
    }
}
