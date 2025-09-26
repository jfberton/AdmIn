using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_CalificacionProveedor : IServ_CalificacionProveedor
    {
        private readonly ICalificacionProveedorRepository _repo;
        public Serv_CalificacionProveedor(ICalificacionProveedorRepository repo)
        {
            _repo = repo;
        }
        public Task<DTO<CalificacionProveedor>> Crear(CalificacionProveedor calificacionProveedor) => _repo.Crear(calificacionProveedor);
        public Task<DTO<CalificacionProveedor>> Actualizar(CalificacionProveedor calificacionProveedor) => _repo.Actualizar(calificacionProveedor);
        public Task<DTO<bool>> Eliminar(CalificacionProveedor calificacionProveedor) => _repo.Eliminar(calificacionProveedor);
        public Task<DTO<CalificacionProveedor>> Obtener_por_id(int calificacionProveedorId) => _repo.Obtener_por_id(calificacionProveedorId);
        public Task<DTO<IEnumerable<CalificacionProveedor>>> Obtener_todos() => _repo.Obtener_todos();
        public Task<DTO<Items_pagina<CalificacionProveedor>>> Obtener_paginado(Filtros_paginado filtros) => _repo.Obtener_paginado(filtros);
    }
}
