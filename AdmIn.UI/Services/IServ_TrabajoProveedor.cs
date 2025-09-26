using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public interface IServ_TrabajoProveedor : IServicioBase<TrabajoProveedor>
    {
        Task<DTO<Items_pagina<TrabajoProveedor>>> Obtener_paginado_filtrado(Filtros_paginado filtros);
    }
}
