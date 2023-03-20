using System.Windows;
using System.Windows.Input;

namespace lawChat.Client.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        Rect rec = SystemParameters.WorkArea;

        public LoginWindow()
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
