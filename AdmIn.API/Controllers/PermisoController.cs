using AdmIn.Business.Servicios;
using AdmIn.Business.Entidades;
using AdmIn.Common;
using AdmIn.API.Utilitarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisoController : ControllerBase
    {
        private readonly IServ_Permiso _servicio;

        public PermisoController(IServ_Permiso servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Permiso>>> Obtener_permisos(dynamic filtros_paginado)
        {
            try
            {
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);
                return await _servicio.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<Permiso>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_por_rol/{rolId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Permiso>>> Obtener_por_rol(int rolId)
        {
            return await _servicio.Obtener_por_rol(rolId);
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Permiso>> Crear_permiso(dynamic permiso)
        {
            try
            {
                var oPermiso = JsonHelper.Deserialize<Permiso>(permiso);
                return await _servicio.Crear(oPermiso);
            }
            catch (Exception ex)
            {
                return new DTO<Permiso>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear el permiso: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Permiso>> Modificar_permiso(dynamic permiso)
        {
            try
            {
                var oPermiso = JsonHelper.Deserialize<Permiso>(permiso);
                return await _servicio.Actualizar(oPermiso);
            }
            catch (Exception ex)
            {
                return new DTO<Permiso>
                {
                    Correcto = false,
                    Mensaje = $"Error al modificar el permiso: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{permisoId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_permiso(int permisoId)
        {
            var permiso = new Permiso { Id = permisoId };
            return await _servicio.Eliminar(permiso);
        }

        [HttpGet("obtener_por_id/{permisoId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Permiso>> Obtener_por_id(int permisoId)
        {
            var permiso = new Permiso { Id = permisoId };
            return await _servicio.Obtener_por_id(permiso);
        }

        [HttpGet("obtener_todos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Permiso>>> Obtener_todos()
        {
            return await _servicio.Obtener_todos();
        }
    }
}

