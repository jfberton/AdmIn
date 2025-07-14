using System;

namespace AdmIn.Data.Entidades
{
    public class T_Proveedor
    {
        public int ProveedorID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public int? UsuarioID { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorID { get; set; }
        public int? UsuarioModificadorID { get; set; }
    }
}