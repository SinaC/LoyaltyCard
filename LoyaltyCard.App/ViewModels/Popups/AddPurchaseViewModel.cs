using System;
using System.Windows.Input;
using EasyMVVM;
using LoyaltyCard.App.Views.Popups;
using LoyaltyCard.Services.Popup;

namespace LoyaltyCard.App.ViewModels.Popups
{
    [PopupAssociatedView(typeof(AddPurchaseView))]
    public class AddPurchaseViewModel : ObservableObject
    {
        private IPopupService PopupService => EasyIoc.IocContainer.Default.Resolve<IPopupService>();

        private readonly Action<decimal> _okAction;

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
            PopupService?.Close(this);

            if (Amount > 0)
                _okAction?.Invoke(Amount);
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand = _cancelCommand ?? new RelayCommand(Cancel);
        private void Cancel()
        {
            PopupService?.Close(this);
        }

        public AddPurchaseViewModel(Action<decimal> okAction)
        {
            _okAction = okAction;
        }
    }

    public class AddPurchaseViewModelDesignData : AddPurchaseViewModel
    {
        public AddPurchaseViewModelDesignData() : base(d => { })
        {
        }
    }
}
