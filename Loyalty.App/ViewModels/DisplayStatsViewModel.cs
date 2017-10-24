using System.Windows.Input;
using EasyMVVM;
using Loyalty.App.Messages;

namespace Loyalty.App.ViewModels
{
    public class DisplayStatsViewModel : ObservableObject
    {
        // TODO: gather stats from DB

        #region Cancel

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand = _cancelCommand ?? new RelayCommand(Cancel);

        private void Cancel()
        {
            // Switch to search mode
            Mediator.Default.Send(new SearchClientMessage());
        }

        #endregion
    }

    public class DisplayStatsViewModelDesignData : DisplayStatsViewModel
    {
    }
}
