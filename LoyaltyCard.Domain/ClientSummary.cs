using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LoyaltyCard.Domain
{
    [DataContract(Namespace = "")]
    public class ClientSummary : INotifyPropertyChanged
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int ClientBusinessId { get; set; }

        #region Last name

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

        #region First name

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

        #region Total since last voucher

        private decimal? _totalSinceLastVoucher;

        [DataMember]
        public decimal? TotalSinceLastVoucher
        {
            get { return _totalSinceLastVoucher; }
            set
            {
                if (_totalSinceLastVoucher != value)
                {
                    _totalSinceLastVoucher = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Total

        private decimal? _total;

        [DataMember]
        public decimal? Total
        {
            get { return _total; }
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Last purchase

        private Purchase _lastPurchase;

        [DataMember]
        public Purchase LastPurchase
        {
            get { return _lastPurchase; }
            set {
                if (_lastPurchase != value)
                {
                    _lastPurchase = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Is birthday

        private bool _isBirthDay;

        [DataMember]
        public bool IsBirthDay
        {
            get { return _isBirthDay; }
            set
            {
                if (_isBirthDay != value)
                {
                    _isBirthDay = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Running voucher

        private Voucher _oldestActiveVoucher;

        [DataMember]
        public Voucher OldestActiveVoucher
        {
            get { return _oldestActiveVoucher; }
            set
            {
                if (_oldestActiveVoucher != value)
                {
                    _oldestActiveVoucher = value;
                    OnPropertyChanged();
                    OnPropertyChanged("HasActiveVoucher");
                }
            }
        }

        #endregion

        public bool HasActiveVoucher => OldestActiveVoucher != null;

        public ClientSummary()
        {
        }

        public ClientSummary(Client client)
        {
            Initialize(client);
        }

        public void Initialize(Client client)
        {
            Id = client.Id;
            ClientBusinessId = client.ClientBusinessId;
            FirstName = client.FirstName;
            LastName = client.LastName;
            TotalSinceLastVoucher = client.TotalSinceLastVoucher;
            Total = client.TotalPurchases;
            LastPurchase = client.LastPurchase;
            OldestActiveVoucher = client.OldestActiveVoucher;
            IsBirthDay = client.IsBirthDay;
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
