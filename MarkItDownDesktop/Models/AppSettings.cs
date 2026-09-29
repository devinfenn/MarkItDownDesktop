namespace MarkItDownDesktop.Models;

public sealed class AppSettings
{
    public bool UseCustomFolder { get; set; }
    public string CustomFolder { get; set; } = "";
    public bool OpenMarkdownAfterConversion { get; set; }
    public bool ShowInExplorerAfterConversion { get; set; }
    public int Theme { get; set; }
    public string MarkItDownCommand { get; set; } = "markitdown";
}
