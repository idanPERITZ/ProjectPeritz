using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using WcfService;   

namespace WpfHost
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ServiceHost adminServiceHost = new ServiceHost(typeof(ChessServiceAdmin));
            adminServiceHost.Open();

            ServiceHost userServiceHost = new ServiceHost(typeof(ChessServiceUser));
            userServiceHost.Open();
        }
        private void CloseService_Click(object sender, RoutedEventArgs e)
        {
            // הודעת סגירת השירות
            MessageBox.Show("Closing the service...");

            // סגירת השירות
            Application.Current.Shutdown();
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

    }
}