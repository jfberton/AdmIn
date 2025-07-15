using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class ApiToken
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreServicio { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime? FechaExpiracion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        #region Propiedades de navegación
        public Usuario Usuario { get; set; } = new Usuario();
        #endregion
    }
}
