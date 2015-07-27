using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Monizze.Api.Model;

namespace Monizze.Api.Client
{
    public class MockClient : IMonizzeClient
    {
        public Task<bool> Login(string email, string password)
        {
            return new Task<bool>(() => true);
        }

        public Task<Account> GetAccount()
        {
           return new TaskFactory().StartNew(() => new Account { Balance = "30.39", CardNumbers = new List<string> { "32321313131", "3323424242" }, FirstName = "Test", LastName = "User" });
            //return new Task<Account>(() => new Account { Balance = "30.39", CardNumbers = new List<string> { "32321313131", "3323424242" }, FirstName = "Test", LastName = "User" });
        }

        public Task<List<Transaction>> GetTransactions(string transactionId = "0")
        {
            var list = new List<Transaction>
            {
                new Transaction
                {
                    Amount = "13.90",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.91",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.92",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.93",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.94",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.95",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.96",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.97",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.98",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "13.99",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "14.00",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "14.10",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "14.20",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "14.30",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "14.40",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                },
                new Transaction
                {
                    Amount = "14.50",
                    MerchantName = "This is a merchant",
                    TimeStamp = "12-09-2014",
                    Transactionid = "1"
                }
            };
            return new TaskFactory().StartNew(() => list);
            //return new Task<List<Transaction>>(() => list);
        }
    }
}
