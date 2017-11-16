using EasyMVVM;
using LoyaltyCard.Log;

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
    }
}
