using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
    public interface IContratoRepository
    {
        Task<DTO<ContratoRenta>> Crear(ContratoRenta contrato);
        Task<DTO<ContratoRenta>> Obtener_por_id(int id);
    }
}
