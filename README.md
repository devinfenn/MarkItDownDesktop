# MarkItDownDesktop

An independent WinUI 3 desktop interface for [Microsoft MarkItDown](https://github.com/microsoft/markitdown). This project is not affiliated with Microsoft.

The installer bundles MarkItDown 0.1.8. Its [MIT license](Packaging/MARKITDOWN-LICENSE.txt) is included with the distribution.

## 使用

- 从 GitHub Releases 下载 `MarkItDownDesktop-Setup-win-x64.exe`，在向导中选择安装位置。
- 安装包自带 MarkItDown，无需用户安装 Python。
- 安装版可在安装向导中勾选文件右键菜单。右键选择“使用 MarkItDown 转换为 Markdown”会直接在原文件目录生成同名 `.md` 文件；若已存在，则依次使用 ` (2)`、` (3)` 等后缀，不打开桌面界面。

支持 PDF、DOCX、PPTX、XLSX、XLS、CSV、HTML 和 TXT。默认输出到原文件目录；可在设置中修改。历史记录与设置保存在本机用户数据目录。

## 构建

1. 用 Visual Studio 打开 `MarkItDownDesktop.slnx`，安装 .NET 8 和 Windows App SDK 开发工具。
2. 运行 `Packaging/build-release.ps1 -PythonExe <Python 3.12 路径>`，构建自带 CLI 的 x64 程序。
3. 安装 Inno Setup 7 后运行 `Packaging/build-installer.ps1 -InnoCompiler <ISCC.exe 路径>`，生成安装向导。

构建产物位于 `Releases/`，临时文件位于 `.build/`。发布前请分别测试安装、转换和卸载。
