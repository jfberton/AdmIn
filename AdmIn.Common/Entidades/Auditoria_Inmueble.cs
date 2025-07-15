using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class Auditoria_Inmueble
    {
        public int Id { get; set; }
        public int? InmuebleId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Accion { get; set; } = string.Empty;
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public int? UsuarioEditorId { get; set; }

        #region Propiedades de Navegación
        public Inmueble? Inmueble { get; set; }
        public Usuario? UsuarioEditor { get; set; }

        #endregion
    }
}
