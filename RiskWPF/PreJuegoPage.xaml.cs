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
        private List<string> colores = new() { "Rojo", "Azul", "Verde", "Amarillo" };

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

            if (action == "leaveSalaRS")
            {
                DependencyObject parent = this;
                while (parent != null && !(parent is Frame))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent is Frame frame)
                    frame.Content = null;
            }
            if (action == Constants.ActualizarSala)
            {
                var jugadoresArray = obj["jugadores"];
                if (jugadoresArray != null)
                {
                    Utils.partida.jugadorList.Clear();
                    foreach (var jToken in jugadoresArray)
                    {
                        var jugador = jToken.ToObject<Jugador>();
                        Utils.sala.Jugadores.Add(jugador);
                    }
                    lbJugadores.ItemsSource = null;
                    lbJugadores.ItemsSource = Utils.partida.jugadorList;
                }
            }
            else if (action == Constants.EmpezarPartida)
            {
                Conection.StopListening();
                Conection.OnMessageReceived -= MensajeRecibidoWebSocket;
                IniciarJuego();
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
                Utils.partida.jugadorList = Utils.sala.Jugadores; //Temporal
                IniciarJuego();
            }
            
        }

        private void IniciarJuego()
        {
            Window.GetWindow(this).Content = new JuegoPage();
        }

        private void btnListo_Click(object sender, RoutedEventArgs e)
        {
            Jugador j = Utils.sala.Jugadores.FirstOrDefault(e => e.UserId == Utils.user.Id);

            j.Estado = !j.Estado;
            UserChanged();
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
                DependencyObject parent = this;
                while (parent != null && !(parent is Frame))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent is Frame frame)
                    frame.Content = null;
            
                

          
        }

        private void cboColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utils.sala.Jugadores.FirstOrDefault(e => e.UserId == Utils.user.Id).Color = cboColor.SelectedValue.ToString();
            UserChanged();
        }

        private void UserChanged()
        {
            if (!Utils.demo)
            {

            }
            // Mandar mensaje que he cambiado
        }
    }
}
