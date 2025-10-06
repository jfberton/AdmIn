using System;
using System.Collections.Generic;

namespace AdmIn.Common.Entidades
{
    public class DetalleTrabajo
    {
        public int DetalleTrabajoId { get; set; }
        public int TrabajoProveedorId { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Costo { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public List<Imagen> Imagenes { get; set; } = new();

        // New flag: when true, the inquilino cannot change the responsable anymore
        public bool InquilinoBloqueado { get; set; } = false;
    }
}
