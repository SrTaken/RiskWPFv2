using Datos;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Lógica de interacción para PreJuegoPage.xaml
    /// </summary>
    public partial class PreJuegoPage : Page
    {
        private List<string> colores = new() { "VERMELL", "BLAU", "VERD", "GROC" };

        public PreJuegoPage()
        {
            InitializeComponent();

            cboColor.ItemsSource = colores;
            

            if (Utils.demo)
            {
                Utils.sala.Jugadores.Add(new Jugador(98, "Usuario1", "Rojo", true));
                Utils.sala.Jugadores.Add(new Jugador(99, "Usuario2", "Azul", true));
                Utils.sala.Jugadores.Add(new Jugador(100, "Usuario3", "Amarillo", true));
                //Utils.partida.jugadorList.Add(new Jugador(Utils.user));
            }
            else
            {
                //Conection.StartListening();
                Conection.OnMessageReceived += MensajeRecibidoWebSocket;
                btnIniciar.Visibility = Visibility.Collapsed;
            }
            txtLobbyName.Text = Utils.sala.Nombre;
            lbJugadores.ItemsSource = Utils.sala.Jugadores;

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
            var obj = JObject.Parse(json);
            var action = obj["response"]?.ToString();

            if (action == Constants.RS.SalirSala)
            {
                Conection.OnMessageReceived -= MensajeRecibidoWebSocket;
                DependencyObject parent = this;
                while (parent != null && !(parent is Frame))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent is Frame frame)
                    frame.Content = null;
            }
            else if (json.Contains("response") && JObject.Parse(json)?.Property("response")?.Value.ToString() == Constants.RS.ActualizarSala)
            {
                var jugadoresArray = obj["jugadores"];
                var salaToken = obj["sala"];
                var sala = salaToken.ToObject<Sala>();
                if (sala != null)
                {
                    Utils.sala.Jugadores.Clear();
                    foreach (var jToken in sala.Jugadores)
                    {
                        Utils.sala.Jugadores.Add(jToken);
                        if(jToken.UserId == Utils.user.Id)
                        {
                            Utils.jugadorID = jToken.Id;
                        }
                    }
                    lbJugadores.ItemsSource = null;
                    lbJugadores.ItemsSource = Utils.sala.Jugadores;
                }
            }
            else if (json.Contains("partida"))
            {
                //Conection.StopListening();
                Conection.OnMessageReceived -= MensajeRecibidoWebSocket;
                IniciarJuego(json);
            }
        }

        private void btnInciar_Click(object sender, RoutedEventArgs e)
        {
            if (cboColor.SelectedIndex == -1) return;
            

            int listo = 0;
            foreach(Jugador jugador in Utils.sala.Jugadores)
            {
                if(jugador.Estado)
                {
                    listo++;
                }
            }

            if (listo == Utils.sala.Jugadores.Count)
            {
                //Utils.partida.jugadorList = Utils.sala.Jugadores; //Temporal
                IniciarJuego("");
            }
            
        }

        private void IniciarJuego(string json)
        {
            JObject jo = JObject.Parse(json);
            JToken userToken = jo["partida"];
            Utils.partida = userToken.ToObject<Partida>();
            Window.GetWindow(this).Content = new JuegoPage();
        }

        private void btnListo_Click(object sender, RoutedEventArgs e)
        {
            Jugador j = Utils.sala.Jugadores.FirstOrDefault(e => e.UserId == Utils.user.Id);
            if (j.Color == null)
            {
                return;
            }

            j.Estado = !j.Estado;
            UserChangedAsync();
        }

        private async void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Utils.sala.Jugadores.Clear();
            if(!Utils.demo)
            {
                
                await Conection.SendMessageJoinLeaveSala(Utils.sala.Id, Utils.user.Id, false);
                //string json = await Conection.ReceiveMessage(); 

                //Conection.StopListening();
                //Conection.OnMessageReceived -= MensajeRecibidoWebSocket;

            }
            else
            {
                DependencyObject parent = this;
                while (parent != null && !(parent is Frame))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent is Frame frame)
                    frame.Content = null;
            }
          
        }

        private void cboColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utils.sala.Jugadores.FirstOrDefault(e => e.UserId == Utils.user.Id).Color = cboColor.SelectedValue.ToString();
            UserChangedAsync();
        }

        private async Task UserChangedAsync()
        {
            if (Utils.demo)
            {

            }
            else
            {
                await Conection.SendMessageUpdateSala(Utils.sala, Constants.RQ.ActualizarSala);
            }
            // Mandar mensaje que he cambiado
        }
    }
}
