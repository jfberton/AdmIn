using AdmIn.Common.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Repositorios
{
    public interface IUsuarioRepository : IRepoBase<Usuario>
    {
        Task<DTO<Usuario>> Obtener_por_email(string email);
    }
}
