using AdmIn.Business.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public interface IUsuarioService
    {
        Task<Usuario?> ObtenerUsuarioPorId(int id);
        Task<List<Usuario>> ObtenerUsuarios();
       
    }
}
