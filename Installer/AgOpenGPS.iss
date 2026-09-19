; Inno Setup script for AgOpenGPS.
; Build the app first (dotnet publish, PublishDir = ..\AgOpenGPS per the .csproj files),
; then compile with: ISCC.exe /DMyAppVersion=1.2.3 AgOpenGPS.iss

#ifndef MyAppVersion
  #define MyAppVersion "0.0.0"
#endif
#define MyAppName "AgOpenGPS"
#define MyAppPublisher "AgOpenGPS"
#define MyAppExeName "AgOpenGPS.exe"
#define MyAppURL "https://github.com/farmerbriantee/AgOpenGPS"
#define MyPublishDir "..\AgOpenGPS"

[Setup]
AppId={{7C1B2C3E-4C0B-4B6D-9C7D-2F0F1E9A5D42}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
; No admin rights needed - AgOpenGPS installs per-user, next to its Documents data folder.
PrivilegesRequired=lowest
DefaultDirName={userdocs}\{#MyAppName}_{#MyAppVersion}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; Keep the "Select Destination Location" page so the user can change the install folder.
DisableDirPage=no
AllowNoIcons=yes
OutputDir=.\Output
OutputBaseFilename=AgOpenGPS_{#MyAppVersion}_Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: checkedonce
Name: "backupdata"; Description: "Back up my existing Documents\AgOpenGPS folder (fields, settings) before installing"; GroupDescription: "Backup:"; Flags: checkedonce

[Files]
Source: "{#MyPublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[Code]
// The app stores fields/settings in Documents\AgOpenGPS regardless of where it is installed.
function GetDataFolder(): String;
begin
  Result := ExpandConstant('{userdocs}\AgOpenGPS');
end;

procedure BackupExistingData();
var
  DataDir, ZipPath, DateStr: String;
  ErrCode: Integer;
begin
  DataDir := GetDataFolder();
  if DirExists(DataDir) then
  begin
    DateStr := GetDateTimeString('yyyymmdd_hhnnss', #0, #0);
    ZipPath := ExpandConstant('{userdocs}') + '\AgOpenGPS_backup_' + DateStr + '.zip';
    Exec('powershell.exe',
      '-NoProfile -ExecutionPolicy Bypass -Command "Compress-Archive -Path ''' + DataDir + '\*'' -DestinationPath ''' + ZipPath + ''' -Force"',
      '', SW_HIDE, ewWaitUntilTerminated, ErrCode);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep = ssInstall) and IsTaskSelected('backupdata') then
    BackupExistingData();
end;
