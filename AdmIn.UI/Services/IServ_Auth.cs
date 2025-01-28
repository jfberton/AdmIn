using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;

namespace AdmIn.UI.Services
{
    public interface IServ_Auth
    {
        Task<Usuario> Login(LoginModel model);
    }
}
