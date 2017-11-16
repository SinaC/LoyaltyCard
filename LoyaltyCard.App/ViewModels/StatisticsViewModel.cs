using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EasyMVVM;
using LiveCharts;
using LiveCharts.Wpf;
using LoyaltyCard.App.Messages;
using LoyaltyCard.Common;
using LoyaltyCard.Common.Extensions;
using LoyaltyCard.Domain;
using LoyaltyCard.IBusiness;

namespace LoyaltyCard.App.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private IStatisticsBL StatisticsBL => EasyIoc.IocContainer.Default.Resolve<IStatisticsBL>();

        protected Func<ChartPoint, string> GenericChartLabelPercentagePointFunction => chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})";

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
            protected set
            {
                if (Set(() => WeekBestClient, ref _weekBestClient, value))
                    RaisePropertyChanged(() => IsWeekBestClientFound);
            }
        }

        private decimal? _weekBestClientTotal;
        public decimal? WeekBestClientTotal
        {
            get { return _weekBestClientTotal; }
            protected set { Set(() => WeekBestClientTotal, ref _weekBestClientTotal, value); }
        }

        public bool IsWeekBestClientFound => WeekBestClient != null;

        private async Task SearchWeekBestClientAsync()
        {
            DateTime weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime weekEnd = weekStart.AddDays(7).AddSeconds(-1);

            BestClient bestClient = await AsyncFake.CallAsync(StatisticsBL, x => x.GetBestClientInPeriod(weekStart, weekEnd));

            WeekBestClient = bestClient?.Client;
            WeekBestClientTotal = bestClient?.Amount;
        }

        #endregion

        #region Client by age range

        private SeriesCollection _clientByAgeRangeSeries;
        public SeriesCollection ClientByAgeRangeSeries
        {
            get { return _clientByAgeRangeSeries; }
            protected set { Set(() => ClientByAgeRangeSeries, ref _clientByAgeRangeSeries, value); }
        }

        private async Task CountClientByAgeRangeAsync()
        {
            var clientCountByAgeCategories = await AsyncFake.CallAsync(StatisticsBL, x => x.GetClientCountByAgeCategory());

            SeriesCollection collection = new SeriesCollection();
            foreach (var data in clientCountByAgeCategories.OrderBy(x => x.Key))
            {
                collection.Add(new PieSeries
                {
                    Title = data.Key.DisplayName(),
                    Values = new ChartValues<int> { data.Value },
                    DataLabels = true,
                    LabelPoint = GenericChartLabelPercentagePointFunction
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

        private async Task CountClientBySexAsync()
        {
            var clientCountBySex = await AsyncFake.CallAsync(StatisticsBL, x => x.GetClientCountBySex());

            SeriesCollection collection = new SeriesCollection();
            foreach (var data in clientCountBySex)
            {
                collection.Add(new PieSeries
                {
                    Title = data.Key.DisplayName(),
                    Values = new ChartValues<int> { data.Value },
                    DataLabels = true,
                    LabelPoint = GenericChartLabelPercentagePointFunction,
                });
            }
            ClientBySexSeries = collection;
        }

        #endregion

        #region Average amount/age

        private SeriesCollection _averageAmountByAgeSeries;
        public SeriesCollection AverageAmountByAgeSeries
        {
            get { return _averageAmountByAgeSeries; }
            protected set { Set(() => AverageAmountByAgeSeries, ref _averageAmountByAgeSeries, value); }
        }

        public List<string> AverageAmountByAgeLabels { get; protected set; }

        public Func<double, string> AverageAmountByAgeFormatter => x => x.ToString("C2");

        private async Task CountAverageAmountByAgeAsync()
        {
            var averageAmountByAgeCategories = (await AsyncFake.CallAsync(StatisticsBL, x => x.GetClientAverageAmountByAgeCategory()))
                .OrderBy(x => x.Key);
            SeriesCollection collection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Montant moyen",
                    Values = new ChartValues<decimal>(averageAmountByAgeCategories.Select(x => x.Value))
                }
            };
            AverageAmountByAgeLabels = averageAmountByAgeCategories.Select(x => x.Key.DisplayName()).ToList();
            AverageAmountByAgeSeries = collection;
        }

        #endregion

        public void Refresh()
        {
            RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                IsBusy = true;

                await SearchWeekBestClientAsync();
                await CountClientByAgeRangeAsync();
                await CountClientBySexAsync();
                await CountAverageAmountByAgeAsync();
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
                    LabelPoint = GenericChartLabelPercentagePointFunction
                },
                new PieSeries
                {
                    Title = "Charles",
                    Values = new ChartValues<int>
                    {
                        4
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartLabelPercentagePointFunction
                },
                new PieSeries
                {
                    Title = "Frida",
                    Values = new ChartValues<int>
                    {
                        6
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartLabelPercentagePointFunction
                },
                new PieSeries
                {
                    Title = "Frederic",
                    Values = new ChartValues<int>
                    {
                        2
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartLabelPercentagePointFunction
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
                    LabelPoint = GenericChartLabelPercentagePointFunction
                },
                new PieSeries
                {
                    Title ="Femme",
                    Values = new ChartValues<int>
                    {
                        12
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartLabelPercentagePointFunction
                },
                new PieSeries
                {
                    Title ="Non-spécifié",
                    Values = new ChartValues<int>
                    {
                        1
                    },
                    DataLabels = true,
                    LabelPoint = GenericChartLabelPercentagePointFunction
                }
            };

            AverageAmountByAgeSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Montant",
                    Values = new ChartValues<decimal> { 10, 200, 30, 40}
                }
            };
            AverageAmountByAgeLabels = new List<string> { "10-", "20-29", "40-49", "70+"};
        }
    }
}
