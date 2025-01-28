using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using AdmIn.Data.Entidades;
using AdmIn.Data.Repositorios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_Permiso : IServ_Permiso
    {
        private readonly Rep_PERMISO _repPermiso;
        private readonly Rep_ROL_PERMISO _repRolPermiso;

        public Serv_Permiso()
        {
            _repPermiso = new Rep_PERMISO();
            _repRolPermiso = new Rep_ROL_PERMISO();
        }

        public async Task<DTO<Permiso>> Crear(Permiso permiso)
        {
            var resultado = await _repPermiso.Crear(permiso.ToDataPERMISO());
            return new DTO<Permiso>
            {
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje,
                Datos = resultado.Datos?.ToBusinessPermiso()
            };
        }

        public async Task<DTO<Permiso>> Actualizar(Permiso permiso)
        {
            var resultado = await _repPermiso.Actualizar(permiso.ToDataPERMISO());
            return new DTO<Permiso>
            {
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje,
                Datos = resultado.Datos?.ToBusinessPermiso()
            };
        }

        public async Task<DTO<bool>> Eliminar(Permiso permiso)
        {
            return await _repPermiso.Eliminar(permiso.ToDataPERMISO());
        }

        public async Task<DTO<Permiso>> Obtener_por_id(Permiso permiso)
        {
            var resultado = await _repPermiso.Obtener_por_id(permiso.ToDataPERMISO());
            return new DTO<Permiso>
            {
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje,
                Datos = resultado.Datos?.ToBusinessPermiso()
            };
        }

        public async Task<DTO<IEnumerable<Permiso>>> Obtener_todos()
        {
            var resultado = await _repPermiso.Obtener_todos();
            return new DTO<IEnumerable<Permiso>>
            {
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje,
                Datos = resultado.Datos?.Select(p => p.ToBusinessPermiso()).ToList()
            };
        }

        public async Task<DTO<Items_pagina<Permiso>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = new DTO<Items_pagina<Permiso>>
            {
                Datos = new Items_pagina<Permiso>
                {
                    Items = new List<Permiso>(),
                    Total_items = 0
                }
            };

            filtros.EntityName = "Permiso";

            var permisosRepo = await _repPermiso.Obtener_paginado(filtros);
            if (permisosRepo.Correcto && permisosRepo.Datos != null)
            {
                resultado.Correcto = true;
                resultado.Mensaje = "Permisos obtenidos exitosamente";
                resultado.Datos.Items = permisosRepo.Datos.Items.Select(p => p.ToBusinessPermiso()).ToList();
                resultado.Datos.Total_items = permisosRepo.Datos.Total_items;
            }
            else
            {
                resultado.Correcto = false;
                resultado.Mensaje = "Error al obtener los permisos.";
            }

            return resultado;
        }

        public async Task<DTO<IEnumerable<Permiso>>> Obtener_por_rol(int rolId)
        {
            var resultado = new DTO<IEnumerable<Permiso>>();

            var permisosRepo = await _repRolPermiso.Obtener_por_rol(rolId);

            if (permisosRepo.Correcto && permisosRepo.Datos != null)
            {
                // Obtener todos los permisos para mapear nombres
                var todosPermisos = _repPermiso.Obtener_todos().Result;

                IEnumerable<Permiso> permisos;
                if (todosPermisos.Correcto && todosPermisos.Datos != null)
                {
                    // Convertir los permisos a objetos de negocio
                    permisos = permisosRepo.Datos
                        .Select(rp => todosPermisos.Datos.FirstOrDefault(p => p.PERM_ID == rp.PERM_ID))
                        .Where(p => p != null) // Excluir nulos
                        .Select(p => p.ToBusinessPermiso())
                        .ToList();

                    resultado.Correcto = true;
                    resultado.Mensaje = "Permisos obtenidos exitosamente";
                    resultado.Datos = permisos;
                }
                else
                {
                    resultado.Correcto = false;
                    resultado.Mensaje = todosPermisos.Mensaje ?? "Error al obtener los permisos del rol.";
                    resultado.Datos = null;
                }

            }
            else
            {
                resultado.Correcto = false;
                resultado.Mensaje = permisosRepo.Mensaje ?? "Error al obtener los permisos del rol.";
                resultado.Datos = null;
            }

            return resultado;
        }
    }
}
