using System;

namespace AdmIn.Data.Entidades
{
    public class T_TrabajoProveedor
    {
        public int TrabajoProveedorID { get; set; }
        public int InmuebleID { get; set; }
        public int ProveedorID { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal? Costo { get; set; }
        public int? ContratoID { get; set; }
        public string FacturaUrl { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorID { get; set; }
    }
}