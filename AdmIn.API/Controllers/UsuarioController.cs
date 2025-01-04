using AdmIn.Business.Servicios;
using AdmIn.Business.Utilidades;
using AdmIn.Business.Entidades;
using AdmIn.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        [Authorize(Roles = "CRUD")]
        public async Task<DTO<Items_pagina<Usuario>>> Obtener_usuarios(Filtros_paginado filtros)
        {
            return await _servicio.Obtener_usuarios(filtros);
        }

        [HttpGet("obtener_por_mail/{mail}")]
        [Authorize(Roles = "CRUD")]
        public async Task<DTO<Usuario>> Obtener_por_mail(string mail)
        {
            return await _servicio.Obtener_usuario_mail(mail);
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "CRUD")]
        public async Task<DTO<Usuario>> Crear_usuario(dynamic usuario)
        {
            // Configuración para que la deserialización no sea sensible a mayúsculas/minúsculas
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserializa el JSON a un objeto B_Usuario con las opciones definidas
            Usuario oUsuario = JsonSerializer.Deserialize<Usuario>(Convert.ToString(usuario), options);

            if (oUsuario == null)
            {
                throw new ArgumentException("No se pudo deserializar el usuario.");
            }

            return await _servicio.Crear_usuario(oUsuario);
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "CRUD")]
        public async Task<DTO<Usuario>> Modificar_usuario(dynamic usuario)
        {
            try
            {
                // Configuración para que la deserialización no sea sensible a mayúsculas/minúsculas
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserializa el JSON a un objeto B_Usuario con las opciones definidas
                Usuario oUsuario = JsonSerializer.Deserialize<Usuario>(Convert.ToString(usuario), options);

                if (oUsuario == null)
                {
                    throw new ArgumentException("No se pudo deserializar el usuario.");
                }

                return await _servicio.Actualizar_usuario(oUsuario);
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
        [Authorize(Roles = "sa")]
        public async Task<DTO<bool>> Eliminar_usuario(int usuarioId)
        {
            return await _servicio.Eliminar_usuario(usuarioId);
        }

        [HttpPost("modificar_password")]
        [Authorize(Roles = "CRUD")]
        public async Task<DTO<bool>> Cambiar_password(CambioClaveModel datos)
        {
            return await _servicio.ModificarContraseña(datos);
        }
    }
}
