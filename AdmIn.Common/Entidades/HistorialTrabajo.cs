using System;

namespace AdmIn.Common.Entidades
{
    public class HistorialTrabajo
    {
        public int HistorialTrabajoId { get; set; }
        public int TrabajoProveedorId { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string? Comentario { get; set; }
    }
}
