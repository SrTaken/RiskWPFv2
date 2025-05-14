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
            if (Utils.demo)
            {
                Utils.partida.turno = Utils.user.Id;
                lvJugadores.ItemsSource = Utils.partida.jugadorList;

                //Decir que fase estamos 
                BordeFaseRefuerzo.Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
                BordeFaseAtacar.Background = (Brush)FindResource("FaseAtacar_BG");
                BordeFaseReagrupar.Background = (Brush)FindResource("FaseAtacar_BG");
            }
            ActualizarTurno();
            ActualizarEstado();
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
            else if (JObject.Parse(json)?.Property("result")?.Value.ToString() == "Actualiza")
            {
                //Cargar los nuevos datos
                ///Tienes que recorrer el diccionario y poner a cada pais el value que pone el david
                ActualizarTurno();
                ActualizarEstado();
                RepintarLienzo();
            }
        }


        #region Eventos
        private void MapaVisible_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (TurnWaitOverlay.Visibility == Visibility.Visible)
            //    return;

            Point pos = e.GetPosition(MapaVisible);

            int x = (int)(pos.X * mapaColoresWB.PixelWidth / MapaVisible.ActualWidth);
            int y = (int)(pos.Y * mapaColoresWB.PixelHeight / MapaVisible.ActualHeight);

            byte[] pixel = new byte[4];
            mapaColoresWB.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);
            Color color = Color.FromArgb(255, pixel[2], pixel[1], pixel[0]); // BGRA a RGBA

            // Busca el pais
            if (colorAPais.TryGetValue(color, out PaisMapa pais))
            {
                if (pais.jugadorId != Utils.partida.turno)
                {
                    PintarPaisEnColor(color, Utils.GetColorFromName(Utils.partida.jugadorList.FirstOrDefault(e => e.Id == Utils.partida.turno).Color));
                    //pais.color1 = Utils.GetColorFromName(Utils.partida.jugadorList.FirstOrDefault(e => e.Id == Utils.partida.turno).Color);
                    pais.jugadorId = Utils.partida.turno;
                    //RepintarLienzo();
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

            // Crea una imagen de la máscara del país que va a ser pintado
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
        #endregion
        #region ActualizarPantalla
        private void ActualizarEstado()
        {
            BordeFaseRefuerzo.Background = Utils.partida.fase == "Refuerzo" ? new SolidColorBrush(Colors.LightGoldenrodYellow) : (Brush)FindResource("BordeFaseRefuerzo");
            BordeFaseAtacar.Background = Utils.partida.fase == "Ataque" ? new SolidColorBrush(Colors.LightGoldenrodYellow) : (Brush)FindResource("BordeFaseAtacar");
            BordeFaseReagrupar.Background = Utils.partida.fase == "Reagrupar" ? new SolidColorBrush(Colors.LightGoldenrodYellow) : (Brush)FindResource("BordeFaseReagrupar");
        }
        public void ActualizarTurno()
        {
            bool esMiTurno = Utils.user.Id == Utils.partida.turno;

            TurnWaitOverlay.Visibility = esMiTurno ? Visibility.Collapsed : Visibility.Visible;
            MapaVisible.IsEnabled = esMiTurno;
            OverlayCanvas.IsEnabled = esMiTurno;
            OverlayCanvasHover.IsEnabled = esMiTurno;
        }
        private void RepintarLienzo()
        {
            OverlayCanvas.Children.Clear();

            // Recorre todos los países y repinta usando su color actual
            foreach (var paisEntry in colorAPais)
            {
                //if(paisEntry.Value.color1 == (Color)ColorConverter.ConvertFromString("#00000000")) continue;  //Si no tiene color siguiente
                if (paisEntry.Value.jugadorId == 0) continue;

                Color colorOriginal = paisEntry.Key;
                PaisMapa pais = paisEntry.Value;

                // Si el país se ha pintado antes, usar el color pintado; si no, usa el color original.
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
