using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Inquilino : PersonaBase
    {
        public int Id { get; set; }
        public Inmueble Inmueble { get; set; } // Relación con Inmueble
    }


}
