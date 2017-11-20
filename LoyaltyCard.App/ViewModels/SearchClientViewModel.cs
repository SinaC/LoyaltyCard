﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EasyMVVM;
using LoyaltyCard.App.Messages;
using LoyaltyCard.Common;
using LoyaltyCard.IBusiness;
using LoyaltyCard.Domain;

namespace LoyaltyCard.App.ViewModels
{
    public class SearchClientViewModel : ViewModelBase
    {
        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();

        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set { Set(() => Filter, ref _filter, value); }
        }

        private ClientSummary _selectedClient;
        public ClientSummary SelectedClient
        {
            get { return _selectedClient; }
            set { Set(() => SelectedClient, ref _selectedClient, value); }
        }

        private List<ClientSummary> _clients;
        public List<ClientSummary> Clients
        {
            get { return _clients; }
            set { Set(() => Clients, ref _clients, value); }
        }

        #region Edit client

        private ICommand _editClientCommand;
        public ICommand EditClientCommand => _editClientCommand ?? (_editClientCommand = new RelayCommand(EditClient));

        private void EditClient()
        {
            if (SelectedClient == null)
                return;
            // Display client
            Mediator.Default.Send(new SwitchToDisplayClientMessage
            {
                Client = SelectedClient
            });
        }

        #endregion

        #region Search

        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new AsyncRelayCommand(SearchAsync));

        private async Task SearchAsync()
        {
            try
            {
                IsBusy = true;

                Clients = await AsyncFake.CallAsync(ClientBL, x => x.GetClientSummaries(Filter));
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Create

        private ICommand _createClientCommand;
        public ICommand CreateClientCommand => _createClientCommand ?? (_createClientCommand = new RelayCommand(CreateClient));

        private void CreateClient()
        {
            // Create a new client
            Mediator.Default.Send(new CreateClientMessage());
        }

        #endregion

        #region Stats

        private ICommand _displayStatsCommand;
        public ICommand DisplayStatsCommand => _displayStatsCommand ?? (_displayStatsCommand = new RelayCommand(DisplayStats));

        private void DisplayStats()
        {
            Mediator.Default.Send(new SwitchToStatisticsMessage());
        }

        #endregion

        #region Test

        private ICommand _testCommand;
        public ICommand TestCommand => _testCommand ?? (_testCommand = new AsyncRelayCommand(TestAsync));

        private async Task TestAsync()
        {
            IMailAutomationBL mailAutomationBL = EasyIoc.IocContainer.Default.Resolve<IMailAutomationBL>();
            await mailAutomationBL.SendAutomatedMailsAsync();
        }

        #endregion
    }

    public class SearchClientViewModelDesignData : SearchClientViewModel
    {
        public SearchClientViewModelDesignData()
        {
            Clients = Enumerable.Range(0, 50).Select(x => new ClientSummary
            {
                FirstName = $"Pouet{x}",
                LastName = "Brol",
                Total = x*10,
                TotalSinceLastVoucher = x*5,
                LastPurchaseDate = DateTime.Now.AddDays(-x * 2),
                LastPurchaseAmount = x
            }).ToList();
        }
    }
}
