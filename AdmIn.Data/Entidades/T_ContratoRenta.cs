using System;

namespace AdmIn.Data.Entidades
{
    public class T_ContratoRenta
    {
        public int ContratoRentaID { get; set; }
        public int InmuebleID { get; set; }
        public int InquilinoID { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? MontoRenta { get; set; }
        public string Condiciones { get; set; } = string.Empty;
        public decimal? Deposito { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string DocFirmadoUrl { get; set; } = string.Empty;
        public int? PolizaRentaSeguraID { get; set; }
        public int MonedaID { get; set; } = 1;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorID { get; set; }
        public int? UsuarioModificadorID { get; set; }
    }
}