using AdmIn.Common.Entidades;
using AdmIn.Common.Utilidades;
using AdmIn.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public interface IServ_Usuario : IServicioBase<Usuario>
    {
        Task<DTO<Usuario>> Obtener_usuario_por_email(string email);
        Task<DTO<bool>> Modificar_password(CambioClaveModel datos);
        Task<DTO<IEnumerable<Usuario>>> Buscar_por_termino(string termino);
    }


}
