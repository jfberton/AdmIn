using AdmIn.Data.Entidades;
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

        public List<Permiso> Permisos
        {
            get {
                HashSet<Permiso> permisos = new HashSet<Permiso>();

                foreach (Rol rol in Roles)
                {
                    foreach (Permiso permiso in rol.Permisos)
                    {
                        permisos.Add(permiso);
                    }
                }

                return permisos.ToList();
            }
        }

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

                // Utilizamos un HashSet para evitar duplicados
                HashSet<string> rolesUnicos = new HashSet<string>();

                foreach (Rol rol in Roles)
                {
                    rolesUnicos.Add(rol.Nombre);
                }

                return string.Join(", ", rolesUnicos);
            }
        }

        public string PermisoString
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                {
                    return "No posee permisos asignados.-";
                }

                // Utilizamos un HashSet para evitar duplicados
                HashSet<string> permisosUnicos = new HashSet<string>();

                foreach (Rol rol in Roles)
                {
                    foreach (Permiso permiso in rol.Permisos)
                    {
                        permisosUnicos.Add(permiso.Nombre);
                    }
                }

                return string.Join(", ", permisosUnicos);
            }
        }

        #endregion

        #region Métodos privados

        #endregion
    }
}
