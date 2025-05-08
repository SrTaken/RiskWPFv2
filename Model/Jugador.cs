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
        private int user_id;
        private string partida_id;

        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonProperty("colors")]
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

        [JsonProperty("nombre")]
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
        [JsonProperty("user_id")]
        public int UserId
        {
            get { return user_id; }
            set { user_id = value; }
        }
        [JsonProperty("partida_id")]
        public string PartidaId
        {
            get { return partida_id; }
            set { partida_id = value; }
        }

        public Jugador(int user_id, string nombre, string color, bool estado)
        {
            this.user_id = id;
            this.nombre = nombre;
            this.color = color;
            this.estado = estado;
        }
        public Jugador(User user)
        {
            this.user_id = user.Id;
            this.nombre = user.Nombre;
        }

        public Jugador()
        {
        }
    }
}
