using Datos;
using Model;
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
        private Pais paisResaltado = null;

        private Dictionary<Color, Pais> colorAPais = new Dictionary<Color, Pais>
        {
            { (Color)ColorConverter.ConvertFromString("#FF0000"), new Pais("Alaska") },
            { (Color)ColorConverter.ConvertFromString("#00FF00"), new Pais("Northwest Territory") },
            { (Color)ColorConverter.ConvertFromString("#0000FF"), new Pais("Greenland") },
            { (Color)ColorConverter.ConvertFromString("#FFFF00"), new Pais("Alberta") },
            { (Color)ColorConverter.ConvertFromString("#FF00FF"), new Pais("Ontario") },
            { (Color)ColorConverter.ConvertFromString("#00FFFF"), new Pais("Quebec") },
            { (Color)ColorConverter.ConvertFromString("#800000"), new Pais("Western United States") },
            { (Color)ColorConverter.ConvertFromString("#008000"), new Pais("Eastern United States") },
            { (Color)ColorConverter.ConvertFromString("#000080"), new Pais("Central America") },
            { (Color)ColorConverter.ConvertFromString("#808000"), new Pais("Venezuela") },
            { (Color)ColorConverter.ConvertFromString("#800080"), new Pais("Peru") },
            { (Color)ColorConverter.ConvertFromString("#008080"), new Pais("Brazil") },
            { (Color)ColorConverter.ConvertFromString("#C0C0C0"), new Pais("Argentina") },
            { (Color)ColorConverter.ConvertFromString("#FFA500"), new Pais("North Africa") },
            { (Color)ColorConverter.ConvertFromString("#A52A2A"), new Pais("Egypt") },
            { (Color)ColorConverter.ConvertFromString("#7FFF00"), new Pais("East Africa") },
            { (Color)ColorConverter.ConvertFromString("#DC143C"), new Pais("Congo") },
            { (Color)ColorConverter.ConvertFromString("#00CED1"), new Pais("South Africa"   ) },
            { (Color)ColorConverter.ConvertFromString("#FFD700"), new Pais("Madagascar"     ) },
            { (Color)ColorConverter.ConvertFromString("#4B0082"), new Pais("Western Europe" ) },
            { (Color)ColorConverter.ConvertFromString("#ADFF2F"), new Pais("Southern Europe") },
            { (Color)ColorConverter.ConvertFromString("#FF69B4"), new Pais("Northern Europe") },
            { (Color)ColorConverter.ConvertFromString("#1E90FF"), new Pais("Great Britain"  ) },
            { (Color)ColorConverter.ConvertFromString("#B22222"), new Pais("Iceland"        ) },
            { (Color)ColorConverter.ConvertFromString("#228B22"), new Pais("Scandinavia"    ) },
            { (Color)ColorConverter.ConvertFromString("#DAA520"), new Pais("Ukraine"        ) },
            { (Color)ColorConverter.ConvertFromString("#B8860B"), new Pais("Ural"           ) },
            { (Color)ColorConverter.ConvertFromString("#9932CC"), new Pais("Siberia"        ) },
            { (Color)ColorConverter.ConvertFromString("#00BFFF"), new Pais("Yakutsk"        ) },
            { (Color)ColorConverter.ConvertFromString("#FF4500"), new Pais("Kamchatka"      ) },
            { (Color)ColorConverter.ConvertFromString("#2E8B57"), new Pais("Irkutsk"        ) },
            { (Color)ColorConverter.ConvertFromString("#8B0000"), new Pais("Mongolia"       ) },
            { (Color)ColorConverter.ConvertFromString("#E9967A"), new Pais("Japan"          ) },
            { (Color)ColorConverter.ConvertFromString("#8FBC8F"), new Pais("Afghanistan"    ) },
            { (Color)ColorConverter.ConvertFromString("#483D8B"), new Pais("China"          ) },
            { (Color)ColorConverter.ConvertFromString("#BDB76B"), new Pais("Middle East") },
            { (Color)ColorConverter.ConvertFromString("#556B2F"), new Pais("India") },
            { (Color)ColorConverter.ConvertFromString("#8B008B"), new Pais("Siam") },
            { (Color)ColorConverter.ConvertFromString("#20B2AA"), new Pais("Indonesia") },
            { (Color)ColorConverter.ConvertFromString("#F08080"), new Pais("New Guinea") },
            { (Color)ColorConverter.ConvertFromString("#4682B4"), new Pais("Western Australia") },
            { (Color)ColorConverter.ConvertFromString("#D2691E"), new Pais("Eastern Australia") }
        };

        public JuegoPage()
        {
            InitializeComponent();
            var mapaColores = new BitmapImage(new Uri("pack://application:,,,/Assets/Risk_board_map.png"));
            mapaColoresWB = new WriteableBitmap(mapaColores);
            if (Utils.demo)
            {
                Utils.partida.jugadorTurno = Utils.user.Id;
                lvJugadores.ItemsSource = Utils.partida.jugadorList;

                //Decir que fase estamos 
                BordeFaseRefuerzo.Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
                BordeFaseAtacar.Background = (Brush)FindResource("FaseAtacar_BG");
                BordeFaseReagrupar.Background = (Brush)FindResource("FaseAtacar_BG");
            }
        }

        private void MapaVisible_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(MapaVisible);

            int x = (int)(pos.X * mapaColoresWB.PixelWidth / MapaVisible.ActualWidth);
            int y = (int)(pos.Y * mapaColoresWB.PixelHeight / MapaVisible.ActualHeight);

            byte[] pixel = new byte[4];
            mapaColoresWB.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);
            Color color = Color.FromArgb(255, pixel[2], pixel[1], pixel[0]); // BGRA a RGBA

            // Busca el pais
            if (colorAPais.TryGetValue(color, out Pais pais))
            {
                if (pais.jugadorId != Utils.partida.jugadorTurno)
                {
                    PintarPaisEnColor(color, Utils.GetColorFromName(Utils.partida.jugadorList.FirstOrDefault(e => e.UserId == Utils.partida.jugadorTurno).Color));
                    pais.color1 = Utils.GetColorFromName(Utils.partida.jugadorList.FirstOrDefault(e => e.UserId == Utils.partida.jugadorTurno).Color);
                    pais.jugadorId = Utils.partida.jugadorTurno;
                    RepintarLienzo();
                }
            }
        }


        private void RepintarLienzo()   
        {
            OverlayCanvas.Children.Clear();

            // Recorre todos los países y repinta usando su color actual
            foreach (var paisEntry in colorAPais)
            {
                //if(paisEntry.Value.color1 == (Color)ColorConverter.ConvertFromString("#00000000")) continue;  //Si no tiene color siguiente
                if (paisEntry.Value.jugadorId == 0)continue;
                
                Color colorOriginal = paisEntry.Key;
                Pais pais = paisEntry.Value;

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
        }

        private void MapaVisible_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(MapaVisible);

            int x = (int)(pos.X * mapaColoresWB.PixelWidth / MapaVisible.ActualWidth);
            int y = (int)(pos.Y * mapaColoresWB.PixelHeight / MapaVisible.ActualHeight);

            if (x < 0 || y < 0 || x >= mapaColoresWB.PixelWidth || y >= mapaColoresWB.PixelHeight)
                return;

            byte[] pixel = new byte[4];
            mapaColoresWB.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);
            Color color = Color.FromArgb(255, pixel[2], pixel[1], pixel[0]);

            if (colorAPais.TryGetValue(color, out Pais pais))
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
    }
}
