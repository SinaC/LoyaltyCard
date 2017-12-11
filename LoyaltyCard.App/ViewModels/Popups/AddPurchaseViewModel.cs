using System;
using System.Windows.Input;
using EasyMVVM;
using LoyaltyCard.App.Views.Popups;
using LoyaltyCard.Domain;
using LoyaltyCard.Services.Popup;

namespace LoyaltyCard.App.ViewModels.Popups
{
    [PopupAssociatedView(typeof(AddPurchaseView))]
    public class AddPurchaseViewModel : ObservableObject
    {
        private IPopupService PopupService => EasyIoc.IocContainer.Default.Resolve<IPopupService>();

        private readonly Action<decimal, bool, DateTime> _okAction;

        public bool HasActiveVoucher { get; protected set; }
        public string VoucherValue { get; protected set; }

        public DateTime? MinimumDate { get; set; }
        public DateTime MaximumDate { get; set; }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set { Set(() => SelectedDate, ref _selectedDate, value); }
        }

        private bool _isCollectingVoucher;
        public bool IsCollectingVoucher
        {
            get { return _isCollectingVoucher; }
            set { Set(() => IsCollectingVoucher, ref _isCollectingVoucher, value); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set { Set(() => Amount, ref _amount, value); }
        }

        private ICommand _okCommand;
        public ICommand OkCommand => _okCommand = _okCommand ?? new RelayCommand(Ok);
        private void Ok()
        {
            if (Amount > 0)
            {
                PopupService?.Close(this);

                if (Amount > 0)
                {
                    DateTime date = SelectedDate ?? DateTime.Now;
                    _okAction?.Invoke(Amount, IsCollectingVoucher, date);
                }
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand = _cancelCommand ?? new RelayCommand(Cancel);
        private void Cancel()
        {
            PopupService?.Close(this);
        }

        public AddPurchaseViewModel(DateTime? minDate, Voucher activeVoucher, Action<decimal, bool, DateTime> okAction)
        {
            MinimumDate = minDate;
            MaximumDate = DateTime.Today;
            HasActiveVoucher = activeVoucher != null;
            VoucherValue = activeVoucher?.PercentageDisplay;
            _okAction = okAction;
        }
    }

    public class AddPurchaseViewModelDesignData : AddPurchaseViewModel
    {
        public AddPurchaseViewModelDesignData() : base(null, null, (amount, collect, when) => { })
        {
        }
    }
}
