using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        public static bool demo = true;
        public static Partida partida = new(); 


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

        public static User GetUserFromRequest(string json)
        {
            JObject jo = JObject.Parse(json);
            JToken userToken = jo["user"];
            if (userToken == null)
                return null; 

            return userToken.ToObject<User>();
        }
    }
}
