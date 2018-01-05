using System;
using System.Windows;
using System.Windows.Controls;

namespace LoyaltyCard.App.Behaviours
{
    public static class WebBrowserUtility
    {
        #region BindableSource

        public static readonly DependencyProperty BindableSourceProperty = DependencyProperty.RegisterAttached(
            "BindableSource",
            typeof(string),
            typeof(WebBrowserUtility),
            new UIPropertyMetadata(null, OnBindableSourceChanged));

        public static string GetBindableSource(DependencyObject dependencyObject)
        {
            return (string) dependencyObject.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(BindableSourceProperty, value);
        }

        public static void OnBindableSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = dependencyObject as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = !string.IsNullOrEmpty(uri)
                    ? new Uri(uri)
                    : null;
            }
        }

        #endregion

        #region BindableBody

        public static readonly DependencyProperty BindableBodyProperty = DependencyProperty.RegisterAttached(
            "BindableBody",
            typeof(string),
            typeof(WebBrowserUtility),
            new PropertyMetadata(OnBindableBodyChanged));

        public static string GetBindableBody(DependencyObject dependencyObject)
        {
            return (string) dependencyObject.GetValue(BindableBodyProperty);
        }

        public static void SetBindableBody(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(BindableBodyProperty, value);
        }

        private static void OnBindableBodyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            if (webBrowser != null)
            {
                string body = e.NewValue as string;
                webBrowser.NavigateToString(!string.IsNullOrWhiteSpace(body)
                    ? body
                    : string.Empty);
            }
        }

        #endregion
    }
}
