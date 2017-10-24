using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.Toolkit;

namespace Loyalty.App.Views
{
    /// <summary>
    /// Interaction logic for DisplayClientView.xaml
    /// </summary>
    public partial class DisplayClientView : UserControl
    {
        public DisplayClientView()
        {
            InitializeComponent();
        }

        private void DecimalUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // DecimalUpDown uses current culture and it seems fr-BE doesn't use . but , as decimal separator
            DecimalUpDown @this = sender as DecimalUpDown;
            if (@this == null)
                return;
            @this.Text = @this.Text.Replace('.', ',');
        }

        private void DecimalUpDown_OnGotFocus(object sender, RoutedEventArgs e)
        {
            // Crappy workaround because FocusManager.FocusedElement doesn't set Keyboard focus
            DecimalUpDown @this = sender as DecimalUpDown;
            TextBox partTextBox = @this?.FindVisualChildren<TextBox>().FirstOrDefault(x => x.Name == "PART_TextBox");
            if (partTextBox == null)
                return;
            Dispatcher.BeginInvoke((Action)delegate
            {
                Keyboard.Focus(partTextBox);
                partTextBox.SelectAll();
            }, DispatcherPriority.Render);
        }
    }
}
