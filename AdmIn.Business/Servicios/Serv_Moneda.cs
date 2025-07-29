using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;

namespace AdmIn.Business.Servicios
{
    public class Serv_Moneda : IServ_Moneda
    {
        private readonly IMonedaRepository _monedaRepo;

        public Serv_Moneda(IMonedaRepository monedaRepository)
        {
            _monedaRepo = monedaRepository;
        }

        public async Task<DTO<Moneda>> Crear(Moneda moneda)
        {
            var resultado = await _monedaRepo.Crear(moneda);
            return resultado;
        }

        public async Task<DTO<Moneda>> Actualizar(Moneda moneda)
        {
            var resultado = await _monedaRepo.Actualizar(moneda);
            return resultado;
        }

        public async Task<DTO<bool>> Eliminar(Moneda moneda)
        {
            return await _monedaRepo.Eliminar(moneda);
        }

        public async Task<DTO<Moneda>> Obtener_por_id(Moneda moneda)
        {
            var resultado = await _monedaRepo.Obtener_por_id(moneda);
            return resultado;
        }

        public async Task<DTO<IEnumerable<Moneda>>> Obtener_todos()
        {
            var resultado = await _monedaRepo.Obtener_todos();
            return resultado;
        }

        public async Task<DTO<Items_pagina<Moneda>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _monedaRepo.Obtener_paginado(filtros);
            return resultado;
        }
    }
}