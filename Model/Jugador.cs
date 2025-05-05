using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Jugador : INotifyPropertyChanged
    {
        private bool estado;
        private string color;
        private string nombre;
        private int id;

        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonProperty("color")]
        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        [JsonProperty("estado")]
        public bool Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        [JsonProperty("nom")]
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        [JsonProperty("id")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Jugador(int id, string nombre, string color, bool estado)
        {
            this.id = id;
            this.nombre = nombre;
            this.color = color;
            this.estado = estado;
        }
        public Jugador(User user)
        {
            this.id = user.Id;
            this.nombre = user.Nombre;
        }
    }
}
