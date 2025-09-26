using System;

namespace AdmIn.Common.Entidades
{
    public class CalificacionProveedor
    {
        public int CalificacionProveedorId { get; set; }
        public int TrabajoProveedorId { get; set; }
        public int ProveedorId { get; set; }
        public int UsuarioId { get; set; }
        public int Valor { get; set; }
        public string? Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }
}
