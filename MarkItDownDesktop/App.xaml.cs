using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using MarkItDownDesktop.Models;
using MarkItDownDesktop.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarkItDownDesktop
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public Window MainWindow { get; private set; } = null!;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var commandLine = Environment.GetCommandLineArgs();
            if (commandLine.Length >= 3 && commandLine[1] == "--convert-here")
            {
                try
                {
                    var settings = new AppSettings { UseCustomFolder = false, MarkItDownCommand = ConversionService.BundledCommandPath };
                    await ConversionService.ConvertAsync(commandLine[2], settings);
                }
                catch (Exception ex)
                {
                    MessageBox(IntPtr.Zero, ex.Message, "MarkItDown 转换失败", 0x10);
                }
                finally { Exit(); }
                return;
            }
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr window, string text, string caption, uint type);
    }
}
