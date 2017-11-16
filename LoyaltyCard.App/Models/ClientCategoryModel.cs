using EasyMVVM;
using LoyaltyCard.Common.Extensions;
using LoyaltyCard.Domain;

namespace LoyaltyCard.App.Models
{
    public class ClientCategoryModel : ObservableObject
    {
        public string DisplayName => Category.DisplayName();

        private ClientCategories _category;
        public ClientCategories Category
        {
            get { return _category; }
            set
            {
                if (Set(() => Category, ref _category, value))
                    RaisePropertyChanged(() => DisplayName);
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }
    }
}
