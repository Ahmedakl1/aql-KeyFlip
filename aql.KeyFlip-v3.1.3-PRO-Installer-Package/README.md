# aql.KeyFlip v3.1.3

Windows-native keyboard-layout converter.

Target: Windows 10/11 x64, .NET 10, Visual Studio 2026.

This release fixes the remaining WPF/WinForms namespace ambiguities that prevented the UI project from compiling under `UseWindowsForms=true`, and cleans up nullable warnings in the conversion/coordinator code.
