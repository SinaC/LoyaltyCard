using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract]
    public class Client : INotifyPropertyChanged
    {
        [DataMember]
        public Guid Id { get; set; }

        #region LastName

        private string _lastName;
        [DataMember]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region FirstName

        private string _firstName;
        [DataMember]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region BirthDate

        private DateTime _birthDate;
        [DataMember]
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set
            {
                if (_birthDate != value)
                {
                    _birthDate = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Email

        private string _email;
        [DataMember]
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Mobile

        private string _mobile;
        [DataMember]
        public string Mobile
        {
            get { return _mobile; }
            set
            {
                if (_mobile != value)
                {
                    _mobile = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        [DataMember]
        public virtual ObservableCollection<Purchase> Purchases { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public decimal? TotalPurchases => Purchases?.Sum(x => x.Amount); // TODO: should be updated when a purchase is added
        public Purchase LastPurchase => Purchases?.OrderByDescending(x => x.Date).FirstOrDefault(); // TODO: should be updated when a purchase is added
    }
}
