using System.Windows;
using System.Windows.Controls;
using WpfButton = System.Windows.Controls.Button;
using aql.KeyFlip.Core.Conversion;
using aql.KeyFlip.Services.Settings;

namespace aql.KeyFlip.UI.Views;

public partial class QuickConvertWindow : Window
{
    private readonly SettingsService _settingsService;

    public QuickConvertWindow(SettingsService settingsService)
    {
        InitializeComponent();
        _settingsService = settingsService;
    }

    private void OnPresetClick(object sender, RoutedEventArgs e)
    {
        if (sender is WpfButton btn && btn.Content is string preset)
        {
            InputTextBox.Text = preset;
            PerformConversion();
        }
    }

    private void OnInputTextChanged(object sender, TextChangedEventArgs e)
    {
        PerformConversion();
    }

    private void OnModeChanged(object sender, SelectionChangedEventArgs e)
    {
        PerformConversion();
    }

    private void OnFlipClick(object sender, RoutedEventArgs e)
    {
        PerformConversion();
    }

    private void PerformConversion()
    {
        if (InputTextBox == null || OutputTextBox == null) return;

        string text = InputTextBox.Text;
        if (string.IsNullOrEmpty(text))
        {
            OutputTextBox.Text = string.Empty;
            return;
        }

        var mode = QuickModeComboBox.SelectedIndex switch
        {
            1 => ConversionMode.EnglishToArabic,
            2 => ConversionMode.ArabicToEnglish,
            _ => ConversionMode.Auto
        };

        var result = TextConverter.ConvertText(text, mode);
        OutputTextBox.Text = result.Converted;
    }

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(OutputTextBox.Text))
        {
            System.Windows.Clipboard.SetText(OutputTextBox.Text);
        }
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
