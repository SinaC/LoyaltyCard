﻿using LoyaltyCard.Common.Extensions;
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

        [DataMember]
        public DateTime? CreationDate { get; set; }

        [DataMember(Name = "ClientId")]
        public int ClientBusinessId { get; set; }

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
                    OnPropertyChanged("Age");
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

        #region Sex

        private Sex _sex;

        [DataMember]
        public Sex Sex
        {
            get { return _sex; }
            set
            {
                if (_sex != value)
                {
                    _sex = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        [DataMember]
        public DateTime? WelcomeMailDate { get; set; }

        [DataMember]
        public DateTime? LastBirthMailDate { get; set; }

        #region Vouchers

        private ObservableCollection<Voucher> _vouchers;

        [DataMember]
        public virtual ObservableCollection<Voucher> Vouchers
        {
            get { return _vouchers; }
            set
            {
                if (_vouchers != value)
                {
                    _vouchers = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public void VoucherModified() // TODO: should be called automatically when Vouchers collection is modified
        {
            OnPropertyChanged("OldestActiveVoucher");
            OnPropertyChanged("LastVoucherIssueDate");
            OnPropertyChanged("TotalSinceLastVoucher");
        }

        public Voucher OldestActiveVoucher => Vouchers?.Where(x => !x.CollectDate.HasValue && x.ValidityEndDate >= DateTime.Today).OrderBy(x => x.IssueDate).FirstOrDefault();
        public DateTime? LastVoucherIssueDate => Vouchers?.OrderByDescending(x => x.IssueDate).FirstOrDefault()?.IssueDate;
        //public Voucher LatestActiveVoucher => Vouchers?.Where(x => !x.CollectDate.HasValue && x.ValidityEndDate >= DateTime.Today).OrderByDescending(x => x.IssueDate).FirstOrDefault();

        #region Purchases

        private ObservableCollection<Purchase> _purchases;

        [DataMember]
        public virtual ObservableCollection<Purchase> Purchases
        {
            get { return _purchases; }
            set
            {
                if (_purchases != value)
                {
                    _purchases = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public void PurchaseModified() // TODO: should be called automatically when Purchases collection is modified
        {
            OnPropertyChanged("TotalPurchases");
            OnPropertyChanged("LastPurchase");
            OnPropertyChanged("TotalSinceLastVoucher");
        }

        public decimal? TotalPurchases => Purchases?.SumNullIfEmpty(x => x.Amount);

        public Purchase LastPurchase => Purchases?.OrderByDescending(x => x.Date).FirstOrDefault();

        public decimal? TotalSinceLastVoucher
        {
            get
            {
                DateTime? lastVoucherIssueDate = LastVoucherIssueDate;
                return lastVoucherIssueDate.HasValue
                    ? Purchases?.Where(x => x.Date > lastVoucherIssueDate.Value).SumNullIfEmpty(x => x.Amount)
                    : TotalPurchases;
            }
        }

        public bool IsBirthDay => BirthDate.HasValue && ((DateTime.Today.Month == BirthDate.Value.Month && DateTime.Today.Day == BirthDate.Value.Day) || (BirthDate.Value.Month == 2 && BirthDate.Value.Day == 29 && DateTime.Today.Month == 2 && DateTime.Today.Day == 28));

        public int? Age
        {
            get
            {
                //https://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
                if (!BirthDate.HasValue)
                    return null;
                DateTime today = DateTime.Today;
                int age = DateTime.Today.Year - BirthDate.Value.Year;

                if (today.Month < BirthDate.Value.Month || (today.Month == BirthDate.Value.Month && today.Day < BirthDate.Value.Day))
                    age--;

                return age;
            }
        }

        public AgeCategories AgeCategory
        {
            get
            {
                int? clientAge = Age;
                if (!clientAge.HasValue)
                    return AgeCategories.Undefined;
                if (clientAge <= 10)
                    return AgeCategories.LessThan10;
                if (clientAge > 10 && clientAge <= 15)
                    return AgeCategories.Between11And15;
                if (clientAge > 15 && clientAge <= 20)
                    return AgeCategories.Between16And20;
                if (clientAge > 20 && clientAge <= 30)
                    return AgeCategories.Between20And30;
                if (clientAge > 30 && clientAge <= 40)
                    return AgeCategories.Between31And40;
                if (clientAge > 40 && clientAge <= 50)
                    return AgeCategories.Between41And50;
                if (clientAge > 51 && clientAge <= 60)
                    return AgeCategories.Between51And60;
                if (clientAge > 61 && clientAge <= 70)
                    return AgeCategories.Between61And70;
                return AgeCategories.MoreThan71;
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
