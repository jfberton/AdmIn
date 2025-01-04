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
        private readonly Rep_ROL _repRol = new Rep_ROL();
        private readonly Rep_PERMISO _repPermiso = new Rep_PERMISO();
        private readonly Rep_USUARIO_PERMISO _repUsuarioPermiso = new Rep_USUARIO_PERMISO();
        private readonly Rep_USUARIO_ROL _repUsuarioRol = new Rep_USUARIO_ROL();
        public Serv_Usuario()
        {
            _repRol = new Rep_ROL();
            _repPermiso = new Rep_PERMISO();
            _repUsuario = new Rep_USUARIO();
            _repUsuarioPermiso = new Rep_USUARIO_PERMISO();
            _repUsuarioRol = new Rep_USUARIO_ROL();
        }

        public async Task<DTO<Usuario>> ValidarCredenciales(LoginModel login)
        {
            var rta = _repUsuario.Obtener_por_Email(login.Email);

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

        public async Task<DTO<Items_pagina<Usuario>>> Obtener_usuarios(Filtros_paginado filtros)
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

            var usuariosRepo = _repUsuario.Obtener_lista_pagina_usuarios(filtros);
            if (usuariosRepo.Correcto && usuariosRepo.Datos != null)
            {
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

        public async Task<DTO<Usuario>> Obtener_usuario(int id)
        {
            var usuarioData = new DTO<Usuario>();

            var usuarioRepo = _repUsuario.Obtener_por_Id(id);
            if (usuarioRepo.Correcto && usuarioRepo.Datos != null)
            {
                usuarioData.Datos = usuarioRepo.Datos.ToBusinessUsuario();
            }

            return usuarioData;
        }

        public async Task<DTO<Usuario>> Obtener_usuario_mail(string mail)
        {
            var usuarioData = new DTO<Usuario>();

            var usuarioRepo = _repUsuario.Obtener_por_Email(mail);

            if (usuarioRepo.Correcto && usuarioRepo.Datos != null)
            {
                usuarioData.Datos = usuarioRepo.Datos.ToBusinessUsuario();
                usuarioData.Correcto = true;
                usuarioData.Mensaje = "Usuario obtenido correctamente";
            }

            return usuarioData;
        }

        public async Task<DTO<Usuario>> Crear_usuario(Usuario usuario)
        {
            //encripto la contraseña para guardarla en la base de datos
            usuario.Password = MiHash.GenerarHash(usuario.Password);

            var resultado = _repUsuario.Insertar(usuario.ToDataUSUARIO());

            if (resultado.Correcto && resultado.Datos != null)
            {
                usuario.Id = resultado.Datos.USU_ID;

                //Inserto correctamente el usuario ahora como es nuevo inserto los permisos y roles correspondientes
                foreach (var permiso in usuario.Permisos)
                {
                    _repUsuarioPermiso.Insertar(new USUARIO_PERMISO() { PERM_ID = permiso.Id, USU_ID = usuario.Id });
                }

                foreach (var rol in usuario.Roles)
                {
                    _repUsuarioRol.Insertar(new USUARIO_ROL() { ROL_ID = rol.Id, USU_ID = usuario.Id });
                }
            }

            DTO<Usuario> returns = new DTO<Usuario>();
            returns.Datos = resultado.Datos.ToBusinessUsuario();
            returns.Mensaje = resultado.Mensaje;
            returns.Correcto = resultado.Correcto;

            return returns;
        }

        public async Task<DTO<Usuario>> Actualizar_usuario(Usuario usuario)
        {
            var usuarioActualizado = new DTO<Usuario>();

            var usuarioRepo = _repUsuario.Actualizar(usuario.ToDataUSUARIO());
            if (usuarioRepo.Correcto && usuarioRepo.Datos != null)
            {
                //Primero elimino los permisos y roles asociados al usuario y luego agrego los que trae
                var permisos = _repUsuarioPermiso.ObtenerPorUsuario(usuario.Id).Datos;
                foreach (var permiso in permisos)
                {
                    _repUsuarioPermiso.Eliminar(permiso.USU_ID, permiso.PERM_ID);
                }

                var roles = _repUsuarioRol.ObtenerPorUsuario(usuario.Id).Datos;
                foreach (var rol in roles)
                {
                    _repUsuarioRol.Eliminar(rol.USU_ID, rol.ROL_ID);
                }

                //Inserto correctamente el usuario ahora como es nuevo inserto los permisos y roles correspondientes
                foreach (var permiso in usuario.Permisos)
                {
                    _repUsuarioPermiso.Insertar(new USUARIO_PERMISO() { PERM_ID = permiso.Id, USU_ID = usuario.Id });
                }

                foreach (var rol in usuario.Roles)
                {
                    _repUsuarioRol.Insertar(new USUARIO_ROL() { ROL_ID = rol.Id, USU_ID = usuario.Id });
                }
            }

            DTO<Usuario> returns = new DTO<Usuario>();
            returns.Datos = usuarioRepo.Datos.ToBusinessUsuario();
            returns.Mensaje = usuarioRepo.Mensaje;
            returns.Correcto = usuarioRepo.Correcto;

            return returns;
        }

        public async Task<DTO<bool>> Eliminar_usuario(int id)
        {
            var resultado = _repUsuario.Eliminar(id);
            return resultado;
        }

        public async Task<DTO<bool>> ModificarContraseña(CambioClaveModel datos)
        {
            var rta = _repUsuario.Obtener_por_Email(datos.Email);

            string password_hasheado = MiHash.GenerarHash(datos.Password);

            if (rta.Correcto && rta.Datos != null)
            {
                if (rta.Datos.USU_PASSWORD == password_hasheado)
                {
                    rta.Datos.USU_PASSWORD = MiHash.GenerarHash(datos.NuevaPassword);

                    rta = _repUsuario.Actualizar(rta.Datos);

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
