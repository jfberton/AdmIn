using AdmIn.Common;

namespace AdmIn.UI.Services
{
    public interface IServicioBase<T> where T : class
    {
        Task<DTO<T>> Crear(T entidad);
        Task<DTO<T>> Actualizar(T entidad);
        Task<DTO<bool>> Eliminar(int id);
        Task<DTO<T>> Obtener_por_id(int id);
        Task<DTO<Items_pagina<T>>> Obtener_paginado(Filtros_paginado filtros);

        void Diccionario_local_agregar(int id, T objeto);
        void Diccionario_local_eliminar(int id);
        T Diccionario_local_obtener(int id);
    }
}
