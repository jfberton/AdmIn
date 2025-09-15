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
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El nombre del proveedor es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(proveedor.RFC))
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El RFC del proveedor es requerido."
                };
            }

            // El email es obligatorio para crear el usuario asociado
            if (string.IsNullOrWhiteSpace(proveedor.Email))
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El email del proveedor es requerido para crear el usuario asociado."
                };
            }

            // Validar RFC único
            var rfcExiste = await Validar_rfc_unico(proveedor.RFC);
            if (!rfcExiste.Correcto)
            {
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
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "Ya existe un proveedor con ese email."
                };
            }

            // Validar formato de email básico
            if (!EsEmailValido(proveedor.Email))
            {
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
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "Ya existe un usuario con ese email en el sistema."
                };
            }

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
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear el usuario asociado: {resultadoUsuario.Mensaje}"
                };
            }

            // Establecer valores por defecto y usuario asociado
            proveedor.UsuarioId = resultadoUsuario.Datos.Id;
            proveedor.FechaCreacion = DateTime.Now;
            proveedor.FechaModificacion = DateTime.Now;
            proveedor.Activo = true;

            var resultado = await _proveedorRepo.Crear(proveedor);
            
            if (!resultado.Correcto)
            {
                // Si falla la creación del proveedor, eliminar el usuario creado
                await _usuarioRepo.Eliminar(resultadoUsuario.Datos);
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear proveedor: {resultado.Mensaje}. El usuario asociado fue eliminado."
                };
            }

            // El repositorio ya asigna el rol automáticamente
            return resultado;
        }

        public async Task<DTO<Proveedor>> Actualizar(Proveedor proveedor)
        {
            // Validaciones de negocio
            if (proveedor.Id <= 0)
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "ID de proveedor inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El nombre del proveedor es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(proveedor.RFC))
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "El RFC del proveedor es requerido."
                };
            }

            // Validar RFC único (excluyendo el proveedor actual)
            var rfcExiste = await Validar_rfc_unico(proveedor.RFC, proveedor.Id);
            if (!rfcExiste.Correcto)
            {
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
                    return new DTO<Proveedor>
                    {
                        Correcto = false,
                        Mensaje = "Ya existe otro proveedor con ese email."
                    };
                }

                // Validar formato de email básico
                if (!EsEmailValido(proveedor.Email))
                {
                    return new DTO<Proveedor>
                    {
                    Correcto = false,
                    Mensaje = "El formato del email no es válido."
                    };
                }
            }

            // Actualizar fecha de modificación
            proveedor.FechaModificacion = DateTime.Now;

            var resultado = await _proveedorRepo.Actualizar(proveedor);
            
            // Si la actualización es exitosa y hay un usuario asociado, actualizar el usuario también
            if (resultado.Correcto && proveedor.UsuarioId.HasValue)
            {
                var usuarioExistente = await _usuarioRepo.Obtener_por_id(new Usuario { Id = proveedor.UsuarioId.Value });
                if (usuarioExistente.Correcto && usuarioExistente.Datos != null)
                {
                    usuarioExistente.Datos.Nombre = proveedor.Nombre;
                    usuarioExistente.Datos.Email = proveedor.Email ?? usuarioExistente.Datos.Email;
                    usuarioExistente.Datos.Telefono = proveedor.Telefono ?? usuarioExistente.Datos.Telefono;
                    usuarioExistente.Datos.FechaModificacion = DateTime.Now;
                    usuarioExistente.Datos.UsuarioModificador = proveedor.UsuarioModificadorId;

                    await _usuarioRepo.Actualizar(usuarioExistente.Datos);
                }
            }

            return resultado;
        }

        public async Task<DTO<bool>> Eliminar(Proveedor proveedor)
        {
            if (proveedor.Id <= 0)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "ID de proveedor inválido."
                };
            }

            return await _proveedorRepo.Eliminar(proveedor);
        }

        public async Task<DTO<Proveedor>> Obtener_por_id(Proveedor proveedor)
        {
            if (proveedor.Id <= 0)
            {
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = "ID de proveedor inválido."
                };
            }

            var resultado = await _proveedorRepo.Obtener_por_id(proveedor);
            return resultado;
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_todos()
        {
            var resultado = await _proveedorRepo.Obtener_todos();
            return resultado;
        }

        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _proveedorRepo.Obtener_paginado(filtros);
            return resultado;
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_activos()
        {
            return await _proveedorRepo.Obtener_activos();
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