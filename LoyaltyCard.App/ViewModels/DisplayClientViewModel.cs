using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EasyMVVM;
using LoyaltyCard.App.Messages;
using LoyaltyCard.App.Models;
using LoyaltyCard.App.ViewModels.Popups;
using LoyaltyCard.Common;
using LoyaltyCard.IBusiness;
using LoyaltyCard.Domain;
using LoyaltyCard.Services.Popup;

namespace LoyaltyCard.App.ViewModels
{
    public class DisplayClientViewModel : ViewModelBase
    {
        private IPopupService PopupService => EasyIoc.IocContainer.Default.Resolve<IPopupService>();
        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();
        private IPurchaseBL PurchaseBL => EasyIoc.IocContainer.Default.Resolve<IPurchaseBL>();
        private IVoucherBL VoucherBL => EasyIoc.IocContainer.Default.Resolve<IVoucherBL>();
        private IGeoBL GeoBL => EasyIoc.IocContainer.Default.Resolve<IGeoBL>();
        private IMailSender.IMailSender MailSenderBL => EasyIoc.IocContainer.Default.Resolve<IMailSender.IMailSender>();

        private bool _isUnsavedNewClient;
        private bool _automaticCitySearch;

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
            set
            {
                if (Set(() => ZipCode, ref _zipCode, value) && _automaticCitySearch)
                {
                    string city = GeoBL.GetCityFromZip(ZipCode);
                    if (!string.IsNullOrWhiteSpace(city))
                        City = city;
                }
            }
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

        #region Sex

        private Sex _sex;
        public Sex Sex
        {
            get { return _sex; }
            set { Set(() => Sex, ref _sex, value); }
        }

        #endregion

        // Create once then each item is selected/unselected in Initialize/UI
        private List<ClientCategoryModel> _categories;
        public List<ClientCategoryModel> Categories
        {
            get { return _categories; }
            protected set { Set(() => Categories, ref _categories, value); }
        }

        #region Save

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand = _saveCommand ?? new RelayCommand(SaveAndSwitchToSearch);

        private void SaveAndSwitchToSearch()
        {
           SaveClient(Client);

            // Switch to search mode
            Mediator.Default.Send(new SwitchToSearchClientMessage());
        }

        #endregion

        #region Cancel

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand = _cancelCommand ?? new RelayCommand(Cancel);

        private void Cancel()
        {
            // Switch to search mode
            Mediator.Default.Send(new SwitchToSearchClientMessage());
        }

        #endregion

        #region Add purchase

        private ICommand _addPurchaseCommand;
        public ICommand AddPurchaseCommand => _addPurchaseCommand = _addPurchaseCommand ?? new RelayCommand(DisplayAddPurchasePopup);

        private void DisplayAddPurchasePopup()
        {
            AddPurchaseViewModel vm = new AddPurchaseViewModel(Client.LastVoucherIssueDate, Client.OldestActiveVoucher, AddPurchase);
            PopupService.DisplayModal(vm, "Ajout achat");
        }

        private void AddPurchase(decimal amount, bool collectVoucher, DateTime when)
        {
            if (_isUnsavedNewClient)
                SaveClient(Client);

            Voucher oldestActiveVoucher = null;
            if (collectVoucher)
                oldestActiveVoucher = Client.OldestActiveVoucher;
            Purchase purchase = new Purchase
            {
                ClientId = Client.Id,
                Amount = amount,
                Date = when,
                IsPurchaseDeletable = true,
            };
            if (collectVoucher)
                purchase.VoucherId = oldestActiveVoucher.Id;
            // Add purchase
            Client.Purchases = Client.Purchases ?? new ObservableCollection<Purchase>();
            Client.Purchases.Add(purchase);
            Client.PurchaseModified();
            // Save purchase
            PurchaseBL.SavePurchase(purchase); // !! this will add purchase to client if not already added
            // Voucher
            if (collectVoucher)
            {
                oldestActiveVoucher.CollectDate = DateTime.Now;
                VoucherBL.SaveVoucher(oldestActiveVoucher);
                Client.VoucherModified();
            }
        }

