using System;

namespace AdmIn.Data.Entidades
{
    public class T_Caracteristica
    {
        public int CaracteristicaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}