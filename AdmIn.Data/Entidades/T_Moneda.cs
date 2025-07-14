using System;

namespace AdmIn.Data.Entidades
{
    public class T_Moneda
    {
        public int MonedaID { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}