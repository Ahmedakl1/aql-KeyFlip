using System;
using Microsoft.Win32;
using aql.KeyFlip.Core.Common;

namespace aql.KeyFlip.Windows.Startup;

public static class WindowsStartupManager
{
    private const string RunRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public static bool IsStartupEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, false);
            return key?.GetValue(AppConstants.AppName) != null;
        }
        catch
        {
            return false;
        }
    }

    public static bool SetStartup(bool enable, string? executablePath = null)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, true);
            if (key == null) return false;

            if (enable)
            {
                string path = executablePath ?? Environment.ProcessPath ?? string.Empty;
                if (!string.IsNullOrEmpty(path))
                {
                    key.SetValue(AppConstants.AppName, $"\"{path}\" --minimized");
                    return true;
                }
            }
            else
            {
                if (key.GetValue(AppConstants.AppName) != null)
                {
                    key.DeleteValue(AppConstants.AppName, false);
                }
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }
}
