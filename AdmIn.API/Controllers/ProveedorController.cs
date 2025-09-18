using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Business.Servicios;
using AdmIn.API.Utilitarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AdmIn.API.Controllers
{
    // DTO para recibir datos de actualización de servicios
    public class ActualizarServiciosRequest
    {
        public int ProveedorId { get; set; }
        public List<int> ServiciosIds { get; set; } = new();
    }

    // DTO para recibir datos de paginación por estado
    public class ObtenerPaginadoPorEstadoRequest
    {
        [JsonPropertyName("filtros")]
        public Filtros_paginado Filtros { get; set; } = new();
        
        [JsonPropertyName("soloActivos")]
        public bool? SoloActivos { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IServ_Proveedor _servicio;
        private readonly ILogger<ProveedorController> _logger;

        public ProveedorController(IServ_Proveedor servicio, ILogger<ProveedorController> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_proveedores(dynamic filtros_paginado)
        {
            Console.WriteLine("[API] ===== OBTENER PAGINADO =====");
            Console.WriteLine($"[API] Iniciando obtener_paginado, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);
                Console.WriteLine($"[API] Filtros recibidos - Skip: {filtros.Skip}, Top: {filtros.Top}, Filter: {filtros.Filter ?? "null"}, OrderBy: {filtros.OrderBy ?? "null"}");
                
                var resultado = await _servicio.Obtener_paginado(filtros);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[API] Datos: {(resultado.Datos != null ? $"Total: {resultado.Datos.Total_items}, Items: {resultado.Datos.Items?.Count ?? 0}" : "null")}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en obtener_paginado: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<Items_pagina<Proveedor>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpPost("obtener_paginado_por_estado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_proveedores_por_estado([FromBody] dynamic request)
        {
            Console.WriteLine("[API] ===== OBTENER PAGINADO POR ESTADO =====");
            Console.WriteLine($"[API] Iniciando obtener_paginado_por_estado, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                // Deserializar todo el request como ObtenerPaginadoPorEstadoRequest usando JsonHelper
                var requestData = JsonHelper.Deserialize<ObtenerPaginadoPorEstadoRequest>(request);
                
                Console.WriteLine($"[API] Request deserializado correctamente");
                Console.WriteLine($"[API] Filtros recibidos - Skip: {requestData.Filtros.Skip}, Top: {requestData.Filtros.Top}, Filter: {requestData.Filtros.Filter ?? "null"}, OrderBy: {requestData.Filtros.OrderBy ?? "null"}, SoloActivos: {requestData.SoloActivos?.ToString() ?? "null"}");
                
                // Llamar al servicio con los filtros deserializados
                var resultado = await _servicio.Obtener_paginado_por_estado(requestData.Filtros, requestData.SoloActivos);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[API] Datos: {(resultado.Datos != null ? $"Total: {resultado.Datos.Total_items}, Items: {resultado.Datos.Items?.Count ?? 0}" : "null")}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en obtener_paginado_por_estado: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<Items_pagina<Proveedor>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpGet("test/simple")]
        public async Task<IActionResult> TestSimple()
        {
            Console.WriteLine("[API] ===== TEST SIMPLE =====");
            
            try
            {
                return Ok(new
                {
                    success = true,
                    message = "API funcionando correctamente",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] Test simple ERROR: {ex.Message}");
                return Ok(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpPost("test/filtros")]
        // Removed authorization for testing
        public async Task<IActionResult> TestFiltros([FromBody] ObtenerPaginadoPorEstadoRequest request)
        {
            Console.WriteLine("[API] ===== TEST FILTROS =====");
            
            try
            {
                Console.WriteLine($"[API] Request recibido: {(request != null ? "OK" : "NULL")}");
                
                if (request != null)
                {
                    Console.WriteLine($"[API] Filtros: {(request.Filtros != null ? "OK" : "NULL")}");
                    if (request.Filtros != null)
                    {
                        Console.WriteLine($"[API] Skip: {request.Filtros.Skip}, Top: {request.Filtros.Top}");
                        Console.WriteLine($"[API] Filter: '{request.Filtros.Filter}', OrderBy: '{request.Filtros.OrderBy}'");
                    }
                    Console.WriteLine($"[API] SoloActivos: {request.SoloActivos}");
                }
                
                return Ok(new
                {
                    success = true,
                    message = "Test de filtros exitoso - MODEL BINDING FUNCIONANDO",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    receivedData = new
                    {
                        requestIsNull = request == null,
                        filtrosIsNull = request?.Filtros == null,
                        filtros = request?.Filtros,
                        soloActivos = request?.SoloActivos
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] Test filtros ERROR: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // Endpoint adicional para probar con un DTO muy simple
        [HttpPost("test/simple-dto")]
        public async Task<IActionResult> TestSimpleDto([FromBody] dynamic data)
        {
            Console.WriteLine("[API] ===== TEST SIMPLE DTO =====");
            
            try
            {
                Console.WriteLine($"[API] Data recibido: {data}");
                
                return Ok(new
                {
                    success = true,
                    message = "Test con dynamic exitoso",
                    receivedData = data
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] Test simple dto ERROR: {ex.Message}");
                
                return Ok(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpPost("test/json-raw")]
        public async Task<IActionResult> TestJsonRaw()
        {
            Console.WriteLine("[API] ===== TEST JSON RAW =====");
            
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                Console.WriteLine($"[API] Raw body recibido: {body}");
                
                return Ok(new
                {
                    success = true,
                    message = "Raw JSON recibido correctamente",
                    bodyLength = body.Length,
                    rawBody = body
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] Test JSON raw ERROR: {ex.Message}");
                return Ok(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("obtener_por_id/{id}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Obtener_por_id(int id)
        {
            Console.WriteLine($"[API] ===== OBTENER POR ID: {id} =====");
            Console.WriteLine($"[API] Iniciando obtener_por_id, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var proveedor = new Proveedor { Id = id };
                var resultado = await _servicio.Obtener_por_id(proveedor);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                if (resultado.Datos != null)
                {
                    Console.WriteLine($"[API] Proveedor encontrado: ID={resultado.Datos.Id}, Nombre={resultado.Datos.Nombre}");
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en obtener_por_id: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener proveedor: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_activos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_activos()
        {
            Console.WriteLine("[API] ===== OBTENER ACTIVOS =====");
            Console.WriteLine($"[API] Iniciando obtener_activos, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var resultado = await _servicio.Obtener_activos();
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[API] Proveedores activos: {resultado.Datos?.Count() ?? 0}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en obtener_activos: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<IEnumerable<Proveedor>>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener proveedores activos: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_por_rfc/{rfc}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Obtener_por_rfc(string rfc)
        {
            Console.WriteLine($"[API] ===== OBTENER POR RFC: {rfc} =====");
            Console.WriteLine($"[API] Iniciando obtener_por_rfc, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var resultado = await _servicio.Obtener_por_rfc(rfc);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                if (resultado.Datos != null)
                {
                    Console.WriteLine($"[API] Proveedor encontrado: ID={resultado.Datos.Id}, Nombre={resultado.Datos.Nombre}");
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en obtener_por_rfc: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener proveedor por RFC: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_por_email/{email}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Obtener_por_email(string email)
        {
            Console.WriteLine($"[API] ===== OBTENER POR EMAIL: {email} =====");
            Console.WriteLine($"[API] Iniciando obtener_por_email, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var resultado = await _servicio.Obtener_por_email(email);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                if (resultado.Datos != null)
                {
                    Console.WriteLine($"[API] Proveedor encontrado: ID={resultado.Datos.Id}, Nombre={resultado.Datos.Nombre}");
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en obtener_por_email: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener proveedor por email: {ex.Message}"
                };
            }
        }

        [HttpGet("validar_rfc_unico/{rfc}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Validar_rfc_unico(string rfc, [FromQuery] int? proveedorId = null)
        {
            Console.WriteLine($"[API] ===== VALIDAR RFC ÚNICO: {rfc} =====");
            Console.WriteLine($"[API] ProveedorId excluir: {proveedorId?.ToString() ?? "null"}");
            
            try
            {
                var resultado = await _servicio.Validar_rfc_unico(rfc, proveedorId);
                
                Console.WriteLine($"[API] Validación RFC - Correcto: {resultado.Correcto}, Único: {resultado.Datos}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en validar_rfc_unico: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al validar RFC único: {ex.Message}"
                };
            }
        }

        [HttpGet("validar_email_unico/{email}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Validar_email_unico(string email, [FromQuery] int? proveedorId = null)
        {
            Console.WriteLine($"[API] ===== VALIDAR EMAIL ÚNICO: {email} =====");
            Console.WriteLine($"[API] ProveedorId excluir: {proveedorId?.ToString() ?? "null"}");
            
            try
            {
                var resultado = await _servicio.Validar_email_unico(email, proveedorId);
                
                Console.WriteLine($"[API] Validación Email - Correcto: {resultado.Correcto}, Único: {resultado.Datos}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en validar_email_unico: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al validar email único: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_servicios/{proveedorId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<TipoServicio>>> Obtener_servicios_proveedor(int proveedorId)
        {
            Console.WriteLine($"[API] ===== OBTENER SERVICIOS PROVEEDOR: {proveedorId} =====");
            Console.WriteLine($"[API] Iniciando obtener_servicios_proveedor, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var resultado = await _servicio.Obtener_servicios_proveedor(proveedorId);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[API] Servicios encontrados: {resultado.Datos?.Count() ?? 0}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en obtener_servicios_proveedor: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<IEnumerable<TipoServicio>>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener servicios del proveedor: {ex.Message}"
                };
            }
        }

        [HttpPost("actualizar_servicios")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Actualizar_servicios_proveedor([FromBody] ActualizarServiciosRequest request)
        {
            Console.WriteLine($"[API] ===== ACTUALIZAR SERVICIOS PROVEEDOR: {request.ProveedorId} =====");
            Console.WriteLine($"[API] Servicios a asignar: {request.ServiciosIds?.Count ?? 0}");
            Console.WriteLine($"[API] Iniciando actualizar_servicios_proveedor, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var resultado = await _servicio.Actualizar_servicios_proveedor(request.ProveedorId, request.ServiciosIds);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en actualizar_servicios_proveedor: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar servicios del proveedor: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_todos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_todos()
        {
            Console.WriteLine("[API] ===== OBTENER TODOS =====");
            Console.WriteLine($"[API] Iniciando obtener_todos, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var resultado = await _servicio.Obtener_todos();
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[API] Proveedores encontrados: {resultado.Datos?.Count() ?? 0}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en obtener_todos: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<IEnumerable<Proveedor>>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener todos los proveedores: {ex.Message}"
                };
            }
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Crear_proveedor([FromBody] dynamic proveedor)
        {
            Console.WriteLine("[API] ===== CREAR PROVEEDOR =====");
            Console.WriteLine($"[API] Iniciando crear_proveedor, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var oProveedor = JsonHelper.Deserialize<Proveedor>(proveedor);
                Console.WriteLine($"[API] Proveedor deserializado - Nombre: {oProveedor.Nombre}, RFC: {oProveedor.RFC}");
                
                var resultado = await _servicio.Crear(oProveedor);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                if (resultado.Datos != null)
                {
                    Console.WriteLine($"[API] Proveedor creado: ID={resultado.Datos.Id}, Nombre={resultado.Datos.Nombre}");
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en crear_proveedor: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear proveedor: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Proveedor>> Modificar_proveedor([FromBody] dynamic proveedor)
        {
            Console.WriteLine("[API] ===== MODIFICAR PROVEEDOR =====");
            Console.WriteLine($"[API] Iniciando modificar_proveedor, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var oProveedor = JsonHelper.Deserialize<Proveedor>(proveedor);
                Console.WriteLine($"[API] Proveedor deserializado - ID: {oProveedor.Id}, Nombre: {oProveedor.Nombre}, RFC: {oProveedor.RFC}");
                
                var resultado = await _servicio.Actualizar(oProveedor);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                if (resultado.Datos != null)
                {
                    Console.WriteLine($"[API] Proveedor modificado: ID={resultado.Datos.Id}, Nombre={resultado.Datos.Nombre}");
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en modificar_proveedor: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al modificar proveedor: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{id}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_proveedor(int id)
        {
            Console.WriteLine($"[API] ===== ELIMINAR PROVEEDOR: {id} =====");
            Console.WriteLine($"[API] Iniciando eliminar_proveedor, usuario: {User?.Identity?.Name ?? "Anónimo"}");
            
            try
            {
                var proveedor = new Proveedor { Id = id };
                var resultado = await _servicio.Eliminar(proveedor);
                
                Console.WriteLine($"[API] Resultado obtenido - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ERROR en eliminar_proveedor: {ex.Message}");
                Console.WriteLine($"[API] StackTrace: {ex.StackTrace}");
                
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar proveedor: {ex.Message}"
                };
            }
        }
    }
}