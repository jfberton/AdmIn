using AdmIn.Business.Servicios;
using AdmIn.Common.Utilidades;
using AdmIn.Common.Entidades;
using AdmIn.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdmIn.API.Utilitarios;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IServ_Usuario _servicio;
        private readonly IConfiguration _config;

        public UsuarioController(IServ_Usuario servicio, IConfiguration config)
        {
            _servicio = servicio;
            _config = config;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Usuario>>> Obtener_usuarios(dynamic filtros_paginado)
        {
            try
            {
                // Deserializar usando la clase genérica
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);

                // Usar los filtros deserializados
                return await _servicio.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<Usuario>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }

        }

        [HttpGet("obtener_por_mail/{mail}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Usuario>> Obtener_por_mail(string mail)
        {
            return await _servicio.Obtener_por_mail(mail);
        }

        [HttpGet("obtener_por_id/{id}")]
        [Authorize]
        public async Task<DTO<Usuario>> Obtener_por_id(int id)
        {
            // Log entry to help debugging when requests arrive
            Console.WriteLine($"[API] UsuarioController.Obtener_por_id called with id={id}. Caller={User?.Identity?.Name}");
            return await _servicio.Obtener_por_id(new Usuario() { Id = id });
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Usuario>> Crear_usuario(dynamic usuario)
        {
            try
            {
                // Usar JsonHelper para deserializar dinámicamente al objeto Usuario
                Usuario oUsuario = JsonHelper.Deserialize<Usuario>(usuario);

                // Llamar al servicio con el objeto deserializado
                return await _servicio.Crear(oUsuario);
            }
            catch (Exception ex)
            {
                // Manejo de errores claros
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear el usuario: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Usuario>> Modificar_usuario(dynamic usuario)
        {
            try
            {
                // Deserializa el JSON a un objeto B_Usuario con las opciones definidas
                Usuario oUsuario = JsonHelper.Deserialize<Usuario>(usuario);

                return await _servicio.Actualizar(oUsuario);
            }
            catch (Exception ex)
            {
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = ex.Message
                };
            }
        }

        [HttpDelete("eliminar/{usuarioId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_usuario(int usuarioId)
        {
            Usuario usuario = new Usuario() { Id = usuarioId };
            return await _servicio.Eliminar(usuario);
        }

        [HttpPost("modificar_password")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Cambiar_password(CambioClaveModel datos)
        {
            return await _servicio.Modificar_contraseña(datos);
        }

        [HttpGet("buscar_por_termino/{termino}")]
        [Authorize] // allow authenticated users to search; adjust roles if necessary
        public async Task<DTO<IEnumerable<Usuario>>> Buscar_por_termino(string termino)
        {
            return await _servicio.Buscar_por_termino(termino);
        }

        [HttpPost("reset_by_token")]
        [AllowAnonymous]
        public async Task<DTO<bool>> Reset_by_token([FromBody] ResetByTokenDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.token) || string.IsNullOrWhiteSpace(request?.nuevaPassword))
                    return new DTO<bool> { Correcto = false, Mensaje = "Token y nuevaPassword son requeridos" };

                return await _servicio.ResetPasswordByToken(request.token, request.nuevaPassword);
            }
            catch (Exception ex)
            {
                return new DTO<bool> { Correcto = false, Mensaje = ex.Message };
            }
        }

        // DTO class for reset_by_token endpoint
        public class ResetByTokenDto
        {
            public string token { get; set; } = string.Empty;
            public string nuevaPassword { get; set; } = string.Empty;
        }

        [HttpPost("request_reset")]
        [AllowAnonymous]
        public async Task<DTO<bool>> Request_reset([FromBody] RequestResetDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.email)) 
                    return new DTO<bool> { Correcto = false, Mensaje = "Email requerido" };

                var usr = await _servicio.Obtener_por_mail(request.email);
                if (!usr.Correcto || usr.Datos == null) 
                    return new DTO<bool> { Correcto = false, Mensaje = "Email no registrado" };

                var sendRes = await _servicio.GenerateAndSendPasswordResetEmail(usr.Datos.Id);
                if (!sendRes.Correcto) 
                    return new DTO<bool> { Correcto = false, Mensaje = sendRes.Mensaje };

                return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Token enviado" };
            }
            catch (System.Exception ex)
            {
                return new DTO<bool> { Correcto = false, Mensaje = ex.Message };
            }
        }

        // DTO class for request_reset endpoint
        public class RequestResetDto
        {
            public string email { get; set; } = string.Empty;
        }

        [HttpGet("token_info/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> TokenInfo(string token)
        {
            try
            {
                var info = await _servicio.Obtener_info_token(token);
                if (info != null && info.Correcto)
                    return Ok(info);
                return NotFound(info);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new DTO<bool> { Correcto = false, Mensaje = ex.Message });
            }
        }
    }
}
