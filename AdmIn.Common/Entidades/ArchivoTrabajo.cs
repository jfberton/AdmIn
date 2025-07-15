using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class ArchivoTrabajo
    {
        public int Id { get; set; }
        public int TrabajoId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        #region Propiedades de navegación
        public TrabajoProveedor? Trabajo { get; set; }

        #endregion

    }
}
