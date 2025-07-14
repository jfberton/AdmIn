using System;

namespace AdmIn.Data.Entidades
{
    public class T_CaracteristicaInmueble
    {
        public int CaracteristicaInmuebleID { get; set; }
        public int InmuebleID { get; set; }
        public int CaracteristicaID { get; set; }
        public string Valor { get; set; } = string.Empty;
    }
}