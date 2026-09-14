using System;
using System.IO;
using System.Text.Json;
using aql.KeyFlip.Core.Common;

namespace aql.KeyFlip.Services.Settings;

public class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _settingsFilePath;
    private AppSettings _currentSettings;

    public AppSettings Current => _currentSettings;

    public SettingsService(string? customPath = null)
    {
        if (customPath != null)
        {
            _settingsFilePath = customPath;
        }
        else
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appData, AppConstants.AppName);
            Directory.CreateDirectory(appFolder);
            _settingsFilePath = Path.Combine(appFolder, "settings.json");
        }

        _currentSettings = Load();
    }

    public AppSettings Load()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                string json = File.ReadAllText(_settingsFilePath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
                if (loaded != null)
                {
                    // Keep the existing v3.1.3 installation compatible while moving
                    // the default to the simpler two-key shortcut Ctrl + K.
                    // Only migrate the old built-in defaults; never overwrite a
                    // shortcut the user deliberately chose.
                    if (string.Equals(loaded.GlobalShortcut, "Ctrl + Alt + Shift + K", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(loaded.GlobalShortcut, "Ctrl + Alt + K", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(loaded.GlobalShortcut, "Ctrl + Shift + K", StringComparison.OrdinalIgnoreCase))
                    {
                        loaded.GlobalShortcut = AppConstants.HotkeyDefaults.DefaultShortcut;
                        Save(loaded);
                    }

                    _currentSettings = loaded;
                    return _currentSettings;
                }
            }
        }
        catch { }

        _currentSettings = new AppSettings();
        Save(_currentSettings);
        return _currentSettings;
    }

    public void Save(AppSettings settings)
    {
        _currentSettings = settings;
        try
        {
            string dir = Path.GetDirectoryName(_settingsFilePath)!;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(_settingsFilePath, json);
        }
        catch { }
    }

    public void ResetToDefaults()
    {
        Save(new AppSettings());
    }
}
