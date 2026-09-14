using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using aql.KeyFlip.Windows.Native;

namespace aql.KeyFlip.Windows.Hotkeys;

/// <summary>
/// Global hotkeys are owned by Windows via RegisterHotKey. No low-level keyboard hook is used.
/// Registration is transactional: a failed new shortcut never destroys the working shortcut.
/// </summary>
public sealed class NativeHotkeyManager : IDisposable
{
    private readonly HotkeyMessageWindow _window;
    private int _registeredId;
    private int _nextId = 0x4B46;
    private uint _modifiers;
    private uint _vk;

    public event Action? HotkeyPressed;
    public bool IsRegistered => _registeredId != 0;

    public NativeHotkeyManager() => _window = new HotkeyMessageWindow(this);

    public bool RegisterShortcut(string shortcutText, out string? errorMessage)
    {
        errorMessage = null;
        if (!TryParseAndValidate(shortcutText, out uint modifiers, out uint vk, out errorMessage))
            return false;

        int candidateId = NextId();
        if (!Win32Interop.RegisterHotKey(_window.Handle, candidateId, modifiers | Win32Interop.MOD_NOREPEAT, vk))
        {
            int code = Marshal.GetLastWin32Error();
            errorMessage = code == 1409
                ? "This shortcut is already registered by Windows or another application."
                : $"Windows could not register this shortcut (error {code}).";
            return false;
        }

        int oldId = _registeredId;
        _registeredId = candidateId;
        _modifiers = modifiers;
        _vk = vk;
        if (oldId != 0) Win32Interop.UnregisterHotKey(_window.Handle, oldId);
        return true;
    }

    public bool TestShortcut(string shortcutText, out string? errorMessage)
    {
        errorMessage = null;
        if (!TryParseAndValidate(shortcutText, out uint modifiers, out uint vk, out errorMessage)) return false;
        if (IsRegistered && modifiers == _modifiers && vk == _vk) return true;

        int testId = NextId();
        bool ok = Win32Interop.RegisterHotKey(_window.Handle, testId, modifiers | Win32Interop.MOD_NOREPEAT, vk);
        if (!ok)
        {
            int code = Marshal.GetLastWin32Error();
            errorMessage = code == 1409
                ? "This shortcut is already registered by Windows or another application."
                : $"Windows could not register this shortcut (error {code}).";
            return false;
        }
        Win32Interop.UnregisterHotKey(_window.Handle, testId);
        return true;
    }

    public void Unregister()
    {
        if (_registeredId == 0) return;
        Win32Interop.UnregisterHotKey(_window.Handle, _registeredId);
        _registeredId = 0;
    }

    public static bool ParseShortcut(string shortcutText, out uint modifiers, out uint vk, out string? errorMessage) =>
        TryParseAndValidate(shortcutText, out modifiers, out vk, out errorMessage);

    private int NextId()
    {
        if (_nextId == int.MaxValue) _nextId = 0x4B46;
        return _nextId++;
    }

    private static bool TryParseAndValidate(string shortcutText, out uint modifiers, out uint vk, out string? errorMessage)
    {
        modifiers = 0;
        vk = 0;
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(shortcutText))
        {
            errorMessage = "Shortcut cannot be empty.";
            return false;
        }

        string[] parts = shortcutText.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            errorMessage = "Use at least one modifier, for example Ctrl + Alt + Shift + K.";
            return false;
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string part in parts)
        {
            if (!seen.Add(part))
            {
                errorMessage = "The same key/modifier cannot appear twice.";
                return false;
            }

            switch (part.ToUpperInvariant())
            {
                case "CTRL":
                case "CONTROL": modifiers |= Win32Interop.MOD_CONTROL; break;
                case "ALT": modifiers |= Win32Interop.MOD_ALT; break;
                case "SHIFT": modifiers |= Win32Interop.MOD_SHIFT; break;
                case "WIN":
                case "WINDOWS": modifiers |= Win32Interop.MOD_WIN; break;
                default:
                    if (vk != 0)
                    {
                        errorMessage = "Shortcut must contain exactly one main key.";
                        return false;
                    }
                    if (Enum.TryParse<Keys>(part, true, out var key)) vk = (uint)key;
                    else if (part.Length == 1 && char.IsLetterOrDigit(part[0])) vk = char.ToUpperInvariant(part[0]);
                    else
                    {
                        errorMessage = $"Unrecognized key '{part}'.";
                        return false;
                    }
                    break;
            }
        }

        if (vk == 0)
        {
            errorMessage = "Shortcut must contain one main key.";
            return false;
        }
        if ((modifiers & Win32Interop.MOD_WIN) != 0)
        {
            errorMessage = "Win-based shortcuts are blocked to protect Windows shortcuts.";
            return false;
        }
        if (IsReserved(modifiers, vk))
        {
            errorMessage = "This combination is reserved by Windows and cannot be used by KeyFlip.";
            return false;
        }
        return true;
    }

    private static bool IsReserved(uint modifiers, uint vk)
    {
        const uint ctrl = Win32Interop.MOD_CONTROL;
        const uint alt = Win32Interop.MOD_ALT;
        const uint shift = Win32Interop.MOD_SHIFT;
        if ((modifiers & alt) != 0 && (modifiers & ctrl) == 0 && (modifiers & shift) == 0 &&
            (vk == (uint)Keys.Tab || vk == (uint)Keys.F4 || vk == (uint)Keys.Escape)) return true;
        if ((modifiers & ctrl) != 0 && (modifiers & alt) == 0 && (modifiers & shift) == 0 && vk == (uint)Keys.Escape) return true;
        if (modifiers == shift && vk == (uint)Keys.Escape) return true;
        return false;
    }

    internal void OnHotkeyPressed() => HotkeyPressed?.Invoke();

    public void Dispose()
    {
        Unregister();
        _window.DestroyHandle();
    }

    private sealed class HotkeyMessageWindow : NativeWindow
    {
        private readonly NativeHotkeyManager _parent;
        public HotkeyMessageWindow(NativeHotkeyManager parent)
        {
            _parent = parent;
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32Interop.WM_HOTKEY) _parent.OnHotkeyPressed();
            base.WndProc(ref m);
        }
    }
}
