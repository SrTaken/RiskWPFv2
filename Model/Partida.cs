using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Partida : INotifyPropertyChanged
    {
        public int id;
        public List<Jugador> jugadorList = new();
        public int jugadorTurno;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
