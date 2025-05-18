using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Pais
    {
        private int id;
        private string nombre;

        [JsonProperty("id")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [JsonProperty("nom")]
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
    }
}
