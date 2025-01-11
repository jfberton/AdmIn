using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Rol
    {
        #region Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }

        #endregion

        #region Propiedades de navegación
        public List<Permiso>? Permisos { get; set; }

        #endregion
    }
}
