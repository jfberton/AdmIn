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
    public class CaracteristicaController : ControllerBase
    {
        private readonly IServ_Caracteristica _servicio;

        public CaracteristicaController(IServ_Caracteristica servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Caracteristica>>> Obtener_caracteristicas(dynamic filtros_paginado)
        {
            try
            {
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);
                return await _servicio.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<Caracteristica>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_todos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Caracteristica>>> Obtener_todos()
        {
            return await _servicio.Obtener_todos();
        }

        [HttpGet("obtener_por_id/{caracteristicaId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Caracteristica>> Obtener_por_id(int caracteristicaId)
        {
            return await _servicio.Obtener_por_id(new Caracteristica { Id = caracteristicaId });
        }

        [HttpGet("obtener_tipos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<string>>> Obtener_tipos()
        {
            return await _servicio.Obtener_tipos();
        }

        [HttpGet("obtener_por_tipo/{tipo}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Caracteristica>>> Obtener_por_tipo(string tipo)
        {
            return await _servicio.Obtener_por_tipo(tipo);
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Caracteristica>> Crear_caracteristica(dynamic caracteristica)
        {
            try
            {
                var oCaracteristica = JsonHelper.Deserialize<Caracteristica>(caracteristica);
                return await _servicio.Crear(oCaracteristica);
            }
            catch (Exception ex)
            {
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear la característica: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Caracteristica>> Modificar_caracteristica(dynamic caracteristica)
        {
            try
            {
                var oCaracteristica = JsonHelper.Deserialize<Caracteristica>(caracteristica);
                return await _servicio.Actualizar(oCaracteristica);
            }
            catch (Exception ex)
            {
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = $"Error al modificar la característica: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{caracteristicaId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_caracteristica(int caracteristicaId)
        {
            var caracteristica = new Caracteristica { Id = caracteristicaId };
            return await _servicio.Eliminar(caracteristica);
        }
    }
}