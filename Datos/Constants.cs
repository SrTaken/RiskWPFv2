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
        }

        public class RQ
        {
            public const string Login = "login";
            public const string ListaSalas = "getSalasRQ";
            public const string CrearSala = "createSalaRQ";
            public const string JoinSala = "joinSalaRQ";
            public const string ActualizarSala = "updateSalaRQ";
            public const string SalirSala = "leaveSalaRQ";
        }
    }
}
