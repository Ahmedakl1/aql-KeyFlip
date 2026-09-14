using System;
using System.Threading;
using aql.KeyFlip.Core.Conversion;
using aql.KeyFlip.Services.Logging;
using aql.KeyFlip.Services.Settings;
using aql.KeyFlip.Windows.Clipboard;

namespace aql.KeyFlip.Services.Conversion;

public sealed class ConversionCoordinator
{
    private readonly SettingsService _settingsService;
    private int _busy;

    public event Action<ConversionResult>? ConversionCompleted;

    public ConversionCoordinator(SettingsService settingsService) => _settingsService = settingsService;

    public bool ExecuteSelectedTextConversion(out string? statusMessage)
    {
        statusMessage = null;
        if (Interlocked.Exchange(ref _busy, 1) != 0)
        {
            statusMessage = "A conversion is already in progress.";
            return false;
        }

        try
        {
            var settings = _settingsService.Current;
            if (!settings.Enabled)
            {
                statusMessage = "KeyFlip is currently paused.";
                return false;
            }

            if (!NativeClipboardManager.TryCaptureSelectedText(
                    settings.RestoreClipboard,
                    out string? selectedText,
                    out var snapshot,
                    out string? readError) ||
                selectedText is null || snapshot is null)
            {
                statusMessage = readError ?? "No text selected.";
                return false;
            }

            var result = TextConverter.ConvertText(selectedText, settings.ConversionMode);
            if (!result.IsChanged)
            {
                if (settings.RestoreClipboard) NativeClipboardManager.RestoreAfterNoChange(snapshot);
                statusMessage = "Text did not require layout inversion.";
                return false;
            }

            if (!NativeClipboardManager.TryPasteConvertedText(
                    result.Converted,
                    snapshot,
                    settings.RestoreClipboard,
                    out string? pasteError))
            {
                statusMessage = pasteError ?? "Could not paste converted text.";
                return false;
            }

            SafeLogger.LogInfo($"Converted {result.CharsCount} characters ({result.Direction}).");
            ConversionCompleted?.Invoke(result);
            statusMessage = "Success";
            return true;
        }
        catch (Exception ex)
        {
            SafeLogger.LogError("Conversion failed", ex);
            statusMessage = "Conversion failed. The active application or clipboard may be busy.";
            return false;
        }
        finally
        {
            Volatile.Write(ref _busy, 0);
        }
    }
}
