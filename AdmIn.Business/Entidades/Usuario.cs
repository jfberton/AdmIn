using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Usuario
    {
        #region Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        public bool Activo { get; set; }

        public DateTime Creacion { get; set; }

        #endregion

        #region Propiedades de navegación
        public List<Rol>? Roles { get; set; }
        public List<Permiso>? Permisos { get; set; }

        #endregion

        #region Metodos públicos
        public string RolString
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                {
                    return "No posee roles asignados.-";
                }

                StringBuilder sb = new StringBuilder();
                foreach (Rol rol in Roles)
                {
                    sb.Append(rol.Nombre);
                    sb.Append(", ");
                }

                return sb.ToString().TrimEnd(',', ' ');
            }
        }

        public string PermisoString
        {
            get
            {
                if (Permisos == null || Permisos.Count == 0)
                {
                    return "No posee permisos asignados.-";
                }

                StringBuilder sb = new StringBuilder();
                foreach (Permiso permiso in Permisos)
                {
                    sb.Append(permiso.Nombre);
                    sb.Append(", ");
                }

                return sb.ToString().TrimEnd(',', ' ');
            }
        }

        #endregion

        #region Métodos privados

        #endregion
    }
}
