using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using AdmIn.Data.Repositorios;
using AdmIn.Data.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_Usuario : IServ_Usuario
    {
        private readonly Rep_USUARIO _repUsuario;
        private readonly Rep_USUARIO_ROL _repUsuarioRol = new Rep_USUARIO_ROL();

        public Serv_Usuario()
        {
            _repUsuario = new Rep_USUARIO();
            _repUsuarioRol = new Rep_USUARIO_ROL();
        }

        public async Task<DTO<Usuario>> Crear(Usuario usuario)
        {
            //encripto la contraseña para guardarla en la base de datos
            usuario.Password = MiHash.GenerarHash(usuario.Password);
            usuario.Creacion = DateTime.Now;

            var resultado = await _repUsuario.Crear(usuario.ToDataUSUARIO());

            if (resultado.Correcto && resultado.Datos != null)
            {
                usuario.Id = resultado.Datos.USU_ID;

                foreach (var rol in usuario.Roles)
                {
                    await _repUsuarioRol.Crear(new USUARIO_ROL() { ROL_ID = rol.Id, USU_ID = usuario.Id });
                }
            }

            DTO<Usuario> returns = new DTO<Usuario>();
            returns.Datos = resultado.Datos.ToBusinessUsuario();
            returns.Mensaje = resultado.Mensaje;
            returns.Correcto = resultado.Correcto;

            return returns;
        }

        public async Task<DTO<Usuario>> Actualizar(Usuario usuario)
        {
            var usuarioActualizado = new DTO<Usuario>();

            var usuarioRepo = await _repUsuario.Actualizar(usuario.ToDataUSUARIO());
            if (usuarioRepo.Correcto && usuarioRepo.Datos != null)
            {
                var respuesta = _repUsuarioRol.Obtener_por_usuario(usuario.Id).Result;
                if (respuesta != null)
                {
                    var roles = respuesta.Datos;
                    foreach (var rol in roles)
                    {
                        await _repUsuarioRol.Eliminar(rol);
                    }
                }

                foreach (var rol in usuario.Roles)
                {
                    await _repUsuarioRol.Crear(new USUARIO_ROL() { ROL_ID = rol.Id, USU_ID = usuario.Id });
                }
            }

            DTO<Usuario> returns = new DTO<Usuario>();
            returns.Datos = usuarioRepo.Datos.ToBusinessUsuario();
            returns.Mensaje = usuarioRepo.Mensaje;
            returns.Correcto = usuarioRepo.Correcto;

            return returns;
        }

        public async Task<DTO<bool>> Eliminar(Usuario usuario)
        {
            var resultado = await _repUsuario.Eliminar(usuario.ToDataUSUARIO());
            return resultado;
        }

        public async Task<DTO<Items_pagina<Usuario>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = new DTO<Items_pagina<Usuario>>
            {
                Datos = new Items_pagina<Usuario>
                {
                    Items = new List<Usuario>(),
                    Total_items = 0
                }
            };

            filtros.EntityName = "Usuario";

            var usuariosRepo = await _repUsuario.Obtener_paginado(filtros);
            if (usuariosRepo.Correcto && usuariosRepo.Datos != null)
            {
                resultado.Correcto = true;
                resultado.Mensaje = "Usuarios obtenidos exitosamente";
                resultado.Datos.Items = usuariosRepo.Datos.Items.Select(u=>u.ToBusinessUsuario()).ToList();
                resultado.Datos.Total_items = usuariosRepo.Datos.Total_items;
            }
            else
            {
                resultado.Correcto = false;
                resultado.Mensaje = "Error al obtener los usuarios.";
            }

            return resultado;
        }

        public async Task<DTO<IEnumerable<Usuario>>> Obtener_todos()
        {
            var resultado = new DTO<IEnumerable<Usuario>>();

            var usuariosRepo = await _repUsuario.Obtener_todos();
            if (usuariosRepo.Correcto && usuariosRepo.Datos != null)
            {
                resultado.Correcto = true;
                resultado.Mensaje = "Usuarios obtenidos exitosamente";
                resultado.Datos = usuariosRepo.Datos.Select(u => u.ToBusinessUsuario()).ToList();
            }
            else
            {
                resultado.Correcto = false;
                resultado.Mensaje = usuariosRepo.Mensaje ?? "Error al obtener los usuarios.";
                resultado.Datos = null;
            }

            return resultado;
        }

        public async Task<DTO<Usuario>> Obtener_por_id(Usuario usuario)
        {
            var usuarioData = new DTO<Usuario>();

            var usuarioRepo = await _repUsuario.Obtener_por_id(usuario.ToDataUSUARIO());
            if (usuarioRepo.Correcto && usuarioRepo.Datos != null)
            {
                usuarioData.Datos = usuarioRepo.Datos.ToBusinessUsuario();
                usuarioData.Correcto = true;
            }

            return usuarioData;
        }

        public async Task<DTO<Usuario>> Validar_credenciales(LoginModel login)
        {
            var rta = await _repUsuario.Obtener_por_email(login.Email);

            string password_hasheado = MiHash.GenerarHash(login.Password);

            if (rta.Correcto && rta.Datos != null)
            {
                if (rta.Datos.USU_PASSWORD == password_hasheado)
                {
                    var usr = rta.Datos.ToBusinessUsuario();


                    return new DTO<Usuario>
                    {
                        Correcto = true,
                        Mensaje = "Usuario validado correctamente",
                        Datos = usr
                    };
                }
                else
                {
                    return new DTO<Usuario>
                    {
                        Correcto = false,
                        Mensaje = "Contraseña incorrecta"
                    };
                }
            }
            else
            {
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = "Usuario no encontrado"
                };
            }
        }

        public async Task<DTO<Usuario>> Obtener_por_mail(string mail)
        {
            var usuarioData = new DTO<Usuario>();

            var usuarioRepo = await _repUsuario.Obtener_por_email(mail);

            if (usuarioRepo.Correcto && usuarioRepo.Datos != null)
            {
                usuarioData.Datos = usuarioRepo.Datos.ToBusinessUsuario();
                usuarioData.Correcto = true;
                usuarioData.Mensaje = "Usuario obtenido correctamente";
            }

            return usuarioData;
        }

        public async Task<DTO<bool>> Modificar_contraseña(CambioClaveModel datos)
        {
            var rta = await _repUsuario.Obtener_por_email(datos.Email);

            string password_hasheado = MiHash.GenerarHash(datos.Password);

            if (rta.Correcto && rta.Datos != null)
            {
                if (rta.Datos.USU_PASSWORD == password_hasheado)
                {
                    rta.Datos.USU_PASSWORD = MiHash.GenerarHash(datos.NuevaPassword);

                    rta = await _repUsuario.Actualizar(rta.Datos);

                    if (rta.Correcto)
                    {
                        return new DTO<bool>
                        {
                            Correcto = true,
                            Datos = true,
                            Mensaje = "La contraseña se actualizó correctamente."
                        };
                    }
                    else
                    {
                        return new DTO<bool>
                        {
                            Correcto = false,
                            Datos = false,
                            Mensaje = rta.Mensaje //error del repositorio
                        };
                    }
                }
                else
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "La contraseña ingresada es incorrecta."
                    };
                }
            }
            else
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "Usuario no encontrado"
                };
            }
        }
    }
}
