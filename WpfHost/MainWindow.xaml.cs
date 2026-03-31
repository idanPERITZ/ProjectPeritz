using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using WcfService;

namespace WpfHost
{
    // Main window for the WCF service host application
    // Responsible for starting and stopping the chess WCF services
    public partial class MainWindow : Window
    {
        // Constructor: Initializes the window and starts both WCF services
        public MainWindow()
        {
            // Initialize WPF components
            InitializeComponent();

            // Create and start the admin service host
            // Handles all admin operations (game management, user management, etc.)
            ServiceHost adminServiceHost = new ServiceHost(typeof(ChessServiceAdmin));
            adminServiceHost.Open();

            // Create and start the user service host
            // Handles all regular user operations (viewing games, moves, etc.)
            ServiceHost userServiceHost = new ServiceHost(typeof(ChessServiceUser));
            userServiceHost.Open();
        }

        // Event handler: Closes the service when the stop button is clicked
        private void CloseService_Click(object sender, RoutedEventArgs e)
        {
            // Show confirmation message before closing
            MessageBox.Show("Closing the service...");
            // Shut down the entire application including all service hosts
            Application.Current.Shutdown();
        }

        // Event handler: Allows the window to be dragged by clicking the top bar
        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Only drag if left mouse button is pressed
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}