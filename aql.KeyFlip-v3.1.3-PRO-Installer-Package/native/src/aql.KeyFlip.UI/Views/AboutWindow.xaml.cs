using System.Windows;
using aql.KeyFlip.Core.Common;

namespace aql.KeyFlip.UI.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        VersionTextBlock.Text = $"v{AppConstants.Version}";
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
