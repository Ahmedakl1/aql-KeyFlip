# aql.KeyFlip v3.1.3

## Fixes
- Fixed `Brush` ambiguity between `System.Drawing.Brush` and `System.Windows.Media.Brush` in SettingsWindow.
- Fixed `Color` ambiguity between `System.Drawing.Color` and `System.Windows.Media.Color` in MainWindow.
- Fixed `Button` ambiguity between WPF and WinForms in QuickConvertWindow.
- Removed avoidable nullable warnings in TextConverter.
- Added an explicit null guard for the clipboard snapshot in ConversionCoordinator.
- Preserved .NET 10 / Windows 10/11 / x64 targeting.
- Preserved isolated per-version directory structure.

## Build command
`dotnet build .\\aql.KeyFlip.sln -c Release`


### Current working-tree fix (v3.1.3, no version bump)
- Changed clipboard restoration to a delayed, sequence-checked restore (750 ms) to avoid Microsoft Word paste races.
- The project version remains 3.1.3; this is an in-place fix.