        #endregion

        #region Delete purchase

        private ICommand _deletePurchaseCommand;
        public ICommand DeletePurchaseCommand => _deletePurchaseCommand = _deletePurchaseCommand ?? new RelayCommand<Purchase>(DeletePurchase);

        private void DeletePurchase(Purchase purchase)
        {
            PopupService.DisplayQuestion("Suppression d'achat", "Etes-vous certain de vouloir supprimer cet achat?", QuestionActionButton.Yes(() => DeletePurchaseConfirmed(purchase)), QuestionActionButton.No());
        }

        private void DeletePurchaseConfirmed(Purchase purchase)
        {
            if (_isUnsavedNewClient)
                SaveClient(Client);

            // Reactivate voucher if needed
            if (purchase.VoucherId.HasValue)
            {
                Voucher voucher = Client.Vouchers.FirstOrDefault(x => x.Id == purchase.VoucherId.Value);
                if (voucher != null)
                    voucher.CollectDate = null;
                VoucherBL.SaveVoucher(voucher);
                Client.VoucherModified();
            }
            // Delete purchase
            PurchaseBL.DeletePurchase(purchase); // !! this will remove purchase from client
            // Remove purchase
            Client.Purchases.Remove(purchase);
            Client.PurchaseModified();
        }

        #endregion

        #region Voucher

        private ICommand _createVoucherCommand;
        public ICommand CreateVoucherCommand => _createVoucherCommand = _createVoucherCommand ?? new RelayCommand(AskVoucherCreationConfirmation, () => !string.IsNullOrWhiteSpace(Email) && Client.TotalSinceLastVoucher > 0);

        private void AskVoucherCreationConfirmation()
        {
            PopupService.DisplayQuestion("Envoi d'un bon d'achat", 
                "Etes-vous certain(e) de vouloir envoyer un bon d'achat par mail ?", 
                new QuestionActionButton
                {
                    Caption = "Oui",
                    ClickCallback = async () => await CreateVoucherAsync()
                    //ClickCallback =() => CreateVoucherAsync()
                },
                new QuestionActionButton
                {
                    Caption = "Non"
                });
        }

