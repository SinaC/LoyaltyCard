using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EasyMVVM;
using Loyalty.App.Messages;
using Loyalty.IBusiness;
using LoyaltyCard.Domain;

namespace Loyalty.App.ViewModels
{
    public class DisplayClientViewModel : ObservableObject
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
            if (string.IsNullOrWhiteSpace(Client.FirstName)
                || string.IsNullOrWhiteSpace(Client.LastName))
                return; // TODO: inform user

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
            PurchaseAmount = null;
        }
    }

    public class DisplayDisplayClientViewModelDesignData : DisplayClientViewModel
    {
        public DisplayDisplayClientViewModelDesignData()
        {
            Client = new Client
            {
                FirstName = "Pouet",
                LastName = "Brol",
                BirthDate = new DateTime(1999, 12, 31),
                Email = "pouet.brol@hotmail.com",
                Mobile = null,
                Purchases = new ObservableCollection<Purchase>(Enumerable.Range(1, 20).Select(y => new Purchase
                {
                    Amount = y * 10,
                    Date = DateTime.Now.AddDays(-y * 2)
                }))
            };
        }
    }
}
