using MarkItDownDesktop.Views;
using MarkItDownDesktop.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;
using System;
using System.IO;

namespace MarkItDownDesktop;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Navigation.RequestedTheme = AppDataService.LoadSettings().Theme switch
        {
            1 => ElementTheme.Light,
            2 => ElementTheme.Dark,
            _ => ElementTheme.Default
        };
        var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(this)));
        appWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "AppIcon.ico"));
        appWindow.Resize(new Windows.Graphics.SizeInt32(1040, 740));
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumWidth = 720;
            presenter.PreferredMinimumHeight = 560;
        }
        Navigation.SelectedItem = Navigation.MenuItems[0];
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var page = args.IsSettingsSelected ? typeof(SettingsPage) :
            (args.SelectedItemContainer?.Tag?.ToString() == "history" ? typeof(HistoryPage) : typeof(HomePage));
        if (ContentFrame.CurrentSourcePageType != page) ContentFrame.Navigate(page);
    }
}
