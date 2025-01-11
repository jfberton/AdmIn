using AdmIn.Business.Servicios;
using AdmIn.Business.Utilidades;
using AdmIn.Business.Entidades;
using AdmIn.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AdmIn.API.Utilitarios;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IServ_Usuario _servicio;

        public UsuarioController(IServ_Usuario servicio)
        {
            _servicio = servicio;
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
        [Authorize(Roles = "CRUD")]
        public async Task<DTO<bool>> Cambiar_password(CambioClaveModel datos)
        {
            return await _servicio.Modificar_contraseña(datos);
        }
    }
}
