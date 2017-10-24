using EasyMVVM;
using Loyalty.App.Messages;
using Loyalty.App.ViewModels;
using LoyaltyCard.Domain;

namespace Loyalty.App
{
    public enum Modes
    {
        Search,
        Display
    }

    public class MainViewModel : ObservableObject
    {
        public DisplayClientViewModel DisplayClientViewModel { get; protected set; }
        public SearchClientViewModel SearchClientViewModel { get; protected set; }

        private Modes _mode;
        public Modes Mode
        {
            get { return _mode; }
            set { Set(() => Mode, ref _mode, value); }
        }

        public MainViewModel()
        {
            DisplayClientViewModel = new DisplayClientViewModel();
            SearchClientViewModel = new SearchClientViewModel();

            Mode = Modes.Search;

            Mediator.Default.Register<CreateClientMessage>(this, HandleCreateClientMessage);
            Mediator.Default.Register<DisplayClientMessage>(this, HandleDisplayClientMessage);
            Mediator.Default.Register<SearchClientMessage>(this, HandleSearchClientMessage);
        }

        private void HandleSearchClientMessage(SearchClientMessage switchModeMessage)
        {
            Mode = Modes.Search;
        }

        private void HandleDisplayClientMessage(DisplayClientMessage displayClientMessage)
        {
            DisplayClientViewModel.Initialize(displayClientMessage.Client);
            Mode = Modes.Display;
        }

        private void HandleCreateClientMessage(CreateClientMessage createClientMessage)
        {
            Client client = new Client
            {
                LastName = createClientMessage.LastNameFilter,
                FirstName = createClientMessage.FirstNameFilter
            };
            DisplayClientViewModel.Initialize(client);
            Mode = Modes.Display;
        }
    }

    public class MainViewModelDesignData : MainViewModel
    {
        public MainViewModelDesignData()
        {
            DisplayClientViewModel = new DisplayDisplayClientViewModelDesignData();
            SearchClientViewModel = new SearchClientViewModelDesignData();
        }
    }
}
