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
        public List<JugadorJuego> jugadorList = new();
        public int turno;
        public string fase;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
