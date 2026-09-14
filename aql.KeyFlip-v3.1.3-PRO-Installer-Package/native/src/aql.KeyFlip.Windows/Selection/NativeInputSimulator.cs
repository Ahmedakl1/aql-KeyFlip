using System;
using System.Runtime.InteropServices;
using aql.KeyFlip.Windows.Native;

namespace aql.KeyFlip.Windows.Selection;

public static class NativeInputSimulator
{
    public static bool SimulateCopy() => SendKeyCombination(Win32Interop.VK_CONTROL, Win32Interop.VK_C);
    public static bool SimulatePaste() => SendKeyCombination(Win32Interop.VK_CONTROL, Win32Interop.VK_V);

    private static bool SendKeyCombination(ushort modifierVk, ushort keyVk)
    {
        var inputs = new Win32Interop.INPUT[4];
        inputs[0] = Key(modifierVk, keyUp: false);
        inputs[1] = Key(keyVk, keyUp: false);
        inputs[2] = Key(keyVk, keyUp: true);
        inputs[3] = Key(modifierVk, keyUp: true);

        uint sent = Win32Interop.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Win32Interop.INPUT>());
        return sent == inputs.Length;
    }

    private static Win32Interop.INPUT Key(ushort vk, bool keyUp) => new()
    {
        type = Win32Interop.INPUT_KEYBOARD,
        u = new Win32Interop.InputUnion
        {
            ki = new Win32Interop.KEYBDINPUT
            {
                wVk = vk,
                wScan = 0,
                dwFlags = keyUp ? Win32Interop.KEYEVENTF_KEYUP : 0,
                time = 0,
                dwExtraInfo = UIntPtr.Zero
            }
        }
    };
}
