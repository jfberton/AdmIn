using AdmIn.Business.Entidades;
using AdmIn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public interface IServ_Permiso : IServicioBase<Permiso>
    {
        Task<DTO<IEnumerable<Permiso>>> Obtener_por_rol(int rolId);
    }

}
