using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_HistorialDetalleTrabajo : IServ_HistorialDetalleTrabajo
    {
        private readonly IHistorialDetalleTrabajoRepository _repo;
        public Serv_HistorialDetalleTrabajo(IHistorialDetalleTrabajoRepository repo)
        {
            _repo = repo;
        }
        public Task<DTO<HistorialDetalleTrabajo>> Crear(HistorialDetalleTrabajo historialDetalleTrabajo) => _repo.Crear(historialDetalleTrabajo);
        public Task<DTO<HistorialDetalleTrabajo>> Actualizar(HistorialDetalleTrabajo historialDetalleTrabajo) => _repo.Actualizar(historialDetalleTrabajo);
        public Task<DTO<bool>> Eliminar(HistorialDetalleTrabajo historialDetalleTrabajo) => _repo.Eliminar(historialDetalleTrabajo);
        public Task<DTO<HistorialDetalleTrabajo>> Obtener_por_id(int historialDetalleTrabajoId) => _repo.Obtener_por_id(historialDetalleTrabajoId);
        public Task<DTO<IEnumerable<HistorialDetalleTrabajo>>> Obtener_todos() => _repo.Obtener_todos();
        public Task<DTO<Items_pagina<HistorialDetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros) => _repo.Obtener_paginado(filtros);
    }
}
