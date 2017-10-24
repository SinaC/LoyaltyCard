using System;
using System.Windows.Input;
using EasyMVVM;
using Loyalty.App.Messages;
using Loyalty.IBusiness;
using LoyaltyCard.Domain;

namespace Loyalty.App.ViewModels
{
    public class ClientEncodingViewModel : ObservableObject
    {
        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();

        private Client _client;
        public Client Client
        {
            get { return _client; }
            protected set { Set(() => Client, ref _client, value); }
        }

        private decimal? _purchaseAmount;
        public decimal? PurchaseAmount
        {
            get { return _purchaseAmount; }
            set { Set(() => PurchaseAmount, ref _purchaseAmount, value); }
        }

        #region Save

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand = _saveCommand ?? new RelayCommand(Save);

        private void Save()
        {
            ClientBL.SaveClient(Client);
            if (PurchaseAmount.HasValue)
            {
                Purchase purchase = new Purchase
                {
                    Amount = PurchaseAmount.Value,
                    Date = DateTime.Now,
                    ClientId = Client.Id
                };
                ClientBL.SavePurchase(Client, purchase);
            }
            // Switch to search mode
            Mediator.Default.Send(new SearchClientMessage());
        }

        #endregion

        #region Cancel

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand = _cancelCommand ?? new RelayCommand(Cancel);

        private void Cancel()
        {
            // Switch to search mode
            Mediator.Default.Send(new SearchClientMessage());
        }

        #endregion

        public void Initialize(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            Client = client;
        }
    }

    public class ClientEncodingViewModelDesignData : ClientEncodingViewModel
    {
        public ClientEncodingViewModelDesignData()
        {
            Client = new Client
            {
                FirstName = "Pouet",
                LastName = "Brol",
                BirthDate = new DateTime(1999, 12, 31),
                Email = "pouet.brol@hotmail.com",
                Mobile = null
            };
        }
    }
}
