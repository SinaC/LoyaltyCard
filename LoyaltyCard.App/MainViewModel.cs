﻿using EasyMVVM;
using LoyaltyCard.App.Messages;
using LoyaltyCard.App.ViewModels;
using LoyaltyCard.Domain;

namespace LoyaltyCard.App
{
    public enum Modes
    {
        Search,
        Display,
        Stats
    }

    public class MainViewModel : ObservableObject
    {
        public DisplayClientViewModel DisplayClientViewModel { get; protected set; }
        public SearchClientViewModel SearchClientViewModel { get; protected set; }
        public StatisticsViewModel StatisticsViewModel { get; protected set; }

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
            StatisticsViewModel = new StatisticsViewModel();

             Mode = Modes.Search;

            Mediator.Default.Register<CreateClientMessage>(this, HandleCreateClientMessage);
            Mediator.Default.Register<SwitchToDisplayClientMessage>(this, HandleSwitchToDisplayClientMessage);
            Mediator.Default.Register<SwitchToSearchClientMessage>(this, HandleSwitchToSearchClientMessage);
            Mediator.Default.Register< SwitchToStatisticsMessage>(this, HandleSwitchToStatisticsMessage);
        }

        private void HandleSwitchToStatisticsMessage(SwitchToStatisticsMessage switchToStatisticsMessage)
        {
            StatisticsViewModel.Initialize();
            Mode = Modes.Stats;
        }

        private void HandleSwitchToSearchClientMessage(SwitchToSearchClientMessage switchToSearchClientMessage)
        {
            Mode = Modes.Search;
        }

        private void HandleSwitchToDisplayClientMessage(SwitchToDisplayClientMessage switchToDisplayClientMessage)
        {
            DisplayClientViewModel.Initialize(switchToDisplayClientMessage.Client);
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
            DisplayClientViewModel = new DisplayClientViewModelDesignData();
            SearchClientViewModel = new SearchClientViewModelDesignData();
            StatisticsViewModel = new StatisticsViewModelDesignData();
        }
    }
}
