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
    public class HistorialDetalleTrabajoController : ControllerBase
    {
        private readonly IServ_HistorialDetalleTrabajo _servicio;
        public HistorialDetalleTrabajoController(IServ_HistorialDetalleTrabajo servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("crear")]
        [Authorize]
        public async Task<DTO<HistorialDetalleTrabajo>> Crear([FromBody] HistorialDetalleTrabajo historial)
            => await _servicio.Crear(historial);

        [HttpPut("actualizar")]
        [Authorize]
        public async Task<DTO<HistorialDetalleTrabajo>> Actualizar([FromBody] HistorialDetalleTrabajo historial)
            => await _servicio.Actualizar(historial);

        [HttpDelete("eliminar/{id}")]
        [Authorize]
        public async Task<DTO<bool>> Eliminar(int id)
            => await _servicio.Eliminar(new HistorialDetalleTrabajo { HistorialDetalleTrabajoId = id });

        [HttpGet("obtener_por_id/{id}")]
        [Authorize]
        public async Task<DTO<HistorialDetalleTrabajo>> ObtenerPorId(int id)
            => await _servicio.Obtener_por_id(id);

        [HttpGet("obtener_todos")]
        [Authorize]
        public async Task<DTO<IEnumerable<HistorialDetalleTrabajo>>> ObtenerTodos()
            => await _servicio.Obtener_todos();

        [HttpPost("obtener_paginado")]
        [Authorize]
        public async Task<DTO<Items_pagina<HistorialDetalleTrabajo>>> ObtenerPaginado([FromBody] Filtros_paginado filtros)
            => await _servicio.Obtener_paginado(filtros);
    }
}
