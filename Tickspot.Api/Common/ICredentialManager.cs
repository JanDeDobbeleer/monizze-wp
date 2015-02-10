using System.Collections.Generic;
using Tickspot.Api.Model;

namespace Tickspot.Api.Common
{
    public interface ICredentialManager
    {
        bool HasCredentials();
        List<Token> GetTokens();
        string GetTokenForId(string id);
        void SaveToken(Token token);
        void Clear();
    }
}
