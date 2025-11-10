using AdmIn.Common.Utilidades;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_Usuario : IServ_Usuario
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IPasswordResetTokenRepository _tokenRepo;
        private readonly AdmIn.Common.Services.IEmailService _emailService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public Serv_Usuario(IUsuarioRepository usuarioRepository, IPasswordResetTokenRepository tokenRepository, AdmIn.Common.Services.IEmailService emailService, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _usuarioRepo = usuarioRepository;
            _tokenRepo = tokenRepository;
            _emailService = emailService;
            _config = config;
        }

        public async Task<DTO<Usuario>> Crear(Usuario usuarioNuevo)
        {
            // Primero verificar si existe un usuario con ese email
            if (!string.IsNullOrWhiteSpace(usuarioNuevo.Email))
            {
                var existente = await _usuarioRepo.Obtener_por_email(usuarioNuevo.Email);
                if (existente != null && existente.Correcto && existente.Datos != null)
                {
                    // Usuario ya existe: agregar roles que falten y no tocar la contraseña.
                    var user = existente.Datos;

                    bool cambios = false;
                    if (usuarioNuevo.Roles?.Any() == true)
                    {
                        foreach (var rol in usuarioNuevo.Roles)
                        {
                            if (user.Roles == null || !user.Roles.Any(r => r.Id == rol.Id))
                            {
                                if (user.Roles == null) user.Roles = new System.Collections.Generic.List<Rol>();
                                user.Roles.Add(rol);
                                cambios = true;
                            }
                        }
                    }

                    if (cambios)
                    {
                        var upd = await _usuarioRepo.Actualizar(user);
                        // Do NOT send email when user already exists; user is assumed validated.
                        return upd;
                    }

                    return new DTO<Usuario> { Correcto = true, Datos = user, Mensaje = "Usuario ya existe. Roles no cambiaron." };
                }
            }

            // Nuevo usuario: hashear contraseña si existe
            if (!string.IsNullOrWhiteSpace(usuarioNuevo.Password))
            {
                usuarioNuevo.Password = MiHash.GenerarHashBcrypt(usuarioNuevo.Password);
            }

            var resultado = await _usuarioRepo.Crear(usuarioNuevo);

            if (resultado != null && resultado.Correcto && resultado.Datos != null)
            {
   var creado = resultado.Datos;
 // Generar token y enviar email para crear contraseña
    var tokenRes = await GenerarTokenYGuardar(creado.Id, creado.PersonaId);
       if (tokenRes.Correcto)
          {
      var frontend = _config["Frontend:BaseUrl"] ?? "http://localhost:5000";
 var link = frontend.TrimEnd('/') + $"/confirm-password-reset?token={tokenRes.Datos}";
    var model = new System.Collections.Generic.Dictionary<string,string>
     {
          { "Name", creado.Nombre },
 { "Link", link },
 { "ExpiryHours", "24" },
        { "Year", DateTime.Now.Year.ToString() }
       };
        var body = _emailService.RenderTemplate("NewUser_SetPassword.html", model) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(body)) body = $"Hola {creado.Nombre}, use el siguiente link: {link}";
    await _emailService.SendAsync(creado.Email, "Bienvenido a m3tria - Establece tu contraseña", body);
  }
  }

            return resultado;
        }

        public async Task<DTO<Usuario>> Actualizar(Usuario usuario)
        {
            var resultado = await _usuarioRepo.Actualizar(usuario);
            return resultado;
        }

        public async Task<DTO<bool>> Eliminar(Usuario usuario)
        {
            return await _usuarioRepo.Eliminar(usuario);
        }

        public async Task<DTO<Usuario>> Obtener_por_id(Usuario usuario)
        {
            var resultado = await _usuarioRepo.Obtener_por_id(usuario);

            return resultado;
        }

        public async Task<DTO<IEnumerable<Usuario>>> Obtener_todos()
        {
            var resultado = await _usuarioRepo.Obtener_todos();
            return resultado;
        }

        public async Task<DTO<Items_pagina<Usuario>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _usuarioRepo.Obtener_paginado(filtros);
            return resultado;
        }

        public async Task<DTO<Usuario>> Validar_credenciales(LoginModel login)
        {
            Console.WriteLine($"[BUSINESS] ========== Serv_Usuario.Validar_credenciales ==========");
            Console.WriteLine($"[BUSINESS] Email recibido: {login?.Email ?? "null"}");
            // NUNCA MOSTRAR LA CONTRASEÑA - Solo indicar si está presente
            Console.WriteLine($"[BUSINESS] Password: {(string.IsNullOrWhiteSpace(login?.Password) ? "NO PROPORCIONADO" : "PROPORCIONADO")}");
            
            try
            {
                Console.WriteLine($"[BUSINESS] Llamando a _usuarioRepo.Obtener_por_email...");
                var usuarioResult = await _usuarioRepo.Obtener_por_email(login.Email);
                
                Console.WriteLine($"[BUSINESS] ✓ Respuesta del repositorio recibida");
                Console.WriteLine($"[BUSINESS] ✓ Operación correcta: {usuarioResult?.Correcto == true}");
                Console.WriteLine($"[BUSINESS] ✓ Usuario encontrado: {usuarioResult?.Datos != null}");
                
                if (!usuarioResult.Correcto || usuarioResult.Datos == null)
                {
                    Console.WriteLine($"[BUSINESS] ✗ Usuario no encontrado en base de datos");
                    Console.WriteLine($"[BUSINESS] ✗ Mensaje del repositorio: {usuarioResult?.Mensaje ?? "null"}");
                    
                    return new DTO<Usuario>
                    {
                        Correcto = false,
                        Mensaje = "Usuario no encontrado"
                    };
                }

                Console.WriteLine($"[BUSINESS] ✓ Usuario encontrado - ID: {usuarioResult.Datos.Id}");
                Console.WriteLine($"[BUSINESS] ✓ Usuario encontrado - Nombre: {usuarioResult.Datos.Nombre}");
                Console.WriteLine($"[BUSINESS] ✓ Usuario encontrado - Email: {usuarioResult.Datos.Email}");
                Console.WriteLine($"[BUSINESS] ✓ Usuario encontrado - Activo: {usuarioResult.Datos.Activo}");
                Console.WriteLine($"[BUSINESS] ✓ Usuario encontrado - Roles count: {usuarioResult.Datos.Roles?.Count ?? 0}");

                if (usuarioResult.Datos.Roles?.Any() == true)
                {
                    foreach (var rol in usuarioResult.Datos.Roles)
                    {
                        Console.WriteLine($"[BUSINESS]   - Usuario tiene rol: {rol.Nombre}");
                    }
                }

                // Verificar si la contraseña almacenada es bcrypt o SHA512
                bool passwordCorrecta = false;
                Console.WriteLine($"[BUSINESS] Iniciando verificación de contraseña...");
                Console.WriteLine($"[BUSINESS] Hash almacenado length: {usuarioResult.Datos.Password?.Length ?? 0}");

                if (MiHash.EsHashBcrypt(usuarioResult.Datos.Password))
                {
                    Console.WriteLine($"[BUSINESS] ✓ Password almacenado es formato BCrypt");
                    Console.WriteLine($"[BUSINESS] Verificando con BCrypt...");
                    
                    // Usar verificación bcrypt
                    passwordCorrecta = MiHash.VerificarHashBcrypt(login.Password, usuarioResult.Datos.Password);
                    Console.WriteLine($"[BUSINESS] ✓ Verificación BCrypt completada - Resultado: {passwordCorrecta}");
                }
                else
                {
                    Console.WriteLine($"[BUSINESS] ✓ Password almacenado es formato SHA512 (legacy)");
                    Console.WriteLine($"[BUSINESS] Verificando con SHA512...");
                    
                    // Compatibilidad con SHA512 existente
                    string passwordHasheado = MiHash.GenerarHash(login.Password);
                    passwordCorrecta = usuarioResult.Datos.Password == passwordHasheado;
                    Console.WriteLine($"[BUSINESS] ✓ Verificación SHA512 completada - Resultado: {passwordCorrecta}");
                    
                    // Si la contraseña es correcta pero está en SHA512, migrar a bcrypt
                    if (passwordCorrecta)
                    {
                        Console.WriteLine($"[BUSINESS] Password correcto, iniciando migración a BCrypt...");
                        await MigrarPasswordABcrypt(usuarioResult.Datos, login.Password);
                        Console.WriteLine($"[BUSINESS] ✓ Migración a BCrypt completada");
                    }
                }

                if (passwordCorrecta)
                {
                    Console.WriteLine($"[BUSINESS] ✓ CREDENCIALES VALIDADAS CORRECTAMENTE");
                    Console.WriteLine($"[BUSINESS] ✓ Retornando usuario completo con roles");
                    
                    return new DTO<Usuario>
                    {
                        Correcto = true,
                        Datos = usuarioResult.Datos,
                        Mensaje = "Credenciales validadas correctamente."
                    };
                }
                else
                {
                    Console.WriteLine($"[BUSINESS] ✗ PASSWORD INCORRECTO");
                    Console.WriteLine($"[BUSINESS] ✗ La contraseña no coincide con la almacenada");
                    
                    return new DTO<Usuario>
                    {
                        Correcto = false,
                        Mensaje = "Contraseña incorrecta."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSINESS] ✗ ERROR en Validar_credenciales");
                Console.WriteLine($"[BUSINESS] ✗ Exception: {ex.GetType().Name}");
                Console.WriteLine($"[BUSINESS] ✗ Message: {ex.Message}");
                Console.WriteLine($"[BUSINESS] ✗ Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[BUSINESS] ✗ Inner exception: {ex.InnerException.Message}");
                }
                
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = $"Error interno al validar credenciales: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Usuario>> Obtener_por_mail(string mail)
        {
            return await _usuarioRepo.Obtener_por_email(mail);
        }

        public async Task<DTO<bool>> Modificar_contraseña(CambioClaveModel datos)
        {
            var usuarioResult = await _usuarioRepo.Obtener_por_email(datos.Email);

            if (!usuarioResult.Correcto || usuarioResult.Datos == null)
                return new DTO<bool> { Correcto = false, Mensaje = "Usuario no encontrado." };

            // Verificar contraseña actual (compatible con ambos formatos)
            bool passwordActualCorrecta = false;

            if (MiHash.EsHashBcrypt(usuarioResult.Datos.Password))
            {
                passwordActualCorrecta = MiHash.VerificarHashBcrypt(datos.Password, usuarioResult.Datos.Password);
            }
            else
            {
                string passwordActualHasheada = MiHash.GenerarHash(datos.Password);
                passwordActualCorrecta = usuarioResult.Datos.Password == passwordActualHasheada;
            }

            if (!passwordActualCorrecta)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Datos = false,
                    Mensaje = "Contraseña actual incorrecta."
                };
            }

            // Siempre usar bcrypt para la nueva contraseña
            usuarioResult.Datos.Password = MiHash.GenerarHashBcrypt(datos.NuevaPassword);

            var actualizado = await _usuarioRepo.Actualizar(usuarioResult.Datos);

            // Enviar email de notificación de cambio de contraseña
            try
            {
                var frontend = _config["Frontend:BaseUrl"] ?? "http://localhost:5000";
                var securityLink = frontend.TrimEnd('/') + "/request-password-reset";

                var model = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "Name", usuarioResult.Datos.Nombre },
                    { "Timestamp", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                    { "Device", "Navegador Web" },
                    { "Location", "No disponible" },
                    { "SecurityLink", securityLink },
                    { "Year", DateTime.Now.Year.ToString() }
                };

                var body = _emailService.RenderTemplate("PasswordChanged_Notification.html", model);
                if (!string.IsNullOrWhiteSpace(body))
                {
                    await _emailService.SendAsync(usuarioResult.Datos.Email, "Tu contraseña ha sido actualizada", body);
                }
            }
            catch (Exception emailEx)
            {
                // No fallar el cambio de contraseña si el email falla
                Console.WriteLine($"[BUSINESS] ⚠️ Error enviando email de notificación: {emailEx.Message}");
            }

            return new DTO<bool>
            {
                Correcto = actualizado.Correcto,
                Datos = actualizado.Correcto,
                Mensaje = actualizado.Correcto
                    ? "Contraseña actualizada correctamente."
                    : actualizado.Mensaje
            };
        }

        public async Task<DTO<IEnumerable<Usuario>>> Buscar_por_termino(string termino)
        {
            return await _usuarioRepo.Buscar_por_termino(termino);
        }

        /// <summary>
        /// Migra una contraseña de SHA512 a bcrypt de forma transparente
        /// </summary>
        private async Task MigrarPasswordABcrypt(Usuario usuario, string passwordTextoPlano)
        {
            try
            {
                Console.WriteLine($"[BUSINESS] Iniciando migración de password para usuario {usuario.Id}");
                usuario.Password = MiHash.GenerarHashBcrypt(passwordTextoPlano);
                await _usuarioRepo.Actualizar(usuario);
                Console.WriteLine($"[BUSINESS] ✓ Password migrado exitosamente a BCrypt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSINESS] ✗ Error en migración de password: {ex.Message}");
                // Si falla la migración, no afectar el login
                // Se puede loggear el error si se desea
            }
        }

        public async Task<DTO<string>> GenerarTokenYGuardar(int? usuarioId, int? personaId, int expiryHours =24, string purpose = "SetPassword")
        {
            try
            {
                var token = GenerarTokenSeguro();

                var prt = new PasswordResetToken
                {
                    UsuarioId = usuarioId,
                    PersonaId = personaId,
                    Token = token,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(expiryHours),
                    IsConsumed = false,
                    Purpose = purpose
                };

                var res = await _tokenRepo.Crear(prt);
                if (!res.Correcto) return new DTO<string> { Correcto = false, Mensaje = res.Mensaje };

                return new DTO<string> { Correcto = true, Datos = token, Mensaje = "Token generado" };
            }
            catch (Exception ex)
            {
                return new DTO<string> { Correcto = false, Mensaje = ex.Message };
            }
        }

        private static string GenerarTokenSeguro(int length =32)
        {
            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            // url-safe base64
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public async Task<DTO<bool>> ResetPasswordByToken(string token, string nuevaPassword)
        {
            try
            {
                var tokenRes = await _tokenRepo.Obtener_por_token(token);
                if (!tokenRes.Correcto || tokenRes.Datos == null)
                    return new DTO<bool> { Correcto = false, Mensaje = "Token inválido" };

                var prt = tokenRes.Datos;
                if (prt.IsConsumed) return new DTO<bool> { Correcto = false, Mensaje = "Token ya consumido" };
                if (prt.ExpiresAt < DateTime.UtcNow) return new DTO<bool> { Correcto = false, Mensaje = "Token expirado" };

                if (!prt.UsuarioId.HasValue) return new DTO<bool> { Correcto = false, Mensaje = "Token no asociado a usuario" };

                var userRes = await _usuarioRepo.Obtener_por_id(new Usuario { Id = prt.UsuarioId.Value });
                if (!userRes.Correcto || userRes.Datos == null) return new DTO<bool> { Correcto = false, Mensaje = "Usuario no encontrado" };

                var user = userRes.Datos;
                user.Password = MiHash.GenerarHashBcrypt(nuevaPassword);
                var upd = await _usuarioRepo.Actualizar(user);
                if (!upd.Correcto) return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar contraseña" };

                await _tokenRepo.Marcar_consumido(prt.Id);

                // Enviar email de notificación de cambio de contraseña
                try
                {
                    var frontend = _config["Frontend:BaseUrl"] ?? "http://localhost:5000";
                    var securityLink = frontend.TrimEnd('/') + "/request-password-reset";
   
                    var model = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "Name", user.Nombre },
                        { "Timestamp", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                        { "Device", "Navegador Web" },
                        { "Location", "No disponible" },
                        { "SecurityLink", securityLink },
                        { "Year", DateTime.Now.Year.ToString() }
                    };

                    var body = _emailService.RenderTemplate("PasswordChanged_Notification.html", model);
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        await _emailService.SendAsync(user.Email, "Tu contraseña ha sido actualizada", body);
                    }
                }
                catch (Exception emailEx)
                {
                    // No fallar el reseteo si el email falla
                    Console.WriteLine($"[BUSINESS] ⚠️ Error enviando email de notificación: {emailEx.Message}");
                }

                return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Contraseña actualizada" };
            }
            catch (Exception ex)
            {
                return new DTO<bool> { Correcto = false, Mensaje = ex.Message };
            }
        }

        public async Task<DTO<bool>> GenerateAndSendPasswordResetEmail(int usuarioId)
        {
            try
            {
       var userRes = await _usuarioRepo.Obtener_por_id(new Usuario { Id = usuarioId });
        if (!userRes.Correcto || userRes.Datos == null) return new DTO<bool> { Correcto = false, Mensaje = "Usuario no encontrado" };

    var tokenRes = await GenerarTokenYGuardar(usuarioId, userRes.Datos.PersonaId);
  if (!tokenRes.Correcto) return new DTO<bool> { Correcto = false, Mensaje = tokenRes.Mensaje };

     var frontend = _config["Frontend:BaseUrl"] ?? "http://localhost:5000";
  var link = frontend.TrimEnd('/') + $"/confirm-password-reset?token={tokenRes.Datos}";
    var model = new System.Collections.Generic.Dictionary<string,string>
  {
  { "Name", userRes.Datos.Nombre },
   { "Link", link },
     { "ExpiryHours", "24" },
         { "Year", DateTime.Now.Year.ToString() }
      };
     var body = _emailService.RenderTemplate("PasswordReset_Request.html", model) ?? string.Empty;
          if (string.IsNullOrWhiteSpace(body)) body = $"Hola {userRes.Datos.Nombre}, use el siguiente link: {link}";
      await _emailService.SendAsync(userRes.Datos.Email, "Restablecer tu contraseña - m3tria", body);
     return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Email enviado" };
 }
     catch (Exception ex)
         {
return new DTO<bool> { Correcto = false, Mensaje = ex.Message };
 }
  }

        public async Task<DTO<TokenInfo>> Obtener_info_token(string token)
        {
            try
            {
                var tokenRes = await _tokenRepo.Obtener_por_token(token);
                if (!tokenRes.Correcto || tokenRes.Datos == null) return new DTO<TokenInfo> { Correcto = false, Mensaje = "Token no encontrado" };

                var prt = tokenRes.Datos;
                if (prt.IsConsumed) return new DTO<TokenInfo> { Correcto = false, Mensaje = "Token ya consumido" };
                if (prt.ExpiresAt < DateTime.UtcNow) return new DTO<TokenInfo> { Correcto = false, Mensaje = "Token expirado" };

                if (!prt.UsuarioId.HasValue) return new DTO<TokenInfo> { Correcto = false, Mensaje = "Token no asociado a usuario" };

                var userRes = await _usuarioRepo.Obtener_por_id(new Usuario { Id = prt.UsuarioId.Value });
                if (!userRes.Correcto || userRes.Datos == null) return new DTO<TokenInfo> { Correcto = false, Mensaje = "Usuario no encontrado" };

                // Usar clase concreta TokenInfo
                var result = new TokenInfo
                {
                    Usuario = userRes.Datos,
                    Token = prt
                };

                return new DTO<TokenInfo> { Correcto = true, Datos = result, Mensaje = "Token válido" };
            }
            catch (Exception ex)
            {
                return new DTO<TokenInfo> { Correcto = false, Mensaje = ex.Message };
            }
        }
    }
}
