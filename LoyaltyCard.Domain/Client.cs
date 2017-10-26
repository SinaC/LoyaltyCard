using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
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

        private DateTime? _birthDate;
        [DataMember]
        public DateTime? BirthDate
        {
            get { return _birthDate; }
            set
            {
                if (_birthDate != value)
                {
                    _birthDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged("IsBirthDay");
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

        #region Street name

        private string _streetName;
        [DataMember]
        public string StreetName
        {
            get { return _streetName; }
            set
            {
                if (_streetName != value)
                {
                    _streetName = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Street number

        private string _streetNumber;
        [DataMember]
        public string StreetNumber
        {
            get { return _streetNumber; }
            set
            {
                if (_streetNumber != value)
                {
                    _streetNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Zip

        private string _zipCode;
        [DataMember]
        public string ZipCode
        {
            get { return _zipCode; }
            set
            {
                if (_zipCode != value)
                {
                    _zipCode = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region City

        private string _city;
        [DataMember]
        public string City
        {
            get { return _city; }
            set
            {
                if (_city != value)
                {
                    _city = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Comment

        private string _comment;
        [DataMember]
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Last voucher date

        private DateTime? _lastVoucherDate;
        [DataMember]
        public DateTime? LastVoucherDate
        {
            get { return _lastVoucherDate; }
            set
            {
                if (_lastVoucherDate != value)
                {
                    _lastVoucherDate = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Categories

        private List<ClientCategories> _categories;
        [DataMember]
        public List<ClientCategories> Categories
        {
            get { return _categories; }
            set
            {
                if (_categories != value)
                {
                    _categories = value;
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
        public bool IsBirthDay => BirthDate.HasValue && DateTime.Today.Month == BirthDate.Value.Month && DateTime.Today.Day == BirthDate.Value.Day; // TODO: 29th february =D
        public int? Age => BirthDate.HasValue ? DateTime.Today.Year - BirthDate.Value.Date.Year : (int?)null;
    }
}
