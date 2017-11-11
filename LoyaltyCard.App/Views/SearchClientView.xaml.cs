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

        public IInputElement ElementToFocusOnActivation => FilterTextBox;

        //private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    Dispatcher.BeginInvoke((Action)delegate
        //    {
        //        ClientsDataGrid.SelectedIndex = 0;
        //        ClientsDataGrid.Focus();
        //        Keyboard.Focus(ClientsDataGrid);
        //    }, DispatcherPriority.Render);
        //}
    }
}
