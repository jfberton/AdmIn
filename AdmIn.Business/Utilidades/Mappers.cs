using AdmIn.Business.Entidades;
using AdmIn.Data.Entidades;
using AdmIn.Data.Repositorios;

namespace AdmIn.Business.Utilidades
{
    public static class Mappers
    {
        public static Usuario ToBusinessUsuario(this USUARIO dataUsuario)
        {
            if (dataUsuario == null) return null;

            Rep_USUARIO_PERMISO rup = new Rep_USUARIO_PERMISO();
            Rep_PERMISO rp = new Rep_PERMISO();
            Rep_USUARIO_ROL rur = new Rep_USUARIO_ROL();
            Rep_ROL rr = new Rep_ROL();

            // Crear el usuario base
            var usuario = new Usuario
            {
                Id = dataUsuario.USU_ID,
                Nombre = dataUsuario.USU_NOMBRE,
                Password = dataUsuario.USU_PASSWORD,
                Email = dataUsuario.USU_EMAIL,
                Permisos = new List<Permiso>(), // Inicializa para evitar null
                Roles = new List<Rol>()        // Inicializa para evitar null
            };

            // Cargar permisos
            var permisos = rup.ObtenerPorUsuario(usuario.Id);
            if (permisos.Correcto && permisos.Datos != null)
            {
                var permisosDetalles = rp.Obtener_todos();
                if (permisosDetalles.Correcto && permisosDetalles.Datos != null)
                {
                    usuario.Permisos = permisos.Datos
                        .Select(p => permisosDetalles.Datos.FirstOrDefault(perm => perm.PERM_ID == p.PERM_ID)?.ToBusinessPermiso())
                        .Where(p => p != null) // Excluye nulos
                        .ToList();
                }
            }

            // Cargar roles
            var roles = rur.ObtenerPorUsuario(usuario.Id);
            if (roles.Correcto && roles.Datos != null)
            {
                var rolesDetalles = rr.Obtener_todos();
                if (rolesDetalles.Correcto && rolesDetalles.Datos != null)
                {
                    usuario.Roles = roles.Datos
                        .Select(r => rolesDetalles.Datos.FirstOrDefault(rol => rol.ROL_ID == r.ROL_ID)?.ToBusinessRol())
                        .Where(r => r != null) // Excluye nulos
                        .ToList();
                }
            }

            return usuario;
        }

        // Métodos auxiliares (permiso y rol)
        public static Permiso ToBusinessPermiso(this PERMISO dataPermiso)
        {
            if (dataPermiso == null) return null;

            return new Permiso
            {
                Id = dataPermiso.PERM_ID,
                Nombre = dataPermiso.PERM_NOMBRE
            };
        }

        public static Rol ToBusinessRol(this ROL dataRol)
        {
            if (dataRol == null) return null;

            return new Rol
            {
                Id = dataRol.ROL_ID,
                Nombre = dataRol.ROL_NOMBRE
            };
        }

        public static USUARIO ToDataUSUARIO(this Usuario data)
        {
            if (data == null) return null;

            return new USUARIO
            {
                USU_ID = data.Id,
                USU_ACTIVO = data.Activo,
                USU_EMAIL = data.Email,
                USU_FECHA_CREACION = data.Creacion,
                USU_NOMBRE = data.Nombre,
                USU_PASSWORD = data.Password
            };
        }

        public static PERMISO ToDataPERMISO(this Permiso data)
        {
            if (data == null) return null;

            return new PERMISO
            {
                PERM_ID = data.Id,
                PERM_NOMBRE = data.Nombre
            };
        }

        public static ROL ToDataROL(this Rol data)
        {
            if (data == null) return null;

            return new ROL
            {
                ROL_ID = data.Id,
                ROL_NOMBRE = data.Nombre
            };
        }
    }
}
