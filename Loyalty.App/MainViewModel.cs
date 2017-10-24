using EasyMVVM;
using Loyalty.App.Messages;
using Loyalty.App.ViewModels;
using LoyaltyCard.Domain;

namespace Loyalty.App
{
    public enum Modes
    {
        Search,
        Encoding
    }

    public class MainViewModel : ObservableObject
    {
        public ClientEncodingViewModel ClientEncodingViewModel { get; protected set; }
        public SearchClientViewModel SearchClientViewModel { get; protected set; }

        private Modes _mode;
        public Modes Mode
        {
            get { return _mode; }
            set { Set(() => Mode, ref _mode, value); }
        }

        public MainViewModel()
        {
            ClientEncodingViewModel = new ClientEncodingViewModel();
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
            ClientEncodingViewModel.Initialize(displayClientMessage.Client);
            Mode = Modes.Encoding;
        }

        private void HandleCreateClientMessage(CreateClientMessage createClientMessage)
        {
            Client client = new Client
            {
                LastName = createClientMessage.LastNameFilter,
                FirstName = createClientMessage.FirstNameFilter
            };
            ClientEncodingViewModel.Initialize(client);
            Mode = Modes.Encoding;
        }
    }

    public class MainViewModelDesignData : MainViewModel
    {
        public MainViewModelDesignData()
        {
            ClientEncodingViewModel = new ClientEncodingViewModelDesignData();
            SearchClientViewModel = new SearchClientViewModelDesignData();
        }
    }
}
