using System;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using aql.KeyFlip.Core.Common;
using aql.KeyFlip.Core.Conversion;
using aql.KeyFlip.Services.Conversion;
using aql.KeyFlip.Services.Logging;
using aql.KeyFlip.Services.Settings;
using aql.KeyFlip.UI.Views;
using aql.KeyFlip.Windows.Hotkeys;
using aql.KeyFlip.Windows.SingleInstance;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace aql.KeyFlip.UI;

public partial class App : Application
{
    private SingleInstanceMutex? _singleInstanceMutex;
    private NotifyIcon? _trayIcon;
    private Icon? _appIcon;
    private SettingsService? _settingsService;
    private ConversionCoordinator? _conversionCoordinator;
    private NativeHotkeyManager? _hotkeyManager;
    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Single Instance Verification
        _singleInstanceMutex = new SingleInstanceMutex();
        if (!_singleInstanceMutex.IsOnlyInstance)
        {
            MessageBox.Show(
                "aql.KeyFlip is already running in the background. Check your Windows System Tray.",
                AppConstants.DisplayName,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
            Shutdown();
            return;
        }

        // 2. Initialize Services
        SafeLogger.Initialize();
        SafeLogger.LogInfo($"Starting {AppConstants.AppName} v{AppConstants.Version}...");

        _settingsService = new SettingsService();
        _conversionCoordinator = new ConversionCoordinator(_settingsService);
        _hotkeyManager = new NativeHotkeyManager();

        // 3. Register Global Hotkey
        if (!RegisterHotkey())
        {
            SafeLogger.LogWarning("No global shortcut is currently registered. Choose another shortcut in Settings.");
        }
        _hotkeyManager.HotkeyPressed += OnHotkeyPressed;

        // 4. Initialize Windows System Tray Icon & Menu
        InitializeSystemTray();

        // 5. Initialize Main Window
        _mainWindow = new MainWindow(_settingsService, _conversionCoordinator, OnHotkeyChanged);

        bool startMinimized = e.Args.Contains("--minimized") || _settingsService.Current.StartMinimized;
        if (!startMinimized)
        {
            _mainWindow.Show();
        }
    }

    private bool RegisterHotkey(string? requestedShortcut = null)
    {
        if (_settingsService == null || _hotkeyManager == null) return false;

        string shortcut = requestedShortcut ?? _settingsService.Current.GlobalShortcut;
        if (!_hotkeyManager.RegisterShortcut(shortcut, out string? error))
        {
            SafeLogger.LogWarning($"Failed to register hotkey {shortcut}: {error}");
            return false;
        }

        SafeLogger.LogInfo($"Hotkey {shortcut} registered successfully.");
        return true;
    }

    private bool OnHotkeyChanged(string newShortcut)
    {
        return RegisterHotkey(newShortcut);
    }

    private void OnHotkeyPressed()
    {
        if (_conversionCoordinator != null)
        {
            // Never operate on text fields inside KeyFlip itself.
            IntPtr foreground = aql.KeyFlip.Windows.Native.Win32Interop.GetForegroundWindow();
            aql.KeyFlip.Windows.Native.Win32Interop.GetWindowThreadProcessId(foreground, out uint pid);
            if (pid == (uint)Environment.ProcessId) return;

            _conversionCoordinator.ExecuteSelectedTextConversion(out string? status);
            if (status == "Success" && _settingsService?.Current.ShowSuccessNotification == true)
            {
                _trayIcon?.ShowBalloonTip(1000, AppConstants.DisplayName, "Keyboard layout inverted successfully.", ToolTipIcon.Info);
            }
        }
    }

