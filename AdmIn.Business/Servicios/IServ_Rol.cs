using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public interface IServ_Rol : IServicioBase<Rol>
    {
        Task<DTO<Usuario>> Obtener_por_usuario(string mail);
    }
}
