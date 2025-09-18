using AdmIn.Common.Entidades;
using AdmIn.Common.Utilidades;

namespace AdmIn.UI.Services
{
    public interface IServ_Auth
    {
        Task<Usuario> Login(LoginModel model);
    }
}
