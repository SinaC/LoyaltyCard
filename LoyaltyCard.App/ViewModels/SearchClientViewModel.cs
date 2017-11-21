using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<ClientSummary> _clients;
        public ObservableCollection<ClientSummary> Clients
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

                List<ClientSummary> clients = await AsyncFake.CallAsync(ClientBL, x => x.GetClientSummaries(Filter));
                Clients = new ObservableCollection<ClientSummary>(clients);
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

        public SearchClientViewModel()
        {
            Mediator.Default.Register<SaveClientMessage>(this, HandleSaveClientMessage);
        }

        private void HandleSaveClientMessage(SaveClientMessage msg)
        {
            // Refresh client summary
            ClientSummary existingSummary = Clients?.FirstOrDefault(x => x.Id == msg.Client.Id);
            if (existingSummary != null)
                existingSummary.Initialize(msg.Client);
            else
            {
                ClientSummary clientSummary = new ClientSummary(msg.Client);
                Clients = Clients ?? new ObservableCollection<ClientSummary>();
                Clients.Insert(0, clientSummary); // insert at the beginning
            }
        }
    }

    public class SearchClientViewModelDesignData : SearchClientViewModel
    {
        public SearchClientViewModelDesignData()
        {
            Clients = new ObservableCollection<ClientSummary>(Enumerable.Range(0, 50).Select(x => new ClientSummary
            {
                ClientBusinessId = 10000+x,
                FirstName = $"Pouet{x}",
                LastName = "Brol",
                Total = x * 10,
                TotalSinceLastVoucher = x * 5,
                LastPurchase = new Purchase
                {
                    Date = DateTime.Now.AddDays(-x * 2),
                    Amount = x
                },
                OldestActiveVoucher = new Voucher
                {
                    IssueDate = DateTime.Now.AddDays(-x),
                    Percentage = 20,
                    ValidityEndDate = DateTime.Now.AddDays(-x).AddMonths(1)
                }
            }));
        }
    }
}
