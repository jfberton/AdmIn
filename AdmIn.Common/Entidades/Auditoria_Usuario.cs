using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class Auditoria_Usuario
    {
        public int Id { get; set; }
        public int? UsuarioID { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Accion { get; set; } = string.Empty;
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public int? UsuarioEditorID { get; set; }

        #region Propiedades de Navegación
        public Usuario? Usuario { get; set; }

        #endregion
    }
}
