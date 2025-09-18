using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Business.Servicios;
using AdmIn.API.Utilitarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoServicioController : ControllerBase
    {
        private readonly IServ_TipoServicio _servicio;

        public TipoServicioController(IServ_TipoServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<TipoServicio>>> Obtener_tipos_servicio(dynamic filtros_paginado)
        {
            try
            {
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);
                return await _servicio.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<TipoServicio>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_todos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<TipoServicio>>> Obtener_todos()
        {
            return await _servicio.Obtener_todos();
        }

        [HttpGet("obtener_por_id/{tipoServicioId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<TipoServicio>> Obtener_por_id(int tipoServicioId)
        {
            return await _servicio.Obtener_por_id(new TipoServicio { Id = tipoServicioId });
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<TipoServicio>> Crear_tipo_servicio(dynamic tipoServicio)
        {
            try
            {
                var oTipoServicio = JsonHelper.Deserialize<TipoServicio>(tipoServicio);
                return await _servicio.Crear(oTipoServicio);
            }
            catch (Exception ex)
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear el tipo de servicio: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<TipoServicio>> Modificar_tipo_servicio(dynamic tipoServicio)
        {
            try
            {
                var oTipoServicio = JsonHelper.Deserialize<TipoServicio>(tipoServicio);
                return await _servicio.Actualizar(oTipoServicio);
            }
            catch (Exception ex)
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = $"Error al modificar el tipo de servicio: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{tipoServicioId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_tipo_servicio(int tipoServicioId)
        {
            var tipoServicio = new TipoServicio { Id = tipoServicioId };
            return await _servicio.Eliminar(tipoServicio);
        }
    }
}