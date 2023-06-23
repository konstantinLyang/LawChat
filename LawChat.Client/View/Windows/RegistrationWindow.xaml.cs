using System.Windows;

namespace LawChat.Client.View.Windows
{
    public partial class RegistrationWindow
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        public override void HandleCloseButtonClick(object sender, RoutedEventArgs e) => Hide();
    }
}
