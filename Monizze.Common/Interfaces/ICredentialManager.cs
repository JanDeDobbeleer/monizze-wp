namespace Monizze.Common.Interfaces
{
    public interface ICredentialManager
    {
        bool IsLoggedIn();
        string GetToken();
        void SaveToken(string token);
        void Logout();
    }
}
