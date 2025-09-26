using System;

namespace AdmIn.Common.Entidades
{
    public class HistorialDetalleTrabajo
    {
        public int HistorialDetalleTrabajoId { get; set; }
        public int DetalleTrabajoId { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string? Comentario { get; set; }
    }
}
