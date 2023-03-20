using System.Windows;
using System.Windows.Input;

namespace lawChat.Client.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindowView.xaml
    /// </summary>
    public partial class LoginWindowView : Window
    {
        Rect rec = SystemParameters.WorkArea;

        public LoginWindowView()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BtnHide_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void BtnMinimize_Click_1(object sender, RoutedEventArgs e)
        {
            if (Width == rec.Size.Width && Height == rec.Size.Height && Top == rec.Top && Left == rec.Left)
            {
                Width = MinWidth;
                Height = MinHeight;
                Top = (SystemParameters.FullPrimaryScreenHeight - Height) / 2;
                Left = (SystemParameters.FullPrimaryScreenWidth - Width) / 2;
            }

            else if (this.WindowState == WindowState.Normal)
            {
                Width = rec.Size.Width;
                Height = rec.Size.Height;
                Top = rec.Top;
                Left = rec.Left;
            }
        }
        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                Width = rec.Size.Width;
                Height = rec.Size.Height;
                Top = rec.Top;
                Left = rec.Left;
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Width == rec.Size.Width && Height == rec.Size.Height)
                {
                    Width = MinWidth;
                    Height = MinHeight;
                    Left = e.GetPosition(null).X - Width / 2;
                }
                DragMove();
            }
        }

    }
}
