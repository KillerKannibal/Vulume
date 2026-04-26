using System;
using System.Windows;
using System.Threading;

namespace VolumeHUD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private static Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
           
            _mutex = new Mutex(true, "Vulume-Unique-ID-12345", out bool createdNew);

            if (!createdNew)
            {
                // App is already running! Show a message and kill this second instance
                System.Windows.MessageBox.Show("Vulume is already running in the system tray.", "Vulume");
                System.Windows.Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
        
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
            }
            base.OnExit(e);
        }
    } 
}