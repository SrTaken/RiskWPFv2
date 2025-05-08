using Datos;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
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
    /// Lógica de interacción para LobbysPage.xaml
    /// </summary>
    public partial class LobbysPage : Page
    {
        List<Sala> Lobbys { get; set; }
        public LobbysPage()
        {
            InitializeComponent();
            GetSalasAsync();
        }

        private async Task GetSalasAsync()
        {
            string json = "";
            if (Utils.demo)
            {
                //Lobbys = new List<Sala>
                //{
                //    new Sala { Id = 1, Nombre = "Riskers", MaxJugadores = 6, Jugadores = new List<Jugador> { new Jugador(), new Jugador() } },
                //    new Sala { Id = 2, Nombre = "Guerreros", MaxJugadores = 4, Jugadores = new List<Jugador> { new Jugador() } },
                //    new Sala { Id = 3, Nombre = "Random", MaxJugadores = 3, Jugadores = new List<Jugador> { new Jugador(), new Jugador(), new Jugador() } }
                //};
                json = "{\"response\":\"getSalasRS\",\"salas\":[{\"id\":1,\"nombre\":\"dada\",\"jugadores\":[],\"maxPlayers\":3},{\"id\":2,\"nombre\":\"Jose\",\"jugadores\":[],\"maxPlayers\":3},{\"id\":3,\"nombre\":\"JoseMaria\",\"jugadores\":[],\"maxPlayers\":3},{\"id\":4,\"nombre\":\"s\",\"jugadores\":[],\"maxPlayers\":3},{\"id\":5,\"nombre\":\"Nombre\",\"jugadores\":[],\"maxPlayers\":3},{\"id\":6,\"nombre\":\"Prueba1\",\"jugadores\":[],\"maxPlayers\":3},{\"id\":7,\"nombre\":\"asjdkwa\",\"jugadores\":[],\"maxPlayers\":3},{\"id\":8,\"nombre\":\"hola\",\"jugadores\":[],\"maxPlayers\":3},{\"id\":9,\"nombre\":\"Pirulo2\",\"jugadores\":[],\"maxPlayers\":3}],\"code\":200}";
            }
            else
            {
                await Conection.SendMessage(Constants.ListaSalas);
                json = await Conection.ReceiveMessage();

            }
            if (!string.IsNullOrEmpty(json))
            {
                List<Sala> salas = Utils.getSalasFromRequest(json);
                lvLobbys.ItemsSource = salas;
            }
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            GetSalasAsync();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is Frame))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent is Frame frame)
                frame.Content = null;
        }
        private async void lvLobbys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvLobbys.SelectedItem is Sala selectedSala)
            {
                int salaId = selectedSala.Id;
                string json = "";
                if (Utils.demo)
                {

                }
                else
                {
                    await Conection.SendMessageJoinLeaveSala(salaId, Utils.user.Id, true);
                    json = await Conection.ReceiveMessage();

                }

                if(string.IsNullOrEmpty(json))
                {
                    MessageBox.Show("Unexpected Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (JObject.Parse(json)?.Property("request")?.Value.ToString() == "KO")
                {
                    MessageBox.Show("Sala llena", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Utils.sala = Utils.getSalaFromRequest(json);
                this.NavigationService.Navigate(new PreJuegoPage());
            }
        }

    }
}
