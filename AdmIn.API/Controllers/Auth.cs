using AdmIn.API.Utilitarios;
using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdmIn.API.Services;


namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IServ_Usuario _serv_usuario;
        private readonly IApiLoggerService _apiLogger;

        public Auth(IConfiguration config, IServ_Usuario serv_usuario, IApiLoggerService apiLogger)
        {
            _config = config;
            _serv_usuario = serv_usuario;
            _apiLogger = apiLogger;
        }


        [HttpPost("login")]
        public async Task<DTO<Usuario>> Login([FromBody] dynamic duser)
        {
            Console.WriteLine("========== INICIO PROCESO LOGIN ==========");
            _apiLogger.WriteLog("========== INICIO PROCESO LOGIN ==========", "INFO", "AUTH");
            
            try
            {
                // ===== PASO 1: VALIDAR Y DESERIALIZAR DATOS =====
                Console.WriteLine($"[AUTH API] 🔄 Recibiendo datos de login...");
                _apiLogger.WriteLog("🔄 Recibiendo datos de login...", "INFO", "AUTH");
                
                Console.WriteLine($"[AUTH API] 📊 Tipo de datos recibidos: {duser?.GetType()?.Name ?? "null"}");
                // NO MOSTRAR datos raw que pueden contener contraseñas
                _apiLogger.WriteLog($"Tipo de datos recibidos: {duser?.GetType()?.Name ?? "null"}", "DEBUG", "AUTH");
                
                LoginModel user = null;
                
                try
                {
                    user = JsonHelper.Deserialize<LoginModel>(duser);
                    Console.WriteLine($"[AUTH API] ✅ Deserialización exitosa");
                    Console.WriteLine($"[AUTH API] 👤 Usuario: {user?.Email ?? "null"}");
                    // NUNCA MOSTRAR EL PASSWORD - Solo indicar si está presente
                    Console.WriteLine($"[AUTH API] 🔐 Password: {(string.IsNullOrWhiteSpace(user?.Password) ? "NO PROPORCIONADO" : "PROPORCIONADO")}");
                    _apiLogger.WriteLog($"Login para usuario: {user?.Email ?? "null"}", "INFO", "AUTH");
                }
                catch (Exception deserEx)
                {
                    Console.WriteLine($"[AUTH API] ❌ ERROR EN DESERIALIZACIÓN");
                    Console.WriteLine($"[AUTH API] ❌ Exception: {deserEx.GetType().Name}");
                    Console.WriteLine($"[AUTH API] ❌ Message: {deserEx.Message}");
                    _apiLogger.WriteLog($"ERROR DESERIALIZACIÓN: {deserEx.Message}", "ERROR", "AUTH");
                    
                    return new DTO<Usuario>()
                    {
                        Correcto = false,
                        Mensaje = $"Error en el formato de datos: {deserEx.Message}",
                        Datos = null
                    };
                }

                if (user == null)
                {
                    Console.WriteLine($"[AUTH API] ❌ Usuario deserializado es null");
                    _apiLogger.WriteLog("ERROR: Usuario deserializado es null", "ERROR", "AUTH");
                    
                    return new DTO<Usuario>()
                    {
                        Correcto = false,
                        Mensaje = "Datos de usuario inválidos",
                        Datos = null
                    };
                }

                if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                {
                    Console.WriteLine($"[AUTH API] ❌ Email o password vacío");
                    Console.WriteLine($"[AUTH API] 📧 Email: '{user.Email ?? "null"}'");
                    Console.WriteLine($"[AUTH API] 🔐 Password: {(string.IsNullOrWhiteSpace(user.Password) ? "VACÍO" : "PROPORCIONADO")}");
                    _apiLogger.WriteLog("ERROR: Email o password vacío", "ERROR", "AUTH");
                    
                    return new DTO<Usuario>()
                    {
                        Correcto = false,
                        Mensaje = "Email y contraseña son requeridos",
                        Datos = null
                    };
                }

                Console.WriteLine($"[AUTH API] ✅ Datos de entrada validados correctamente");
                Console.WriteLine($"[AUTH API] 🕒 Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                _apiLogger.WriteLog("✅ Datos de entrada validados correctamente", "INFO", "AUTH");
                
                // ===== PASO 2: VERIFICAR CONFIGURACIÓN =====
                Console.WriteLine($"[AUTH API] 🔧 Verificando configuración del sistema...");
                _apiLogger.WriteLog("🔧 Verificando configuración del sistema...", "INFO", "AUTH");
                
                var jwtKey = _config.GetSection("Jwt:Key").Value;
                Console.WriteLine($"[AUTH API] ✅ JWT Key configurado: {(!string.IsNullOrEmpty(jwtKey) ? "SÍ" : "NO")} (length: {jwtKey?.Length ?? 0})");
                _apiLogger.WriteLog($"JWT Key configurado: {(!string.IsNullOrEmpty(jwtKey) ? "SÍ" : "NO")}", "INFO", "AUTH");
                
                if (string.IsNullOrEmpty(jwtKey))
                {
                    Console.WriteLine($"[AUTH API] ❌ JWT Key no configurado - no se puede generar token");
                    _apiLogger.WriteLog("ERROR: JWT Key no configurado", "ERROR", "AUTH");
                    
                    return new DTO<Usuario>()
                    {
                        Correcto = false,
                        Mensaje = "Error de configuración del servidor",
                        Datos = null
                    };
                }
                
                // ===== PASO 3: VALIDAR CREDENCIALES =====
                Console.WriteLine($"[AUTH API] 🔍 Iniciando validación de credenciales...");
                _apiLogger.WriteLog($"Iniciando validación de credenciales para: {user.Email}", "INFO", "AUTH");
                
                DTO<Usuario> respuesta = null;
                
                try
                {
                    respuesta = await _serv_usuario.Validar_credenciales(user);
                }
                catch (Exception validEx)
                {
                    Console.WriteLine($"[AUTH API] ❌ EXCEPCIÓN EN VALIDACIÓN DE CREDENCIALES");
                    Console.WriteLine($"[AUTH API] ❌ Exception: {validEx.GetType().Name}");
                    Console.WriteLine($"[AUTH API] ❌ Message: {validEx.Message}");
                    _apiLogger.WriteLog($"ERROR EN VALIDACIÓN: {validEx.Message}", "ERROR", "AUTH");
                    
                    return new DTO<Usuario>()
                    {
                        Correcto = false,
                        Mensaje = $"Error interno al validar credenciales: {validEx.Message}",
                        Datos = null
                    };
                }
                
                Console.WriteLine($"[AUTH API] ✅ Validación completada");
                Console.WriteLine($"[AUTH API] 📊 Resultado: {(respuesta?.Correcto == true ? "ÉXITO" : "FALLO")}");
                Console.WriteLine($"[AUTH API] 💬 Mensaje: {respuesta?.Mensaje ?? "null"}");
                Console.WriteLine($"[AUTH API] 👤 Usuario encontrado: {respuesta?.Datos != null}");

                _apiLogger.WriteLog($"Validación completada - Resultado: {(respuesta?.Correcto == true ? "ÉXITO" : "FALLO")}", "INFO", "AUTH");
                _apiLogger.WriteLog($"Mensaje: {respuesta?.Mensaje ?? "null"}", "INFO", "AUTH");

                // ===== PASO 4: PROCESAR RESULTADO =====
                if (respuesta?.Correcto == true && respuesta.Datos != null)
                {
                    Console.WriteLine($"[AUTH API] 🎉 CREDENCIALES VÁLIDAS");
                    Console.WriteLine($"[AUTH API] 👤 Usuario: {respuesta.Datos.Nombre ?? "null"}");
                    Console.WriteLine($"[AUTH API] 📧 Email: {respuesta.Datos.Email ?? "null"}");
                    Console.WriteLine($"[AUTH API] 🏷️ Roles: {respuesta.Datos.Roles?.Count ?? 0}");
                    
                    _apiLogger.WriteLog($"LOGIN VÁLIDO para: {respuesta.Datos.Nombre}", "INFO", "AUTH");
                    
                    if (respuesta.Datos.Roles?.Any() == true)
                    {
                        foreach (var rol in respuesta.Datos.Roles)
                        {
                            Console.WriteLine($"[AUTH API]   - Rol: {rol?.Nombre ?? "null"}");
                            _apiLogger.WriteLog($"Usuario tiene rol: {rol?.Nombre ?? "null"}", "DEBUG", "AUTH");
                        }
                    }

                    // ===== PASO 5: GENERAR TOKEN JWT =====
                    Console.WriteLine($"[AUTH API] 🔐 Generando token JWT...");
                    _apiLogger.WriteLog("🔐 Generando token JWT...", "INFO", "AUTH");
                    
                    try
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_config.GetSection("Jwt:Key").Value);
                        var claims = new List<Claim>() { new Claim(ClaimTypes.Name, respuesta.Datos.Nombre ?? "") };

                        // Add user id claims for clients and SignalR
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, respuesta.Datos.Id.ToString()));
                        claims.Add(new Claim("Id", respuesta.Datos.Id.ToString()));

                        Console.WriteLine($"[AUTH API] ✅ Claims base creados - Name: {respuesta.Datos.Nombre ?? "null"}");

                        foreach (var permiso in respuesta.Datos.Roles ?? new List<Rol>())
                        {
                            if (!string.IsNullOrEmpty(permiso?.Nombre))
                            {
                                claims.Add(new Claim(ClaimTypes.Role, permiso.Nombre));
                                Console.WriteLine($"[AUTH API] ✅ Claim de rol agregado: {permiso.Nombre}");
                            }
                        }

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.UtcNow.AddHours(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        var tokenString = tokenHandler.WriteToken(token);

                        Console.WriteLine($"[AUTH API] ✅ Token JWT generado correctamente");
                        Console.WriteLine($"[AUTH API] ⏰ Token expira en: {DateTime.UtcNow.AddHours(1):yyyy-MM-dd HH:mm:ss} UTC");
                        Console.WriteLine($"[AUTH API] 📏 Token length: {tokenString?.Length ?? 0}");
                        
                        _apiLogger.WriteLog($"Token JWT generado - Length: {tokenString?.Length ?? 0}", "INFO", "AUTH");

                        var usuarioResponse = new DTO<Usuario>()
                        {
                            Correcto = true,
                            Mensaje = "Usuario validado correctamente",
                            Datos = new Usuario
                            {
                                Id = respuesta.Datos.Id,
                                Nombre = respuesta.Datos.Nombre,
                                Email = respuesta.Datos.Email,
                                Roles = respuesta.Datos.Roles,
                                ImagenPerfilId = respuesta.Datos.ImagenPerfilId,
                                ImagenPerfil = respuesta.Datos.ImagenPerfil,
                                Token = tokenString
                            }
                        };

                        Console.WriteLine($"[AUTH API] ✅ Respuesta final preparada correctamente");
                        Console.WriteLine("========== LOGIN EXITOSO ==========");
                        _apiLogger.WriteLog("========== LOGIN EXITOSO ==========", "INFO", "AUTH");
                        
                        return usuarioResponse;
                    }
                    catch (Exception tokenEx)
                    {
                        Console.WriteLine($"[AUTH API] ❌ ERROR GENERANDO TOKEN JWT");
                        Console.WriteLine($"[AUTH API] ❌ Exception: {tokenEx.GetType().Name}");
                        Console.WriteLine($"[AUTH API] ❌ Message: {tokenEx.Message}");
                        _apiLogger.WriteLog($"ERROR GENERANDO TOKEN: {tokenEx.Message}", "ERROR", "AUTH");
                        
                        return new DTO<Usuario>()
                        {
                            Correcto = false,
                            Mensaje = "Error interno generando token de autenticación",
                            Datos = null
                        };
                    }
                }
                else
                {
                    Console.WriteLine($"[AUTH API] ❌ CREDENCIALES INVÁLIDAS");
                    Console.WriteLine($"[AUTH API] ❌ Razón: {respuesta?.Mensaje ?? "Respuesta nula del servicio"}");
                    Console.WriteLine("========== LOGIN FALLIDO ==========");
                    
                    _apiLogger.WriteLog($"LOGIN FALLIDO - Razón: {respuesta?.Mensaje ?? "Respuesta nula"}", "WARNING", "AUTH");
                    _apiLogger.WriteLog("========== LOGIN FALLIDO ==========", "WARNING", "AUTH");
                    
                    return new DTO<Usuario>()
                    {
                        Correcto = false,
                        Mensaje = respuesta?.Mensaje ?? "Credenciales inválidas",
                        Datos = null
                    };
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AUTH API] 💥 ERROR CRÍTICO EN PROCESO DE LOGIN");
                Console.WriteLine($"[AUTH API] ❌ Exception: {ex.GetType().Name}");
                Console.WriteLine($"[AUTH API] ❌ Message: {ex.Message}");
                Console.WriteLine($"[AUTH API] ❌ Stack trace: {ex.StackTrace}");
                
                _apiLogger.WriteLog($"ERROR CRÍTICO: {ex.GetType().Name} - {ex.Message}", "ERROR", "AUTH");
                _apiLogger.WriteLog("========== LOGIN ERROR CRÍTICO ==========", "ERROR", "AUTH");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[AUTH API] ❌ Inner exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
                    _apiLogger.WriteLog($"Inner exception: {ex.InnerException.Message}", "ERROR", "AUTH");
                }
                
                Console.WriteLine("========== LOGIN ERROR ==========");
                
                return new DTO<Usuario>()
                {
                    Correcto = false,
                    Mensaje = $"Error interno del servidor: {ex.Message}",
                    Datos = null
                };
            }
        }

        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok();
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult GetUser()
        {
            var claims = User.Identity as ClaimsIdentity;
            return Ok(claims.Name);
        }

        [HttpGet("protectedwithscope")]
        [Authorize(Roles = "admin_usuario")]
        public IActionResult GetUserWithScope()
        {
            var claims = User.Identity as ClaimsIdentity;
            return Ok(claims.Name);
        }

        [HttpGet("protectedwithscope2")]
        [Authorize(Roles = "admin_usuario")]
        public IActionResult GetUserWithScope2()
        {
            var claims = User.Identity as ClaimsIdentity;
            return Ok(claims.Name);
        }
    }
}
