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
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"

[CustomMessages]
english.PinTaskbar=Pin {#MyAppName} to the taskbar (Windows 7/8 only)
english.BackupData=Back up my existing Documents\AgOpenGPS folder (fields, settings) before installing
english.BackupGroup=Backup:
english.PinTaskbarNote=Note: Windows 10/11 no longer allows installers to pin apps to the taskbar automatically. After launching {#MyAppName}, right-click its taskbar icon and choose 'Pin to taskbar'.
english.StartupGroup=Windows startup:
english.AutoStart=Start {#MyAppName} automatically when Windows starts (older versions are removed from startup)
english.FirewallGroup=Network:
english.Firewall=Allow AgIO and GPS_Out through Windows Firewall on private and public networks (asks for administrator approval)
english.FirewallFailed=The Windows Firewall rules for AgIO and GPS_Out could not be added (administrator approval was declined or failed).%n%nIf AgIO does not receive data from the modules, allow AgIO.exe and GPS_Out.exe in Windows Security > Firewall > Allow an app through firewall, for both Private and Public networks.
hungarian.PinTaskbar={#MyAppName} rögzítése a tálcán (csak Windows 7/8)
hungarian.BackupData=A meglévő Dokumentumok\AgOpenGPS mappa (táblák, beállítások) biztonsági mentése telepítés előtt
hungarian.BackupGroup=Biztonsági mentés:
hungarian.PinTaskbarNote=Megjegyzés: a Windows 10/11 már nem engedi, hogy a telepítők automatikusan a tálcára rögzítsenek alkalmazásokat. Az {#MyAppName} elindítása után kattintson jobb gombbal a tálcán lévő ikonjára, és válassza a „Rögzítés a tálcán” lehetőséget.
hungarian.StartupGroup=Windows indítás:
hungarian.AutoStart=Az {#MyAppName} automatikus indítása a Windows indulásakor (a régebbi verziók kikerülnek az automatikus indításból)
hungarian.FirewallGroup=Hálózat:
hungarian.Firewall=Az AgIO és a GPS_Out engedélyezése a Windows tűzfalon magán- és nyilvános hálózatokon (rendszergazdai jóváhagyást kér)
hungarian.FirewallFailed=Az AgIO és a GPS_Out Windows tűzfalszabályait nem sikerült létrehozni (a rendszergazdai jóváhagyás elmaradt vagy sikertelen volt).%n%nHa az AgIO nem kap adatot a moduloktól, engedélyezze az AgIO.exe és a GPS_Out.exe programot a Windows biztonság > Tűzfal > Alkalmazás engedélyezése a tűzfalon keresztül menüben, magán- és nyilvános hálózatokra is.

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce
Name: "pintaskbar"; Description: "{cm:PinTaskbar}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce
Name: "backupdata"; Description: "{cm:BackupData}"; GroupDescription: "{cm:BackupGroup}"; Flags: checkedonce
Name: "autostart"; Description: "{cm:AutoStart}"; GroupDescription: "{cm:StartupGroup}"; Flags: checkedonce
Name: "firewall"; Description: "{cm:Firewall}"; GroupDescription: "{cm:FirewallGroup}"

[Files]
Source: "{#MyPublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
; AgOpenGPS launches AgIO itself, so only the main app needs to auto-start.
Name: "{userstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: autostart

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

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
      // English 'taskbar' or Hungarian 'tálcá(n)' - the verb name is localized with Windows.
      if (Pos('taskbar', Lowercase(VerbName)) > 0) or (Pos('tálc', Lowercase(VerbName)) > 0) then
      begin
        Verb.DoIt;
        Break;
      end;
    end;
  except
    // Verb not available on this Windows version - nothing we can do programmatically.
  end;
end;

// Each version installs into its own folder, so a startup entry left by an older install
// would launch an outdated copy next to the new one. Those entries are disabled the same
// way Task Manager does it: a StartupApproved value whose first byte is odd (03) means
// "disabled". The entry itself stays, so it can be switched back on in Task Manager.
const
  RunKey = 'Software\Microsoft\Windows\CurrentVersion\Run';
  ApprovedRunKey = 'Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run';
  ApprovedFolderKey = 'Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder';
  OwnStartupLnk = '{#MyAppName}.lnk';

procedure DisableStartupEntry(const ApprovedKey, ValueName: String);
begin
  Log('Disabling old AgOpenGPS startup entry: ' + ValueName);
  RegWriteBinaryValue(HKCU, ApprovedKey, ValueName, #3#0#0#0#0#0#0#0#0#0#0#0);
end;

function LaunchesAgOpenGPS(const CommandOrName: String): Boolean;
var
  S: String;
begin
  S := Lowercase(CommandOrName);
  Result := (Pos('agopengps.exe', S) > 0) or (Pos('agio.exe', S) > 0) or (Pos('gps_out.exe', S) > 0);
end;

function GetShortcutTarget(const LnkPath: String): String;
var
  Shell, Lnk: Variant;
begin
  Result := '';
  if CompareText(ExtractFileExt(LnkPath), '.lnk') <> 0 then
    Exit;
  try
    Shell := CreateOleObject('WScript.Shell');
    Lnk := Shell.CreateShortcut(LnkPath);
    Result := Lnk.TargetPath;
  except
    // Unreadable shortcut - fall back to matching on the file name only.
  end;
end;

// Only per-user entries are checked - machine-wide ones (HKLM) would need admin rights.
procedure DisableOldAgOpenGPSStartupEntries();
var
  Names: TArrayOfString;
  Cmd, StartupDir: String;
  FindRec: TFindRec;
  i: Integer;
begin
  if RegGetValueNames(HKCU, RunKey, Names) then
    for i := 0 to GetArrayLength(Names) - 1 do
    begin
      if not RegQueryStringValue(HKCU, RunKey, Names[i], Cmd) then
        Cmd := '';
      if LaunchesAgOpenGPS(Names[i] + ' ' + Cmd) then
        DisableStartupEntry(ApprovedRunKey, Names[i]);
    end;

  StartupDir := ExpandConstant('{userstartup}');
  if FindFirst(StartupDir + '\*', FindRec) then
  try
    repeat
      if ((FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY) = 0) and
         (CompareText(FindRec.Name, OwnStartupLnk) <> 0) and
         LaunchesAgOpenGPS(FindRec.Name + ' ' + GetShortcutTarget(StartupDir + '\' + FindRec.Name)) then
        DisableStartupEntry(ApprovedFolderKey, FindRec.Name);
    until not FindNext(FindRec);
  finally
    FindClose(FindRec);
  end;
end;

procedure UpdateStartupEntries();
begin
  if IsTaskSelected('autostart') then
  begin
    // Clear any earlier "disabled" mark (Task Manager or a previous install) on our own shortcut.
    RegDeleteValue(HKCU, ApprovedFolderKey, OwnStartupLnk);
    DisableOldAgOpenGPSStartupEntries();
  end
  else
  begin
    // Re-running setup without the task should actually turn auto-start off.
    DeleteFile(ExpandConstant('{userstartup}\') + OwnStartupLnk);
  end;
end;

// The installer runs without admin rights, but firewall rules need them, so this step runs
// an elevated PowerShell (one UAC prompt). Rules are per exe path, and every version has its
// own folder, so each install adds its own rules and leaves those of older versions alone.
function AddFirewallRules(): Boolean;
var
  Script: TArrayOfString;
  ScriptPath, AppDir: String;
  ErrCode: Integer;
begin
  ErrCode := -1;
  AppDir := ExpandConstant('{app}');
  StringChangeEx(AppDir, '''', '''''', True);
  SetArrayLength(Script, 14);
  Script[0] := '$ErrorActionPreference = ''Stop''';
  Script[1] := '$app = ''' + AppDir + '''';
  Script[2] := 'try {';
  Script[3] := '  foreach ($exe in ''AgIO.exe'', ''GPS_Out.exe'') {';
  Script[4] := '    $path = Join-Path $app $exe';
  Script[5] := '    $name = ''{#MyAppName} {#MyAppVersion} - '' + $exe';
  Script[6] := '    Remove-NetFirewallRule -DisplayName $name -ErrorAction SilentlyContinue';
  // Windows' first-run "allow access" prompt adds Block rules for the network types left
  // unticked, and a Block rule wins over any Allow rule, so those have to go.
  Script[7] := '    Get-NetFirewallApplicationFilter -Program $path -ErrorAction SilentlyContinue | Get-NetFirewallRule | Where-Object { $_.Action -eq ''Block'' } | Remove-NetFirewallRule';
  Script[8] := '    New-NetFirewallRule -DisplayName $name -Direction Inbound -Action Allow -Program $path -Profile Private,Public -Enabled True | Out-Null';
  Script[9] := '  }';
  Script[10] := '  exit 0';
  Script[11] := '} catch {';
  Script[12] := '  exit 1';
  Script[13] := '}';

  // UTF-8 with BOM, so Windows PowerShell 5.1 reads non-ASCII user names in the path correctly.
  ScriptPath := ExpandConstant('{tmp}\AddFirewallRules.ps1');
  Result := SaveStringsToUTF8File(ScriptPath, Script, False) and
    ShellExec('runas', 'powershell.exe',
      '-NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File "' + ScriptPath + '"',
      '', SW_HIDE, ewWaitUntilTerminated, ErrCode) and (ErrCode = 0);
  if not Result then
    Log('Adding firewall rules failed or was declined, code ' + IntToStr(ErrCode));
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep = ssInstall) and IsTaskSelected('backupdata') then
    BackupExistingData();
  if (CurStep = ssPostInstall) and IsTaskSelected('pintaskbar') then
    TryPinToTaskbar(ExpandConstant('{app}\{#MyAppExeName}'));
  if CurStep = ssPostInstall then
    UpdateStartupEntries();
  if (CurStep = ssPostInstall) and IsTaskSelected('firewall') then
    if not AddFirewallRules() then
      SuppressibleMsgBox(CustomMessage('FirewallFailed'), mbInformation, MB_OK, IDOK);
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if (CurPageID = wpFinished) and IsTaskSelected('pintaskbar') then
    WizardForm.FinishedLabel.Caption := WizardForm.FinishedLabel.Caption + #13#10#13#10 +
      CustomMessage('PinTaskbarNote');
end;
