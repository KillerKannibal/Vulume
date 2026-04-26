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
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
  DownloadUrl: string;
begin
  if (CurStep = ssPostInstall) and IsComponentSelected('hideosd') then
  begin
    // Updated stable link for the 1.4 version
    DownloadUrl := 'http://wordpress.venturi.de/wp-content/uploads/2022/12/HideVolumeOSD-1.4.exe';
    
    if MsgBox('The installer will now open the download for HideVolumeOSD. Please install it to hide the default Windows volume bar. Continue?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      // Using SW_SHOWNORMAL and ewNoWait to ensure it pops up over the installer
      if not ShellExec('open', DownloadUrl, '', '', SW_SHOWNORMAL, ewNoWait, ResultCode) then
      begin
        MsgBox('Failed to open download link. Please visit: http://wordpress.venturi.de/?p=379 manually.', mbError, MB_OK);
      end;
    end;
  end;
end;

[Run]
Filename: "{app}\VolumeHUD.exe"; Description: "Launch Vulume"; Flags: nowait postinstall skipifsilent