using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AdmIn.UI.Services
{
    public class Serv_Proveedor : ServicioBase<Proveedor>, IServ_Proveedor
    {
        public Serv_Proveedor(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Proveedor> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Proveedor")
        {
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_activos()
        {
            Console.WriteLine("[UI SERVICE] ===== OBTENER ACTIVOS =====");
            Console.WriteLine("[UI SERVICE] Llamando al API para obtener proveedores activos...");
            
            try
            {
                var resultado = await EjecutarPeticion<DTO<IEnumerable<Proveedor>>>(HttpMethod.Get, "obtener_activos");
                
                if (resultado == null)
                {
                    Console.WriteLine("[UI SERVICE] ERROR: respuesta nula al obtener proveedores activos");
                    return new DTO<IEnumerable<Proveedor>> { Correcto = false, Mensaje = "No se obtuvo respuesta del servidor", Datos = new List<Proveedor>() };
                }

                Console.WriteLine($"[UI SERVICE] Respuesta del API - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[UI SERVICE] Proveedores recibidos: {resultado.Datos?.Count() ?? 0}");
                
                if (resultado.Datos != null && resultado.Datos.Any())
                {
                    foreach (var proveedor in resultado.Datos.Take(3))
                    {
                        Console.WriteLine($"[UI SERVICE] Proveedor recibido: ID={proveedor.Id}, Nombre={proveedor.Nombre}, RFC={proveedor.RFC}, Activo={proveedor.Activo}");
                    }
                    if (resultado.Datos.Count() > 3)
                    {
                        Console.WriteLine($"[UI SERVICE] ...y {resultado.Datos.Count() - 3} proveedores más");
                    }
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI SERVICE] ERROR en Obtener_activos: {ex.Message}");
                Console.WriteLine($"[UI SERVICE] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado_por_estado(Filtros_paginado filtros, bool? soloActivos = null)
        {
            Console.WriteLine("[UI SERVICE] ===== OBTENER PAGINADO POR ESTADO =====");
            Console.WriteLine($"[UI SERVICE] Filtros - Skip: {filtros.Skip}, Top: {filtros.Top}, Filter: {filtros.Filter ?? "null"}, OrderBy: {filtros.OrderBy ?? "null"}, SoloActivos: {soloActivos?.ToString() ?? "null"}");
            Console.WriteLine("[UI SERVICE] Llamando al API para obtener proveedores paginados por estado...");
            
            try
            {
                // Validar que filtros no sea null
                if (filtros == null)
                {
                    Console.WriteLine("[UI SERVICE] ERROR: filtros es null, creando filtros por defecto");
                    filtros = new Filtros_paginado
                    {
                        Skip = 0,
                        Top = 10,
                        OrderBy = "Nombre",
                        Filter = ""
                    };
                }

                // Crear el objeto con los nombres exactos que espera el JsonPropertyName
                var request = new
                {
                    filtros = new
                    {
                        filter = filtros.Filter ?? "",     // [JsonPropertyName("filter")]
                        top = filtros.Top,                 // [JsonPropertyName("top")]
                        skip = filtros.Skip,               // [JsonPropertyName("skip")]
                        orderby = filtros.OrderBy ?? "Nombre" // [JsonPropertyName("orderby")]
                    },
                    soloActivos = soloActivos              // [JsonPropertyName("soloActivos")]
                };
                
                Console.WriteLine($"[UI SERVICE] Request estructurado correctamente:");
                Console.WriteLine($"[UI SERVICE] - filtros.filter: '{request.filtros.filter}'");
                Console.WriteLine($"[UI SERVICE] - filtros.top: {request.filtros.top}");
                Console.WriteLine($"[UI SERVICE] - filtros.skip: {request.filtros.skip}");
                Console.WriteLine($"[UI SERVICE] - filtros.orderby: '{request.filtros.orderby}'");
                Console.WriteLine($"[UI SERVICE] - soloActivos: {request.soloActivos}");
                
                var resultado = await EjecutarPeticion<DTO<Items_pagina<Proveedor>>>(HttpMethod.Post, "obtener_paginado_por_estado", request);
                
                // Validar que resultado no sea null
                if (resultado == null)
                {
                    Console.WriteLine("[UI SERVICE] ERROR: resultado es null");
                    return new DTO<Items_pagina<Proveedor>>
                    {
                        Correcto = false,
                        Mensaje = "No se recibió respuesta del servidor"
                    };
                }
                
                Console.WriteLine($"[UI SERVICE] Respuesta del API - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje ?? "null"}");
                
                // Validar que resultado.Datos no sea null antes de acceder
                if (resultado.Datos != null)
                {
                    Console.WriteLine($"[UI SERVICE] Datos: Total: {resultado.Datos.Total_items}, Items: {resultado.Datos.Items?.Count() ?? 0}");
                    
                    if (resultado.Datos.Items != null && resultado.Datos.Items.Any())
                    {
                        foreach (var proveedor in resultado.Datos.Items.Take(3))
                        {
                            Console.WriteLine($"[UI SERVICE] Proveedor recibido: ID={proveedor.Id}, Nombre={proveedor.Nombre}, RFC={proveedor.RFC}, Activo={proveedor.Activo}");
                        }
                        if (resultado.Datos.Items.Count() > 3)
                        {
                            Console.WriteLine($"[UI SERVICE] ...y {resultado.Datos.Items.Count() - 3} proveedores más");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[UI SERVICE] Items es null o vacío");
                    }
                }
                else
                {
                    Console.WriteLine("[UI SERVICE] Datos es null");
                }
                
                return resultado;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[UI SERVICE] ERROR HTTP en Obtener_paginado_por_estado: {httpEx.Message}");
                Console.WriteLine($"[UI SERVICE] StackTrace HTTP: {httpEx.StackTrace}");
                
                return new DTO<Items_pagina<Proveedor>>
                {
                    Correcto = false,
                    Mensaje = $"Error de conectividad: {httpEx.Message}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI SERVICE] ERROR en Obtener_paginado_por_estado: {ex.Message}");
                Console.WriteLine($"[UI SERVICE] StackTrace: {ex.StackTrace}");
                
                return new DTO<Items_pagina<Proveedor>>
                {
                    Correcto = false,
                    Mensaje = $"Error inesperado: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Proveedor>> Obtener_por_rfc(string rfc)
        {
            Console.WriteLine($"[UI SERVICE] Obteniendo proveedor por RFC: {rfc}");
            return await EjecutarPeticion<DTO<Proveedor>>(HttpMethod.Get, $"obtener_por_rfc/{rfc}");
        }

        public async Task<DTO<Proveedor>> Obtener_por_email(string email)
        {
            Console.WriteLine($"[UI SERVICE] Obteniendo proveedor por Email: {email}");
            return await EjecutarPeticion<DTO<Proveedor>>(HttpMethod.Get, $"obtener_por_email/{email}");
        }

        public async Task<DTO<bool>> Validar_rfc_unico(string rfc, int? proveedorId = null)
        {
            var url = $"validar_rfc_unico/{rfc}";
            if (proveedorId.HasValue)
            {
                url += $"?proveedorId={proveedorId}";
            }
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Get, url);
        }

        public async Task<DTO<bool>> Validar_email_unico(string email, int? proveedorId = null)
        {
            var url = $"validar_email_unico/{email}";
            if (proveedorId.HasValue)
            {
                url += $"?proveedorId={proveedorId}";
            }
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Get, url);
        }

        public async Task<DTO<IEnumerable<TipoServicio>>> Obtener_servicios_proveedor(int proveedorId)
        {
            Console.WriteLine($"[UI SERVICE] Obteniendo servicios del proveedor ID: {proveedorId}");
            return await EjecutarPeticion<DTO<IEnumerable<TipoServicio>>>(HttpMethod.Get, $"obtener_servicios/{proveedorId}");
        }

        public async Task<DTO<bool>> Actualizar_servicios_proveedor(int proveedorId, List<int> serviciosIds)
        {
            Console.WriteLine($"[UI SERVICE] Actualizando servicios del proveedor ID: {proveedorId}");
            var datos = new { ProveedorId = proveedorId, ServiciosIds = serviciosIds };
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "actualizar_servicios", datos);
        }

        // Método específico para obtener proveedor por ID
        public async Task<DTO<Proveedor>> Obtener_por_id(int proveedorId)
        {
            Console.WriteLine($"[UI SERVICE] Obteniendo proveedor por ID: {proveedorId}");
            try
            {
                var resultado = await EjecutarPeticion<DTO<Proveedor>>(HttpMethod.Get, $"obtener_por_id/{proveedorId}");
                
                if (resultado == null)
                {
                    Console.WriteLine("[UI SERVICE] ERROR: resultado es null");
                    return new DTO<Proveedor>
                    {
                        Correcto = false,
                        Mensaje = "No se recibió respuesta del servidor"
                    };
                }
                
                Console.WriteLine($"[UI SERVICE] Resultado - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI SERVICE] ERROR obteniendo proveedor por ID: {ex.Message}");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener proveedor: {ex.Message}"
                };
            }
        }

        public async Task<object> PruebaEndpointSimple()
        {
            Console.WriteLine("[UI SERVICE] ===== PRUEBA ENDPOINT SIMPLE =====");
            try
            {
                return await EjecutarPeticion<object>(HttpMethod.Get, "test/simple");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI SERVICE] ERROR prueba simple: {ex.Message}");
                return new { error = ex.Message };
            }
        }

        public async Task<object> PruebaEndpointFiltros()
        {
            Console.WriteLine("[UI SERVICE] ===== PRUEBA ENDPOINT FILTROS =====");
            try
            {
                var request = new
                {
                    filtros = new
                    {
                        filter = "",         // [JsonPropertyName("filter")]
                        top = 10,           // [JsonPropertyName("top")]
                        skip = 0,           // [JsonPropertyName("skip")]
                        orderby = "Nombre"  // [JsonPropertyName("orderby")]
                    },
                    soloActivos = (bool?)null  // [JsonPropertyName("soloActivos")]
                };
                
                Console.WriteLine("[UI SERVICE] Request de prueba estructurado con nombres JSON correctos");
                
                return await EjecutarPeticion<object>(HttpMethod.Post, "test/filtros", request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI SERVICE] ERROR prueba filtros: {ex.Message}");
                return new { error = ex.Message };
            }
        }

        public async Task<object> PruebaEndpointJsonRaw()
        {
            Console.WriteLine("[UI SERVICE] ===== PRUEBA ENDPOINT JSON RAW =====");
            try
            {
                var request = new
                {
                    filtros = new Filtros_paginado
                    {
                        Skip = 0,
                        Top = 10,
                        OrderBy = "Nombre",
                        Filter = ""
                    },
                    soloActivos = (bool?)null
                };
                
                return await EjecutarPeticion<object>(HttpMethod.Post, "test/json-raw", request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI SERVICE] ERROR prueba JSON raw: {ex.Message}");
                return new { error = ex.Message };
            }
        }
    }
}