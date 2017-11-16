using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using LoyaltyCard.App.Interfaces;
using Xceed.Wpf.AvalonDock.Controls;

namespace LoyaltyCard.App.Views
{
    /// <summary>
    /// Interaction logic for DisplayClientView.xaml
    /// </summary>
    public partial class DisplayClientView : UserControl, IFocusInputElementOnActivation
    {
        public DisplayClientView()
        {
            InitializeComponent();

            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/855d5127-e66c-47b6-ae0a-744a203c9096/no-gotfocus-event-for-wpf-datepickertextbox?forum=wpf
            // GotFocus event is handled internally by DatePicker
            BirthDatePicker.AddHandler(GotFocusEvent, new RoutedEventHandler(DatePicker_OnGotFocus), true);
        }

        private void DatePicker_OnGotFocus(object sender, RoutedEventArgs e)
        {
            DatePicker @this = sender as DatePicker;
            TextBox partTextBox = @this?.FindVisualChildren<TextBox>().FirstOrDefault(x => x.Name == "PART_TextBox");
            if (partTextBox == null)
                return;
            Dispatcher.BeginInvoke((Action)delegate
            {
                Keyboard.Focus(partTextBox);
                partTextBox.SelectAll();
            }, DispatcherPriority.Render);
        }

        public IInputElement ElementToFocusOnActivation => LastNameTextBox;
    }
}
