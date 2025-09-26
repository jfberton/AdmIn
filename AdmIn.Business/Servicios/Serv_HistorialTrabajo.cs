using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_HistorialTrabajo : IServ_HistorialTrabajo
    {
        private readonly IHistorialTrabajoRepository _repo;
        public Serv_HistorialTrabajo(IHistorialTrabajoRepository repo)
        {
            _repo = repo;
        }
        public Task<DTO<HistorialTrabajo>> Crear(HistorialTrabajo historialTrabajo) => _repo.Crear(historialTrabajo);
        public Task<DTO<HistorialTrabajo>> Actualizar(HistorialTrabajo historialTrabajo) => _repo.Actualizar(historialTrabajo);
        public Task<DTO<bool>> Eliminar(HistorialTrabajo historialTrabajo) => _repo.Eliminar(historialTrabajo);
        public Task<DTO<HistorialTrabajo>> Obtener_por_id(int historialTrabajoId) => _repo.Obtener_por_id(historialTrabajoId);
        public Task<DTO<IEnumerable<HistorialTrabajo>>> Obtener_todos() => _repo.Obtener_todos();
        public Task<DTO<Items_pagina<HistorialTrabajo>>> Obtener_paginado(Filtros_paginado filtros) => _repo.Obtener_paginado(filtros);
    }
}
