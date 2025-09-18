using AdmIn.Common.Entidades;
using AdmIn.Common;

namespace AdmIn.UI.Services
{
    public interface IServ_Caracteristica : IServicioBase<Caracteristica>
    {
        Task<DTO<IEnumerable<Caracteristica>>> Obtener_por_tipo(string tipo);
        Task<DTO<IEnumerable<string>>> Obtener_tipos();
    }
}