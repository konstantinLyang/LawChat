using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LawChat.Client.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : System.Windows.Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        Rect _rec = SystemParameters.WorkArea;

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
                if (Width == _rec.Size.Width && Height == _rec.Size.Height)
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
