using Datos;
using Model;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RiskWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!Utils.demo)
            {
                Conection.startConection();

                //Conection.StartListening();
                Conection.OnMessageReceived += MensajeRecibidoWebSocket;
            }
                
        }

        private void MensajeRecibidoWebSocket(string json)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProcesaMensajeDelServidor(json);
            });
        }

        private void ProcesaMensajeDelServidor(string json)
        {

            if (json == null)
            {
                MessageBox.Show("Unexpected Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (JObject.Parse(json)?.Property("code")?.Value.ToString() == "200")
            {
                Utils.user = Utils.GetUserFromRequest(json);

                MenuWindow menuWindow = new MenuWindow();
                menuWindow.Show();
                Conection.OnMessageReceived -= MensajeRecibidoWebSocket;
                this.Close();
            }
            else if (JObject.Parse(json)?.Property("code")?.Value.ToString() == "422")
            {
                MessageBox.Show("Invalid username or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (JObject.Parse(json)?.Property("code")?.Value.ToString() == "101")
            {
                MessageBox.Show("Timeout", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Unexpected Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return;

            Utils.user = new User(username, password);

            string result = "";
            if (Utils.demo)
            {
                result = "{\"response\":\"loginRS\",\"status\":\"Bien\",\"code\":200,\"user\":{\"id\":3,\"nom\":\"test3\",\"login\":\"test3\",\"password\":\"81DC9BDB52D04DC20036DBD8313ED055\",\"avatar\":\"\",\"wins\":0,\"games\":0}}";

                if (result == null)
                {
                    MessageBox.Show("Unexpected Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (JObject.Parse(result)?.Property("code")?.Value.ToString() == "200")
                {
                    Utils.user = Utils.GetUserFromRequest(result);

                    MenuWindow menuWindow = new MenuWindow();
                    menuWindow.Show();
                    this.Close();
                }
                else if (JObject.Parse(result)?.Property("code")?.Value.ToString() == "422")
                {
                    MessageBox.Show("Invalid username or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (JObject.Parse(result)?.Property("code")?.Value.ToString() == "101")
                {
                    MessageBox.Show("Timeout", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Unexpected Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    await Conection.SendMessage(Utils.user, Constants.RQ.Login);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting to the server: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //result = await Conection.ReceiveMessage();
            }


        }
    }
}