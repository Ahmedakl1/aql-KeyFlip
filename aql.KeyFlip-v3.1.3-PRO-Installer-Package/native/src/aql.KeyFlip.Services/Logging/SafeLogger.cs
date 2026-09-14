using System;
using System.IO;
using aql.KeyFlip.Core.Common;

namespace aql.KeyFlip.Services.Logging;

/// <summary>
/// Privacy-Safe Logger.
/// CRITICAL: Strictly forbids logging any user keystrokes, clipboard text, or converted text!
/// Only logs internal application lifecycle state (e.g. "Daemon started", "Hotkey registered").
/// </summary>
public static class SafeLogger
{
    private static readonly object LockObj = new();
    private static string? _logPath;

    public static void Initialize(string? customPath = null)
    {
        if (customPath != null)
        {
            _logPath = customPath;
        }
        else
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, AppConstants.AppName, "logs");
            Directory.CreateDirectory(folder);
            _logPath = Path.Combine(folder, "app.log");
        }
    }

    public static void LogInfo(string message)
    {
        Write("INFO", message);
    }

    public static void LogWarning(string message)
    {
        Write("WARN", message);
    }

    public static void LogError(string message, Exception? ex = null)
    {
        string full = ex != null ? $"{message} - {ex.Message}" : message;
        Write("ERROR", full);
    }

    private static void Write(string level, string message)
    {
        try
        {
            if (_logPath == null) return;
            string line = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}{Environment.NewLine}";
            lock (LockObj)
            {
                File.AppendAllText(_logPath, line);
            }
        }
        catch { }
    }
}
