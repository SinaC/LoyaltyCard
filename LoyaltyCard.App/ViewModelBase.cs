using EasyMVVM;
using LoyaltyCard.Log;
using LoyaltyCard.Services.Popup;

namespace LoyaltyCard.App
{
    public class ViewModelBase : ObservableObject
    {
        protected ILog Logger => EasyIoc.IocContainer.Default.Resolve<ILog>();

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(() => IsBusy, ref _isBusy, value); }
        }

        protected void NotYetImplemented()
        {
            EasyIoc.IocContainer.Default.Resolve<IPopupService>().DisplayError("Non-disponible", "Cette fonctionnalité n'est pas encore disponible");
        }
    }
}
