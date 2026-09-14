@echo off
setlocal EnableExtensions
cd /d "%~dp0.."
set "SOLUTION=%CD%\aql.KeyFlip.sln"
set "UI=%CD%\src\aql.KeyFlip.UI\aql.KeyFlip.UI.csproj"
set "TEST=%CD%\src\aql.KeyFlip.Tests\aql.KeyFlip.Tests.csproj"
set "OUT=%CD%\dist-release"
set "INSTALLER_OUT=%CD%\dist-installer"
set "ISCC="

for %%P in ("%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" "%ProgramFiles%\Inno Setup 6\ISCC.exe") do (
  if exist "%%~P" if not defined ISCC set "ISCC=%%~P"
)

if exist "%OUT%" rmdir /s /q "%OUT%"
if exist "%INSTALLER_OUT%" rmdir /s /q "%INSTALLER_OUT%"
mkdir "%INSTALLER_OUT%"

echo ==========================================================
echo   aql.KeyFlip v3.1.3 - Native Windows x64 Release
echo   .NET 10 / Visual Studio 2026
echo ==========================================================

echo [1/4] Running tests...
dotnet test "%TEST%" -c Release
if errorlevel 1 goto :fail

echo [2/4] Publishing self-contained x64 single-file...
dotnet publish "%UI%" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -o "%OUT%"
if errorlevel 1 goto :fail

if not exist "%OUT%\aql.KeyFlip.exe" goto :fail

echo [3/4] Verifying executable...
for %%A in ("%OUT%\aql.KeyFlip.exe") do echo EXE size: %%~zA bytes

if not defined ISCC (
  echo [4/4] Inno Setup compiler was not found.
  echo Install Inno Setup 6, then run this script again to create the installer.
  echo Release EXE is ready in: %OUT%\aql.KeyFlip.exe
  goto :success
)

echo [4/4] Building professional installer...
"%ISCC%" "%CD%\build\installer-setup.iss"
if errorlevel 1 goto :fail

if not exist "%INSTALLER_OUT%\aql.KeyFlip Setup.exe" goto :fail

echo.
echo ==========================================================
echo   BUILD SUCCESSFUL
echo   EXE:       %OUT%\aql.KeyFlip.exe
echo   INSTALLER: %INSTALLER_OUT%\aql.KeyFlip Setup.exe
echo ==========================================================
goto :success

:fail
echo.
echo BUILD FAILED.
exit /b 1

:success
exit /b 0
