using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
    public interface IInmuebleRepository
    {
        Task<DTO<Inmueble>> Crear(Inmueble inmueble);
        Task<DTO<Inmueble>> Actualizar(Inmueble inmueble);
        Task<DTO<bool>> Eliminar(Inmueble inmueble);
        Task<DTO<Inmueble>> Obtener_por_id(Inmueble inmueble);
        Task<DTO<IEnumerable<Inmueble>>> Obtener_todos();
        Task<DTO<Items_pagina<Inmueble>>> Obtener_paginado(Filtros_paginado filtros);
        
        // Métodos específicos para Inmueble
        Task<DTO<IEnumerable<string>>> Obtener_estados();
        Task<DTO<IEnumerable<Inmueble>>> Obtener_por_estado(string estado);
        Task<DTO<IEnumerable<Inmueble>>> Obtener_por_administrador(int administradorId);
        Task<DTO<IEnumerable<Inmueble>>> Obtener_por_ubicacion(string pais, string estado, string ciudad);
        Task<DTO<IEnumerable<Inmueble>>> Obtener_por_rango_precio(decimal precioMin, decimal precioMax);
        
        // Métodos específicos para características
        Task<DTO<IEnumerable<CaracteristicaInmueble>>> Obtener_caracteristicas_inmueble(int inmuebleId);
        Task<DTO<CaracteristicaInmueble>> Agregar_caracteristica_inmueble(CaracteristicaInmueble caracteristica);
        Task<DTO<CaracteristicaInmueble>> Actualizar_caracteristica_inmueble(CaracteristicaInmueble caracteristica);
        Task<DTO<bool>> Eliminar_caracteristica_inmueble(int caracteristicaInmuebleId);
    }
}