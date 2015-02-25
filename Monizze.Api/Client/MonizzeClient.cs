using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Monizze.Api.Common;
using Monizze.Api.Model;
using Monizze.Common.Interfaces;
using Newtonsoft.Json;

namespace Monizze.Api.Client
{
    public interface IMonizzeClient
    {
        Task<bool> Login(string email, string password);
        Task<Account> GetAccount();
        Task<List<Transaction>> GetTransactions(string transactionId = "0");
    }

    public class MonizzeClient : StoreToCache, IMonizzeClient
    {
        public struct Parameter
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        private readonly ICredentialManager _credentialManager;

        public MonizzeClient(ILogger logger, ICredentialManager credentialManager)
            : base(logger)
        {
            _credentialManager = credentialManager;
        }

        public async Task<bool> Login(string email, string password)
        {
            try
            {
                using (var client = GetHttpClient())
                {
                    var parameterlist = new List<Parameter>
                    {
                        new Parameter { Name = "login", Value = email},
                        new Parameter { Name= "password", Value = password}
                    };
                    var response = await client.PostAsync("/en/api/json/login", new StringContent(GetQueryRequestParameters(parameterlist).Remove(0, 1), Encoding.UTF8, "application/x-www-form-urlencoded"));
                    if (!response.IsSuccessStatusCode)
                        return false;
                    var reponseString = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(reponseString);
                    _credentialManager.SaveToken(tokenResponse.Token);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(GetType()+ " Issue logging in", e);
                return false;
            }
        }

        public async Task<Account> GetAccount()
        {
            try
            {
                using (var client = GetHttpClient())
                {
                    var response = await client.GetAsync("/en/api/json/account");
                    if (!response.IsSuccessStatusCode)
                        return new Account();
                    var responseString = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserResponse>(responseString);
                    return user.User;
                }
            }
            catch (Exception e)
            {
                Logger.Error(GetType() + " Issue getting account", e);
                return new Account();
            }
        }

        public async Task<List<Transaction>> GetTransactions(string transactionId = "0")
        {
            try
            {
                using (var client = GetHttpClient())
                {
                    var parameters = new List<Parameter>
                    {
                        new Parameter {Name = "count", Value = "50"},
                        new Parameter {Name = "since_id", Value = transactionId}
                    };
                    var response = await client.GetAsync("/en/api/json/history" + GetQueryRequestParameters(parameters));
                    if (!response.IsSuccessStatusCode)
                        return new List<Transaction>();
                    var responseString = await response.Content.ReadAsStringAsync();
                    var history = JsonConvert.DeserializeObject<HistoryResponse>(responseString);
                    return history.Transactions;
                }
            }
            catch (Exception e)
            {
                Logger.Error(GetType() + " Issue getting transactions", e);
                return new List<Transaction>();
            }
        }

        private string GetQueryRequestParameters(IList<Parameter> parameters)
        {
            if (parameters == null || !parameters.Any())
                return string.Empty;
            var sb = new StringBuilder();
            sb.Append("?");
            for (var i = 0; i < parameters.Count; i++)
            {
                var p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }
            return sb.ToString();
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient {BaseAddress = new Uri("https://www.monizze.be")};
            var xtime = DateTime.Now.Ticks / 1000;
            client.DefaultRequestHeaders.Add("X-Version", "1");
            client.DefaultRequestHeaders.Add("X-Time", xtime.ToString());
            client.DefaultRequestHeaders.Add("x-Application", "map");
            client.DefaultRequestHeaders.Add("X-MONIZZE-APP-Token", GetToken(xtime.ToString()));
            if(_credentialManager.IsLoggedIn())
                client.DefaultRequestHeaders.Add("X-MONIZZE-LOGIN-Token", _credentialManager.GetToken());
            return client;
        }

        private string GetToken(string xtime)
        {
            const int version = 1;
            var token = string.Format("{0}{1}PinchMon", xtime, version);
            return ComputeMD5(token);
        }

        private string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
