using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;


namespace AdmIn.Business.Servicios
{
    public class Serv_Rol : IServ_Rol
    {
        private readonly IRolRepository _rolRepo;

        public Serv_Rol(IRolRepository rolRepository)
        {
            _rolRepo = rolRepository;
        }

        public async Task<DTO<Rol>> Crear(Rol rol)
        {
            var resultado = await _rolRepo.Crear(rol);
            return resultado;
        }

        public async Task<DTO<Rol>> Actualizar(Rol rol)
        {
            var resultado = await _rolRepo.Actualizar(rol);
            return resultado;
        }

        public async Task<DTO<bool>> Eliminar(Rol rol)
        {
            return await _rolRepo.Eliminar(rol);
        }

        public async Task<DTO<Rol>> Obtener_por_id(Rol rol)
        {
            var resultado = await _rolRepo.Obtener_por_id(rol);
            return resultado;
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_todos()
        {
            var resultado = await _rolRepo.Obtener_todos();
            return resultado;
        }

        public async Task<DTO<Items_pagina<Rol>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _rolRepo.Obtener_paginado(filtros);
            return resultado;
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId)
        {
            return await _rolRepo.Obtener_por_usuario(usuarioId);
        }
    }
}
