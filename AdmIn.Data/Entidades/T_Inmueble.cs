using System;

namespace AdmIn.Data.Entidades
{
    public class T_Inmueble
    {
        public int InmuebleID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string CP { get; set; } = string.Empty;
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public decimal? Valor { get; set; }
        public decimal? ConstruccionM2 { get; set; }
        public decimal? RentaMensual { get; set; }
        public int? AdministradorID { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int MonedaID { get; set; } = 1;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorID { get; set; }
        public int? UsuarioModificadorID { get; set; }
    }
}