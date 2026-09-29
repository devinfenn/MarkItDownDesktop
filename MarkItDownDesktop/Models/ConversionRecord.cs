using System;
using System.IO;

namespace MarkItDownDesktop.Models;

public sealed class ConversionRecord
{
    public string SourcePath { get; set; } = "";
    public string OutputPath { get; set; } = "";
    public DateTimeOffset CompletedAt { get; set; }
    public string SourceName => Path.GetFileName(SourcePath);
    public string OutputName => Path.GetFileName(OutputPath);
    public string FileType => Path.GetExtension(SourcePath).TrimStart('.').ToUpperInvariant();
    public string Time => CompletedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
    public string Status => "已完成";
}
