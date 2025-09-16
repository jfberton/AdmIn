using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using AdmIn.Common.Utilidades;

namespace AdmIn.Business.Servicios
{
    public class Serv_Proveedor : IServ_Proveedor
    {
        private readonly IProveedorRepository _proveedorRepo;
        private readonly IUsuarioRepository _usuarioRepo;

        public Serv_Proveedor(IProveedorRepository proveedorRepository, IUsuarioRepository usuarioRepository)
        {
            _proveedorRepo = proveedorRepository;
            _usuarioRepo = usuarioRepository;
        }

        public async Task<DTO<Proveedor>> Crear(Proveedor proveedor)
        {
            Console.WriteLine($"[BUSINESS] ===== CREAR PROVEEDOR =====");
            Console.WriteLine($"[BUSINESS] Creando proveedor: {proveedor.Nombre}, RFC: {proveedor.RFC}, Email: {proveedor.Email}");

            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
            {
                Console.WriteLine("[BUSINESS] ERROR: Nombre requerido");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El nombre del proveedor es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(proveedor.RFC))
            {
                Console.WriteLine("[BUSINESS] ERROR: RFC requerido");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El RFC del proveedor es requerido."
                };
            }

            // El email es obligatorio para crear el usuario asociado
            if (string.IsNullOrWhiteSpace(proveedor.Email))
            {
                Console.WriteLine("[BUSINESS] ERROR: Email requerido");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El email del proveedor es requerido para crear el usuario asociado."
                };
            }

            Console.WriteLine("[BUSINESS] Validaciones básicas pasadas, continuando...");

