using System;
using System.IO;
using MarkItDownDesktop.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace MarkItDownDesktop.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        var settings = AppDataService.LoadSettings();
        OutputLocation.SelectedIndex = settings.UseCustomFolder ? 1 : 0;
        CustomFolderPath.Text = settings.CustomFolder;
        OpenAfterToggle.IsOn = settings.OpenMarkdownAfterConversion;
        ShowFolderToggle.IsOn = settings.ShowInExplorerAfterConversion;
        ThemePicker.SelectedIndex = settings.Theme is >= 0 and <= 2 ? settings.Theme : 0;
        CommandText.Text = settings.MarkItDownCommand;
        CliStatusText.Text = File.Exists(ConversionService.BundledCommandPath) ? "已内置" : "未检测";
        Unloaded += (_, _) => SaveSettings();
    }

    private void SaveSettings()
    {
        AppDataService.SaveSettings(new()
        {
            UseCustomFolder = OutputLocation.SelectedIndex == 1,
            CustomFolder = CustomFolderPath.Text,
            OpenMarkdownAfterConversion = OpenAfterToggle.IsOn,
            ShowInExplorerAfterConversion = ShowFolderToggle.IsOn,
            Theme = ThemePicker.SelectedIndex,
            MarkItDownCommand = CommandText.Text
        });
    }

    private void OutputLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CustomFolderRow is not null) CustomFolderRow.Visibility = OutputLocation.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void BrowseFolder_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FolderPicker();
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(((App)Application.Current).MainWindow));
        picker.FileTypeFilter.Add("*");
        var folder = await picker.PickSingleFolderAsync();
        if (folder is not null) { CustomFolderPath.Text = folder.Path; SaveSettings(); }
    }

    private void ThemePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (XamlRoot?.Content is FrameworkElement root)
            root.RequestedTheme = ThemePicker.SelectedIndex switch { 1 => ElementTheme.Light, 2 => ElementTheme.Dark, _ => ElementTheme.Default };
        if (OpenAfterToggle is not null) SaveSettings();
    }
}
