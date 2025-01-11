using AdmIn.Business.Entidades;
using AdmIn.Data.Entidades;
using AdmIn.Data.Repositorios;
using AdmIn.Data.Repositorios.AdmIn.Data.Repositorios;

namespace AdmIn.Business.Utilidades
{
    public static class Mappers
    {
        public static Usuario ToBusinessUsuario(this USUARIO dataUsuario)
        {
            if (dataUsuario == null) return null;
            
            Rep_ROL rr = new Rep_ROL();
            Rep_USUARIO_ROL rur = new Rep_USUARIO_ROL();

            // Crear el usuario base
            var usuario = new Usuario
            {
                Id = dataUsuario.USU_ID,
                Nombre = dataUsuario.USU_NOMBRE,
                Password = dataUsuario.USU_PASSWORD,
                Email = dataUsuario.USU_EMAIL,
                Roles = new List<Rol>()        // Inicializa para evitar null
            };

            // Cargar roles
            var roles = rur.Obtener_por_usuario(usuario.Id).Result;
            if (roles.Correcto && roles.Datos != null)
            {
                var rolesDetalles = rr.Obtener_todos().Result;
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

            // Instancia de los repositorios necesarios
            Rep_ROL_PERMISO repRolPermiso = new Rep_ROL_PERMISO();
            Rep_PERMISO repPermiso = new Rep_PERMISO();

            // Crear la instancia del rol
            var rol = new Rol
            {
                Id = dataRol.ROL_ID,
                Nombre = dataRol.ROL_NOMBRE,
                Permisos = new List<Permiso>() // Inicializa lista vacía
            };

            // Obtener los permisos asociados al rol
            var permisosAsociados = repRolPermiso.Obtener_por_rol(dataRol.ROL_ID).Result;
            if (permisosAsociados.Correcto && permisosAsociados.Datos != null)
            {
                // Obtener todos los permisos para mapear nombres
                var todosPermisos = repPermiso.Obtener_todos();
                if (todosPermisos.Correcto && todosPermisos.Datos != null)
                {
                    // Convertir los permisos a objetos de negocio
                    rol.Permisos = permisosAsociados.Datos
                        .Select(rp => todosPermisos.Datos.FirstOrDefault(p => p.PERM_ID == rp.PERM_ID))
                        .Where(p => p != null) // Excluir nulos
                        .Select(p => p.ToBusinessPermiso())
                        .ToList();
                }
            }

            return rol;
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
