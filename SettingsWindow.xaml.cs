using System;
using System.Windows;
using System.Windows.Media;

namespace VolumeHUD
{
    public partial class SettingsWindow : Window
    {
        public event Action<string, double>? SettingsChanged;

        public SettingsWindow(string currentColor, double currentOffset)
        {
            InitializeComponent();

            // Set the initial slider value
            if (OffsetSlider != null) 
                OffsetSlider.Value = currentOffset;

            // Initialize the ColorPicker and Hex box
            try
            {
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (System.Windows.Media.SolidColorBrush)converter.ConvertFromString(currentColor)!;
                
                if (HUDColorPicker != null)
                    HUDColorPicker.SelectedColor = brush.Color;
                
                if (ColorInput != null)
                    ColorInput.Text = currentColor;
            }
            catch
            {
                if (HUDColorPicker != null)
                    HUDColorPicker.SelectedColor = System.Windows.Media.Color.FromRgb(0, 255, 65);
                if (ColorInput != null)
                    ColorInput.Text = "#00FF41";
            }
        }

        private void HUDColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                System.Windows.Media.Color c = e.NewValue.Value;
                string hex = $"#{c.R:X2}{c.G:X2}{c.B:X2}";

                if (ColorInput != null) ColorInput.Text = hex;

                SettingsChanged?.Invoke(hex, OffsetSlider.Value);
            }
        }

        private void OffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ColorInput != null && OffsetSlider != null)
            {
                SettingsChanged?.Invoke(ColorInput.Text, e.NewValue);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ColorInput != null && OffsetSlider != null)
            {
                SettingsChanged?.Invoke(ColorInput.Text, OffsetSlider.Value);
            }
            this.Close();
        }
    }
}