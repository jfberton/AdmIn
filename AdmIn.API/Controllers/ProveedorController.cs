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
    public class ProveedorController : ControllerBase
    {
        private readonly IServ_Proveedor _servicio;

        public ProveedorController(IServ_Proveedor servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_proveedores(dynamic filtros_paginado)
        {
            try
            {
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);
                return await _servicio.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<Proveedor>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_todos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_todos()
        {
            return await _servicio.Obtener_todos();
        }

        [HttpGet("obtener_activos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_activos()
        {
            return await _servicio.Obtener_activos();
        }

        [HttpGet("obtener_por_id/{proveedorId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Obtener_por_id(int proveedorId)
        {
            return await _servicio.Obtener_por_id(new Proveedor { Id = proveedorId });
        }

        [HttpGet("obtener_por_rfc/{rfc}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Obtener_por_rfc(string rfc)
        {
            return await _servicio.Obtener_por_rfc(rfc);
        }

        [HttpGet("obtener_por_email/{email}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Obtener_por_email(string email)
        {
            return await _servicio.Obtener_por_email(email);
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Crear_proveedor(dynamic proveedor)
        {
            try
            {
                var oProveedor = JsonHelper.Deserialize<Proveedor>(proveedor);
                return await _servicio.Crear(oProveedor);
            }
            catch (Exception ex)
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear el proveedor: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Modificar_proveedor(dynamic proveedor)
        {
            try
            {
                var oProveedor = JsonHelper.Deserialize<Proveedor>(proveedor);
                return await _servicio.Actualizar(oProveedor);
            }
            catch (Exception ex)
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al modificar el proveedor: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{proveedorId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_proveedor(int proveedorId)
        {
            var proveedor = new Proveedor { Id = proveedorId };
            return await _servicio.Eliminar(proveedor);
        }

        [HttpGet("validar_rfc_unico/{rfc}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Validar_rfc_unico(string rfc, [FromQuery] int? proveedorId = null)
        {
            return await _servicio.Validar_rfc_unico(rfc, proveedorId);
        }

        [HttpGet("validar_email_unico/{email}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Validar_email_unico(string email, [FromQuery] int? proveedorId = null)
        {
            return await _servicio.Validar_email_unico(email, proveedorId);
        }
    }
}