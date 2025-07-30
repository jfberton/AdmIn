using AdmIn.Common;
using AdmIn.Common.Entidades;

namespace AdmIn.Business.Servicios
{
    public interface IServ_Caracteristica : IServicioBase<Caracteristica>
    {
        Task<DTO<IEnumerable<Caracteristica>>> Obtener_por_tipo(string tipo);
        Task<DTO<IEnumerable<string>>> Obtener_tipos();
    }
}