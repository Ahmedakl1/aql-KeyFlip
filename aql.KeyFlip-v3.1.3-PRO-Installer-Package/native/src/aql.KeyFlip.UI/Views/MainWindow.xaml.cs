using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MediaColor = System.Windows.Media.Color;
using aql.KeyFlip.Core.Conversion;
using aql.KeyFlip.Services.Conversion;
using aql.KeyFlip.Services.Settings;

namespace aql.KeyFlip.UI.Views;

public partial class MainWindow : Window
{
    private readonly SettingsService _settingsService;
    private readonly ConversionCoordinator _conversionCoordinator;
    private readonly Func<string, bool> _onHotkeyChanged;

    public MainWindow(
        SettingsService settingsService,
        ConversionCoordinator conversionCoordinator,
        Func<string, bool> onHotkeyChanged)
    {
        InitializeComponent();
        _settingsService = settingsService;
        _conversionCoordinator = conversionCoordinator;
        _onHotkeyChanged = onHotkeyChanged;

        UpdateStatusDisplay();
    }

    public void UpdateStatusDisplay()
    {
        var settings = _settingsService.Current;

        // Status Button
        if (settings.Enabled)
        {
            StatusToggleButton.Content = "● Running";
            StatusToggleButton.Background = new SolidColorBrush(MediaColor.FromRgb(6, 78, 59));
            StatusToggleButton.BorderBrush = new SolidColorBrush(MediaColor.FromRgb(4, 120, 87));
            StatusToggleButton.Foreground = new SolidColorBrush(MediaColor.FromRgb(16, 185, 129));
        }
        else
        {
            StatusToggleButton.Content = "○ Paused";
            StatusToggleButton.Background = new SolidColorBrush(MediaColor.FromRgb(120, 53, 15));
            StatusToggleButton.BorderBrush = new SolidColorBrush(MediaColor.FromRgb(180, 83, 9));
            StatusToggleButton.Foreground = new SolidColorBrush(MediaColor.FromRgb(245, 158, 11));
        }

        // Shortcut
        ShortcutTextBlock.Text = settings.GlobalShortcut;

        // Mode
        ModeComboBox.SelectedIndex = settings.ConversionMode switch
        {
            ConversionMode.Auto => 0,
            ConversionMode.EnglishToArabic => 1,
            ConversionMode.ArabicToEnglish => 2,
            _ => 0
        };
    }

    private void OnToggleStatusClick(object sender, RoutedEventArgs e)
    {
        var settings = _settingsService.Current;
        settings.Enabled = !settings.Enabled;
        _settingsService.Save(settings);
        UpdateStatusDisplay();
    }

    private void OnModeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null || _settingsService == null) return;

        var mode = ModeComboBox.SelectedIndex switch
        {
            1 => ConversionMode.EnglishToArabic,
            2 => ConversionMode.ArabicToEnglish,
            _ => ConversionMode.Auto
        };

        var settings = _settingsService.Current;
        if (settings.ConversionMode != mode)
        {
            settings.ConversionMode = mode;
            _settingsService.Save(settings);
        }
    }

    private void OnConvertClick(object sender, RoutedEventArgs e)
    {
        var quickWindow = new QuickConvertWindow(_settingsService);
        quickWindow.Owner = this;
        quickWindow.ShowDialog();
    }

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_settingsService, _onHotkeyChanged);
        settingsWindow.Owner = this;
        settingsWindow.ShowDialog();
        UpdateStatusDisplay();
    }

    private void OnAboutClick(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.Owner = this;
        aboutWindow.ShowDialog();
    }

    private void OnContactClick(object sender, RoutedEventArgs e)
    {
        var contactWindow = new ContactWindow();
        contactWindow.Owner = this;
        contactWindow.ShowDialog();
    }

    private void OnMinimizeToTrayClick(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // When user clicks the 'X' button, minimize to tray instead of quitting if configured
        if (_settingsService.Current.MinimizeToTrayOnClose)
        {
            e.Cancel = true;
            Hide();
        }
        else
        {
            base.OnClosing(e);
        }
    }
}
