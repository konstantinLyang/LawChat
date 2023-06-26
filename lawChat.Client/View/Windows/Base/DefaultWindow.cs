using System.Runtime.InteropServices;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace LawChat.Client.View.Windows.Base
{
    public class DefaultWindow : Window
    {
        private bool _restoreIfMove;

        public void Grid_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }

        public void HandleMinimazeButtonClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        public void HandleMaximazeButtonClick(object sender, RoutedEventArgs e) => SwitchWindowState();

        public virtual void HandleCloseButtonClick(object sender, RoutedEventArgs e) => Close();

        public void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(mWindowHandle)!.AddHook(ProcessWindow);
        }

        public void HandleTopBarPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs eventArgs)
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

        public void HandleTopBarPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _restoreIfMove = false;

        public void HandleTopBarPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_restoreIfMove)
            {
                return;
            }

            _restoreIfMove = false;
            var mousePosition = e.GetPosition(this);
            double percentHorizontal = mousePosition.X / ActualWidth;
            double targetHorizontal = RestoreBounds.Width * percentHorizontal;

            double percentVertical = mousePosition.Y / ActualHeight;
            double targetVertical = RestoreBounds.Height * percentVertical;

            WindowState = WindowState.Normal;

            GetCursorPos(out var globalMousePosition);
            Left = globalMousePosition.X - targetHorizontal;
            Top = globalMousePosition.Y - targetVertical;

            DragMove();
        }

        public void SwitchWindowState()
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

        #region SetMaxMinInfo

        public static IntPtr ProcessWindow(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    SetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        public static void SetMinMaxInfo(IntPtr hwnd, IntPtr minMaxInfoPointer)
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
        public static extern bool GetCursorPos(out POINT point);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromPoint(POINT point, MonitorOptions dwFlags);

        public enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

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
            public RECT rcMonitor = new();
            public RECT rcWork = new();
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
