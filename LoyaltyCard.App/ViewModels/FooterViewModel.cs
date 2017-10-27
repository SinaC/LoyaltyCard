using EasyMVVM;

namespace LoyaltyCard.App.ViewModels
{
    public class FooterViewModel : ObservableObject
    {
        private int _totalClientCount;
        public int TotalClientCount
        {
            get { return _totalClientCount; }
            protected set { Set(() => TotalClientCount, ref _totalClientCount, value); }
        }

        private int _totalNewClientCount;
        public int TotalNewClientCount
        {
            get { return _totalNewClientCount; }
            protected set { Set(() => TotalNewClientCount, ref _totalNewClientCount, value); }
        }

        private decimal _daySales;
        public decimal DaySales
        {
            get { return _daySales; }
            protected set { Set(() => DaySales, ref _daySales, value); }
        }

        private decimal _weekSales;
        public decimal WeekSales
        {
            get { return _weekSales; }
            protected set { Set(() => WeekSales, ref _weekSales, value); }
        }

        public FooterViewModel()
        {
            // TODO: register to NewClient event and NewSale event
        }
    }

    public class FooterViewModelDesignData : FooterViewModel
    {
        public FooterViewModelDesignData()
        {
            TotalClientCount = 15;
            TotalNewClientCount = 3;
            DaySales = 150;
            WeekSales = 720;
        }
    }
}
