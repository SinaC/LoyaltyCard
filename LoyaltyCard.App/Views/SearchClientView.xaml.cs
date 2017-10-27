using System.Windows;
using System.Windows.Controls;
using LoyaltyCard.App.Interfaces;

namespace LoyaltyCard.App.Views
{
    /// <summary>
    /// Interaction logic for SearchClientView.xaml
    /// </summary>
    public partial class SearchClientView : UserControl, IFocusInputElementOnActivation
    {
        public SearchClientView()
        {
            InitializeComponent();
        }

        public IInputElement ElementToFocus => LastNameTextBox;
    }
}
