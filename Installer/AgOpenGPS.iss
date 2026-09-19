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
Name: "pintaskbar"; Description: "Pin {#MyAppName} to the taskbar (Windows 7/8 only)"; GroupDescription: "Additional icons:"; Flags: checkedonce
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

// Windows 10 (1607+) and 11 removed the ability for installers to pin apps to the
// taskbar, so this only actually does anything on Windows 7/8. It fails silently
// otherwise; the wizard's final page reminds the user to pin it manually instead.
procedure TryPinToTaskbar(const ExePath: String);
var
  ShellObj, Folder, FolderItem, Verbs, Verb: Variant;
  VerbName: String;
  i: Integer;
begin
  try
    ShellObj := CreateOleObject('Shell.Application');
    Folder := ShellObj.NameSpace(ExtractFileDir(ExePath));
    FolderItem := Folder.ParseName(ExtractFileName(ExePath));
    Verbs := FolderItem.Verbs;
    for i := 0 to Verbs.Count - 1 do
    begin
      Verb := Verbs.Item(i);
      // Verb names carry an accelerator ampersand (e.g. 'Pin to Tas&kbar'), so strip it before matching.
      VerbName := Verb.Name;
      StringChangeEx(VerbName, '&', '', True);
      if Pos('taskbar', Lowercase(VerbName)) > 0 then
      begin
        Verb.DoIt;
        Break;
      end;
    end;
  except
    // Verb not available on this Windows version - nothing we can do programmatically.
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep = ssInstall) and IsTaskSelected('backupdata') then
    BackupExistingData();
  if (CurStep = ssPostInstall) and IsTaskSelected('pintaskbar') then
    TryPinToTaskbar(ExpandConstant('{app}\{#MyAppExeName}'));
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if (CurPageID = wpFinished) and IsTaskSelected('pintaskbar') then
    WizardForm.FinishedLabel.Caption := WizardForm.FinishedLabel.Caption + #13#10#13#10 +
      'Note: Windows 10/11 no longer allows installers to pin apps to the taskbar automatically. ' +
      'After launching {#MyAppName}, right-click its taskbar icon and choose ''Pin to taskbar''.';
end;
