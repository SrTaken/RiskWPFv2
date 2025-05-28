using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class Constants
    {

        public class RS
        {
            public const string ListaSalas = "getSalasRS";
            public const string JoinSala = "joinSalaRS";
            public const string ActualizarSala = "updateUserBC";
            public const string EmpezarPartida = "empezarBC";
            public const string SalirSala = "leaveSalaRS";
            public const string CrearSala = "createSalaRS";
            public const string ResultadoAtaque = "resultadoAtaqueRS";
            public const string Error = "errorRS";
            public const string Ganador = "ganadorRS";
            public const string Perdido = "hasPerdidoRS";
            public const string TeAtacan = "teAtacanRS";
            public const string HasConquistado = "hasConquistadoRS";
        }

        public class RQ
        {
            public const string Login = "login";
            public const string ListaSalas = "getSalasRQ";
            public const string CrearSala = "createSalaRQ";
            public const string JoinSala = "joinSalaRQ";
            public const string ActualizarSala = "updateSalaRQ";
            public const string SalirSala = "leaveSalaRQ";
            public const string MoverTropas = "moverTropasRQ";
            public const string Conquista = "conquistaRQ";
            public const string Defensa = "meAtacanRQ";
            public const string Ataque = "atacarRQ";
            public const string SaltarTurno = "saltarTurnoRQ";
            public const string ReforzarPais = "reforzarPaisRQ";
            public const string ReforzarTurno = "reforzarTurnoRQ";
            public const string SeleccionarPais = "seleccionarPaisRQ";
            public const string ModoDemo = "tunearJueguitoRQ";
        }
    }
}
