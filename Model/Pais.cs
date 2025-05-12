using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Model
{
    public class Pais
    {
        public string nombre;
        [JsonIgnore]
        public Color color1;
        public int jugadorId;
        public int CentroX;
        public int CentroY;
        public int Tropas;

        public Pais(string nombre)
        {
            this.nombre = nombre;
        }

        public Pais(string nombre, Color color1)
        {
            this.nombre = nombre;
            this.color1 = color1;
        }

    }
}
