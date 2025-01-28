using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;

namespace AdmIn.UI.Services
{
    public interface IServ_Usuario : IServicioBase<Usuario>
    {
        Task<DTO<Usuario>> Obtener_usuario_por_email(string email);
        Task<DTO<bool>> Modificar_password(CambioClaveModel datos);
    }


}
