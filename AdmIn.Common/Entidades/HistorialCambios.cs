using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class HistorialCambios
    {
        public int Id { get; set; }
        public int? EntidadId { get; set; }
        public int? UsuarioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Cambios { get; set; } = string.Empty;

        #region Propiedades de Navegación
        public Usuario? Usuario { get; set; }
        public string Entidad { get; set; } = string.Empty;

        #endregion

    }
}
