using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;

namespace AdmIn.Business.Servicios
{
    public class Serv_Caracteristica : IServ_Caracteristica
    {
        private readonly ICaracteristicaRepository _caracteristicaRepo;

        public Serv_Caracteristica(ICaracteristicaRepository caracteristicaRepository)
        {
            _caracteristicaRepo = caracteristicaRepository;
        }

        public async Task<DTO<Caracteristica>> Crear(Caracteristica caracteristica)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(caracteristica.Nombre))
            {
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = "El nombre de la característica es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(caracteristica.Tipo))
            {
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = "El tipo de la característica es requerido."
                };
            }

            var resultado = await _caracteristicaRepo.Crear(caracteristica);
            return resultado;
        }

        public async Task<DTO<Caracteristica>> Actualizar(Caracteristica caracteristica)
        {
            // Validaciones de negocio
            if (caracteristica.Id <= 0)
            {
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = "ID de característica inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(caracteristica.Nombre))
            {
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = "El nombre de la característica es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(caracteristica.Tipo))
            {
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = "El tipo de la característica es requerido."
                };
            }

            var resultado = await _caracteristicaRepo.Actualizar(caracteristica);
            return resultado;
        }

        public async Task<DTO<bool>> Eliminar(Caracteristica caracteristica)
        {
            if (caracteristica.Id <= 0)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "ID de característica inválido."
                };
            }

            return await _caracteristicaRepo.Eliminar(caracteristica);
        }

        public async Task<DTO<Caracteristica>> Obtener_por_id(Caracteristica caracteristica)
        {
            if (caracteristica.Id <= 0)
            {
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = "ID de característica inválido."
                };
            }

            var resultado = await _caracteristicaRepo.Obtener_por_id(caracteristica);
            return resultado;
        }

        public async Task<DTO<IEnumerable<Caracteristica>>> Obtener_todos()
        {
            var resultado = await _caracteristicaRepo.Obtener_todos();
            return resultado;
        }

        public async Task<DTO<Items_pagina<Caracteristica>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _caracteristicaRepo.Obtener_paginado(filtros);
            return resultado;
        }

        public async Task<DTO<IEnumerable<Caracteristica>>> Obtener_por_tipo(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
            {
                return new DTO<IEnumerable<Caracteristica>>
                {
                    Correcto = false,
                    Mensaje = "El tipo es requerido."
                };
            }

            return await _caracteristicaRepo.Obtener_por_tipo(tipo);
        }

        public async Task<DTO<IEnumerable<string>>> Obtener_tipos()
        {
            return await _caracteristicaRepo.Obtener_tipos();
        }
    }
}