using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using EasyMVVM;
using LiveCharts;
using LiveCharts.Wpf;
using LoyaltyCard.App.Helpers;
using LoyaltyCard.App.Messages;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;

namespace LoyaltyCard.App.ViewModels
{
    public class StatisticsViewModel : ObservableObject
    {
        protected Func<ChartPoint, string> GenericChartPointFunction => chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})";

        private IClientBL ClientBL => EasyIoc.IocContainer.Default.Resolve<IClientBL>();

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

        private Func<Client, DateTime, DateTime, decimal?> RangePurchaseFunc => (client, from, till) => client.Purchases?.Where(p => p.Date >= from && p.Date <= till).Sum(p => p.Amount);

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

        #region Client by age range

        private SeriesCollection _clientByAgeRangeSeries;
        public SeriesCollection ClientByAgeRangeSeries
        {
            get { return _clientByAgeRangeSeries; }
            protected set { Set(() => ClientByAgeRangeSeries, ref _clientByAgeRangeSeries, value); }
        }

        private enum AgeCategory
        {
            [Description("10-")]
            LessThan10,
            [Description("11->15")]
            Between11And15,
            [Description("16->20")]
            Between16And20,
            [Description("21->30")]
            Between20And30,
            [Description("31->40")]
            Between31And40,
            [Description("41->50")]
            Between41And50,
            [Description("51->60")]
            Between51And60,
            [Description("61->70")]
            Between61And70,
            [Description("70+")]
            MoreThan71,
        }

        private AgeCategory GetAgeCategory(Client client)
        {
            if (client.Age <= 10)
                return AgeCategory.LessThan10;
            if (client.Age > 10 && client.Age <= 15)
                return AgeCategory.Between11And15;
            if (client.Age > 15 && client.Age <= 20)
                return AgeCategory.Between16And20;
            if (client.Age > 20 && client.Age <= 30)
                return AgeCategory.Between20And30;
            if (client.Age > 30 && client.Age <= 40)
                return AgeCategory.Between31And40;
            if (client.Age > 40 && client.Age <= 50)
                return AgeCategory.Between41And50;
            if (client.Age > 51 && client.Age <= 60)
                return AgeCategory.Between51And60;
            if (client.Age > 61 && client.Age <= 70)
                return AgeCategory.Between61And70;
            return AgeCategory.MoreThan71;
        }

        private void CountClientByAgeRange()
        {
            var clientCountByAgeCategory = _clients.Select(client =>
                new
                {
                    client,
                    category = GetAgeCategory(client)
                })
                .GroupBy(x => x.category)
                .Select(g => new
                {
                    category = g.Key,
                    count = g.Count()
                })
                .OrderBy(x => x.category);
            SeriesCollection collection = new SeriesCollection();
            foreach (var data in clientCountByAgeCategory)
            {
                collection.Add(new PieSeries
                {
                    Title = data.category.DisplayName(),
                    Values = new ChartValues<int> { data.count },
                    DataLabels = true,
                    LabelPoint = GenericChartPointFunction
                });
            }
            ClientByAgeRangeSeries = collection;
        }

        #endregion

        #region Client by sex

        private SeriesCollection _clientBySexSeries;
        public SeriesCollection ClientBySexSeries
        {
            get { return _clientBySexSeries; }
            protected set { Set(() => ClientBySexSeries, ref _clientBySexSeries, value); }
        }

        private void CountClientBySex()
        {
            var clientCountBySex = _clients.Select(client =>
                new
                {
                    client,
                    sex = client.Sex
                })
                .GroupBy(x => x.sex)
                .Select(g => new
                {
                    sex = g.Key,
                    count = g.Count()
                });
            SeriesCollection collection = new SeriesCollection();
            foreach (var data in clientCountBySex)
            {
                collection.Add(new PieSeries
                {
                    Title = data.sex.DisplayName(),
                    Values = new ChartValues<int> { data.count },
                    DataLabels = true,
                    LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})",
                });
            }
            ClientBySexSeries = collection;
        }

        #endregion

        public void Initialize()
        {
            _now = DateTime.Now;
            _clients = ClientBL.GetClients();

            SearchWeekBestClient();
            CountClientByAgeRange();
            CountClientBySex();
        }
    }

    public class StatisticsViewModelDesignData : StatisticsViewModel
    {
        public StatisticsViewModelDesignData()
        {
            WeekBestClientTotal = 150;
            WeekBestClient = new Client
            {
                FirstName = "Pouet",
                LastName = "Tagada",
                Sex = Sex.Female
            };

            ClientByAgeRangeSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Maria",
                    Values = new ChartValues<int>
                    {
                        3
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartPointFunction
                },
                new PieSeries
                {
                    Title = "Charles",
                    Values = new ChartValues<int>
                    {
                        4
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartPointFunction
                },
                new PieSeries
                {
                    Title = "Frida",
                    Values = new ChartValues<int>
                    {
                        6
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartPointFunction
                },
                new PieSeries
                {
                    Title = "Frederic",
                    Values = new ChartValues<int>
                    {
                        2
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartPointFunction
                }
            };

            ClientBySexSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title ="Homme",
                    Values = new ChartValues<int>
                    {
                        10
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartPointFunction
                },
                new PieSeries
                {
                    Title ="Femme",
                    Values = new ChartValues<int>
                    {
                        12
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartPointFunction
                },
                new PieSeries
                {
                    Title ="Non-spécifié",
                    Values = new ChartValues<int>
                    {
                        1
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartPointFunction
                }
            };
        }
    }
}
