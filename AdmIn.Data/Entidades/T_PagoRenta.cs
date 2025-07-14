using System;

namespace AdmIn.Data.Entidades
{
    public class T_PagoRenta
    {
        public int PagoRentaID { get; set; }
        public int ContratoID { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal? Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public bool? EsParcial { get; set; }
        public bool? Atraso { get; set; }
        public decimal? Penalizacion { get; set; }
        public string ComprobanteUrl { get; set; } = string.Empty;
        public int MonedaID { get; set; } = 1;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorID { get; set; }
        public int? UsuarioModificadorID { get; set; }
    }
}