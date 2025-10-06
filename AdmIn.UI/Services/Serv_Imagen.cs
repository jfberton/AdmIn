using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text;

namespace AdmIn.UI.Services
{
    public class Serv_Imagen : ServicioBase<Imagen>, IServ_Imagen
    {
        public Serv_Imagen(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Imagen> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Imagen")
        {
        }

        public async Task<DTO<Imagen>> SubirImagen(Stream archivoStream, string nombreArchivo, string? descripcion = null)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                
                // Agregar archivo
                var streamContent = new StreamContent(archivoStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ObtenerContentType(nombreArchivo));
                content.Add(streamContent, "archivo", nombreArchivo);
                
                // Agregar descripción si existe
                if (!string.IsNullOrEmpty(descripcion))
                {
                    content.Add(new StringContent(descripcion, Encoding.UTF8), "descripcion");
                }

                return await EjecutarPeticionConArchivos<DTO<Imagen>>(HttpMethod.Post, "upload", content);
            }
            catch (Exception ex)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al subir imagen: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Imagen>> SubirImagenParaInmueble(
            Stream archivoStream, 
            string nombreArchivo, 
            int inmuebleId, 
            string? descripcion = null, 
            bool establecerComoPrincipal = false)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                
                // Agregar archivo
                var streamContent = new StreamContent(archivoStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ObtenerContentType(nombreArchivo));
                content.Add(streamContent, "archivo", nombreArchivo);
                
                // Agregar descripción si existe
                if (!string.IsNullOrEmpty(descripcion))
                {
                    content.Add(new StringContent(descripcion, Encoding.UTF8), "descripcion");
                }
                
                // Agregar flag de imagen principal
                content.Add(new StringContent(establecerComoPrincipal.ToString(), Encoding.UTF8), "establecerComoPrincipal");

                return await EjecutarPeticionConArchivos<DTO<Imagen>>(HttpMethod.Post, $"upload-for-property/{inmuebleId}", content);
            }
            catch (Exception ex)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al subir imagen para inmueble: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Imagen>> SubirImagenParaUsuario(
            Stream archivoStream, 
            string nombreArchivo, 
            int usuarioId, 
            string? descripcion = null, 
            bool establecerComoPerfil = true)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                
                // Agregar archivo
                var streamContent = new StreamContent(archivoStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ObtenerContentType(nombreArchivo));
                content.Add(streamContent, "archivo", nombreArchivo);
                
                // Agregar descripción si existe
                if (!string.IsNullOrEmpty(descripcion))
                {
                    content.Add(new StringContent(descripcion, Encoding.UTF8), "descripcion");
                }
                
                // Agregar flag de imagen de perfil
                content.Add(new StringContent(establecerComoPerfil.ToString(), Encoding.UTF8), "establecerComoPerfil");

                return await EjecutarPeticionConArchivos<DTO<Imagen>>(HttpMethod.Post, $"upload-for-user/{usuarioId}", content);
            }
            catch (Exception ex)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al subir imagen para usuario: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Imagen>> SubirImagenParaTrabajo(Stream archivoStream, string nombreArchivo, int trabajoId, string? descripcion = null)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(archivoStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ObtenerContentType(nombreArchivo));
                content.Add(streamContent, "archivo", nombreArchivo);
                if (!string.IsNullOrEmpty(descripcion)) content.Add(new StringContent(descripcion, Encoding.UTF8), "descripcion");

                return await EjecutarPeticionConArchivos<DTO<Imagen>>(HttpMethod.Post, $"upload-for-trabajo/{trabajoId}", content);
            }
            catch (Exception ex)
            {
                return new DTO<Imagen> { Correcto = false, Mensaje = $"Error al subir imagen para trabajo: {ex.Message}" };
            }
        }

        public async Task<DTO<Imagen>> SubirImagenParaDetalle(Stream archivoStream, string nombreArchivo, int detalleId, string? descripcion = null)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(archivoStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ObtenerContentType(nombreArchivo));
                content.Add(streamContent, "archivo", nombreArchivo);
                if (!string.IsNullOrEmpty(descripcion)) content.Add(new StringContent(descripcion, Encoding.UTF8), "descripcion");

                return await EjecutarPeticionConArchivos<DTO<Imagen>>(HttpMethod.Post, $"upload-for-detalle/{detalleId}", content);
            }
            catch (Exception ex)
            {
                return new DTO<Imagen> { Correcto = false, Mensaje = $"Error al subir imagen para detalle: {ex.Message}" };
            }
        }

        public async Task<DTO<Imagen>> ObtenerPorId(Guid imagenId)
        {
            return await EjecutarPeticion<DTO<Imagen>>(HttpMethod.Get, $"obtener_por_id/{imagenId}");
        }

        public async Task<DTO<IEnumerable<Imagen>>> ObtenerTodas()
        {
            return await EjecutarPeticion<DTO<IEnumerable<Imagen>>>(HttpMethod.Get, "obtener_todos");
        }

        public async Task<DTO<Items_pagina<Imagen>>> ObtenerPaginado(Filtros_paginado filtros)
        {
            return await EjecutarPeticion<DTO<Items_pagina<Imagen>>>(HttpMethod.Post, "obtener_paginado", filtros);
        }

        public async Task<DTO<IEnumerable<Imagen>>> ObtenerPorInmueble(int inmuebleId)
        {
            return await EjecutarPeticion<DTO<IEnumerable<Imagen>>>(HttpMethod.Get, $"obtener_por_inmueble/{inmuebleId}");
        }

        public new async Task<DTO<Imagen>> Actualizar(Imagen imagen)
        {
            return await EjecutarPeticion<DTO<Imagen>>(HttpMethod.Put, $"actualizar/{imagen.Id}", imagen);
        }

        public async Task<DTO<bool>> EstablecerComoPrincipal(Guid imagenId, int inmuebleId)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, $"establecer-principal/{imagenId}/inmueble/{inmuebleId}");
        }

        public async Task<DTO<bool>> EstablecerImagenPerfil(Guid imagenId, int usuarioId)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, $"establecer-perfil/{imagenId}/usuario/{usuarioId}");
        }

        public async Task<DTO<bool>> Eliminar(Guid imagenId)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Delete, $"eliminar/{imagenId}");
        }

        public async Task<DTO<bool>> AsociarA_Detalle(Guid imagenId, int detalleId)
        {
            // The API endpoint expects the image Id in the request body (JSON) and detalleId in route
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, $"asociar-a-detalle/{detalleId}", imagenId);
        }

        #region Métodos auxiliares

        protected async Task<T> EjecutarPeticionConArchivos<T>(HttpMethod metodo, string endpoint, MultipartFormDataContent content)
        {
            try
            {
                var token = await base.ObtenerToken();
                var client = _httpClientFactory.CreateClient();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var request = new HttpRequestMessage(metodo, $"{_pathApi}{endpoint}")
                {
                    Content = content
                };

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>() ?? default(T)!;
                }
                else
                {
                    throw new Exception($"Error en la API: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en petición con archivos: {ex.Message}");
            }
        }

        private string ObtenerContentType(string nombreArchivo)
        {
            var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };
        }

        #endregion
    }
}