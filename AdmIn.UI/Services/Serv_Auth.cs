using AdmIn.Common.Entidades;
using AdmIn.Common.Utilidades;
using AdmIn.Common;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AdmIn.UI.Services
{
    public class Serv_Auth : IServ_Auth
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _auth;
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _config;
        private string path_api;
        private ILogger<Usuario> _logger;
        private string token = string.Empty;
        private Usuario usuarioLogueado;

        public Serv_Auth(IHttpClientFactory httpClientFactory, IHostEnvironment env, IConfiguration config, ILogger<Usuario> logger, AuthenticationStateProvider auth)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _env = env;
            _logger = logger;
            _auth = auth;

            if (_env.IsDevelopment())
                path_api = _config["Path_api_dev"];
            else
                path_api = _config["Path_api_prod"];

            _auth = auth;
        }

        public async Task<Usuario> Login(LoginModel login)
        {
            Console.WriteLine("========== INICIO LOGIN DESDE UI ==========");
            Console.WriteLine($"[UI AUTH] Iniciando login desde UI para: {login?.Email ?? "null"}");
            Console.WriteLine($"[UI AUTH] Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"[UI AUTH] Entorno: {(_env.IsDevelopment() ? "Development" : "Production")}");
            Console.WriteLine($"[UI AUTH] Path API configurado: {path_api ?? "null"}");
            
            try
            {
                Console.WriteLine($"[UI AUTH] ✓ Configuración cargada correctamente");
                Console.WriteLine($"[UI AUTH] Creando cliente HTTP...");
                
                var clienteHttp = _httpClientFactory.CreateClient();
                
                // Configurar timeout más largo para diagnóstico
                clienteHttp.Timeout = TimeSpan.FromSeconds(30);
                Console.WriteLine($"[UI AUTH] ✓ Cliente HTTP creado con timeout de 30 segundos");
                
                var apiUrl = path_api + "Auth/login";
                Console.WriteLine($"[UI AUTH] URL completa del API: {apiUrl}");
                Console.WriteLine($"[UI AUTH] Datos a enviar - Email: {login?.Email}, Password length: {login?.Password?.Length ?? 0}");
                
                // ===== PASO 1: DIAGNÓSTICO PREVIO - PING AL API =====
                Console.WriteLine($"[UI AUTH] 🏥 DIAGNÓSTICO: Probando conectividad con el API...");
                var pingSuccess = await TestApiConnectivity(clienteHttp, path_api);
                
                if (!pingSuccess)
                {
                    Console.WriteLine($"[UI AUTH] ❌ DIAGNÓSTICO FALLÓ: No hay conectividad con el API");
                    Console.WriteLine($"[UI AUTH] 💡 Problema: El API no responde o no está disponible en: {path_api}");
                    return null;
                }
                
                Console.WriteLine($"[UI AUTH] ✅ DIAGNÓSTICO OK: API está respondiendo correctamente");
                
                // ===== PASO 2: ENVIAR PETICIÓN DE LOGIN =====
                Console.WriteLine($"[UI AUTH] 🚀 Enviando petición POST al endpoint de login...");
                
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                var response = await clienteHttp.PostAsJsonAsync(apiUrl, login);
                
                stopwatch.Stop();
                var responseTime = stopwatch.ElapsedMilliseconds;
                
                Console.WriteLine($"[UI AUTH] ✓ Respuesta HTTP recibida en {responseTime}ms");
                Console.WriteLine($"[UI AUTH] ✓ Status Code: {response.StatusCode} ({(int)response.StatusCode})");
                Console.WriteLine($"[UI AUTH] ✓ Is Success: {response.IsSuccessStatusCode}");
                Console.WriteLine($"[UI AUTH] ✓ Content Type: {response.Content?.Headers?.ContentType?.MediaType ?? "null"}");
                Console.WriteLine($"[UI AUTH] ✓ Content Length: {response.Content?.Headers?.ContentLength ?? 0}");

                // Logging detallado de headers
                Console.WriteLine($"[UI AUTH] 📋 Response Headers:");
                foreach (var header in response.Headers)
                {
                    Console.WriteLine($"[UI AUTH]   - {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[UI AUTH] ✓ Respuesta HTTP exitosa, deserializando JSON...");
                    
                    // Leer contenido como string primero para diagnóstico
                    var rawContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[UI AUTH] 📄 Raw response content length: {rawContent?.Length ?? 0}");
                    
                    var previewLength = Math.Min(200, rawContent?.Length ?? 0);
                    var preview = rawContent?.Substring(0, previewLength) ?? "null";
                    Console.WriteLine($"[UI AUTH] 📄 Raw response preview: {preview}...");
                    
                    try
                    {
                        var dto = await response.Content.ReadFromJsonAsync<DTO<Usuario>>();
                        
                        Console.WriteLine($"[UI AUTH] ✓ JSON deserializado correctamente");
                        Console.WriteLine($"[UI AUTH] ✓ DTO.Correcto: {dto?.Correcto == true}");
                        Console.WriteLine($"[UI AUTH] ✓ DTO.Mensaje: {dto?.Mensaje ?? "null"}");
                        Console.WriteLine($"[UI AUTH] ✓ DTO.Datos es null: {dto?.Datos == null}");

                        if (dto?.Correcto == true && dto.Datos != null)
                        {
                            usuarioLogueado = dto.Datos;
                            Console.WriteLine($"[UI AUTH] ✓ Usuario logueado asignado");
                            Console.WriteLine($"[UI AUTH] ✓ Usuario ID: {usuarioLogueado?.Id ?? 0}");
                            Console.WriteLine($"[UI AUTH] ✓ Usuario Nombre: {usuarioLogueado?.Nombre ?? "null"}");
                            Console.WriteLine($"[UI AUTH] ✓ Usuario Email: {usuarioLogueado?.Email ?? "null"}");
                            Console.WriteLine($"[UI AUTH] ✓ Usuario Token length: {usuarioLogueado?.Token?.Length ?? 0}");
                            Console.WriteLine($"[UI AUTH] ✓ Usuario Roles count: {usuarioLogueado?.Roles?.Count ?? 0}");
                            
                            if (usuarioLogueado?.Roles?.Any() == true)
                            {
                                foreach (var rol in usuarioLogueado.Roles)
                                {
                                    Console.WriteLine($"[UI AUTH]   - Usuario tiene rol: {rol.Nombre}");
                                }
                            }
                            
                            Console.WriteLine($"[UI AUTH] 🎉 LOGIN EXITOSO - Retornando usuario completo");
                        }
                        else
                        {
                            Console.WriteLine($"[UI AUTH] ❌ Login fallido según respuesta del API");
                            Console.WriteLine($"[UI AUTH] ❌ Razón: {dto?.Mensaje ?? "Sin mensaje"}");
                            Console.WriteLine($"[UI AUTH] 💡 El API respondió pero las credenciales son incorrectas");
                        }

                        Console.WriteLine($"[UI AUTH] ✓ Retornando usuario: {dto?.Datos != null}");
                        Console.WriteLine("========== LOGIN UI COMPLETADO ==========");
                        
                        return dto?.Datos;
                    }
                    catch (System.Text.Json.JsonException jsonEx)
                    {
                        Console.WriteLine($"[UI AUTH] ❌ ERROR DE DESERIALIZACIÓN JSON");
                        Console.WriteLine($"[UI AUTH] ❌ JsonException: {jsonEx.Message}");
                        Console.WriteLine($"[UI AUTH] 📄 Contenido que causó error: {rawContent}");
                        Console.WriteLine($"[UI AUTH] 💡 El API respondió pero no con JSON válido");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine($"[UI AUTH] ❌ Respuesta HTTP no exitosa");
                    Console.WriteLine($"[UI AUTH] ❌ Status: {response.StatusCode} - {response.ReasonPhrase}");
                    
                    // Análisis específico por código de estado
                    await AnalyzeHttpErrorResponse(response);
                    
                    Console.WriteLine("========== LOGIN UI FALLIDO - HTTP ERROR ==========");
                    return null;
                }
            }
            catch (TaskCanceledException tcEx) when (tcEx.InnerException is TimeoutException || tcEx.CancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"[UI AUTH] ⏰ TIMEOUT ERROR - La petición tardó más de 30 segundos");
                Console.WriteLine($"[UI AUTH] ❌ TimeoutException: {tcEx.Message}");
                Console.WriteLine($"[UI AUTH] 💡 Causas posibles:");
                Console.WriteLine($"[UI AUTH]   - El API está sobrecargado");
                Console.WriteLine($"[UI AUTH]   - Problemas de red entre UI y API");
                Console.WriteLine($"[UI AUTH]   - El API está procesando pero muy lento");
                Console.WriteLine("========== LOGIN UI TIMEOUT ==========");
                return null;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[UI AUTH] 🌐 ERROR DE CONECTIVIDAD HTTP");
                Console.WriteLine($"[UI AUTH] ❌ HttpRequestException: {httpEx.Message}");
                Console.WriteLine($"[UI AUTH] ❌ Stack trace: {httpEx.StackTrace}");
                
                // Análisis específico del error HTTP
                await AnalyzeHttpRequestException(httpEx, path_api);
                
                Console.WriteLine("========== LOGIN UI ERROR HTTP ==========");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI AUTH] 💥 ERROR GENERAL INESPERADO");
                Console.WriteLine($"[UI AUTH] ❌ Exception: {ex.GetType().Name}");
                Console.WriteLine($"[UI AUTH] ❌ Message: {ex.Message}");
                Console.WriteLine($"[UI AUTH] ❌ Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[UI AUTH] ❌ Inner exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
                }
                
                Console.WriteLine("========== LOGIN UI ERROR GENERAL ==========");
                return null;
            }
        }

        // Método para probar conectividad con el API antes del login
        private async Task<bool> TestApiConnectivity(HttpClient client, string apiBasePath)
        {
            try
            {
                Console.WriteLine($"[UI AUTH] 🏥 Probando ping a: {apiBasePath}Diagnostic/ping");
                
                var pingUrl = apiBasePath + "Diagnostic/ping";
                var pingResponse = await client.GetAsync(pingUrl);
                
                Console.WriteLine($"[UI AUTH] 🏥 Ping Status: {pingResponse.StatusCode}");
                
                if (pingResponse.IsSuccessStatusCode)
                {
                    var pingContent = await pingResponse.Content.ReadAsStringAsync();
                    var pingPreviewLength = Math.Min(100, pingContent?.Length ?? 0);
                    var pingPreview = pingContent?.Substring(0, pingPreviewLength) ?? "null";
                    Console.WriteLine($"[UI AUTH] 🏥 Ping Response: {pingPreview}...");
                    return true;
                }
                else
                {
                    Console.WriteLine($"[UI AUTH] 🏥 Ping falló: {pingResponse.StatusCode} - {pingResponse.ReasonPhrase}");
                    return false;
                }
            }
            catch (Exception pingEx)
            {
                Console.WriteLine($"[UI AUTH] 🏥 Ping excepción: {pingEx.GetType().Name} - {pingEx.Message}");
                return false;
            }
        }

        // Análisis detallado de errores HTTP
        private async Task AnalyzeHttpErrorResponse(HttpResponseMessage response)
        {
            Console.WriteLine($"[UI AUTH] 🔍 ANÁLISIS DETALLADO DEL ERROR HTTP:");
            
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    Console.WriteLine($"[UI AUTH] 💡 404 NOT FOUND - El endpoint Auth/login no existe");
                    Console.WriteLine($"[UI AUTH] 💡 Verifica que el controlador Auth esté registrado correctamente");
                    break;
                    
                case System.Net.HttpStatusCode.InternalServerError:
                    Console.WriteLine($"[UI AUTH] 💡 500 INTERNAL SERVER ERROR - Error en el API");
                    Console.WriteLine($"[UI AUTH] 💡 Revisa los logs del API para más detalles");
                    break;
                    
                case System.Net.HttpStatusCode.BadRequest:
                    Console.WriteLine($"[UI AUTH] 💡 400 BAD REQUEST - Datos enviados incorrectos");
                    break;
                    
                case System.Net.HttpStatusCode.Unauthorized:
                    Console.WriteLine($"[UI AUTH] 💡 401 UNAUTHORIZED - Problema de autenticación");
                    break;
                    
                case System.Net.HttpStatusCode.ServiceUnavailable:
                    Console.WriteLine($"[UI AUTH] 💡 503 SERVICE UNAVAILABLE - API temporalmente no disponible");
                    break;
            }
            
            // Intentar leer el contenido del error
            try
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[UI AUTH] 📄 Error content: {errorContent ?? "null"}");
            }
            catch (Exception readEx)
            {
                Console.WriteLine($"[UI AUTH] ❌ No se pudo leer contenido del error: {readEx.Message}");
            }
        }

        // Análisis detallado de excepciones de petición HTTP
        private async Task AnalyzeHttpRequestException(HttpRequestException ex, string apiPath)
        {
            Console.WriteLine($"[UI AUTH] 🔍 ANÁLISIS DETALLADO DE HttpRequestException:");
            
            if (ex.Message.Contains("timeout") || ex.Message.Contains("timed out"))
            {
                Console.WriteLine($"[UI AUTH] ⏰ TIPO: Timeout de conexión");
                Console.WriteLine($"[UI AUTH] 💡 El API no respondió en el tiempo esperado");
            }
            else if (ex.Message.Contains("refused") || ex.Message.Contains("connection refused"))
            {
                Console.WriteLine($"[UI AUTH] 🚫 TIPO: Conexión rechazada");
                Console.WriteLine($"[UI AUTH] 💡 El API no está ejecutándose en: {apiPath}");
            }
            else if (ex.Message.Contains("host") || ex.Message.Contains("DNS"))
            {
                Console.WriteLine($"[UI AUTH] 🌐 TIPO: Error de resolución de host/DNS");
                Console.WriteLine($"[UI AUTH] 💡 No se puede resolver la URL: {apiPath}");
            }
            else if (ex.Message.Contains("SSL") || ex.Message.Contains("certificate"))
            {
                Console.WriteLine($"[UI AUTH] 🔒 TIPO: Error de certificado SSL/TLS");
                Console.WriteLine($"[UI AUTH] 💡 Problema con certificados HTTPS");
            }
            else
            {
                Console.WriteLine($"[UI AUTH] ❓ TIPO: Error HTTP genérico");
            }
            
            Console.WriteLine($"[UI AUTH] 🔧 SUGERENCIAS:");
            Console.WriteLine($"[UI AUTH]   1. Verifica que el API esté ejecutándose");
            Console.WriteLine($"[UI AUTH]   2. Verifica la URL en appsettings: {apiPath}");
            Console.WriteLine($"[UI AUTH]   3. Verifica firewall/proxy entre UI y API");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[UI AUTH] 🔍 Inner exception análisis: {ex.InnerException.GetType().Name}");
                Console.WriteLine($"[UI AUTH] 🔍 Inner message: {ex.InnerException.Message}");
            }
        }
    }
}
