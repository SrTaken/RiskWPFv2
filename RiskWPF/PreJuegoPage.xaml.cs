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
            Conection.OnMessageReceived += MensajeRecibidoWebSocket;

            if (Utils.demo)
            {
                Utils.partida.jugadorList.Add(new Jugador(98, "Usuario1", "Rojo", true));
                Utils.partida.jugadorList.Add(new Jugador(99, "Usuario2", "Azul", true));
                Utils.partida.jugadorList.Add(new Jugador(100, "Usuario3", "Amarillo", true));
                Utils.partida.jugadorList.Add(new Jugador(Utils.user));

                lbJugadores.ItemsSource = Utils.partida.jugadorList;
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
            var obj = JObject.Parse(json);
            var action = obj["action"]?.ToString();

            if (action == "update-jugadores")
            {
                var jugadoresArray = obj["jugadores"];
                if (jugadoresArray != null)
                {
                    Utils.partida.jugadorList.Clear();
                    foreach (var jToken in jugadoresArray)
                    {
                        var jugador = jToken.ToObject<Jugador>();
                        Utils.partida.jugadorList.Add(jugador);
                    }
                    lbJugadores.ItemsSource = null;
                    lbJugadores.ItemsSource = Utils.partida.jugadorList;
                }
            }
            else if (action == "empezar")
            {
                IniciarJuego();
            }
        }

        private void btnInciar_Click(object sender, RoutedEventArgs e)
        {
            //if(!Utils.demo)
            //    jugadores = Conection.ReciveObject(); //Actualizar jugadores 


            if (cboColor.SelectedIndex == -1) return;

            int listo = 0;
            foreach(Jugador jugador in Utils.partida.jugadorList)
            {
                if(jugador.Estado)
                {
                    listo++;
                }
            }

            if (listo == Utils.partida.jugadorList.Count)
            {
                IniciarJuego();
            }
            
        }

        private void IniciarJuego()
        {
            if (!Utils.demo) return;

            Utils.partida.jugadorList.FirstOrDefault(e => e.Id == Utils.user.Id).Color = cboColor.SelectedValue.ToString();

            Window.GetWindow(this).Content = new JuegoPage();
        }

        private void btnListo_Click(object sender, RoutedEventArgs e)
        {
            Jugador j = Utils.partida.jugadorList.FirstOrDefault(e => e.Id == Utils.user.Id);

            j.Estado = !j.Estado;
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Content = null;
        }
    }
}
