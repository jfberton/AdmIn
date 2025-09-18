using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class CalificacionProveedor
    {
        public string Id { get; set; }
        public int TrabajoId { get; set; }
        public int CalificadorId { get; set; }
        public int? Calificacion { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;

    }
}
