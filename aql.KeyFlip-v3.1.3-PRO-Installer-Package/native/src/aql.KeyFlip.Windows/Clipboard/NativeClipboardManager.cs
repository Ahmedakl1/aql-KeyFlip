using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormsClipboard = System.Windows.Forms.Clipboard;
using aql.KeyFlip.Windows.Native;
using aql.KeyFlip.Windows.Selection;

namespace aql.KeyFlip.Windows.Clipboard;

/// <summary>
/// Bounded, race-aware clipboard transaction. User clipboard content is never logged or persisted.
/// </summary>
public static class NativeClipboardManager
{
    private const int CopyTimeoutMs = 1200;
    private const int PollMs = 20;
    private const int Retries = 8;

    public sealed record ClipboardSnapshot(object? Data, uint SequenceAfterCopy);

    public static bool TryCaptureSelectedText(bool preserveClipboard, out string? text, out ClipboardSnapshot? snapshot, out string? error)
    {
        text = null;
        snapshot = null;
        error = null;

        try
        {
            uint beforeCopy = Win32Interop.GetClipboardSequenceNumber();
            object? original = null;
            if (preserveClipboard)
            {
                try { original = FormsClipboard.GetDataObject(); } catch { }
            }

            if (!NativeInputSimulator.SimulateCopy())
            {
                error = "Windows rejected the Copy command.";
                return false;
            }

            if (!WaitForSequenceChange(beforeCopy, CopyTimeoutMs, out uint afterCopy))
            {
                error = "The active application did not complete Copy in time.";
                return false;
            }

            snapshot = new ClipboardSnapshot(original, afterCopy);

            for (int i = 0; i < Retries; i++)
            {
                try
                {
                    if (FormsClipboard.ContainsText(TextDataFormat.UnicodeText))
                    {
                        string value = FormsClipboard.GetText(TextDataFormat.UnicodeText);
                        if (!string.IsNullOrEmpty(value)) return SetSuccess(value, snapshot, out text);
                    }
                }
                catch { }
                Thread.Sleep(PollMs);
            }

            error = "No text was selected or the active application does not expose text through Copy.";
            if (preserveClipboard && original is not null) TryRestoreIfUnchanged(original, afterCopy);
            return false;
        }
        catch (Exception ex)
        {
            error = $"Clipboard read failed: {ex.Message}";
            return false;
        }
    }

    private static bool SetSuccess(string value, ClipboardSnapshot snapshot, out string? text)
    {
        text = value;
        return true;
    }

    public static bool TryPasteConvertedText(string convertedText, ClipboardSnapshot snapshot, bool restoreClipboard, out string? error)
    {
        error = null;
        try
        {
            if (string.IsNullOrEmpty(convertedText))
            {
                error = "Converted text is empty.";
                return false;
            }

            // Do not clobber clipboard data changed by the user or another application.
            if (Win32Interop.GetClipboardSequenceNumber() != snapshot.SequenceAfterCopy)
            {
                error = "Clipboard changed while KeyFlip was processing. Nothing was overwritten.";
                return false;
            }

            if (!TrySetText(convertedText))
            {
                error = "Windows clipboard is busy.";
                return false;
            }

            uint convertedSequence = Win32Interop.GetClipboardSequenceNumber();
            if (!NativeInputSimulator.SimulatePaste())
            {
                if (restoreClipboard && snapshot.Data is not null) TryRestoreIfUnchanged(snapshot.Data, convertedSequence);
                error = "Windows rejected the Paste command.";
                return false;
            }

            if (restoreClipboard && snapshot.Data is not null)
            {
                // Some applications (notably Microsoft Word) may read the clipboard
                // asynchronously after Ctrl+V. Restoring it immediately can race with
                // that read and cause paste errors. Delay the restoration and re-check
                // the clipboard sequence so we never overwrite newer user data.
                ScheduleClipboardRestore(snapshot.Data, convertedSequence);
            }

            return true;
        }
        catch (Exception ex)
        {
            error = $"Clipboard paste failed: {ex.Message}";
            return false;
        }
    }

    public static void RestoreAfterNoChange(ClipboardSnapshot? snapshot)
    {
        if (snapshot?.Data is not null)
            TryRestoreIfUnchanged(snapshot.Data, snapshot.SequenceAfterCopy);
    }

    private static bool TrySetText(string text)
    {
        for (int i = 0; i < Retries; i++)
        {
            try
            {
                FormsClipboard.SetText(text, TextDataFormat.UnicodeText);
                return true;
            }
            catch { Thread.Sleep(PollMs); }
        }
        return false;
    }

    private static void ScheduleClipboardRestore(object data, uint expectedSequence)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(750).ConfigureAwait(false);
                TryRestoreIfUnchanged(data, expectedSequence);
            }
            catch
            {
                // Clipboard restoration is best-effort and must never affect the app.
            }
        });
    }

    private static void TryRestoreIfUnchanged(object data, uint expectedSequence)
    {
        if (Win32Interop.GetClipboardSequenceNumber() != expectedSequence) return;
        for (int i = 0; i < Retries; i++)
        {
            try
            {
                FormsClipboard.SetDataObject(data, true);
                return;
            }
            catch { Thread.Sleep(PollMs); }
        }
    }

    private static bool WaitForSequenceChange(uint initial, int timeoutMs, out uint sequence)
    {
        long start = Environment.TickCount64;
        while (Environment.TickCount64 - start < timeoutMs)
        {
            sequence = Win32Interop.GetClipboardSequenceNumber();
            if (sequence != initial) return true;
            Thread.Sleep(PollMs);
        }
        sequence = Win32Interop.GetClipboardSequenceNumber();
        return sequence != initial;
    }
}
