#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif

#ifndef MyOutputBaseFilename
  #define MyOutputBaseFilename "ReifeManager_Setup_v" + MyAppVersion
#endif

[Setup]
AppName=ReifeManager R01
AppVersion={#MyAppVersion}
AppPublisher=ReifeManager Team
AppPublisherURL=https://github.com/Acid31-31/ReifeschrankTracker
DefaultDirName={autopf}\ReifeManager
DefaultGroupName=ReifeManager
OutputDir=installer
OutputBaseFilename={#MyOutputBaseFilename}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
DisableProgramGroupPage=yes
DisableDirPage=yes
DisableReadyPage=yes
AlwaysRestart=no

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
Source: "publish\ReifeManager\ReifeManager_R01.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\ReifeManager\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\ReifeManager R01"; Filename: "{app}\ReifeManager_R01.exe"
Name: "{group}\{cm:UninstallProgram,ReifeManager R01}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\ReifeManager R01"; Filename: "{app}\ReifeManager_R01.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\ReifeManager_R01.exe"; Description: "{cm:LaunchProgram,ReifeManager R01}"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
end;
