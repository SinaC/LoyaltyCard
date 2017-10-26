using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using EasyMVVM;
using LoyaltyCard.App.Helpers;
using LoyaltyCard.App.Messages;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;

namespace LoyaltyCard.App.ViewModels
{
    public class StatisticsViewModel : ObservableObject
    {
        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();

        private Func<Client, DateTime, DateTime, decimal?> RangePurchaseFunc => (client, from, till) => client.Purchases?.Where(p => p.Date >= from && p.Date <= till).Sum(p => p.Amount);

        private DateTime _now;
        private List<Client> _clients;

        #region Close

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand = _closeCommand ?? new RelayCommand(Close);

        private void Close()
        {
            // Switch to search mode
            Mediator.Default.Send(new SwitchToSearchClientMessage());
        }

        #endregion

        #region Week best client

        private Client _weekBestClient;
        public Client WeekBestClient
        {
            get { return _weekBestClient; }
            set { Set(() => WeekBestClient, ref _weekBestClient, value); }
        }

        private decimal? _weekBestClientTotal;
        public decimal? WeekBestClientTotal
        {
            get { return _weekBestClientTotal; }
            set { Set(() => WeekBestClientTotal, ref _weekBestClientTotal, value); }
        }


        private void SearchWeekBestClient()
        {
            DateTime weekStart = _now.AddDays(-(int) _now.DayOfWeek);
            DateTime weekEnd = weekStart.AddDays(7).AddSeconds(-1);

            var weekPurchaseByClient = _clients.Select(client =>
                new
                {
                    client,
                    total = RangePurchaseFunc(client, weekStart, weekEnd)
                })
                .WhereMax(x => x.total);
            if (weekPurchaseByClient != null)
            {
                WeekBestClient = weekPurchaseByClient.client;
                WeekBestClientTotal = weekPurchaseByClient.total;
            }
            else
            {
                WeekBestClient = null;
                WeekBestClientTotal = null;
            }
        }

        #endregion

        public void Initialize()
        {
            _now = DateTime.Now;
            _clients = ClientBL.GetClients();

            SearchWeekBestClient();
        }
    }

    public class StatisticsViewModelDesignData : StatisticsViewModel
    {
    }
}
