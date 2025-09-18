using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class Bitacora
    {
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public int? EntidadID { get; set; }
        public string Descripcion { get; set; } = string.Empty;

    }
}
