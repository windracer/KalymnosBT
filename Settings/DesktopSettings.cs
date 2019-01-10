using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace KalymnosBT.Settings
{
    public class BoolToWindowStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return WindowState.Maximized;
            return WindowState.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ws = (WindowState)value;
            if (WindowState.Maximized == ws)
                return true;
            return false;
        }
    }

    public class DesktopSettings : LocalSettings
    {
        public static DesktopSettings Default { get; } = new DesktopSettings();
        protected override string AppName => Properties.Resources.AppTitle;

        //--------------------------------
        // Main window position and sizes:
        //--------------------------------
        public double MainWindowTopPosition { get => GetValue<double>(150); set => SetValue(value); }
        public double MainWindowLeftPosition { get => GetValue<double>(150); set => SetValue(value); }
        public double MainWindowHeight { get => GetValue<double>(500);  set => SetValue(value); }
        public double MainWindowWidth { get => GetValue<double>(850);  set => SetValue(value);  }
        public double MainWindowDetailsPanelWidth { get => GetValue<double>(200); set => SetValue(value); }
        public WindowState MainWindowState { get => GetValue<bool>(false, "MainWindowIsMaximized") ? WindowState.Maximized : WindowState.Normal; set { var isMaximized = WindowState.Maximized == value; SetValue(isMaximized, "MainWindowIsMaximized"); } }

        //--------------------------------
        // Main window position and sizes:
        //--------------------------------
        public double IssueWindowTopPosition { get => GetValue<double>(175); set => SetValue(value); }
        public double IssueWindowLeftPosition { get => GetValue<double>(175); set => SetValue(value); }
        public double IssueWindowHeight { get => GetValue<double>(450); set => SetValue(value); }
        public double IssueWindowWidth { get => GetValue<double>(800); set => SetValue(value); }
        public double IssueWindowDetailsPanelWidth { get => GetValue<double>(200); set => SetValue(value); }
        public WindowState IssueWindowState { get => GetValue<bool>(false, "IssueWindowIsMaximized") ? WindowState.Maximized : WindowState.Normal; set { var isMaximized = WindowState.Maximized == value; SetValue(isMaximized, "IssueWindowIsMaximized"); } }

        //--------------
        // GUI Settings:
        //--------------
        public string LastSelectedProject { get => GetValue<string>(); set => SetValue(value); }
        public bool ShowFixedIssuesSelected { get => GetValue<bool>(); set => SetValue(value); }
        public double ZoomFactor { get => GetValue<double>(1); set => SetValue(value); }

        public DesktopSettings()
        {
            //MainWindowHeight = 500;
            //MainWindowWidth = 850;
            //MainWindowLeftPosition = 150;
            //MainWindowTopPosition = 150;
            //MainWindowDetailsPanelWidth = MainWindowWidth / 4;

            //IssueWindowWidth = 800;
            //IssueWindowHeight = 450;
            //IssueWindowLeftPosition = 175;
            //IssueWindowTopPosition = 175;
            Load();

            if (ZoomFactor > 2 || ZoomFactor < 0.8)
                ZoomFactor = 1;
        }

    }
}
