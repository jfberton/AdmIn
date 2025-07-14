using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using AdmIn.Data.Repositorios;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_Rol : IServ_Rol
    {
        private readonly R_Rol _repRol;
        private readonly R_Usuario_Rol _repUsuarioRol;

        public Serv_Rol()
        {
            _repRol = new R_Rol();
            _repUsuarioRol = new R_Usuario_Rol();
        }

        public async Task<DTO<Rol>> Crear(Rol rol)
        {
            var resultado = await _repRol.Crear(rol);

            return new DTO<Rol>
            {
                Datos = resultado.Datos,
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje
            };
        }

        public async Task<DTO<Rol>> Actualizar(Rol rol)
        {
            var resultado = await _repRol.Actualizar(rol);

            return new DTO<Rol>
            {
                Datos = resultado.Datos,
                Correcto = resultado.Correcto,
                Mensaje = resultado.Mensaje
            };
        }

        public async Task<DTO<bool>> Eliminar(Rol rol)
        {
            return await _repRol.Eliminar(rol);
        }

        public async Task<DTO<Rol>> Obtener_por_id(int id)
        {
            return await _repRol.Obtener_por_id(new Rol { Id = id });
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_todos()
        {
            return await _repRol.Obtener_todos();
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId)
        {
            var rolesUsuario = await _repUsuarioRol.Obtener_todos();
            var roles = rolesUsuario.Datos.Where(r => r.UsuarioId == usuarioId).Select(r => new Rol { Id = r.RolId }).ToList();

            return new DTO<IEnumerable<Rol>>
            {
                Datos = roles,
                Correcto = true,
                Mensaje = "Roles obtenidos correctamente."
            };
        }
    }
}
