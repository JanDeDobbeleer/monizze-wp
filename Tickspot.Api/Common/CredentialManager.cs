using System.Collections.Generic;
using System.Linq;
using Windows.Security.Credentials;
using Tickspot.Api.Model;

namespace Tickspot.Api.Common
{
    public class CredentialManager: ICredentialManager
    {
        public bool HasCredentials()
        {
            var vault = new PasswordVault();
            var credentialList = vault.RetrieveAll();
            return credentialList.Any();
        }

        public List<Token> GetTokens()
        {
            var vault = new PasswordVault();
            return vault.RetrieveAll().Select(passwordVault => new Token {Company = passwordVault.UserName, SubscriptionId = passwordVault.Resource}).ToList();
        }

        public string GetTokenForId(string id)
        {
            var vault = new PasswordVault();
            var passwordVault = vault.FindAllByResource(id).FirstOrDefault();
            if (passwordVault == null)
                return string.Empty;
            passwordVault.RetrievePassword();
            return passwordVault.Password;
        }

        public void SaveToken(Token token)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential { Resource = token.SubscriptionId, UserName = token.Company, Password = token.ApiToken });
        }

        public void Clear()
        {
            var vault = new PasswordVault();
            foreach (var passwordVault in vault.RetrieveAll())
            {
                vault.Remove(passwordVault);
            }
        }
    }
}
