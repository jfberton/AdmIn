using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class InmuebleCopropietario
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public int? PersonaId { get; set; }
        public int? EmpresaId { get; set; }
        public decimal Porcentaje { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorId { get; set; }
    }
}
