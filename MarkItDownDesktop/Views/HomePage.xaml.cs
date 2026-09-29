using System;
using System.Linq;
using System.IO;
using MarkItDownDesktop.Models;
using MarkItDownDesktop.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using WinRT.Interop;

namespace MarkItDownDesktop.Views;

public sealed partial class HomePage : Page
{
    private static readonly string[] Extensions = [".pdf", ".docx", ".pptx", ".xlsx", ".xls", ".csv", ".html", ".txt"];
    private string? _selectedPath;

    public HomePage()
    {
        InitializeComponent();
        RefreshRecent();
    }

    private void RefreshRecent()
    {
        var recent = AppDataService.LoadHistory().OrderByDescending(r => r.CompletedAt).Take(3).ToList();
        RecentEmpty.Visibility = recent.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        RecentList.Visibility = recent.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        RecentList.ItemsSource = recent;
    }

    private async void ChooseFile_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(((App)Application.Current).MainWindow));
        foreach (var extension in Extensions) picker.FileTypeFilter.Add(extension);
        var file = await picker.PickSingleFileAsync();
        if (file is not null) await ShowFileAsync(file);
    }

    private async System.Threading.Tasks.Task ShowFileAsync(StorageFile file)
    {
        if (!Extensions.Contains(file.FileType.ToLowerInvariant()))
        {
            ConversionInfo.Title = "不支持的文件格式";
            ConversionInfo.Message = "请选择 PDF、Office 文档、CSV、HTML 或 TXT 文件。";
            ConversionInfo.Severity = InfoBarSeverity.Warning;
            ConversionInfo.IsOpen = true;
            return;
        }
        var properties = await file.GetBasicPropertiesAsync();
        _selectedPath = file.Path;
        FileNameText.Text = file.Name;
        FileDetailsText.Text = $"{file.FileType.TrimStart('.').ToUpperInvariant()} 文件 · {FormatSize(properties.Size)}";
        OutputNameText.Text = System.IO.Path.GetFileNameWithoutExtension(file.Name) + ".md";
        EmptyContent.Visibility = Visibility.Collapsed;
        SelectedContent.Visibility = Visibility.Visible;
        ConversionInfo.IsOpen = false;
    }

    private static string FormatSize(ulong size) => size >= 1024 * 1024 ? $"{size / 1048576d:0.#} MB" : $"{size / 1024d:0.#} KB";

    private async void Convert_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedPath is null) return;
        ConvertButton.IsEnabled = false;
        ConvertButton.Content = "转换中";
        ConversionProgress.Visibility = Visibility.Visible;
        ConversionInfo.IsOpen = false;
        try
        {
            var settings = AppDataService.LoadSettings();
            var output = await ConversionService.ConvertAsync(_selectedPath, settings);
            var history = AppDataService.LoadHistory();
            history.Insert(0, new ConversionRecord { SourcePath = _selectedPath, OutputPath = output, CompletedAt = DateTimeOffset.Now });
            AppDataService.SaveHistory(history);
            OutputNameText.Text = Path.GetFileName(output);
            RefreshRecent();
            ConversionInfo.Title = "转换完成";
            ConversionInfo.Message = $"已保存到 {output}";
            ConversionInfo.Severity = InfoBarSeverity.Success;
            if (settings.OpenMarkdownAfterConversion)
                await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(output));
            if (settings.ShowInExplorerAfterConversion)
                await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(output)!));
        }
        catch (Exception ex)
        {
            ConversionInfo.Title = "转换失败";
            ConversionInfo.Message = ex.Message;
            ConversionInfo.Severity = InfoBarSeverity.Error;
        }
        finally
        {
            ConvertButton.IsEnabled = true;
            ConvertButton.Content = "开始转换";
            ConversionProgress.Visibility = Visibility.Collapsed;
            ConversionInfo.IsOpen = true;
        }
    }

    private void DropZone_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
        e.DragUIOverride.Caption = "选择要转换的文件";
    }

    private async void DropZone_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.FirstOrDefault() is StorageFile file) await ShowFileAsync(file);
        }
        ResetBorder();
    }

    private void DropZone_PointerEntered(object sender, PointerRoutedEventArgs e) => DropZone.BorderBrush = (Brush)Application.Current.Resources["AccentFillColorDefaultBrush"];
    private void DropZone_PointerExited(object sender, PointerRoutedEventArgs e) => ResetBorder();
    private void ResetBorder() => DropZone.BorderBrush = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"];
}
