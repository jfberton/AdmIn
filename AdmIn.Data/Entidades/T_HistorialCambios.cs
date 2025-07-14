using System;

namespace AdmIn.Data.Entidades
{
    public class T_HistorialCambios
    {
        public int HistorialCambiosID { get; set; }
        public string Entidad { get; set; } = string.Empty;
        public int? EntidadID { get; set; }
        public int? UsuarioID { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Cambios { get; set; } = string.Empty;
    }
}