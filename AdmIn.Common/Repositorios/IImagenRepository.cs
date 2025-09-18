using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
    public interface IImagenRepository
    {
        Task<DTO<Imagen>> Crear(Imagen imagen);
        Task<DTO<Imagen>> Actualizar(Imagen imagen);
        Task<DTO<bool>> Eliminar(Imagen imagen);
        Task<DTO<Imagen>> Obtener_por_id(Imagen imagen);
        Task<DTO<IEnumerable<Imagen>>> Obtener_todos();
        Task<DTO<Items_pagina<Imagen>>> Obtener_paginado(Filtros_paginado filtros);
        
        // Métodos específicos para Inmueble
        Task<DTO<IEnumerable<Imagen>>> Obtener_por_inmueble(int inmuebleId);
        Task<DTO<bool>> Asociar_a_inmueble(Guid imagenId, int inmuebleId);
        Task<DTO<bool>> Desasociar_de_inmueble(Guid imagenId, int inmuebleId);
        Task<DTO<bool>> Establecer_como_principal(Guid imagenId, int inmuebleId);
        
        // Métodos específicos para Usuario
        Task<DTO<bool>> Establecer_imagen_perfil_usuario(Guid imagenId, int usuarioId);
    }
}