#define AppName "MarkItDownDesktop"
#define AppVersion "0.1.4"
#define PublishDir "..\.build\publish"

[Setup]
AppId={{36012f5a-3d36-437b-9afa-e0b0eb4ede93}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
DefaultDirName={localappdata}\Programs\{#AppName}
DefaultGroupName={#AppName}
DisableDirPage=no
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=..\Releases
OutputBaseFilename=MarkItDownDesktop-Setup-win-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\MarkItDownDesktop.exe
SetupIconFile=..\MarkItDownDesktop\Assets\AppIcon.ico
CloseApplications=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "创建桌面快捷方式"; GroupDescription: "其他选项："; Flags: unchecked
Name: "contextmenu"; Description: "添加文件右键菜单：转换为 Markdown（保存到当前目录）"; GroupDescription: "其他选项："; Flags: checkedonce

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\MarkItDownDesktop.exe"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\MarkItDownDesktop.exe"; Tasks: desktopicon

[Registry]
Root: HKA; Subkey: "Software\Classes\*\shell\MarkItDownDesktopConvert"; ValueType: string; ValueName: ""; ValueData: "使用 MarkItDown 转换为 Markdown"; Tasks: contextmenu; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\*\shell\MarkItDownDesktopConvert"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\MarkItDownDesktop.exe"; Tasks: contextmenu
Root: HKA; Subkey: "Software\Classes\*\shell\MarkItDownDesktopConvert\command"; ValueType: string; ValueName: ""; ValueData: """{app}\MarkItDownDesktop.exe"" --convert-here ""%1"""; Tasks: contextmenu

[Run]
Filename: "{app}\MarkItDownDesktop.exe"; Description: "启动 {#AppName}"; Flags: nowait postinstall skipifsilent
