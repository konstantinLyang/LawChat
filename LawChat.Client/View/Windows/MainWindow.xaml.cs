using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace LawChat.Client.View.Windows
{
    public partial class MainWindow : Window
    {
        private bool _restoreIfMove;
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
            this.Hide();
        }
        private void BtnMinimize_Click_1(object sender, RoutedEventArgs e)
        {
            SwitchWindowState();
        }
        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;

                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs eventArgs)
        {
            if (eventArgs.ClickCount == 2)
            {
                if (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip)
                {
                    SwitchWindowState();
                }

                return;
            }

            if (WindowState == WindowState.Maximized)
            {
                _restoreIfMove = true;
                return;
            }

            DragMove();
        }
        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer scrollViewer &&
                Math.Abs(e.ExtentHeightChange) > 0.0)
            {
                scrollViewer.ScrollToBottom();
            }
        }

        #region SetMaxMinInfo

        private static IntPtr ProcessWindow(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    SetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private static void SetMinMaxInfo(IntPtr hwnd, IntPtr minMaxInfoPointer)
        {
            GetCursorPos(out var mousePosition);
            var currentScreenPointer = MonitorFromPoint(mousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);
            var currentScreen = new MONITORINFO();

            if (!GetMonitorInfo(currentScreenPointer, currentScreen))
            {
                return;
            }

            MINMAXINFO minMaxInfo = (MINMAXINFO)Marshal.PtrToStructure(minMaxInfoPointer, typeof(MINMAXINFO))!;
            minMaxInfo.ptMaxPosition.X = currentScreen.rcWork.Left - currentScreen.rcMonitor.Left;
            minMaxInfo.ptMaxPosition.Y = currentScreen.rcWork.Top - currentScreen.rcMonitor.Top;
            minMaxInfo.ptMaxSize.X = currentScreen.rcWork.Right - currentScreen.rcWork.Left;
            minMaxInfo.ptMaxSize.Y = currentScreen.rcWork.Bottom - currentScreen.rcWork.Top;

            Marshal.StructureToPtr(minMaxInfo, minMaxInfoPointer, true);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT point);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr MonitorFromPoint(POINT point, MonitorOptions dwFlags);

        private enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        #endregion
    }
}
