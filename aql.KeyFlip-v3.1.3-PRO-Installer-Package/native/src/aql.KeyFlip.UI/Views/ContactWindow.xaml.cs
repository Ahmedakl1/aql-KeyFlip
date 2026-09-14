using System.Diagnostics;
using System.Windows;
using aql.KeyFlip.Core.Common;

namespace aql.KeyFlip.UI.Views;

public partial class ContactWindow : Window
{
    public ContactWindow()
    {
        InitializeComponent();
    }

    private void OnSendEmailClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = AppConstants.Developer.EmailMailto,
                UseShellExecute = true
            });
        }
        catch { }
    }

    private void OnWhatsAppClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = AppConstants.Developer.WhatsAppUrl,
                UseShellExecute = true
            });
        }
        catch { }
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
