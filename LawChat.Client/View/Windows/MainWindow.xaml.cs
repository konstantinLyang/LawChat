using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace lawChat.Client.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            Environment.Exit(0);
        }
        private void BtnHide_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void BtnMinimize_Click_1(object sender, RoutedEventArgs e)
        {
            var screen = WpfScreenHelper.Screen.FromWindow(this);
            if (screen != null)
            {
                if (Width == screen.WpfWorkingArea.Width && Height == screen.WpfWorkingArea.Height && Top == screen.WpfWorkingArea.Top && Left == screen.WpfWorkingArea.Left)
                {
                    this.WindowState = WindowState.Normal;
                    this.Left = screen.WpfWorkingArea.Left / 2;
                    this.Top = screen.WpfWorkingArea.Top / 2;
                    this.Width = MinWidth;
                    this.Height = MinHeight;
                }
                else if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Normal;
                    this.Left = screen.WpfWorkingArea.Left;
                    this.Top = screen.WpfWorkingArea.Top;
                    this.Width = screen.WpfWorkingArea.Width;
                    this.Height = screen.WpfWorkingArea.Height;
                }
            }
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                var screen = WpfScreenHelper.Screen.FromWindow(this);
                if (screen != null)
                {
                    this.WindowState = WindowState.Normal;
                    this.Left = screen.WpfWorkingArea.Left;
                    this.Top = screen.WpfWorkingArea.Top;
                    this.Width = screen.WpfWorkingArea.Width;
                    this.Height = screen.WpfWorkingArea.Height;
                }
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var screen = WpfScreenHelper.Screen.FromWindow(this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Width == screen.WpfWorkingArea.Width
                    && Height == screen.WpfWorkingArea.Height
                    && screen != null)
                {

                    this.WindowState = WindowState.Normal;
                    this.Left = WpfScreenHelper.MouseHelper.MousePosition.X - MinWidth / 2;
                    this.Top = screen.WpfWorkingArea.Top;
                    this.Width = MinWidth;
                    this.Height = MinHeight;
                }
                DragMove();
            }
        }
        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer scrollViewer &&
                Math.Abs(e.ExtentHeightChange) > 0.0)
            {
                scrollViewer.ScrollToBottom();
            }
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("explorer.exe", e.Uri.OriginalString);
            e.Handled = true;
        }
    }
}
