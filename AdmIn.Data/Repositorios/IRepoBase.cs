using AdmIn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Repositorios
{
    public interface IRepoBase<T>
    {
        Task<DTO<T>> Crear(T entidad);
        Task<DTO<T>> Actualizar(T entidad);
        Task<DTO<bool>> Eliminar(T entidad);
        Task<DTO<T?>> Obtener_por_id(T entidad);
        Task<DTO<IEnumerable<T>>> Obtener_todos();
        Task<DTO<Items_pagina<T>>> Obtener_paginado(Filtros_paginado filtros);
    }
}
