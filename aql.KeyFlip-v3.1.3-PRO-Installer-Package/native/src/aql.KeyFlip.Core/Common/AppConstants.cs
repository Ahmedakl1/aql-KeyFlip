namespace aql.KeyFlip.Core.Common;

/// <summary>
/// Single Source of Truth for Application Identity, Version, Developer, and Brand.
/// </summary>
public static class AppConstants
{
    public const string AppName = "aql.KeyFlip";
    public const string ExecutableName = "aql.KeyFlip.exe";
    public const string InstallerName = "aql.KeyFlip Setup.exe";
    public const string DisplayName = "aql.KeyFlip";
    public const string Description = "Windows Keyboard Layout Converter";
    public const string Slogan = "Type Wrong. Flip. Continue.";
    public const string Version = "3.1.3";
    public const string BuildDate = "September 2026";
    public const string TargetPlatform = "Windows 10 / Windows 11 (x64)";
    public const string DevelopmentEnvironment = "Visual Studio 2026 (18.10.0)";
    public const string SingleInstanceMutexName = @"Local\aql_KeyFlip_SingleInstance_Mutex_v31";

    public static class Developer
    {
        public const string Name = "Eng. Ahmed Salah Aql";
        public const string Brand = "AQL";
        public const string Slogan = "Building Ideas Into Software";
        public const string Email = "info@ahmedaql.online";
        public const string WhatsApp = "01098486663";
        public const string WhatsAppUrl = "https://wa.me/201098486663";
        public const string EmailMailto = "mailto:info@ahmedaql.online?subject=aql.KeyFlip%20v3.1.3%20Feedback";
        public const string Country = "Egypt";
    }

    public static class HotkeyDefaults
    {
        public const string DefaultShortcut = "Ctrl + K";
        public const string Alternative1 = "Ctrl + Alt + K";
        public const string Alternative2 = "Ctrl + Shift + K";
    }
}
