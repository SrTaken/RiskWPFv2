using Datos;
using Model;
using Newtonsoft.Json.Linq;
using Stomp.Net.Stomp;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RiskWPF
{
    /// <summary>
    /// Lógica de interacción para SalaCreationPage.xaml
    /// </summary>
    public partial class SalaCreationPage : Page
    {
        public SalaCreationPage()
        {
            InitializeComponent();
            Conection.OnMessageReceived += MensajeRecibidoWebSocket;
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
            if (json.Contains(Constants.RS.CrearSala))
            {
                Utils.sala = Utils.getSalaFromRequest(json);
                Conection.OnMessageReceived -= MensajeRecibidoWebSocket;
                this.NavigationService.Navigate(new PreJuegoPage());
            }
            
        }
        private async void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombreLobby.Text)) return;

            if (Utils.demo)
            {
                //Utils.sala.Nombre = txtNombreLobby.Text;
                //Utils.sala.Jugadores.Add(new Jugador(Utils.user));
                string json = "{\"response\":\"Sala creada con éxito\",\"sala\":{\"id\":4,\"nombre\":\"s\",\"jugadores\":[{\"id\":0,\"user_id\":3,\"nombre\":\"test1\",\"partida_id\":\"4\",\"estado\":false,\"colors\":null}],\"maxPlayers\":3}}";
                Utils.sala = Utils.getSalaFromRequest(json);
                this.NavigationService.Navigate(new PreJuegoPage());
            }
            else
            {
                await Conection.SendMessageSala(Utils.user, txtNombreLobby.Text);
                //string sala = await Conection.ReceiveMessage();
                //Utils.sala = Utils.getSalaFromRequest(sala);
            }
                
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is Frame))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent is Frame frame)
                frame.Content = null;
        }
    }
}