        private async Task CreateVoucherAsync()
        {
            // Send mail if client has an email
            try
            {
                IsBusy = true;

                decimal percentage = 20;
                DateTime maxValidity = DateTime.Today.AddMonths(1);

                bool byMail = false;
                if (!string.IsNullOrWhiteSpace(Email))
                {
                    await MailSenderBL.SendVoucherMailAsync(Email, FirstName, Sex, percentage, maxValidity);
                    byMail = true;
                }

                if (_isUnsavedNewClient)
                    SaveClient(Client);

                Voucher voucher = new Voucher
                {
                    ClientId = Client.Id,
                    IssueDate = DateTime.Now,
                    Percentage = percentage,
                    ValidityEndDate = maxValidity,
                };
                // Add voucher
                Client.Vouchers = Client.Vouchers ?? new ObservableCollection<Voucher>();
                Client.Vouchers.Add(voucher);
                Client.VoucherModified();
                // Save voucher
                VoucherBL.SaveVoucher(voucher); // !! this will add voucher to client if not already added

                // Purchases cannot be deleted anymore
                foreach (Purchase purchase in Client.Purchases)
                    purchase.IsPurchaseDeletable = false;

                if (byMail)
                    PopupService.DisplayQuestion("Envoi d'un bon d'achat", $"Le bon a bien été envoyé par mail à l'adresse {Email} et sera valable jusqu'au {maxValidity:dd/MM/yyyy}", QuestionActionButton.Ok());
                else
                    PopupService.DisplayQuestion("Envoi d'un bon d'achat", $"Un bon d'achat a été émis et sera valable jusqu'au {maxValidity:dd/MM/yyyy}", QuestionActionButton.Ok());
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                PopupService.DisplayError("Envoi d'un bon d'achat", $"Erreur lors de l'envoi du mail à {Email} (numéro de client {Client.ClientBusinessId})");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public DisplayClientViewModel()
        {
            _automaticCitySearch = false;

            Categories = Enum.GetValues(typeof(ClientCategories)).OfType<ClientCategories>().Select(c => new ClientCategoryModel
            {
                Category = c,
                IsSelected = false
            }).ToList();
        }

        public void Initialize(ClientSummary clientSummary)
        {
            _isUnsavedNewClient = false;
            if (clientSummary == null)
                throw new ArgumentNullException(nameof(clientSummary));

            LoadClientAsync(clientSummary.Id);
        }

        public void Initialize()
        {
            _isUnsavedNewClient = true;
            // Create new client
            Client = ClientBL.CreateClient();
        }

        private void SaveClient(Client client)
        {
            _isUnsavedNewClient = false;

            // Save input fields to client
            client.LastName = LastName;
            client.FirstName = FirstName;
            client.BirthDate = BirthDate;
            client.Email = Email;
            client.Mobile = Mobile;
            client.StreetName = StreetName;
            client.StreetNumber = StreetNumber;
            client.ZipCode = ZipCode;
            client.City = City;
            client.Comment = Comment;
            client.Sex = Sex;
            client.Categories = Categories.Where(x => x.IsSelected).Select(x => x.Category).ToList();

            ClientBL.SaveClient(client);

            //
            Mediator.Default.Send(new SaveClientMessage
            {
                Client = client
            });
        }

        private async Task LoadClientAsync(Guid id)
        {
            try
            {
                IsBusy = true;

                Client = await AsyncFake.CallAsync(ClientBL, x => x.GetClient(id));

                // Initialize IsPurchaseDeletable
                if (Client.Purchases != null)
                {
                    DateTime? lastVoucherIssueDate = Client.LastVoucherIssueDate;
                    if (lastVoucherIssueDate.HasValue)
                        foreach (Purchase purchase in Client.Purchases)
                            purchase.IsPurchaseDeletable = purchase.Date > lastVoucherIssueDate.Value;
                    else
                        foreach (Purchase purchase in Client.Purchases)
                            purchase.IsPurchaseDeletable = true;
                }

                // Initialize input fields
                _automaticCitySearch = false; // desactivate while filling client fields
                LastName = Client.LastName;
                FirstName = Client.FirstName;
                BirthDate = Client.BirthDate;
                Email = Client.Email;
                Mobile = Client.Mobile;
                StreetName = Client.StreetName;
                StreetNumber = Client.StreetNumber;
                ZipCode = Client.ZipCode;
                City = Client.City;
                Comment = Client.Comment;
                Sex = Client.Sex;
                foreach (ClientCategoryModel categoryModel in Categories)
                    categoryModel.IsSelected = Client.Categories?.Contains(categoryModel.Category) == true;
                _automaticCitySearch = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                PopupService.DisplayQuestion("Ouverture client", "Le client n'a pu être chargé.");
                Mediator.Default.Send(new SwitchToSearchClientMessage()); // Switch back to Search Client
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    public class DisplayClientViewModelDesignData : DisplayClientViewModel
    {
        public DisplayClientViewModelDesignData()
        {
            Client client = new Client
            {
                FirstName = "Pouet",
                LastName = "Brol",
                BirthDate = new DateTime(1999, 12, 31),
                Email = "pouet.brol@hotmail.com",
                Mobile = null,
                Purchases = new ObservableCollection<Purchase>(Enumerable.Range(1, 20).Select(x => new Purchase
                {
                    Amount = x * 10,
                    Date = DateTime.Now.AddDays(-x * 2)
                })),
                Vouchers = new ObservableCollection<Voucher>
                {
                    new Voucher
                    {
                        IssueDate = DateTime.Today.AddMonths(-1).AddDays(5),
                        ValidityEndDate = DateTime.Today.AddDays(5),
                        Percentage = 20,
                    }
                }
            };
            Client.PurchaseModified();
            Client.VoucherModified();

            Client = client;
        }
    }
}
