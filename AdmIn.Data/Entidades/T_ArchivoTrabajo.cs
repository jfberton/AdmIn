using System;

namespace AdmIn.Data.Entidades
{
    public class T_ArchivoTrabajo
    {
        public int ArchivoTrabajoID { get; set; }
        public int TrabajoID { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}