using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using AdmIn.Data.Entidades;
using AdmIn.Data.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_Rol : IServ_Rol
    {
        private readonly Rep_ROL _repRol;
        private readonly Rep_USUARIO_ROL _repUsuario_Rol;
        private readonly Rep_ROL_PERMISO _repRolPermiso;

        public Serv_Rol()
        {
            _repRol = new Rep_ROL();
            _repRolPermiso = new Rep_ROL_PERMISO();
        }

        public async Task<DTO<Rol>> Crear(Rol rol)
        {
            var resultado = await _repRol.Crear(rol.ToDataROL());

            if (resultado.Correcto && resultado.Datos != null)
            {
                rol.Id = resultado.Datos.ROL_ID;

                foreach (var permiso in rol.Permisos ?? new List<Permiso>())
                {
                    await _repRolPermiso.Crear(new ROL_PERMISO { ROL_ID = rol.Id, PERM_ID = permiso.Id });
                }
            }

            return new DTO<Rol>
            {
                Datos = resultado.Datos?.ToBusinessRol(),
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje
            };
        }

        public async Task<DTO<Rol>> Actualizar(Rol rol)
        {
            var resultado = await _repRol.Actualizar(rol.ToDataROL());

            if (resultado.Correcto && resultado.Datos != null)
            {
                var permisosExistentes = await _repRolPermiso.Obtener_por_rol(rol.Id);

                foreach (var permiso in permisosExistentes.Datos ?? new List<ROL_PERMISO>())
                {
                    await _repRolPermiso.Eliminar(permiso);
                }

                foreach (var permiso in rol.Permisos ?? new List<Permiso>())
                {
                    await _repRolPermiso.Crear(new ROL_PERMISO { ROL_ID = rol.Id, PERM_ID = permiso.Id });
                }
            }

            return new DTO<Rol>
            {
                Datos = resultado.Datos?.ToBusinessRol(),
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje
            };
        }

        public async Task<DTO<bool>> Eliminar(Rol rol)
        {
            var permisosExistentes = await _repRolPermiso.Obtener_por_rol(rol.Id);

            foreach (var permiso in permisosExistentes.Datos ?? new List<ROL_PERMISO>())
            {
                await _repRolPermiso.Eliminar(permiso);
            }

            return await _repRol.Eliminar(rol.ToDataROL());
        }

        public async Task<DTO<Rol>> Obtener_por_id(Rol rol)
        {
            var resultado = await _repRol.Obtener_por_id(rol.ToDataROL());

            if (resultado.Correcto && resultado.Datos != null)
            {
                var rolConPermisos = resultado.Datos.ToBusinessRol();

                return new DTO<Rol> { Datos = rolConPermisos, Correcto = true, Mensaje = "Rol obtenido correctamente" };
            }

            return new DTO<Rol> { Correcto = false, Mensaje = "Rol no encontrado" };
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_todos()
        {
            var resultado = await _repRol.Obtener_todos();

            if (resultado.Correcto && resultado.Datos != null)
            {
                var roles = new List<Rol>();

                foreach (var rolData in resultado.Datos)
                {
                    var rolConPermisos = rolData.ToBusinessRol();
                    roles.Add(rolConPermisos);
                }

                return new DTO<IEnumerable<Rol>> { Datos = roles, Correcto = true, Mensaje = "Roles obtenidos correctamente" };
            }

            return new DTO<IEnumerable<Rol>> { Correcto = false, Mensaje = "No se encontraron roles" };
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId)
        {
            var resultado = await _repUsuario_Rol.Obtener_por_usuario(usuarioId);

            if (resultado.Correcto && resultado.Datos != null)
            {
                var roles = new List<Rol>();

                foreach (var rolData in resultado.Datos)
                {
                    var rol = await this.Obtener_por_id(new Rol() { Id = rolData.ROL_ID});
                    var rolConPermisos = rol.Correcto ? rol.Datos : null;
                    if(rolConPermisos != null)
                    roles.Add(rolConPermisos);
                }

                return new DTO<IEnumerable<Rol>> { Datos = roles, Correcto = true, Mensaje = "Roles obtenidos por usuario correctamente" };
            }

            return new DTO<IEnumerable<Rol>> { Correcto = false, Mensaje = "No se encontraron roles para el usuario" };
        }

        public async Task<DTO<Items_pagina<Rol>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = new DTO<Items_pagina<Rol>>
            {
                Datos = new Items_pagina<Rol>
                {
                    Items = new List<Rol>(),
                    Total_items = 0
                }
            };

            filtros.EntityName = "Rol";

            var rolRepo = await _repRol.Obtener_paginado(filtros);

            if (rolRepo.Correcto && rolRepo.Datos != null)
            {
                resultado.Correcto = true;
                resultado.Mensaje = "Roles obtenidos exitosamente";
                resultado.Datos.Items = rolRepo.Datos.Items.Select(r => r.ToBusinessRol()).ToList();
                resultado.Datos.Total_items = rolRepo.Datos.Total_items;
            }
            else
            {
                resultado.Correcto = false;
                resultado.Mensaje = "Error al obtener los roles.";
            }

            return resultado;
        }
    }
}
