using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MarkItDownDesktop.Models;

namespace MarkItDownDesktop.Services;

public static class ConversionService
{
    public static string BundledCommandPath => Path.Combine(AppContext.BaseDirectory, "Tools", "markitdown.exe");

    public static string ResolveCommand(AppSettings settings)
    {
        var configured = string.IsNullOrWhiteSpace(settings.MarkItDownCommand) ? "markitdown" : settings.MarkItDownCommand.Trim();
        return configured.Equals("markitdown", StringComparison.OrdinalIgnoreCase) && File.Exists(BundledCommandPath)
            ? BundledCommandPath : configured;
    }

    public static async Task<string> ConvertAsync(string sourcePath, AppSettings settings)
    {
        if (!File.Exists(sourcePath)) throw new FileNotFoundException("原文件不存在。", sourcePath);
        var folder = settings.UseCustomFolder ? settings.CustomFolder : Path.GetDirectoryName(sourcePath)!;
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            throw new DirectoryNotFoundException("输出目录不存在，请在设置中重新选择。");
        var output = GetAvailableOutputPath(folder, Path.GetFileNameWithoutExtension(sourcePath));
        var command = ResolveCommand(settings);
        var startInfo = new ProcessStartInfo(command)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };
        startInfo.ArgumentList.Add(sourcePath);
        startInfo.ArgumentList.Add("-o");
        startInfo.ArgumentList.Add(output);
        try
        {
            using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("无法启动 MarkItDown。");
            var errors = process.StandardError.ReadToEndAsync();
            var standardOutput = process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            var errorText = await errors;
            _ = await standardOutput;
            if (process.ExitCode != 0)
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(errorText) ? $"MarkItDown 退出代码：{process.ExitCode}" : errorText.Trim());
            if (!File.Exists(output)) throw new IOException("MarkItDown 已退出，但没有生成 Markdown 文件。");
            return output;
        }
        catch (Win32Exception ex)
        {
            throw new InvalidOperationException("找不到 MarkItDown 命令。请先安装 MarkItDown，或在设置中填写可执行文件的完整路径。", ex);
        }
    }

    private static string GetAvailableOutputPath(string folder, string name)
    {
        var path = Path.Combine(folder, name + ".md");
        for (var number = 2; File.Exists(path); number++) path = Path.Combine(folder, $"{name} ({number}).md");
        return path;
    }
}
