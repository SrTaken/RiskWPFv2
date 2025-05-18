using Datos;
using Model;
using Newtonsoft.Json.Linq;
using RiskWPF.controls;
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
    /// Lógica de interacción para JuegoPage.xaml
    /// </summary>
    public partial class JuegoPage : Page
    {
        
        private WriteableBitmap mapaColoresWB;
        private PaisMapa paisResaltado = null;

        private PaisMapa paisSeleccionado = null;
        private PaisMapa paisAtaque = null;
        private bool esMiTurno;

        private Dictionary<Color, PaisMapa> colorAPais = new Dictionary<Color, PaisMapa>
        {
            { (Color)ColorConverter.ConvertFromString("#FF0000"), new PaisMapa("Alaska") },
            { (Color)ColorConverter.ConvertFromString("#00FF00"), new PaisMapa("Northwest Territory") },
            { (Color)ColorConverter.ConvertFromString("#0000FF"), new PaisMapa("Greenland") },
            { (Color)ColorConverter.ConvertFromString("#FFFF00"), new PaisMapa("Alberta") },
            { (Color)ColorConverter.ConvertFromString("#FF00FF"), new PaisMapa("Ontario") },
            { (Color)ColorConverter.ConvertFromString("#00FFFF"), new PaisMapa("Quebec") },
            { (Color)ColorConverter.ConvertFromString("#800000"), new PaisMapa("Western United States") },
            { (Color)ColorConverter.ConvertFromString("#008000"), new PaisMapa("Eastern United States") },
            { (Color)ColorConverter.ConvertFromString("#000080"), new PaisMapa("Central America") },
            { (Color)ColorConverter.ConvertFromString("#808000"), new PaisMapa("Venezuela") },
            { (Color)ColorConverter.ConvertFromString("#800080"), new PaisMapa("Peru") },
            { (Color)ColorConverter.ConvertFromString("#008080"), new PaisMapa("Brazil") },
            { (Color)ColorConverter.ConvertFromString("#C0C0C0"), new PaisMapa("Argentina") },
            { (Color)ColorConverter.ConvertFromString("#FFA500"), new PaisMapa("North Africa") },
            { (Color)ColorConverter.ConvertFromString("#A52A2A"), new PaisMapa("Egypt") },
            { (Color)ColorConverter.ConvertFromString("#7FFF00"), new PaisMapa("East Africa") },
            { (Color)ColorConverter.ConvertFromString("#DC143C"), new PaisMapa("Congo") },
            { (Color)ColorConverter.ConvertFromString("#00CED1"), new PaisMapa("South Africa"   ) },
            { (Color)ColorConverter.ConvertFromString("#FFD700"), new PaisMapa("Madagascar"     ) },
            { (Color)ColorConverter.ConvertFromString("#4B0082"), new PaisMapa("Western Europe" ) },
            { (Color)ColorConverter.ConvertFromString("#ADFF2F"), new PaisMapa("Southern Europe") },
            { (Color)ColorConverter.ConvertFromString("#FF69B4"), new PaisMapa("Northern Europe") },
            { (Color)ColorConverter.ConvertFromString("#1E90FF"), new PaisMapa("Great Britain"  ) },
            { (Color)ColorConverter.ConvertFromString("#B22222"), new PaisMapa("Iceland"        ) },
            { (Color)ColorConverter.ConvertFromString("#228B22"), new PaisMapa("Scandinavia"    ) },
            { (Color)ColorConverter.ConvertFromString("#DAA520"), new PaisMapa("Ukraine"        ) },
            { (Color)ColorConverter.ConvertFromString("#B8860B"), new PaisMapa("Ural"           ) },
            { (Color)ColorConverter.ConvertFromString("#9932CC"), new PaisMapa("Siberia"        ) },
            { (Color)ColorConverter.ConvertFromString("#00BFFF"), new PaisMapa("Yakutsk"        ) },
            { (Color)ColorConverter.ConvertFromString("#FF4500"), new PaisMapa("Kamchatka"      ) },
            { (Color)ColorConverter.ConvertFromString("#2E8B57"), new PaisMapa("Irkutsk"        ) },
            { (Color)ColorConverter.ConvertFromString("#8B0000"), new PaisMapa("Mongolia"       ) },
            { (Color)ColorConverter.ConvertFromString("#E9967A"), new PaisMapa("Japan"          ) },
            { (Color)ColorConverter.ConvertFromString("#8FBC8F"), new PaisMapa("Afghanistan"    ) },
            { (Color)ColorConverter.ConvertFromString("#483D8B"), new PaisMapa("China"          ) },
            { (Color)ColorConverter.ConvertFromString("#BDB76B"), new PaisMapa("Middle East") },
            { (Color)ColorConverter.ConvertFromString("#556B2F"), new PaisMapa("India") },
            { (Color)ColorConverter.ConvertFromString("#8B008B"), new PaisMapa("Siam") },
            { (Color)ColorConverter.ConvertFromString("#20B2AA"), new PaisMapa("Indonesia") },
            { (Color)ColorConverter.ConvertFromString("#F08080"), new PaisMapa("New Guinea") },
            { (Color)ColorConverter.ConvertFromString("#4682B4"), new PaisMapa("Western Australia") },
            { (Color)ColorConverter.ConvertFromString("#D2691E"), new PaisMapa("Eastern Australia") }
        };

        public JuegoPage()
        {
            InitializeComponent();
            var mapaColores = new BitmapImage(new Uri("pack://application:,,,/Assets/Risk_board_map.png"));
            mapaColoresWB = new WriteableBitmap(mapaColores);
            Conection.OnMessageReceived += MensajeRecibidoWebSocket; 
            sliderTropasPaDentro.ValueChanged += SliderTropasPaDentro_ValueChanged;
            if (Utils.demo)
            {
                Utils.partida.turno = Utils.user.Id;
                lvJugadores.ItemsSource = Utils.partida.jugadores;

                //Decir que fase estamos 
                BordeFaseRefuerzo.Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
                BordeFaseAtacar.Background = (Brush)FindResource("FaseAtacar_BG");
                BordeFaseReagrupar.Background = (Brush)FindResource("FaseAtacar_BG");
            }
            //lvJugadores.ItemsSource = Utils.partida.jugadorList;
            //ActualizarTurno();
            //ActualizarEstado();
            CargarPaises();
            ActualizarTurno();
            ActualizarEstado();
            RepintarLienzo();

            
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
            //return;   
            if (json == null)
            {
                MessageBox.Show("Unexpected Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if ("" == "Dados")
            {
                //logica de visibilidad de los dados
            }
            else
            {
                CargarPartida(json);
                CargarPaises();
                ActualizarTurno();
                ActualizarEstado();
                RepintarLienzo();

                
            }
            
        }

        #region ActualizarPartida
        private void CargarPartida(string json)
        {
            JObject jo = JObject.Parse(json);
            JToken userToken = jo["partida"];
            Utils.partida = userToken.ToObject<Partida>();
            lvJugadores.ItemsSource = Utils.partida.jugadores;
        }
        private void CargarPaises()
        {
            //// Limpiar primero todos los países del tablero
            //foreach (var pais in colorAPais.Values)
            //{
            //    pais.jugadorId = 0;
            //    pais.Tropas = 0;
            //    pais.color1 = Colors.Gray;
            //}

            // Para cada jugador
            foreach (var jugador in Utils.partida.jugadores)
            {
                // Para cada país controlado por este jugador
                foreach (var kvp in jugador.PaisesControlados)
                {
                    string paisJugador = kvp.Key;
                    int tropas = kvp.Value;

                    // Busca el PaisMapa por nombre
                    var paisMapa = colorAPais.Values.FirstOrDefault(p =>
                        string.Equals(p.nombre, paisJugador, StringComparison.OrdinalIgnoreCase));
                    if (paisMapa != null)
                    {
                        paisMapa.jugadorId = jugador.Id;
                        paisMapa.Tropas = tropas;
                        paisMapa.color1 = Utils.GetColorFromName(jugador.Color);
                    }
                }
            }
        }
        public void ActualizarTurno()
        {
            esMiTurno = Utils.jugadorID == Utils.partida.turno;

            TurnWaitOverlay.Visibility = esMiTurno ? Visibility.Collapsed : Visibility.Visible;
            MapaVisible.IsEnabled = esMiTurno;
            OverlayCanvas.IsEnabled = esMiTurno;
            OverlayCanvasHover.IsEnabled = esMiTurno;
            //btnFlecha.Visibility = !esMiTurno ? Visibility.Collapsed : Visibility.Visible;
            //txtTropasPaDentro.Visibility = !esMiTurno ? Visibility.Collapsed : Visibility.Visible;

            if(esMiTurno)
            {
                sliderTropasPaDentro.Maximum = Utils.partida.jugadores.FirstOrDefault(e => e.Id == Utils.jugadorID).TropasTurno;
                if (sliderTropasPaDentro.Value > sliderTropasPaDentro.Maximum)
                    sliderTropasPaDentro.Value = sliderTropasPaDentro.Maximum;
            }

            paisSeleccionado = null;
            paisAtaque = null;
            PintarSelecciones();
        }
        #endregion
        #region Eventos
        private void SliderTropasPaDentro_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtTropasSliderValor != null)
                txtTropasSliderValor.Text = ((int)sliderTropasPaDentro.Value).ToString();
        }
        private async void btnFlecha_Click(object sender, RoutedEventArgs e)
        {
            if (esMiTurno)
            {
                await Conection.FinalizaTurno(Utils.user.Token);
            }
        }
        private async void MapaVisible_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TurnWaitOverlay.Visibility == Visibility.Visible)
                return; 

            Point pos = e.GetPosition(MapaVisible);

            int x = (int)(pos.X * mapaColoresWB.PixelWidth / MapaVisible.ActualWidth);
            int y = (int)(pos.Y * mapaColoresWB.PixelHeight / MapaVisible.ActualHeight);

            byte[] pixel = new byte[4];
            mapaColoresWB.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);
            Color color = Color.FromArgb(255, pixel[2], pixel[1], pixel[0]); // BGRA a RGBA

            if (colorAPais.TryGetValue(color, out PaisMapa paisClic))
            {
                if (Utils.partida.fase == Estat.REFORC_PAIS || Utils.partida.fase == Estat.REFORC_TROPES)
                {
                    if (paisClic.jugadorId == Utils.jugadorID)
                    {
                        paisSeleccionado = paisClic;
                        paisAtaque = null;
                        PintarSelecciones();

                        int tropasParaPoner = (int)sliderTropasPaDentro.Value;
                        if (tropasParaPoner > 0)
                        {
                            await Conection.EnviarRefuerzo(Utils.user.Token, paisSeleccionado.nombre, tropasParaPoner, Utils.partida.fase);

                        }
                    }
                }
                else if (Utils.partida.fase == Estat.COL_LOCAR_INICIAL)
                {
                    if (paisClic.jugadorId != 0)
                        return; 

                    paisSeleccionado = paisClic; 
                    paisAtaque = null;
                    PintarSelecciones();
                    await Conection.SendSeleccion(Utils.user.Token, paisSeleccionado.nombre);
                }
                else if (paisSeleccionado == null)
                {
                    if (paisClic.jugadorId == Utils.jugadorID)
                    {
                        paisSeleccionado = paisClic;
                        paisAtaque = null;
                        PintarSelecciones();
                    }
                }
                else if (paisSeleccionado != null && Utils.partida.fase == Estat.COMBAT)
                {
                    if (paisClic != paisSeleccionado && paisClic.jugadorId != Utils.user.Id)
                    {
                        paisAtaque = paisClic;
                        PintarSelecciones();
                    }
                }
                else if (paisClic == paisSeleccionado)
                {
                    paisSeleccionado = null;
                    paisAtaque = null;
                    PintarSelecciones();
                }
            }
        }
        private void MapaVisible_MouseMove(object sender, MouseEventArgs e)
        {
            //if (TurnWaitOverlay.Visibility == Visibility.Visible)
            //    return;
            Point pos = e.GetPosition(MapaVisible);

            int x = (int)(pos.X * mapaColoresWB.PixelWidth / MapaVisible.ActualWidth);
            int y = (int)(pos.Y * mapaColoresWB.PixelHeight / MapaVisible.ActualHeight);

            if (x < 0 || y < 0 || x >= mapaColoresWB.PixelWidth || y >= mapaColoresWB.PixelHeight)
                return;

            byte[] pixel = new byte[4];
            mapaColoresWB.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);
            Color color = Color.FromArgb(255, pixel[2], pixel[1], pixel[0]);

            if (colorAPais.TryGetValue(color, out PaisMapa pais))
            {
                paisResaltado = pais;
                ResaltarPais(color);
            }
            else
            {
                paisResaltado = null;
                OverlayCanvasHover.Children.Clear();
            }

        }
        #endregion
        #region ActualizarPantallaPorEvento
        private void ResaltarPais(Color colorPais)
        {
            OverlayCanvasHover.Children.Clear();

            // Crea una imagen de la máscara del país resaltado
            var mask = new WriteableBitmap(mapaColoresWB.PixelWidth, mapaColoresWB.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
            int stride = mapaColoresWB.BackBufferStride;
            byte[] pixels = new byte[mapaColoresWB.PixelHeight * stride];
            mapaColoresWB.CopyPixels(pixels, stride, 0);

            byte[] maskPixels = new byte[pixels.Length];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                // BGRA
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                if (r == colorPais.R && g == colorPais.G && b == colorPais.B)
                {
                    maskPixels[i] = 0;         // B
                    maskPixels[i + 1] = 255;   // G
                    maskPixels[i + 2] = 255;   // R
                    maskPixels[i + 3] = 120;   // A (transparencia)
                }
                else
                {
                    maskPixels[i + 3] = 0;
                }
            }

            mask.WritePixels(new Int32Rect(0, 0, mask.PixelWidth, mask.PixelHeight), maskPixels, stride, 0);

            var img = new System.Windows.Controls.Image
            {
                Source = mask,
                Width = MapaVisible.ActualWidth,
                Height = MapaVisible.ActualHeight,
                Stretch = Stretch.Fill
            };

            OverlayCanvasHover.Width = MapaVisible.ActualWidth;
            OverlayCanvasHover.Height = MapaVisible.ActualHeight;
            OverlayCanvasHover.Children.Add(img);
        }
        private void PintarPaisEnColor(Color colorPaisOriginal, Color colorNuevo)
        {
            //OverlayCanvas.Children.Clear();

            var mask = new WriteableBitmap(mapaColoresWB.PixelWidth, mapaColoresWB.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
            int stride = mapaColoresWB.BackBufferStride;
            byte[] pixels = new byte[mapaColoresWB.PixelHeight * stride];
            mapaColoresWB.CopyPixels(pixels, stride, 0);

            byte[] maskPixels = new byte[pixels.Length];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                // BGRA
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                if (r == colorPaisOriginal.R && g == colorPaisOriginal.G && b == colorPaisOriginal.B)
                {
                    maskPixels[i] = colorNuevo.B;          // B
                    maskPixels[i + 1] = colorNuevo.G;      // G
                    maskPixels[i + 2] = colorNuevo.R;      // R
                    maskPixels[i + 3] = 255;   // A (transparencia)
                }
                else
                {
                    maskPixels[i + 3] = 0;
                }
            }

            mask.WritePixels(new Int32Rect(0, 0, mask.PixelWidth, mask.PixelHeight), maskPixels, stride, 0);

            var img = new System.Windows.Controls.Image
            {
                Source = mask,
                Width = MapaVisible.ActualWidth,
                Height = MapaVisible.ActualHeight,
                Stretch = Stretch.Fill
            };

            OverlayCanvas.Width = MapaVisible.ActualWidth;
            OverlayCanvas.Height = MapaVisible.ActualHeight;
            OverlayCanvas.Children.Add(img);
            
        }
        private void PintarSelecciones()
        {
            CanvasSelecciones.Children.Clear();

            if (paisSeleccionado != null)
                PintarResaltePais(paisSeleccionado, Colors.LimeGreen, 255);
            if (paisAtaque != null)
                PintarResaltePais(paisAtaque, Colors.MediumVioletRed, 255);
        }
        private void PintarResaltePais(PaisMapa pais, Color colorResalte, byte alpha = 120)
        {
            var colorPais = colorAPais.FirstOrDefault(kvp => kvp.Value == pais).Key;
            var mask = new WriteableBitmap(mapaColoresWB.PixelWidth, mapaColoresWB.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
            int stride = mapaColoresWB.BackBufferStride;
            byte[] pixels = new byte[mapaColoresWB.PixelHeight * stride];
            mapaColoresWB.CopyPixels(pixels, stride, 0);

            byte[] maskPixels = new byte[pixels.Length];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                if (r == colorPais.R && g == colorPais.G && b == colorPais.B)
                {
                    maskPixels[i] = colorResalte.B;
                    maskPixels[i + 1] = colorResalte.G;
                    maskPixels[i + 2] = colorResalte.R;
                    maskPixels[i + 3] = alpha;
                }
                else
                {
                    maskPixels[i + 3] = 0;
                }
            }

            mask.WritePixels(new Int32Rect(0, 0, mask.PixelWidth, mask.PixelHeight), maskPixels, stride, 0);

            var img = new System.Windows.Controls.Image
            {
                Source = mask,
                Width = MapaVisible.ActualWidth,
                Height = MapaVisible.ActualHeight,
                Stretch = Stretch.Fill
            };
            CanvasSelecciones.Width = MapaVisible.ActualWidth;
            CanvasSelecciones.Height = MapaVisible.ActualHeight;
            CanvasSelecciones.Children.Add(img);
        }
        #endregion
        #region ActualizarPantallaTurnos
        private void ActualizarEstado()
        {
            BordeFaseRefuerzo.Background = Utils.partida.fase == Estat.REFORC_TROPES ? new SolidColorBrush(Colors.LightGoldenrodYellow) : (Brush)FindResource("FaseRefuerzo_BG");
            BordeFaseAtacar.Background = Utils.partida.fase == Estat.COMBAT ? new SolidColorBrush(Colors.LightGoldenrodYellow) : (Brush)FindResource("FaseAtacar_BG");
            BordeFaseReagrupar.Background = Utils.partida.fase == Estat.RECOL_LOCACIO ? new SolidColorBrush(Colors.LightGoldenrodYellow) : (Brush)FindResource("FaseReagrupar_BG");
        }
        private void RepintarLienzo()
        {
            OverlayCanvas.Children.Clear();

            foreach (var paisEntry in colorAPais)
            {
                if (paisEntry.Value.jugadorId == 0) continue;

                Color colorOriginal = paisEntry.Key;
                PaisMapa pais = paisEntry.Value;

                Color colorUsar = pais.color1;

                var mask = new WriteableBitmap(mapaColoresWB.PixelWidth, mapaColoresWB.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
                int stride = mapaColoresWB.BackBufferStride;
                byte[] pixels = new byte[mapaColoresWB.PixelHeight * stride];
                mapaColoresWB.CopyPixels(pixels, stride, 0);

                byte[] maskPixels = new byte[pixels.Length];

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    // BGRA
                    byte b = pixels[i];
                    byte g = pixels[i + 1];
                    byte r = pixels[i + 2];

                    if (r == colorOriginal.R && g == colorOriginal.G && b == colorOriginal.B)
                    {
                        maskPixels[i] = colorUsar.B;     // B
                        maskPixels[i + 1] = colorUsar.G; // G
                        maskPixels[i + 2] = colorUsar.R; // R
                        maskPixels[i + 3] = 255;         // A (transparencia)
                    }
                    else
                    {
                        maskPixels[i + 3] = 0;
                    }
                }

                mask.WritePixels(new Int32Rect(0, 0, mask.PixelWidth, mask.PixelHeight), maskPixels, stride, 0);

                var img = new System.Windows.Controls.Image
                {
                    Source = mask,
                    Width = MapaVisible.ActualWidth,
                    Height = MapaVisible.ActualHeight,
                    Stretch = Stretch.Fill
                };

                OverlayCanvas.Width = MapaVisible.ActualWidth;
                OverlayCanvas.Height = MapaVisible.ActualHeight;
                OverlayCanvas.Children.Add(img);
            }
            PonerEjercito();
        }
        private void PonerEjercito()
        {
            foreach (var paisEntry in colorAPais)
            {
                var colorPais = paisEntry.Key;
                var pais = paisEntry.Value;

                if (pais.jugadorId == 0) continue;

                Point centro = Utils.CalcularCentroidePaisMask(colorPais, mapaColoresWB);
                if (centro.X < 0) continue; // saltar si no existe

                double x = centro.X * (MapaVisible.ActualWidth / mapaColoresWB.PixelWidth);
                double y = centro.Y * (MapaVisible.ActualHeight / mapaColoresWB.PixelHeight);

                Border marcador = new Border
                {
                    CornerRadius = new CornerRadius(9),
                    Background = new SolidColorBrush(Color.FromArgb(180, 255, 255, 224)),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Child = new TextBlock
                    {
                        Text = pais.Tropas.ToString(),
                        FontWeight = FontWeights.Bold,
                        FontSize = 17,
                        Foreground = Brushes.Black,
                        TextAlignment = TextAlignment.Center,
                        Padding = new Thickness(6, 0, 6, 0)
                    }
                };
                Canvas.SetLeft(marcador, x - 15);
                Canvas.SetTop(marcador, y - 15);
                OverlayCanvas.Children.Add(marcador);
            }
        }
        #endregion

        
    }
}
