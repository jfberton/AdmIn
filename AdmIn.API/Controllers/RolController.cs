using AdmIn.Business.Servicios;
using AdmIn.Business.Entidades;
using AdmIn.Common;
using AdmIn.API.Utilitarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly IServ_Rol _servicio;

        public RolController(IServ_Rol servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Rol>>> Obtener_roles(dynamic filtros_paginado)
        {
            try
            {
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);
                return await _servicio.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<Rol>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_todos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Rol>>> Obtener_todos()
        {
            return await _servicio.Obtener_todos();
        }

        [HttpGet("obtener_por_id/{rolId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Rol>> Obtener_por_id(int rolId)
        {
            return await _servicio.Obtener_por_id(new Rol { Id = rolId });
        }

        [HttpGet("obtener_por_usuario/{usuarioId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId)
        {
            return await _servicio.Obtener_por_usuario(usuarioId);
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Rol>> Crear_rol(dynamic rol)
        {
            try
            {
                var oRol = JsonHelper.Deserialize<Rol>(rol);
                return await _servicio.Crear(oRol);
            }
            catch (Exception ex)
            {
                return new DTO<Rol>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear el rol: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Rol>> Modificar_rol(dynamic rol)
        {
            try
            {
                var oRol = JsonHelper.Deserialize<Rol>(rol);
                return await _servicio.Actualizar(oRol);
            }
            catch (Exception ex)
            {
                return new DTO<Rol>
                {
                    Correcto = false,
                    Mensaje = $"Error al modificar el rol: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{rolId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_rol(int rolId)
        {
            var rol = new Rol { Id = rolId };
            return await _servicio.Eliminar(rol);
        }
    }
}
