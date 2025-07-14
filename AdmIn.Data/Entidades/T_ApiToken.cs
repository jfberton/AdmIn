using System;

namespace AdmIn.Data.Entidades
{
    public class T_ApiToken
    {
        public int ApiTokenID { get; set; }
        public int UsuarioID { get; set; }
        public string NombreServicio { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime? FechaExpiracion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}