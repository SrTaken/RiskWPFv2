using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Sala
    {
        private int id;
        private string nombre;
        private int maxJugadores;
        private List<Jugador> jugadores = new();

        public Sala()
        {
        }

        public Sala(int id, string nombre, int maxJugadores)
        {
            this.id = id;
            this.nombre = nombre;
            this.maxJugadores = maxJugadores;
            this.jugadores = new List<Jugador>();
        }

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

        [JsonProperty("maxPlayers")]
        public int MaxJugadores
        {
            get { return maxJugadores; }
            set { maxJugadores = value; }
        }

        [JsonProperty("jugadores")]
        public List<Jugador> Jugadores
        {
            get { return jugadores; }
            set { jugadores = value; }
        }
    }
}
