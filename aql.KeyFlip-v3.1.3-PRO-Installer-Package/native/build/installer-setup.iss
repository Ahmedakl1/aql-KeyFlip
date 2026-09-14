; ================================================================
; aql.KeyFlip v3.1.3 - Professional Windows 11 Style Installer
; AQL / Eng. Ahmed Salah Aql
; ================================================================
#define MyAppName "aql.KeyFlip"
#define MyAppVersion "3.1.3"
#define MyAppPublisher "AQL (Eng. Ahmed Salah Aql)"
#define MyAppURL "https://ahmedaql.online"
#define MyAppExeName "aql.KeyFlip.exe"
#define InstallerAssets "..\assets\installer"
#define LogoFile "..\assets\logo\aql.KeyFlip.ico"

[Setup]
AppId={{D37D15E8-4BA6-4B4B-91BC-3BCDE7C5B56A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
DisableWelcomePage=yes
DisableReadyPage=no
DisableFinishedPage=no
WizardStyle=modern
WizardResizable=no
WizardSizePercent=110
WizardImageStretch=no
SetupIconFile={#LogoFile}
UninstallDisplayIcon={app}\{#MyAppExeName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputBaseFilename=aql.KeyFlip Setup
Compression=lzma2/ultra64
SolidCompression=yes
OutputDir=..\dist-installer
PrivilegesRequired=admin
ChangesAssociations=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Shortcuts:"
Name: "startup"; Description: "Start aql.KeyFlip with Windows"; GroupDescription: "Windows integration:"

[Files]
Source: "..\dist-release\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Parameters: "--minimized"; Tasks: startup

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch aql.KeyFlip now"; Flags: nowait postinstall skipifsilent

[Code]
var
  IntroPage: TWizardPage;
  HowPage: TWizardPage;
  FeaturesPage: TWizardPage;
  DeveloperPage: TWizardPage;

procedure AddHeroImage(Page: TWizardPage; const FileName: String);
var
  Img: TBitmapImage;
begin
  Img := TBitmapImage.Create(Page);
  Img.Parent := Page.Surface;
  Img.Left := 0;
  Img.Top := 0;
  Img.Width := Page.SurfaceWidth;
  Img.Height := 310;
  Img.Stretch := True;
  Img.Bitmap.LoadFromFile(FileName);
end;

procedure AddFooter(Page: TWizardPage; const TextValue: String);
var
  Txt: TNewStaticText;
begin
  Txt := TNewStaticText.Create(Page);
  Txt.Parent := Page.Surface;
  Txt.Left := 8;
  Txt.Top := 322;
  Txt.Width := Page.SurfaceWidth - 16;
  Txt.Height := 55;
  Txt.Caption := TextValue;
  Txt.Font.Size := 9;
  Txt.Font.Color := $006A7485;
  Txt.AutoSize := False;
  Txt.WordWrap := True;
end;

procedure OpenWhatsApp(Sender: TObject);
var
  ErrorCode: Integer;
begin
  ShellExec('open', 'https://wa.me/201098486663', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
end;

procedure OpenEmail(Sender: TObject);
var
  ErrorCode: Integer;
begin
  ShellExec('open', 'mailto:info@ahmedaql.online?subject=aql.KeyFlip%20Feedback', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
end;

procedure AddContactButtons(Page: TWizardPage);
var
  WhatsAppButton: TNewButton;
  EmailButton: TNewButton;
begin
  WhatsAppButton := TNewButton.Create(Page);
  WhatsAppButton.Parent := Page.Surface;
  WhatsAppButton.Left := Page.SurfaceWidth div 2 - 150;
  WhatsAppButton.Top := 382;
  WhatsAppButton.Width := 135;
  WhatsAppButton.Height := 30;
  WhatsAppButton.Caption := 'WhatsApp';
  WhatsAppButton.OnClick := @OpenWhatsApp;

  EmailButton := TNewButton.Create(Page);
  EmailButton.Parent := Page.Surface;
  EmailButton.Left := Page.SurfaceWidth div 2 + 15;
  EmailButton.Top := 382;
  EmailButton.Width := 135;
  EmailButton.Height := 30;
  EmailButton.Caption := 'Email';
  EmailButton.OnClick := @OpenEmail;
end;

procedure InitializeWizard;
begin
  { Page 1 - product introduction }
  IntroPage := CreateCustomPage(wpWelcome,
    'Welcome to aql.KeyFlip',
    'مرحبًا بك في aql.KeyFlip — اكتب، اقلب التخطيط، واستمر.');
  AddHeroImage(IntroPage, ExpandConstant('{#InstallerAssets}\01-welcome.png'));
  AddFooter(IntroPage,
    'A lightweight native Windows utility that fixes text typed with the wrong keyboard layout.  ' +
    'Designed for Windows 10/11 x64 and ready to work quietly from the system tray.');

  { Page 2 - how it works }
  HowPage := CreateCustomPage(IntroPage.ID,
    'How aql.KeyFlip works',
    'فكرة بسيطة: النص الخطأ يتحول فورًا إلى التخطيط الصحيح.');
  AddHeroImage(HowPage, ExpandConstant('{#InstallerAssets}\02-how-it-works.png'));
  AddFooter(HowPage,
    'Select the text, press Ctrl + K, and continue typing.  The shortcut uses the same physical key ' +
    'in English and Arabic layouts, so it stays familiar wherever you work.');

  { Page 3 - features }
  FeaturesPage := CreateCustomPage(HowPage.ID,
    'Built for everyday Windows use',
    'مميزات مصممة لتكون سريعة، هادئة، وآمنة.');
  AddHeroImage(FeaturesPage, ExpandConstant('{#InstallerAssets}\03-features.png'));
  AddFooter(FeaturesPage,
    'Offline by design • No cloud dependency • No keylogger • Global hotkey • System tray • ' +
    'Safe clipboard handling • Auto / English→Arabic / Arabic→English modes.');

  { Page 4 - developer/contact }
  DeveloperPage := CreateCustomPage(FeaturesPage.ID,
    'Made by AQL',
    'نبذة عن المطور وطرق التواصل.');
  AddHeroImage(DeveloperPage, ExpandConstant('{#InstallerAssets}\04-developer.png'));
  AddFooter(DeveloperPage,
    'Eng. Ahmed Salah Aql • AQL — Building Ideas Into Software' + #13#10 +
    'WhatsApp: 01098486663    •    Email: info@ahmedaql.online');
  AddContactButtons(DeveloperPage);
end;
