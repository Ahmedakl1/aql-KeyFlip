# v3.1.3 Verification

Completed in this environment:

- Native release tree is isolated under `versions/3.1.3/`.
- All current project files target .NET 10 (`net10.0` / `net10.0-windows`).
- `global.json` pins SDK 10.0.400 and disables prerelease SDK selection.
- WPF/XAML and project XML files parse successfully.
- C# source brace integrity was checked.
- Legacy Electron/React/TypeScript/Vite/PowerShell/VBScript files are absent from the v3.1.3 release tree.
- The global hotkey implementation is transactional and uses Windows `RegisterHotKey`.
- Known Windows-reserved hotkey patterns are rejected.
- Clipboard handling uses a bounded sequence-number/race-aware transaction instead of fixed 40ms sleeps.
- The original clipboard is restored only when the clipboard was not changed by another process.
- The final logo remains intentionally unimplemented for the next branding step.

## Windows build note

The current execution environment is not Windows and does not have the .NET SDK installed, so a real Windows GUI build/run cannot be honestly claimed from this environment. The included `build-release.cmd` is the authoritative Windows build/test path for Visual Studio 2026 + .NET SDK 10.0.400.
