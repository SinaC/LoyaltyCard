using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public class Voucher : INotifyPropertyChanged
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid ClientId { get; set; }

        [DataMember]
        public DateTime IssueDate { get; set; }

        [DataMember]
        public decimal Percentage { get; set; }

        [DataMember]
        public DateTime ValidityEndDate { get; set; }

        #region Collect date

        private DateTime? _collectDate;

        [DataMember]
        public DateTime? CollectDate
        {
            get { return _collectDate; }
            set {
                if (_collectDate != value)
                {
                    _collectDate = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        //[DataMember]
        //public virtual Client Client { get; set; }

        public string PercentageDisplay => $"{Percentage / 100:P0}";

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
