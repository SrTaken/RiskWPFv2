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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RiskWPF.controls
{
    /// <summary>
    /// Lógica de interacción para JugadorMiniFichaControl.xaml
    /// </summary>
    public partial class JugadorMiniFichaControl : UserControl
    {
        public JugadorMiniFichaControl()
        {
            InitializeComponent();
        }

        public Jugador MyJugador
        {
            get { return (Jugador)GetValue(MyJugadorProperty); }
            set { SetValue(MyJugadorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyJugador.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyJugadorProperty =
            DependencyProperty.Register("MyJugador", typeof(Jugador), typeof(JugadorMiniFichaControl), new PropertyMetadata(StaticOnJugadorChanged));

        private static void StaticOnJugadorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as JugadorMiniFichaControl;
            control.OnJugadorChanged();
        }

        private void OnJugadorChanged()
        {
            elipseColor.Fill = new SolidColorBrush(Utils.GetColorFromName(MyJugador.Color));
        }
    }
}
