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
            var usuarioResult = await _usuarioRepo.Obtener_por_email(login.Email);
            if (!usuarioResult.Correcto || usuarioResult.Datos == null)
            {
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = "Usuario no encontrado"
                };
            }

            // Verificar si la contraseña almacenada es bcrypt o SHA512
            bool passwordCorrecta = false;

            if (MiHash.EsHashBcrypt(usuarioResult.Datos.Password))
            {
                // Usar verificación bcrypt
                passwordCorrecta = MiHash.VerificarHashBcrypt(login.Password, usuarioResult.Datos.Password);
            }
            else
            {
                // Compatibilidad con SHA512 existente
                string passwordHasheado = MiHash.GenerarHash(login.Password);
                passwordCorrecta = usuarioResult.Datos.Password == passwordHasheado;
                
                // Si la contraseña es correcta pero está en SHA512, migrar a bcrypt
                if (passwordCorrecta)
                {
                    await MigrarPasswordABcrypt(usuarioResult.Datos, login.Password);
                }
            }

            if (passwordCorrecta)
            {
                return new DTO<Usuario>
                {
                    Correcto = true,
                    Datos = usuarioResult.Datos,
                    Mensaje = "Credenciales validadas correctamente."
                };
            }
            else
            {
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = "Contraseña incorrecta."
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
                usuario.Password = MiHash.GenerarHashBcrypt(passwordTextoPlano);
                await _usuarioRepo.Actualizar(usuario);
            }
            catch (Exception)
            {
                // Si falla la migración, no afectar el login
                // Se puede loggear el error si se desea
            }
        }
    }
}
