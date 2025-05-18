using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class JugadorJuego
    {
        private int id;
        private string nombre;
        private string color;
        private int totalTropas;
        private int tropasTurno;
        private Dictionary<string, int> paisesControlados = new(); //pais, tropas
        private string token;

        [JsonProperty("id")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty("nombre")]
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        [JsonProperty("color")]
        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        [JsonProperty("totalTropas")]
        public int TotalTropas
        {
            get { return totalTropas; }
            set { totalTropas = value; }
        }

        [JsonProperty("tropasTurno")]
        public int TropasTurno
        {
            get { return tropasTurno; }
            set { tropasTurno = value; }
        }

        [JsonProperty("paisesControlados")]
        public Dictionary<string, int> PaisesControlados
        {
            get { return paisesControlados; }
            set { paisesControlados = value; }
        }

        [JsonProperty("token")]
        public string Token
        {
            get { return token; }
            set { token = value; }
        }
        public JugadorJuego()
        {
        }
    }
}
