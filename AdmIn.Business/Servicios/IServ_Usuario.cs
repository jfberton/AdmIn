using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;


namespace AdmIn.Business.Servicios
{
    public interface IServ_Usuario:IServicioBase<Usuario>
    {
        Task<DTO<Usuario>> Validar_credenciales(LoginModel login);
        Task<DTO<Usuario>> Obtener_por_mail(string mail);
        Task<DTO<bool>> Modificar_contraseña(CambioClaveModel datos);
    }
}
