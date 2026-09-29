using System;
using System.IO;
using System.Linq;
using MarkItDownDesktop.Models;
using MarkItDownDesktop.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.System;

namespace MarkItDownDesktop.Views;

public sealed partial class HistoryPage : Page
{
    public HistoryPage()
    {
        InitializeComponent();
        ApplyFilter();
    }

    private void Filter_Changed(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) => ApplyFilter();
    private void TypeFilter_Changed(object sender, SelectionChangedEventArgs e) => ApplyFilter();

    private void ApplyFilter()
    {
        if (SearchBox is null || TypeFilter is null || HistoryEmpty is null || HistoryList is null) return;
        var query = SearchBox.Text.Trim();
        var type = TypeFilter.SelectedIndex switch { 1 => ".pdf", 2 => ".docx", 3 => ".pptx", 4 => ".xlsx", _ => "" };
        var records = AppDataService.LoadHistory()
            .Where(r => r.SourceName.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Where(r => type.Length == 0 || (type == ".xlsx" ? Path.GetExtension(r.SourcePath) is ".xlsx" or ".xls" : Path.GetExtension(r.SourcePath).Equals(type, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(r => r.CompletedAt).ToList();
        HistoryEmpty.Visibility = records.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        HistoryList.Visibility = records.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        HistoryList.ItemsSource = records;
    }

    private async void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is ConversionRecord record && File.Exists(record.OutputPath))
            await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(record.OutputPath));
    }

    private async void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is ConversionRecord record && Directory.Exists(Path.GetDirectoryName(record.OutputPath)))
            await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(record.OutputPath)!));
    }
}
