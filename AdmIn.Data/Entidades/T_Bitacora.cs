using System;

namespace AdmIn.Data.Entidades
{
    public class T_Bitacora
    {
        public int BitacoraID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public int? EntidadID { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}