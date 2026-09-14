using aql.KeyFlip.Core.Common;
using aql.KeyFlip.Core.Conversion;

namespace aql.KeyFlip.Services.Settings;

public class AppSettings
{
    public bool Enabled { get; set; } = true;
    public string GlobalShortcut { get; set; } = AppConstants.HotkeyDefaults.DefaultShortcut;
    public ConversionMode ConversionMode { get; set; } = ConversionMode.Auto;
    public bool StartWithWindows { get; set; } = true;
    public bool StartMinimized { get; set; } = true;
    public bool MinimizeToTrayOnClose { get; set; } = true;
    public bool ShowSuccessNotification { get; set; } = true;
    public bool RestoreClipboard { get; set; } = true;
    public string Theme { get; set; } = "dark"; // "system", "dark", "light"
    public string Language { get; set; } = "ar"; // "ar", "en"
    public int SchemaVersion { get; set; } = 4;
}
