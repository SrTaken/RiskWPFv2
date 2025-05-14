using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Datos
{
    public class Utils
    {
        public static User user;
        public static bool demo = true;
        public static Partida partida = new(); 
        public static Sala sala = new();


        public static Color GetColorFromName(string color)
        {
            switch (color)
            {
                case "Rojo":
                    return Colors.Red;
                case "Azul":
                    return Colors.Blue;
                case "Verde":
                    return Colors.Green;
                case "Amarillo":
                    return Colors.Yellow;
                default:
                    return Colors.Transparent;

            }
        }

        public static Sala getSalaFromRequest(string json)
        {
            JObject jo = JObject.Parse(json);
            JToken userToken = jo["sala"];
            if (userToken == null)
                return null;

            return userToken.ToObject<Sala>();
        }

        public static List<Sala> getSalasFromRequest(string json)
        {
            JObject jo = JObject.Parse(json);
            JToken userToken = jo["salas"];
            if (userToken == null)
                return null;

            return userToken.ToObject<List<Sala>>();
        }

        public static User GetUserFromRequest(string json)
        {
            json = json.Replace("\"login\":", "\"username\":");
            JObject jo = JObject.Parse(json);
            JToken userToken = jo["user"];
            if (userToken == null)
                return null; 

            User u =  userToken.ToObject<User>();
            u.Token = jo["token"]?.ToString();
            return u;
        }

        public static Point CalcularCentroidePaisMask(Color colorPais, WriteableBitmap mapa)
        {
            long sumaX = 0;
            long sumaY = 0;
            long cuenta = 0;
            int width = mapa.PixelWidth;
            int height = mapa.PixelHeight;
            int stride = mapa.BackBufferStride;
            byte[] pixels = new byte[height * stride];
            mapa.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    byte b = pixels[index];
                    byte g = pixels[index + 1];
                    byte r = pixels[index + 2];

                    if (r == colorPais.R && g == colorPais.G && b == colorPais.B)
                    {
                        sumaX += x;
                        sumaY += y;
                        cuenta++;
                    }
                }
            }

            if (cuenta == 0) return new Point(-1, -1);

            double cx = sumaX / (double)cuenta;
            double cy = sumaY / (double)cuenta;

            return new Point(cx, cy);
        }
    }
}
