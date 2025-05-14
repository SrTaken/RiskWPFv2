using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Model
{
    public class PaisMapa
    {
        public string nombre;
        public Color color1;
        public int jugadorId;
        public int CentroX;
        public int CentroY;
        public int Tropas;

        public PaisMapa(string nombre)
        {
            this.nombre = nombre;
        }

        public PaisMapa(string nombre, Color color1)
        {
            this.nombre = nombre;
            this.color1 = color1;
        }

    }
}
