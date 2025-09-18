using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;

namespace AdmIn.Business.Servicios
{
    public class Serv_TipoServicio : IServ_TipoServicio
    {
        private readonly ITipoServicioRepository _tipoServicioRepo;

        public Serv_TipoServicio(ITipoServicioRepository tipoServicioRepository)
        {
            _tipoServicioRepo = tipoServicioRepository;
        }

        public async Task<DTO<TipoServicio>> Crear(TipoServicio tipoServicio)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(tipoServicio.Nombre))
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = "El nombre del tipo de servicio es requerido."
                };
            }

            if (tipoServicio.Nombre.Length > 100)
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = "El nombre del tipo de servicio no puede exceder 100 caracteres."
                };
            }

            if (!string.IsNullOrEmpty(tipoServicio.Descripcion) && tipoServicio.Descripcion.Length > 500)
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = "La descripción no puede exceder 500 caracteres."
                };
            }

            var resultado = await _tipoServicioRepo.Crear(tipoServicio);
            return resultado;
        }

        public async Task<DTO<TipoServicio>> Actualizar(TipoServicio tipoServicio)
        {
            // Validaciones de negocio
            if (tipoServicio.Id <= 0)
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = "ID de tipo de servicio inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(tipoServicio.Nombre))
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = "El nombre del tipo de servicio es requerido."
                };
            }

            if (tipoServicio.Nombre.Length > 100)
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = "El nombre del tipo de servicio no puede exceder 100 caracteres."
                };
            }

            if (!string.IsNullOrEmpty(tipoServicio.Descripcion) && tipoServicio.Descripcion.Length > 500)
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = "La descripción no puede exceder 500 caracteres."
                };
            }

            var resultado = await _tipoServicioRepo.Actualizar(tipoServicio);
            return resultado;
        }

        public async Task<DTO<bool>> Eliminar(TipoServicio tipoServicio)
        {
            if (tipoServicio.Id <= 0)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "ID de tipo de servicio inválido."
                };
            }

            return await _tipoServicioRepo.Eliminar(tipoServicio);
        }

        public async Task<DTO<TipoServicio>> Obtener_por_id(TipoServicio tipoServicio)
        {
            if (tipoServicio.Id <= 0)
            {
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = "ID de tipo de servicio inválido."
                };
            }

            var resultado = await _tipoServicioRepo.Obtener_por_id(tipoServicio);
            return resultado;
        }

        public async Task<DTO<IEnumerable<TipoServicio>>> Obtener_todos()
        {
            var resultado = await _tipoServicioRepo.Obtener_todos();
            return resultado;
        }

        public async Task<DTO<Items_pagina<TipoServicio>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _tipoServicioRepo.Obtener_paginado(filtros);
            return resultado;
        }
    }
}