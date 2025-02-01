using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Administrador : PersonaBase
    {
        public int AdministradorId { get; set; } // ID propio del Administrador
        public int? SuperiorId { get; set; } // ID del Administrador superior (puede ser null)
    }
}
