using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_DetalleTrabajo : IServ_DetalleTrabajo
    {
        private readonly IDetalleTrabajoRepository _repo;
        public Serv_DetalleTrabajo(IDetalleTrabajoRepository repo)
        {
            _repo = repo;
        }
        public Task<DTO<DetalleTrabajo>> Crear(DetalleTrabajo detalleTrabajo) => _repo.Crear(detalleTrabajo);
        public Task<DTO<DetalleTrabajo>> Actualizar(DetalleTrabajo detalleTrabajo) => _repo.Actualizar(detalleTrabajo);
        public Task<DTO<bool>> Eliminar(DetalleTrabajo detalleTrabajo) => _repo.Eliminar(detalleTrabajo);
        public Task<DTO<DetalleTrabajo>> Obtener_por_id(int detalleTrabajoId) => _repo.Obtener_por_id(detalleTrabajoId);
        public Task<DTO<IEnumerable<DetalleTrabajo>>> Obtener_todos() => _repo.Obtener_todos();
        public Task<DTO<IEnumerable<DetalleTrabajo>>> Obtener_por_trabajo(int trabajoId) => _repo.Obtener_por_trabajo(trabajoId);
        public Task<DTO<Items_pagina<DetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros) => _repo.Obtener_paginado(filtros);
    }
}
