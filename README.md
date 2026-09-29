# MarkItDownDesktop

MarkItDownDesktop 是 [Microsoft MarkItDown](https://github.com/microsoft/markitdown) 的独立 WinUI 3 桌面界面，与 Microsoft 无隶属关系。

## 下载和安装

**普通用户直接下载 [Windows x64 安装包](https://github.com/devinfenn/MarkItDownDesktop/releases/download/v0.1.4/MarkItDownDesktop-Setup-win-x64.exe)，双击运行并按向导选择安装位置即可。无需编译，也无需安装 Python。**

安装时可选择创建桌面快捷方式，以及添加文件右键菜单。安装包已自带 MarkItDown。

## 使用

- 从桌面快捷方式启动：打开图形界面，选择文件并转换。默认把 Markdown 保存到原文件旁边，也可在设置中修改输出目录。
- 右键文件选择“使用 MarkItDown 转换为 Markdown”：直接在该文件所在目录生成 `.md`，不打开图形界面。
- 如果目标目录已有同名 `.md`，程序会自动添加数字后缀，不覆盖原文件。

支持 PDF、DOCX、PPTX、XLSX、XLS、CSV、HTML 和 TXT。历史记录与设置保存在本机用户数据目录。

## 关于源码

仓库中的 `MarkItDownDesktop/` 和 `Packaging/` 是开发源码及打包脚本，普通用户无需使用。安装包内附有 MarkItDown 0.1.8 的 [MIT 许可证](Packaging/MARKITDOWN-LICENSE.txt)。
