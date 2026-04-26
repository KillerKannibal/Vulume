using System;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

// Aliases to prevent conflict between WPF and WinForms
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using ContextMenuStrip = System.Windows.Forms.ContextMenuStrip;

namespace VolumeHUD {
    public partial class MainWindow : Window {
        private CoreAudioController audioController = new CoreAudioController();
        private DispatcherTimer fadeTimer = new DispatcherTimer();
        private NotifyIcon? _trayIcon;
        private SettingsWindow? _settingsWin;

        // Configuration State
        private string _currentHex = "#00FF41";
        private double _verticalOffset = 100;
        private bool _forceVisible = false;

        public MainWindow() {
            InitializeComponent();
            this.Hide();

            // 1. Setup Tray Icon
            try {
                System.Drawing.Icon? appIcon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
                _trayIcon = new NotifyIcon {
                    Icon = appIcon ?? System.Drawing.SystemIcons.Application,
                    Visible = true,
                    Text = "Vulume Settings"
                };

                var menu = new ContextMenuStrip();
                menu.Items.Add("Settings", null, (s, e) => ShowSettings());
                menu.Items.Add("Exit", null, (s, e) => System.Windows.Application.Current.Shutdown());
                _trayIcon.ContextMenuStrip = menu;
            } catch {
                // Fallback for environment issues
            }

            // 2. Setup Hide Timer
            fadeTimer.Interval = TimeSpan.FromSeconds(2);
            fadeTimer.Tick += (s, e) => { 
                if (!_forceVisible) {
                    this.Hide(); 
                    fadeTimer.Stop(); 
                }
            };

            // 3. Setup Audio Listener
            var observer = new VolumeObserver(vol => {
                System.Windows.Application.Current.Dispatcher.Invoke(() => UpdateUI(vol));
            });

            audioController.DefaultPlaybackDevice.VolumeChanged.Subscribe(observer);
            
            ApplyStyles();
        }

        private void ShowSettings() {
            if (_settingsWin != null && _settingsWin.IsLoaded) {
                _settingsWin.Activate();
                return;
            }

            _settingsWin = new SettingsWindow(_currentHex, _verticalOffset);
            _forceVisible = true;
            UpdateUI(audioController.DefaultPlaybackDevice.Volume);

            _settingsWin.SettingsChanged += (newHex, newOffset) => {
                _currentHex = newHex;
                _verticalOffset = newOffset;
                ApplyStyles();
                UpdateUI(audioController.DefaultPlaybackDevice.Volume);
            };

            _settingsWin.Closed += (s, e) => {
                _forceVisible = false;
                fadeTimer.Start(); 
            };

            _settingsWin.Show();
        }

        private void ApplyStyles() {
            if (MainBorder == null || VolBar == null) return;

            try {
                var converter = new System.Windows.Media.BrushConverter();
                // Resolved ambiguity by using full namespace
                var brush = (System.Windows.Media.Brush)converter.ConvertFromString(_currentHex)!;
                
                MainBorder.BorderBrush = brush;
                VolBar.Foreground = brush;
                VolLabel.Foreground = brush; 
                
                if (MainBorder.Effect is DropShadowEffect shadow) {
                    shadow.Opacity = 0.7; 
                    if (brush is SolidColorBrush scb) {
                        shadow.Color = scb.Color;
                    }
                }
            } catch { 
                // Handles invalid hex input during typing
            }
        }   

        private void UpdateUI(double volume) {
            bool isMuted = audioController.DefaultPlaybackDevice.IsMuted;
            VolBar.Value = volume;

            // Visibility Toggles
            VolLabel.Visibility = isMuted ? Visibility.Collapsed : Visibility.Visible;
            MuteLabel.Visibility = isMuted ? Visibility.Visible : Visibility.Collapsed;

            if (isMuted) {
                // Resolved ambiguity: System.Windows.Media.Color
                var greyBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(80, 80, 80));
                MainBorder.BorderBrush = greyBrush;
                VolBar.Foreground = greyBrush;
                
                if (MainBorder.Effect is DropShadowEffect shadow) {
                    shadow.Opacity = 0; 
                }
            } else {
                ApplyStyles();
            }

            // Window Positioning
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height - _verticalOffset;
            
            this.Topmost = false;
            this.Topmost = true;
            this.Show();

            if (!_forceVisible) {
                fadeTimer.Stop();
                fadeTimer.Start();
            }
        }

        // --- Win32 Click-through logic ---
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }

    public class VolumeObserver : IObserver<DeviceVolumeChangedArgs> {
        private readonly Action<double> _onVolumeChanged;
        public VolumeObserver(Action<double> onVolumeChanged) => _onVolumeChanged = onVolumeChanged;
        public void OnNext(DeviceVolumeChangedArgs value) => _onVolumeChanged(value.Volume);
        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }
}