using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Datos
{
    public class Utils
    {
        public static User user;
        public static bool demo = false;
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

            return userToken.ToObject<User>();
        }
    }
}
