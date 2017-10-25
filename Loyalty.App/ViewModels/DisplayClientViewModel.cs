using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EasyMVVM;
using Loyalty.App.Messages;
using Loyalty.App.Models;
using Loyalty.App.ViewModels.Popups;
using Loyalty.IBusiness;
using LoyaltyCard.Domain;
using LoyaltyCard.Services.Popup;

namespace Loyalty.App.ViewModels
{
    public class DisplayClientViewModel : ObservableObject
    {
        private IPopupService PopupService => EasyIoc.IocContainer.Default.Resolve<IPopupService>();
        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();

        private Client _client;
        public Client Client
        {
            get { return _client; }
            protected set { Set(() => Client, ref _client, value); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { Set(() => LastName, ref _lastName, value); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { Set(() => FirstName, ref _firstName, value); }
        }

        private DateTime? _birthDate;
        public DateTime? BirthDate
        {
            get { return _birthDate; }
            set { Set(() => BirthDate, ref _birthDate, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { Set(() => Email, ref _email, value); }
        }

        private string _mobile;
        public string Mobile
        {
            get { return _mobile; }
            set { Set(() => Mobile, ref _mobile, value); }
        }

        private string _streetName;
        public string StreetName
        {
            get { return _streetName; }
            set { Set(() => StreetName, ref _streetName, value); }
        }

        private string _streetNumber;
        public string StreetNumber
        {
            get { return _streetNumber; }
            set { Set(() => StreetNumber, ref _streetNumber, value); }
        }

        private string _zipCode;
        public string ZipCode
        {
            get { return _zipCode; }
            set { Set(() => ZipCode, ref _zipCode, value); }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set { Set(() => City, ref _city, value); }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { Set(() => Comment, ref _comment, value); }
        }

        // Create once then each item is selected/unselected in Initialize/UI
        private List<ClientCategoryModel> _categories;
        public List<ClientCategoryModel> Categories
        {
            get { return _categories; }
            protected set { Set(() => Categories, ref _categories, value); }
        }

        #region Save

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand = _saveCommand ?? new RelayCommand(Save);

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(FirstName)
                || string.IsNullOrWhiteSpace(LastName))
                return; // TODO: inform user

            Client.LastName = LastName;
            Client.FirstName = FirstName;
            Client.BirthDate = BirthDate;
            Client.Email = Email;
            Client.Mobile = Mobile;
            Client.StreetName = StreetName;
            Client.StreetNumber = StreetNumber;
            Client.ZipCode = ZipCode;
            Client.City = City;
            Client.Comment = Comment;
            Client.Categories = Categories.Where(x => x.IsSelected).Select(x => x.Category).ToList();

            ClientBL.SaveClient(Client);

            //// Switch to search mode
            //Mediator.Default.Send(new SearchClientMessage());
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

        #region Add purchase

        private ICommand _addPurchaseCommand;
        public ICommand AddPurchaseCommand => _addPurchaseCommand = _addPurchaseCommand ?? new RelayCommand(DisplayAddPurchasePopup);

        private void DisplayAddPurchasePopup()
        {
            AddPurchaseViewModel vm = new AddPurchaseViewModel(AddPurchase);
            PopupService.DisplayModal(vm, "Ajout achat");
        }

        private void AddPurchase(decimal amount)
        {
            Purchase purchase = new Purchase
            {
                Amount = amount,
                Date = DateTime.Now
            };
            ClientBL.SavePurchase(Client, purchase);
        }

        #endregion

        #region Voucher

        private ICommand _createVoucherCommand;
        public ICommand CreateVoucherCommand => _createVoucherCommand = _createVoucherCommand ?? new RelayCommand(CreateVoucher);

        private void CreateVoucher()
        {
            Client.LastVoucherDate = DateTime.Now;
            ClientBL.SaveClient(Client);
        }

        #endregion

        public DisplayClientViewModel()
        {
            Categories = Enum.GetValues(typeof(ClientCategories)).OfType<ClientCategories>().Select(c => new ClientCategoryModel
            {
                Category = c,
                IsSelected = false
            }).ToList();
        }

        public void Initialize(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            Client = client;

            LastName = client.LastName;
            FirstName = client.FirstName;
            BirthDate = client.BirthDate;
            Email = client.Email;
            Mobile = client.Mobile;
            StreetName = client.StreetName;
            StreetNumber = client.StreetNumber;
            ZipCode = client.ZipCode;
            City = client.City;
            Comment = client.Comment;
            foreach (ClientCategoryModel categoryModel in Categories)
                categoryModel.IsSelected = client.Categories?.Contains(categoryModel.Category) == true;
        }
    }

    public class DisplayDisplayClientViewModelDesignData : DisplayClientViewModel
    {
        public DisplayDisplayClientViewModelDesignData()
        {
            Client client = new Client
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
            Initialize(client);
        }
    }
}
