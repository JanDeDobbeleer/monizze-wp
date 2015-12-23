using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Monizze.Api.Client;
using Monizze.Api.Model;
using Monizze.Interfaces;
using Monizze.View;

namespace Monizze.ViewModel
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        private readonly IMonizzeClient _client;
        private readonly INavigationService _navigationService;

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand AccountCommand { get; set; }

        public string Name { get; set; } = "...";
        public string Balance { get; set; } = "--.--";
        public List<Transaction> Transactions { get; set; }
        public bool Loading { get; set; }

        public MainViewModel(IMonizzeClient client, INavigationService navigationService)
        {
            _client = client;
            _navigationService = navigationService;
            RefreshCommand = new RelayCommand(async () =>
            {
                if(Loading)
                    return;
                await Refresh();
            });
            AccountCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo(typeof(AccountView));
            });
        }

        private async Task Refresh()
        {
            Transactions = new List<Transaction>();
            Loading = true;
            var account = await _client.GetAccount();
            if (account == null)
            {
                await ShowMessage("You account details could not be loaded");
            }
            else
            {
                Name = account.FirstName + " " + account.LastName;
                Balance = "€" + account.Balance;
            }
            var temp = await _client.GetTransactions();
            Loading = false;
            Transactions = temp;
            Transactions.AddRange(await  _client.GetTransactions());
            if (Transactions == null)
                await ShowMessage("Your transactions could not be loaded");
        }

        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        #region INavigable
        public async Task Activate()
        {
            await Refresh();
        }

        public void Deactivate()
        {
            //TODO: handle this
        }

        public void OnBackKeyPress()
        {
            _navigationService.Exit();
        }
        #endregion
    }
}