using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class ApiLog
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public string Servicio { get; set; } = string.Empty;
        public string Metodo { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        #region Propiedades de Navegación
        public Usuario? Usuario { get; set; }

        #endregion
    }
}
