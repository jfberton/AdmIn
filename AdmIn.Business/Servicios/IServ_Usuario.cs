using AdmIn.Business.Utilidades;
using AdmIn.Common.Entidades;
using AdmIn.Common.Utilidades;
using AdmIn.Common;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AdmIn.Business.Servicios
{
    public interface IServ_Usuario : IServicioBase<Usuario>
    {
        Task<DTO<Usuario>> Validar_credenciales(LoginModel login);
        Task<DTO<Usuario>> Obtener_por_mail(string mail);
        Task<DTO<bool>> Modificar_contraseña(CambioClaveModel datos);
        Task<DTO<IEnumerable<Usuario>>> Buscar_por_termino(string termino);

        // Token generation and reset
        Task<DTO<string>> GenerarTokenYGuardar(int? usuarioId, int? personaId, int expiryHours =24, string purpose = "SetPassword");
        Task<DTO<bool>> ResetPasswordByToken(string token, string nuevaPassword);
        Task<DTO<bool>> GenerateAndSendPasswordResetEmail(int usuarioId);
        Task<DTO<object>> Obtener_info_token(string token);
    }
}
