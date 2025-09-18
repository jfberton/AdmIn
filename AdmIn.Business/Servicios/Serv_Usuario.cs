using AdmIn.Common.Utilidades;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;

namespace AdmIn.Business.Servicios
{
    public class Serv_Usuario : IServ_Usuario
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public Serv_Usuario(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepo = usuarioRepository;
        }

        public async Task<DTO<Usuario>> Crear(Usuario usuarioNuevo)
        {
            // Usar bcrypt para encriptar la contraseña de usuarios nuevos
            usuarioNuevo.Password = MiHash.GenerarHashBcrypt(usuarioNuevo.Password);

            var resultado = await _usuarioRepo.Crear(usuarioNuevo);

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

            return new DTO<bool>
            {
                Correcto = actualizado.Correcto,
                Datos = actualizado.Correcto,
                Mensaje = actualizado.Correcto
                    ? "Contraseña actualizada correctamente."
                    : actualizado.Mensaje
            };
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
    }
}
