using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
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

        public IInputElement ElementToFocusOnActivation => LastNameTextBox;

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
