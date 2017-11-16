using System;
using System.Threading.Tasks;
using EasyMVVM;
using LoyaltyCard.App.Messages;
using LoyaltyCard.Common;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;

namespace LoyaltyCard.App.ViewModels
{
    public class FooterViewModel : ViewModelBase
    {
        private IStatisticsBL StatisticsBL => EasyIoc.IocContainer.Default.Resolve<IStatisticsBL>();

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
            Mediator.Default.Register<SaveClientMessage>(this, async x => await RefreshAsync());

            RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                IsBusy = true;

                FooterInformations infos = await AsyncFake.CallAsync(StatisticsBL, x => x.GetFooterInformations());

                TotalClientCount = infos.TotalClientCount;
                TotalNewClientCount = infos.TotalNewClientCount;
                DaySales = infos.DaySales;
                WeekSales = infos.WeekSales;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            finally
            {
                IsBusy = false;
            }
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
