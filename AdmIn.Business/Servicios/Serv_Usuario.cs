using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using AdmIn.Data.Repositorios;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_Usuario : IServ_Usuario
    {
        private readonly R_Usuario _repUsuario;
        private readonly R_Usuario_Rol _repUsuarioRol;

        public Serv_Usuario()
        {
            _repUsuario = new R_Usuario();
            _repUsuarioRol = new R_Usuario_Rol();
        }

        public async Task<DTO<Usuario>> Crear(Usuario usuario)
        {
            usuario.Password = MiHash.GenerarHash(usuario.Password);
            usuario.Creacion = DateTime.Now;

            var resultado = await _repUsuario.Crear(usuario);

            if (resultado.Correcto && resultado.Datos != null)
            {
                usuario.Id = resultado.Datos.Id;

                foreach (var rol in usuario.Roles)
                {
                    await _repUsuarioRol.Crear(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.Id });
                }
            }

            return new DTO<Usuario>
            {
                Datos = resultado.Datos,
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje
            };
        }

        public async Task<DTO<Usuario>> Actualizar(Usuario usuario)
        {
            var resultado = await _repUsuario.Actualizar(usuario);

            if (resultado.Correcto && resultado.Datos != null)
            {
                var rolesActuales = await _repUsuarioRol.Obtener_todos();
                foreach (var rol in rolesActuales.Datos.Where(r => r.UsuarioId == usuario.Id))
                {
                    await _repUsuarioRol.Eliminar(rol);
                }

                foreach (var rol in usuario.Roles)
                {
                    await _repUsuarioRol.Crear(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.Id });
                }
            }

            return new DTO<Usuario>
            {
                Datos = resultado.Datos,
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje
            };
        }

        public async Task<DTO<bool>> Eliminar(Usuario usuario)
        {
            return await _repUsuario.Eliminar(usuario);
        }

        public async Task<DTO<Usuario>> Obtener_por_id(int id)
        {
            var resultado = await _repUsuario.Obtener_por_id(new Usuario { Id = id });

            if (resultado.Correcto && resultado.Datos != null)
            {
                var roles = await _repUsuarioRol.Obtener_todos();
                resultado.Datos.Roles = roles.Datos.Where(r => r.UsuarioId == id).Select(r => new Rol { Id = r.RolId }).ToList();
            }

            return resultado;
        }

        public async Task<DTO<IEnumerable<Usuario>>> Obtener_todos()
        {
            return await _repUsuario.Obtener_todos();
        }
    }
}
