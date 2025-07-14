using System;

namespace AdmIn.Data.Entidades
{
    public class T_CalificacionProveedor
    {
        public int CalificacionProveedorID { get; set; }
        public int TrabajoID { get; set; }
        public int CalificadorID { get; set; }
        public int? Calificacion { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}