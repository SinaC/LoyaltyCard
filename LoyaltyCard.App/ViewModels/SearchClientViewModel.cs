using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EasyMVVM;
using LoyaltyCard.App.Messages;
using LoyaltyCard.IBusiness;
using LoyaltyCard.Domain;

namespace LoyaltyCard.App.ViewModels
{
    public class SearchClientViewModel : ObservableObject
    {
        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();

        private string _firstNameFilter;
        public string FirstNameFilter
        {
            get { return _firstNameFilter; }
            set { Set(() => FirstNameFilter, ref _firstNameFilter, value); }
        }

        private string _lastNameFilter;
        public string LastNameFilter
        {
            get { return _lastNameFilter; }
            set { Set(() => LastNameFilter, ref _lastNameFilter, value); }
        }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get { return _selectedClient; }
            set { Set(() => SelectedClient, ref _selectedClient, value); }
        }

        private List<Client> _clients;
        public List<Client> Clients
        {
            get { return _clients; }
            set { Set(() => Clients, ref _clients, value); }
        }

        #region Edit client

        private ICommand _editClientCommand;
        public ICommand EditClientCommand => _editClientCommand = _editClientCommand ?? new RelayCommand(EditClient);

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
        public ICommand SearchCommand => _searchCommand = _searchCommand ?? new RelayCommand(Search);

        private void Search()
        {
            Clients = ClientBL.SearchClients(FirstNameFilter, LastNameFilter);
        }

        #endregion

        #region Create

        private ICommand _createClientCommand;
        public ICommand CreateClientCommand => _createClientCommand = _createClientCommand ?? new RelayCommand(CreateClient);

        private void CreateClient()
        {
            // Create a new client
            Mediator.Default.Send(new CreateClientMessage
            {
                FirstNameFilter = FirstNameFilter,
                LastNameFilter = LastNameFilter
            });
        }

        #endregion

        #region Stats

        private ICommand _displayStatsCommand;
        public ICommand DisplayStatsCommand => _displayStatsCommand = _displayStatsCommand ?? new RelayCommand(DisplayStats);

        private void DisplayStats()
        {
            Mediator.Default.Send(new SwitchToStatisticsMessage());
        }

        #endregion

        public void Initialize()
        {
        }
    }

    public class SearchClientViewModelDesignData : SearchClientViewModel
    {
        public SearchClientViewModelDesignData()
        {
            Clients = Enumerable.Range(0, 50).Select(x => new Client
            {
                FirstName = $"Pouet{x}",
                LastName = "Brol",
                BirthDate = x == 5 ? new DateTime(1976, DateTime.Today.Month, DateTime.Today.Day) : new DateTime(1999, 12, 31),
                Email = "pouet.brol@hotmail.com",
                Mobile = null,
                Purchases = new ObservableCollection<Purchase>(Enumerable.Range(1,1+x/2).Select(y => new Purchase
                {
                    Amount = x+y*10,
                    Date = DateTime.Now.AddDays(-y*2)
                }))
            }).ToList();
        }
    }
}
