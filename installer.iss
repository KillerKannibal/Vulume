[Setup]
AppName=Vulume
AppVersion=1.0
AppPublisher=KillerKannibal
DefaultDirName={autopf}\Vulume
DefaultGroupName=Vulume
UninstallDisplayIcon={app}\VolumeHUD.exe
Compression=lzma
SolidCompression=yes
OutputDir=userdocs:VulumeInstaller
OutputBaseFilename=Vulume_Setup
; This ensures the installer runs as admin to handle the OSD install
PrivilegesRequired=admin 

[Tasks]
Name: "startup"; Description: "Run Vulume on Windows Startup"; Flags: unchecked

[Files]
; HUD Core Files
Source: "C:\Users\Yeetb\Documents\Vulume\VolumeHUD\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Vulume"; Filename: "{app}\VolumeHUD.exe"
Name: "{autodesktop}\Vulume"; Filename: "{app}\VolumeHUD.exe"
Name: "{userstartup}\Vulume"; Filename: "{app}\VolumeHUD.exe"; Tasks: startup

[Components]
Name: "main"; Description: "Vulume HUD Core"; Types: full compact custom; Flags: fixed
Name: "hideosd"; Description: "HideVolumeOSD (Removes Windows Default Volume Bar)"; Types: full

[Code]
// This function launches the download in the user's default browser
// It's the most reliable way to handle external dependencies without DLL conflicts
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
  DownloadUrl: string;
begin
  if (CurStep = ssPostInstall) and IsComponentSelected('hideosd') then
  begin
    DownloadUrl := 'https://github.com/marcusheider/HideVolumeOSD/releases/download/v1.4/HideVolumeOSD-1.4.exe';
    
    if MsgBox('The installer will now open the download for HideVolumeOSD. Please install it to hide the default Windows volume bar. Continue?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', DownloadUrl, '', '', SW_SHOWNORMAL, ewNoWait, ResultCode);
    end;
  end;
end;

[Run]
Filename: "{app}\VolumeHUD.exe"; Description: "Launch Vulume"; Flags: nowait postinstall skipifsilent