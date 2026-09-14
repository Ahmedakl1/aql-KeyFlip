using System;
using System.Windows;
using System.Windows.Media;
using MediaBrush = System.Windows.Media.Brush;
using aql.KeyFlip.Core.Common;
using aql.KeyFlip.Core.Conversion;
using aql.KeyFlip.Services.Settings;
using aql.KeyFlip.Windows.Hotkeys;
using aql.KeyFlip.Windows.Startup;

namespace aql.KeyFlip.UI.Views;

public partial class SettingsWindow : Window
{
    private readonly SettingsService _settingsService;
    private readonly Func<string, bool> _onHotkeyChanged;

    public SettingsWindow(SettingsService settingsService, Func<string, bool> onHotkeyChanged)
    {
        InitializeComponent();
        _settingsService = settingsService;
        _onHotkeyChanged = onHotkeyChanged;

        LoadValues();
    }

    private void LoadValues()
    {
        var s = _settingsService.Current;

        // General
        StartWithWindowsCheckBox.IsChecked = s.StartWithWindows;
        StartMinimizedCheckBox.IsChecked = s.StartMinimized;
        MinimizeToTrayCheckBox.IsChecked = s.MinimizeToTrayOnClose;
        NotificationsCheckBox.IsChecked = s.ShowSuccessNotification;
        RestoreClipboardCheckBox.IsChecked = s.RestoreClipboard;

        // Hotkey
        HotkeyTextBox.Text = s.GlobalShortcut;

        // Conversion
        ModeAutoRadio.IsChecked = s.ConversionMode == ConversionMode.Auto;
        ModeEnToArRadio.IsChecked = s.ConversionMode == ConversionMode.EnglishToArabic;
        ModeArToEnRadio.IsChecked = s.ConversionMode == ConversionMode.ArabicToEnglish;

        // Appearance
        ThemeDarkRadio.IsChecked = s.Theme == "dark";
        ThemeLightRadio.IsChecked = s.Theme == "light";
        ThemeSystemRadio.IsChecked = s.Theme == "system";

        // Language
        LangArRadio.IsChecked = s.Language == "ar";
        LangEnRadio.IsChecked = s.Language == "en";
    }

    private void SaveCurrentValues()
    {
        var s = _settingsService.Current;

        s.StartWithWindows = StartWithWindowsCheckBox.IsChecked == true;
        s.StartMinimized = StartMinimizedCheckBox.IsChecked == true;
        s.MinimizeToTrayOnClose = MinimizeToTrayCheckBox.IsChecked == true;
        s.ShowSuccessNotification = NotificationsCheckBox.IsChecked == true;
        s.RestoreClipboard = RestoreClipboardCheckBox.IsChecked == true;

        if (ModeEnToArRadio.IsChecked == true) s.ConversionMode = ConversionMode.EnglishToArabic;
        else if (ModeArToEnRadio.IsChecked == true) s.ConversionMode = ConversionMode.ArabicToEnglish;
        else s.ConversionMode = ConversionMode.Auto;

        if (ThemeLightRadio.IsChecked == true) s.Theme = "light";
        else if (ThemeSystemRadio.IsChecked == true) s.Theme = "system";
        else s.Theme = "dark";

        s.Language = LangEnRadio.IsChecked == true ? "en" : "ar";

        _settingsService.Save(s);
        WindowsStartupManager.SetStartup(s.StartWithWindows);
    }

    private void OnSaveHotkeyClick(object sender, RoutedEventArgs e)
    {
        string text = HotkeyTextBox.Text.Trim();
        if (NativeHotkeyManager.ParseShortcut(text, out _, out _, out string? err))
        {
            if (_onHotkeyChanged(text))
            {
                var s = _settingsService.Current;
                s.GlobalShortcut = text;
                _settingsService.Save(s);
                HotkeyStatusTextBlock.Foreground = (MediaBrush)FindResource("SuccessGreenBrush");
                HotkeyStatusTextBlock.Text = "Shortcut registered successfully ✓";
            }
            else
            {
                HotkeyStatusTextBlock.Foreground = (MediaBrush)FindResource("WarningAmberBrush");
                HotkeyStatusTextBlock.Text = "Windows or another application is already using this shortcut.";
            }
        }
        else
        {
            HotkeyStatusTextBlock.Foreground = (MediaBrush)FindResource("WarningAmberBrush");
            HotkeyStatusTextBlock.Text = err ?? "Invalid shortcut.";
        }
    }

    private void OnTestShortcutClick(object sender, RoutedEventArgs e)
    {
        string text = HotkeyTextBox.Text.Trim();
        var probe = new NativeHotkeyManager();
        try
        {
            if (probe.TestShortcut(text, out string? err))
            {
                HotkeyStatusTextBlock.Foreground = (MediaBrush)FindResource("SuccessGreenBrush");
                HotkeyStatusTextBlock.Text = "Windows accepted this shortcut ✓";
            }
            else
            {
                HotkeyStatusTextBlock.Foreground = (MediaBrush)FindResource("WarningAmberBrush");
                HotkeyStatusTextBlock.Text = err ?? "Shortcut is unavailable.";
            }
        }
        finally
        {
            probe.Dispose();
        }
    }

    private void OnResetDefaultHotkeyClick(object sender, RoutedEventArgs e)
    {
        HotkeyTextBox.Text = AppConstants.HotkeyDefaults.DefaultShortcut;
        OnSaveHotkeyClick(sender, e);
    }

    private void OnRestoreAllDefaultsClick(object sender, RoutedEventArgs e)
    {
        string oldShortcut = _settingsService.Current.GlobalShortcut;
        _settingsService.ResetToDefaults();
        if (!_onHotkeyChanged(AppConstants.HotkeyDefaults.DefaultShortcut))
        {
            var restored = _settingsService.Current;
            restored.GlobalShortcut = oldShortcut;
            _settingsService.Save(restored);
            _onHotkeyChanged(oldShortcut);
            HotkeyStatusTextBlock.Text = "Defaults restored except the hotkey because Windows rejected the default on this PC.";
        }
        else
        {
            HotkeyStatusTextBlock.Text = "All settings restored to defaults.";
        }
        LoadValues();
    }

    private void OnDoneClick(object sender, RoutedEventArgs e)
    {
        SaveCurrentValues();
        Close();
    }
}
