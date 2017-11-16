using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LoyaltyCard.App.Interfaces;

namespace LoyaltyCard.App.CustomControls
{
    public class ContentControlExt : ContentControl
    {

        // Set focus on first input element
        protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
        {
            base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);

            Dispatcher.BeginInvoke((Action)(() => {
                UserControl uc = FindVisualChild<UserControl>(this);
                IFocusInputElementOnActivation elementToFocus = uc as IFocusInputElementOnActivation;
                elementToFocus?.ElementToFocusOnActivation?.Focus();
            }), DispatcherPriority.Render);
        }

        // TODO: move to helpers
        public static T FindVisualChild<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    var visualChild = child as T;
                    if (visualChild != null)
                        return visualChild;
                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null)
                        return childItem;
                }
            }
            return null;
        }
    }
}
