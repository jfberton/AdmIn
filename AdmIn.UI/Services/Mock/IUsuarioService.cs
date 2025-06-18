using AdmIn.Business.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public interface IUsuarioService // Renamed from IMockUsuarioService
    {
        Task<Usuario?> ObtenerUsuarioPorId(int id);
        Task<List<Usuario>> ObtenerUsuarios();
        // Add other methods as needed, for example:
        // Task<Usuario> CrearUsuarioAsync(Usuario usuario);
        // Task<Usuario> ActualizarUsuarioAsync(Usuario usuario);
        // Task EliminarUsuarioAsync(int id);
    }
}
