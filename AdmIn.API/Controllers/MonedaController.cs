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
    public class MonedaController : ControllerBase
    {
        private readonly IServ_Moneda _servicio;

        public MonedaController(IServ_Moneda servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Moneda>>> Obtener_monedas(dynamic filtros_paginado)
        {
            try
            {
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);
                return await _servicio.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<Moneda>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_todos")]
        public async Task<DTO<IEnumerable<Moneda>>> Obtener_todos()
        {
            return await _servicio.Obtener_todos();
        }

        [HttpGet("obtener_por_id/{monedaId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Moneda>> Obtener_por_id(int monedaId)
        {
            return await _servicio.Obtener_por_id(new Moneda { Id = monedaId });
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Moneda>> Crear_moneda(dynamic moneda)
        {
            try
            {
                var oMoneda = JsonHelper.Deserialize<Moneda>(moneda);
                return await _servicio.Crear(oMoneda);
            }
            catch (Exception ex)
            {
                return new DTO<Moneda>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear la moneda: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Moneda>> Modificar_moneda(dynamic moneda)
        {
            try
            {
                var oMoneda = JsonHelper.Deserialize<Moneda>(moneda);
                return await _servicio.Actualizar(oMoneda);
            }
            catch (Exception ex)
            {
                return new DTO<Moneda>
                {
                    Correcto = false,
                    Mensaje = $"Error al modificar la moneda: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{monedaId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_moneda(int monedaId)
        {
            var moneda = new Moneda { Id = monedaId };
            return await _servicio.Eliminar(moneda);
        }
    }
}