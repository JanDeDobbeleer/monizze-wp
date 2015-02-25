using System;
using System.Linq;
using Windows.Security.Credentials;
using Monizze.Common.Interfaces;

namespace Monizze.Common.Model
{
    public class CredentialManager: ICredentialManager
    {
        private const string ResourceName = "monizzeCredentials";


        public bool IsLoggedIn()
        {
            try
            {
                return GetToken() != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetToken()
        {
            var vault = new PasswordVault();
            var credentials = vault.FindAllByResource(ResourceName).FirstOrDefault();
            if (credentials == null)
                return null;
            credentials.RetrievePassword();
            return credentials.Password;
        }

        public void SaveToken(string token)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential{Password = token, Resource = ResourceName, UserName = "username"});
        }

        public void Logout()
        {
            var vault = new PasswordVault();
            foreach (var passwordVault in vault.RetrieveAll())
            {
                vault.Remove(passwordVault);
            }
        }
    }
}
