using System;

namespace AdmIn.Data.Entidades
{
    public class T_PolizaSeguro
    {
        public int PolizaSeguroID { get; set; }
        public int InmuebleID { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string NumPoliza { get; set; } = string.Empty;
        public string Aseguradora { get; set; } = string.Empty;
        public DateTime? VigenciaInicio { get; set; }
        public DateTime? VigenciaFin { get; set; }
        public decimal? MontoAsegurado { get; set; }
        public string ArchivoUrl { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorID { get; set; }
    }
}