    private void InitializeSystemTray()
    {
        _appIcon = TryLoadApplicationIcon();
        _trayIcon = new NotifyIcon
        {
            Icon = _appIcon ?? SystemIcons.Application,
            Text = $"{AppConstants.DisplayName} v{AppConstants.Version}",
            Visible = true
        };

        var contextMenu = new ContextMenuStrip();

        // 1. Header item
        var headerItem = new ToolStripMenuItem($"{AppConstants.DisplayName} v{AppConstants.Version}") { Enabled = false };
        contextMenu.Items.Add(headerItem);

        // 2. Status item
        var statusItem = new ToolStripMenuItem($"Status: {(_settingsService!.Current.Enabled ? "Running" : "Paused")}") { Enabled = false };
        contextMenu.Items.Add(statusItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        // 3. Toggle Enable / Disable
        var toggleItem = new ToolStripMenuItem("Toggle Enable / Disable", null, (s, ev) =>
        {
            var cur = _settingsService.Current;
            cur.Enabled = !cur.Enabled;
            _settingsService.Save(cur);
            statusItem.Text = $"Status: {(cur.Enabled ? "Running" : "Paused")}";
            _mainWindow?.UpdateStatusDisplay();
        });
        contextMenu.Items.Add(toggleItem);

        // 4. Open Main Window
        var openItem = new ToolStripMenuItem("Open Main Window", null, (s, ev) =>
        {
            ShowMainWindow();
        });
        contextMenu.Items.Add(openItem);

        // 5. Convert Now
        var convertItem = new ToolStripMenuItem("Convert Now", null, (s, ev) =>
        {
            OnHotkeyPressed();
        });
        contextMenu.Items.Add(convertItem);

        // 6. Mode Menu
        var modeMenu = new ToolStripMenuItem("Mode");
        var autoMode = new ToolStripMenuItem("Auto", null, (s, ev) => SetMode(ConversionMode.Auto));
        var enToAr = new ToolStripMenuItem("English → Arabic", null, (s, ev) => SetMode(ConversionMode.EnglishToArabic));
        var arToEn = new ToolStripMenuItem("Arabic → English", null, (s, ev) => SetMode(ConversionMode.ArabicToEnglish));
        modeMenu.DropDownItems.AddRange(new ToolStripItem[] { autoMode, enToAr, arToEn });
        contextMenu.Items.Add(modeMenu);

        contextMenu.Items.Add(new ToolStripSeparator());

        // 7. Settings
        var settingsItem = new ToolStripMenuItem("Settings", null, (s, ev) =>
        {
            var sw = new SettingsWindow(_settingsService, OnHotkeyChanged);
            sw.ShowDialog();
        });
        contextMenu.Items.Add(settingsItem);

        // 8. About
        var aboutItem = new ToolStripMenuItem("About", null, (s, ev) =>
        {
            var aw = new AboutWindow();
            aw.ShowDialog();
        });
        contextMenu.Items.Add(aboutItem);

        // 9. Contact Us
        var contactItem = new ToolStripMenuItem("Contact Us", null, (s, ev) =>
        {
            var cw = new ContactWindow();
            cw.ShowDialog();
        });
        contextMenu.Items.Add(contactItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        // 10. Exit
        var exitItem = new ToolStripMenuItem("Exit", null, (s, ev) =>
        {
            ExitApp();
        });
        contextMenu.Items.Add(exitItem);

        _trayIcon.ContextMenuStrip = contextMenu;

        // Double click tray icon opens main window
        _trayIcon.DoubleClick += (s, ev) =>
        {
            ShowMainWindow();
        };
    }

    private static Icon? TryLoadApplicationIcon()
    {
        try
        {
            string? processPath = Environment.ProcessPath;
            return string.IsNullOrWhiteSpace(processPath) ? null : Icon.ExtractAssociatedIcon(processPath);
        }
        catch
        {
            return null;
        }
    }

    private void SetMode(ConversionMode mode)
    {
        var cur = _settingsService!.Current;
        cur.ConversionMode = mode;
        _settingsService.Save(cur);
        _mainWindow?.UpdateStatusDisplay();
    }

    public void ShowMainWindow()
    {
        if (_mainWindow == null)
        {
            _mainWindow = new MainWindow(_settingsService!, _conversionCoordinator!, OnHotkeyChanged);
        }

        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.Activate();
    }

    public void ExitApp()
    {
        _trayIcon?.Dispose();
        _appIcon?.Dispose();
        _hotkeyManager?.Dispose();
        _singleInstanceMutex?.Dispose();
        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon?.Dispose();
        _appIcon?.Dispose();
        _hotkeyManager?.Dispose();
        _singleInstanceMutex?.Dispose();
        base.OnExit(e);
    }
}
