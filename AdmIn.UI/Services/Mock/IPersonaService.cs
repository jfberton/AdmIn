using AdmIn.Business.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public interface IPersonaService
    {
        Task<List<PersonaBase>> BuscarPersonasAsync(string apellido, string rfc);
        Task<PersonaBase> CrearPersonaAsync(PersonaBase persona);
        Task<PersonaBase> ActualizarPersonaAsync(PersonaBase persona);
        Task EliminarPersonaAsync(int personaId);
        Task<PersonaBase?> ObtenerPersonaPorIdAsync(int personaId);
    }
}
