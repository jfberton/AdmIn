using System;

namespace AdmIn.Data.Entidades
{
    public class T_TipoServicio
    {
        public int TipoServicioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}