            // Validar RFC único
            var rfcExiste = await Validar_rfc_unico(proveedor.RFC);
            if (!rfcExiste.Correcto)
            {
                Console.WriteLine("[BUSINESS] ERROR: RFC ya existe");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "Ya existe un proveedor con ese RFC."
                };
            }

            // Validar email único
            var emailExiste = await Validar_email_unico(proveedor.Email);
            if (!emailExiste.Correcto)
            {
                Console.WriteLine("[BUSINESS] ERROR: Email ya existe");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "Ya existe un proveedor con ese email."
                };
            }

            // Validar formato de email básico
            if (!EsEmailValido(proveedor.Email))
            {
                Console.WriteLine("[BUSINESS] ERROR: Formato de email inválido");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El formato del email no es válido."
                };
            }

            // Verificar que no existe un usuario con ese email
            var usuarioExistente = await _usuarioRepo.Obtener_por_email(proveedor.Email);
            if (usuarioExistente.Correcto && usuarioExistente.Datos != null)
            {
                Console.WriteLine("[BUSINESS] ERROR: Usuario con este email ya existe");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "Ya existe un usuario con ese email en el sistema."
                };
            }

            Console.WriteLine("[BUSINESS] Creando usuario asociado...");

            // Crear usuario asociado
            var nuevoUsuario = new Usuario
            {
                Nombre = proveedor.Nombre,
                Email = proveedor.Email,
                Password = GenerarPasswordTemporal(), // Generar password temporal
                Pais = "",
                Telefono = proveedor.Telefono ?? "",
                MonedaId = 1, // Asumir moneda por defecto
                Activo = true,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                UsuarioCreador = proveedor.UsuarioCreadorId,
                UsuarioModificador = proveedor.UsuarioCreadorId
            };

            var resultadoUsuario = await _usuarioRepo.Crear(nuevoUsuario);
            if (!resultadoUsuario.Correcto || resultadoUsuario.Datos == null)
            {
                Console.WriteLine($"[BUSINESS] ERROR creando usuario: {resultadoUsuario.Mensaje}");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear el usuario asociado: {resultadoUsuario.Mensaje}"
                };
            }

            Console.WriteLine($"[BUSINESS] Usuario creado exitosamente, ID: {resultadoUsuario.Datos.Id}");

            // Establecer valores por defecto y usuario asociado
            proveedor.UsuarioId = resultadoUsuario.Datos.Id;
            proveedor.FechaCreacion = DateTime.Now;
            proveedor.FechaModificacion = DateTime.Now;
            proveedor.Activo = true;

            Console.WriteLine("[BUSINESS] Creando proveedor en repositorio...");
            var resultado = await _proveedorRepo.Crear(proveedor);
            
            if (!resultado.Correcto)
            {
                Console.WriteLine($"[BUSINESS] ERROR creando proveedor: {resultado.Mensaje}");
                // Si falla la creación del proveedor, eliminar el usuario creado
                await _usuarioRepo.Eliminar(resultadoUsuario.Datos);
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear proveedor: {resultado.Mensaje}. El usuario asociado fue eliminado."
                };
            }

            Console.WriteLine($"[BUSINESS] Proveedor creado exitosamente: ID={resultado.Datos?.Id}");

            // El repositorio ya asigna el rol automáticamente
            return resultado;
        }

        public async Task<DTO<Proveedor>> Actualizar(Proveedor proveedor)
        {
            Console.WriteLine($"[BUSINESS] ===== ACTUALIZAR PROVEEDOR =====");
            Console.WriteLine($"[BUSINESS] Actualizando proveedor ID: {proveedor.Id}, Nombre: {proveedor.Nombre}, RFC: {proveedor.RFC}, Email: {proveedor.Email}");

            // Validaciones de negocio
            if (proveedor.Id <= 0)
            {
                Console.WriteLine("[BUSINESS] ERROR: ID de proveedor inválido");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "ID de proveedor inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
            {
                Console.WriteLine("[BUSINESS] ERROR: Nombre requerido");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El nombre del proveedor es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(proveedor.RFC))
            {
                Console.WriteLine("[BUSINESS] ERROR: RFC requerido");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El RFC del proveedor es requerido."
                };
            }

            Console.WriteLine("[BUSINESS] Validando unicidad de RFC y Email...");

            // Validar RFC único (excluyendo el proveedor actual)
            var rfcExiste = await Validar_rfc_unico(proveedor.RFC, proveedor.Id);
            if (!rfcExiste.Correcto)
            {
                Console.WriteLine("[BUSINESS] ERROR: RFC ya existe para otro proveedor");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "Ya existe otro proveedor con ese RFC."
                };
            }

            // Validar email único si se proporciona (excluyendo el proveedor actual)
            if (!string.IsNullOrWhiteSpace(proveedor.Email))
            {
                var emailExiste = await Validar_email_unico(proveedor.Email, proveedor.Id);
                if (!emailExiste.Correcto)
                {
                    Console.WriteLine("[BUSINESS] ERROR: Email ya existe para otro proveedor");
                    return new DTO<Proveedor>
                    {
                        Correcto = false,
                        Mensaje = "Ya existe otro proveedor con ese email."
                    };
                }

                // Validar formato de email básico
                if (!EsEmailValido(proveedor.Email))
                {
                    Console.WriteLine("[BUSINESS] ERROR: Formato de email inválido");
                    return new DTO<Proveedor>
                    {
                    Correcto = false,
                    Mensaje = "El formato del email no es válido."
                    };
                }
            }

            // Actualizar fecha de modificación
            proveedor.FechaModificacion = DateTime.Now;
            Console.WriteLine("[BUSINESS] Fecha de modificación actualizada");

            Console.WriteLine("[BUSINESS] Actualizando proveedor en repositorio...");
            var resultado = await _proveedorRepo.Actualizar(proveedor);
            
            if (!resultado.Correcto)
            {
                Console.WriteLine($"[BUSINESS] ERROR actualizando proveedor: {resultado.Mensaje}");
                return resultado;
            }

            Console.WriteLine($"[BUSINESS] Proveedor actualizado exitosamente: ID={resultado.Datos?.Id}");

            // Si la actualización es exitosa y hay un usuario asociado, actualizar el usuario también
            if (resultado.Correcto && proveedor.UsuarioId.HasValue)
            {
                Console.WriteLine("[BUSINESS] Actualizando usuario asociado...");
                var usuarioExistente = await _usuarioRepo.Obtener_por_id(new Usuario { Id = proveedor.UsuarioId.Value });
                if (usuarioExistente.Correcto && usuarioExistente.Datos != null)
                {
                    usuarioExistente.Datos.Nombre = proveedor.Nombre;
                    usuarioExistente.Datos.Email = proveedor.Email ?? usuarioExistente.Datos.Email;
                    usuarioExistente.Datos.Telefono = proveedor.Telefono ?? usuarioExistente.Datos.Telefono;
                    usuarioExistente.Datos.FechaModificacion = DateTime.Now;
                    usuarioExistente.Datos.UsuarioModificador = proveedor.UsuarioModificadorId;

                    await _usuarioRepo.Actualizar(usuarioExistente.Datos);
                    Console.WriteLine($"[BUSINESS] Usuario asociado actualizado: ID={usuarioExistente.Datos.Id}");
                }
                else
                {
                    Console.WriteLine($"[BUSINESS] ERROR: No se encontró el usuario asociado para actualizar (ID={proveedor.UsuarioId.Value})");
                }
            }

            return resultado;
        }

        public async Task<DTO<bool>> Eliminar(Proveedor proveedor)
        {
            Console.WriteLine($"[BUSINESS] ===== ELIMINAR PROVEEDOR =====");
            Console.WriteLine($"[BUSINESS] Eliminando proveedor ID: {proveedor.Id}");

            if (proveedor.Id <= 0)
            {
                Console.WriteLine("[BUSINESS] ERROR: ID de proveedor inválido");
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "ID de proveedor inválido."
                };
            }

            var resultado = await _proveedorRepo.Eliminar(proveedor);
            
            if (resultado.Correcto)
            {
                Console.WriteLine("[BUSINESS] Proveedor eliminado exitosamente");
            }
            else
            {
                Console.WriteLine($"[BUSINESS] ERROR eliminando proveedor: {resultado.Mensaje}");
            }

            return resultado;
        }

        public async Task<DTO<Proveedor>> Obtener_por_id(Proveedor proveedor)
        {
            Console.WriteLine($"[BUSINESS] ===== OBTENER POR ID: {proveedor.Id} =====");
            
            if (proveedor.Id <= 0)
            {
                Console.WriteLine("[BUSINESS] ERROR: ID inválido");
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "ID de proveedor inválido."
                };
            }

            Console.WriteLine("[BUSINESS] Llamando al repositorio...");
            var resultado = await _proveedorRepo.Obtener_por_id(proveedor);
            Console.WriteLine($"[BUSINESS] Resultado del repositorio - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
            
            return resultado;
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_todos()
        {
            Console.WriteLine($"[BUSINESS] ===== OBTENER TODOS =====");
            Console.WriteLine("[BUSINESS] Llamando al repositorio para obtener todos los proveedores...");
            
            try
            {
                var resultado = await _proveedorRepo.Obtener_todos();
                Console.WriteLine($"[BUSINESS] Resultado del repositorio - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[BUSINESS] Proveedores encontrados: {resultado.Datos?.Count() ?? 0}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSINESS] ERROR en Obtener_todos: {ex.Message}");
                Console.WriteLine($"[BUSINESS] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado(Filtros_paginado filtros)
        {
            Console.WriteLine($"[BUSINESS] ===== OBTENER PAGINADO =====");
            Console.WriteLine($"[BUSINESS] Filtros - Skip: {filtros.Skip}, Top: {filtros.Top}, Filter: {filtros.Filter ?? "null"}, OrderBy: {filtros.OrderBy ?? "null"}");
            Console.WriteLine("[BUSINESS] Llamando al repositorio para obtener proveedores paginados...");
            
            try
            {
                var resultado = await _proveedorRepo.Obtener_paginado(filtros);
                Console.WriteLine($"[BUSINESS] Resultado del repositorio - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[BUSINESS] Datos obtenidos: {(resultado.Datos != null ? $"Total: {resultado.Datos.Total_items}, Items: {resultado.Datos.Items?.Count() ?? 0}" : "null")}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSINESS] ERROR en Obtener_paginado: {ex.Message}");
                Console.WriteLine($"[BUSINESS] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_activos()
        {
            Console.WriteLine($"[BUSINESS] ===== OBTENER ACTIVOS =====");
            Console.WriteLine("[BUSINESS] Llamando al repositorio para obtener proveedores activos...");
            
            try
            {
                var resultado = await _proveedorRepo.Obtener_activos();
                Console.WriteLine($"[BUSINESS] Resultado del repositorio - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[BUSINESS] Proveedores activos encontrados: {resultado.Datos?.Count() ?? 0}");
                
                if (resultado.Datos != null && resultado.Datos.Any())
                {
                    foreach (var proveedor in resultado.Datos.Take(3)) // Solo los primeros 3 para no saturar los logs
                    {
                        Console.WriteLine($"[BUSINESS] Proveedor activo: ID={proveedor.Id}, Nombre={proveedor.Nombre}, RFC={proveedor.RFC}, Activo={proveedor.Activo}");
                    }
                    if (resultado.Datos.Count() > 3)
                    {
                        Console.WriteLine($"[BUSINESS] ...y {resultado.Datos.Count() - 3} proveedores más");
                    }
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSINESS] ERROR en Obtener_activos: {ex.Message}");
                Console.WriteLine($"[BUSINESS] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado_por_estado(Filtros_paginado filtros, bool? soloActivos = null)
        {
            Console.WriteLine($"[BUSINESS] ===== OBTENER PAGINADO POR ESTADO =====");
            Console.WriteLine($"[BUSINESS] Filtros - Skip: {filtros.Skip}, Top: {filtros.Top}, Filter: {filtros.Filter ?? "null"}, OrderBy: {filtros.OrderBy ?? "null"}, SoloActivos: {soloActivos?.ToString() ?? "null"}");
            Console.WriteLine("[BUSINESS] Llamando al repositorio para obtener proveedores paginados por estado...");
            
            try
            {
                var resultado = await _proveedorRepo.Obtener_paginado_por_estado(filtros, soloActivos);
                Console.WriteLine($"[BUSINESS] Resultado del repositorio - Correcto: {resultado.Correcto}, Mensaje: {resultado.Mensaje}");
                Console.WriteLine($"[BUSINESS] Datos obtenidos: {(resultado.Datos != null ? $"Total: {resultado.Datos.Total_items}, Items: {resultado.Datos.Items?.Count() ?? 0}" : "null")}");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSINESS] ERROR en Obtener_paginado_por_estado: {ex.Message}");
                Console.WriteLine($"[BUSINESS] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<DTO<Proveedor>> Obtener_por_rfc(string rfc)
        {
            if (string.IsNullOrWhiteSpace(rfc))
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "RFC es requerido."
                };
            }

            return await _proveedorRepo.Obtener_por_rfc(rfc);
        }

        public async Task<DTO<Proveedor>> Obtener_por_email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "Email es requerido."
                };
            }

            return await _proveedorRepo.Obtener_por_email(email);
        }

        public async Task<DTO<bool>> Validar_rfc_unico(string rfc, int? proveedorId = null)
        {
            var resultado = await _proveedorRepo.Obtener_por_rfc(rfc);
            
            if (!resultado.Correcto || resultado.Datos == null)
            {
                // No existe, es único
                return new DTO<bool> { Correcto = true, Datos = true };
            }

            // Si estamos actualizando y es el mismo proveedor, es válido
            if (proveedorId.HasValue && resultado.Datos.Id == proveedorId.Value)
            {
                return new DTO<bool> { Correcto = true, Datos = true };
            }

            // Ya existe otro proveedor con ese RFC
            return new DTO<bool> { Correcto = false, Datos = false };
        }

        public async Task<DTO<bool>> Validar_email_unico(string email, int? proveedorId = null)
        {
            var resultado = await _proveedorRepo.Obtener_por_email(email);
            
            if (!resultado.Correcto || resultado.Datos == null)
            {
                // No existe, es único
                return new DTO<bool> { Correcto = true, Datos = true };
            }

            // Si estamos actualizando y es el mismo proveedor, es válido
            if (proveedorId.HasValue && resultado.Datos.Id == proveedorId.Value)
            {
                return new DTO<bool> { Correcto = true, Datos = true };
            }

            // Ya existe otro proveedor con ese email
            return new DTO<bool> { Correcto = false, Datos = false };
        }

        // Métodos para servicios de proveedor (implementación real con base de datos)
        public async Task<DTO<IEnumerable<TipoServicio>>> Obtener_servicios_proveedor(int proveedorId)
        {
            if (proveedorId <= 0)
            {
                return new DTO<IEnumerable<TipoServicio>>
                {
                    Correcto = false,
                    Mensaje = "ID de proveedor inválido."
                };
            }

            try
            {
                // Usar implementación real del repositorio
                var resultado = await _proveedorRepo.Obtener_servicios_proveedor(proveedorId);
                
                if (resultado.Correcto)
                {
                    return new DTO<IEnumerable<TipoServicio>>
                    {
                        Correcto = true,
                        Datos = resultado.Datos,
                        Mensaje = resultado.Mensaje
                    };
                }
                else
                {
                    return new DTO<IEnumerable<TipoServicio>>
                    {
                        Correcto = true,
                        Datos = new List<TipoServicio>(),
                        Mensaje = "No se encontraron servicios para este proveedor."
                    };
                }
            }
            catch (Exception ex)
            {
                return new DTO<IEnumerable<TipoServicio>>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener servicios del proveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Actualizar_servicios_proveedor(int proveedorId, List<int> serviciosIds)
        {
            if (proveedorId <= 0)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "ID de proveedor inválido."
                };
            }

            if (serviciosIds == null)
            {
                serviciosIds = new List<int>();
            }

            try
            {
                // Usar implementación real del repositorio
                return await _proveedorRepo.Actualizar_servicios_proveedor(proveedorId, serviciosIds);
            }
            catch (Exception ex)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar servicios del proveedor: {ex.Message}"
                };
            }
        }

        private bool EsEmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GenerarPasswordTemporal()
        {
            // Generar una contraseña temporal de 8 caracteres
            var random = new Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}