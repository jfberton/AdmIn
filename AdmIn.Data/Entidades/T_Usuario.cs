using System;

namespace AdmIn.Data.Entidades
{
    public class T_Usuario
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int? PersonaID { get; set; }
        public int? EmpresaID { get; set; }
        public int MonedaID { get; set; } = 1;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorID { get; set; }
        public int? UsuarioModificadorID { get; set; }
    }
}