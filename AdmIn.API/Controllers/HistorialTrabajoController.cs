using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialTrabajoController : ControllerBase
    {
        private readonly IServ_HistorialTrabajo _servicio;
        public HistorialTrabajoController(IServ_HistorialTrabajo servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("crear")]
        [Authorize]
        public async Task<DTO<HistorialTrabajo>> Crear([FromBody] HistorialTrabajo historial)
            => await _servicio.Crear(historial);

        [HttpPut("actualizar")]
        [Authorize]
        public async Task<DTO<HistorialTrabajo>> Actualizar([FromBody] HistorialTrabajo historial)
            => await _servicio.Actualizar(historial);

        [HttpDelete("eliminar/{id}")]
        [Authorize]
        public async Task<DTO<bool>> Eliminar(int id)
            => await _servicio.Eliminar(new HistorialTrabajo { HistorialTrabajoId = id });

        [HttpGet("obtener_por_id/{id}")]
        [Authorize]
        public async Task<DTO<HistorialTrabajo>> ObtenerPorId(int id)
            => await _servicio.Obtener_por_id(id);

        [HttpGet("obtener_todos")]
        [Authorize]
        public async Task<DTO<IEnumerable<HistorialTrabajo>>> ObtenerTodos()
            => await _servicio.Obtener_todos();

        [HttpPost("obtener_paginado")]
        [Authorize]
        public async Task<DTO<Items_pagina<HistorialTrabajo>>> ObtenerPaginado([FromBody] Filtros_paginado filtros)
            => await _servicio.Obtener_paginado(filtros);

        [HttpGet("obtener_por_trabajo/{trabajoId}")]
        [Authorize]
        public async Task<DTO<IEnumerable<HistorialTrabajo>>> ObtenerPorTrabajo(int trabajoId)
            => await _servicio.Obtener_por_trabajo(trabajoId);
    }
}
