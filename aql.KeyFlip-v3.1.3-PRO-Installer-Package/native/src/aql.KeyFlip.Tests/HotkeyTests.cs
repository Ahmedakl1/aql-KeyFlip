using Xunit;
using aql.KeyFlip.Windows.Hotkeys;

namespace aql.KeyFlip.Tests;

public class HotkeyTests
{
    [Theory]
    [InlineData("Ctrl + Alt + Shift + K")]
    [InlineData("Ctrl + Shift + F12")]
    [InlineData("Alt + Ctrl + M")]
    public void ValidShortcutsParse(string value)
    {
        Assert.True(NativeHotkeyManager.ParseShortcut(value, out _, out _, out string? error), error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("K")]
    [InlineData("Win + K")]
    [InlineData("Alt + Tab")]
    [InlineData("Ctrl + Escape")]
    [InlineData("Ctrl + K + M")]
    public void UnsafeOrInvalidShortcutsAreRejected(string value) =>
        Assert.False(NativeHotkeyManager.ParseShortcut(value, out _, out _, out _));
}
