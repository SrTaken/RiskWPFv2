using Datos;
using Model;
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

namespace RiskWPF
{
    /// <summary>
    /// Lógica de interacción para MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            ucUser.MyUser = Utils.user;
        }

        
        private void JugarBoton_Click(object sender, RoutedEventArgs e)
        {
            FrameJuego.Navigate(new LobbysPage());
            //LobbysPage lobbyPage = new();
            //this.Content = lobbyPage;
        }

        private void CreateBoton_Click(object sender, RoutedEventArgs e)
        {
            FrameJuego.Navigate(new SalaCreationPage());
            //PreJuegoPage juegoPage = new();
            //this.Content = juegoPage;
        }

        private void CerrarBoton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
