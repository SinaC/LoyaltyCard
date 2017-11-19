using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public class Purchase : INotifyPropertyChanged
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid ClientId { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        //[DataMember]
        //public virtual Client Client { get; set; }

        // Computed fields
        private bool _isPurchaseDeletable;
        public bool IsPurchaseDeletable
        {
            get { return _isPurchaseDeletable; }
            set
            {
                if (_isPurchaseDeletable != value)
                {
                    _isPurchaseDeletable = value;
                    OnPropertyChanged();
                }
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
