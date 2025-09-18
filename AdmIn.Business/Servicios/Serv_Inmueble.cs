using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;

namespace AdmIn.Business.Servicios
{
    public class Serv_Inmueble : IServ_Inmueble
    {
        private readonly IInmuebleRepository _inmuebleRepo;

        public Serv_Inmueble(IInmuebleRepository inmuebleRepository)
        {
            _inmuebleRepo = inmuebleRepository;
        }

        public async Task<DTO<Inmueble>> Crear(Inmueble inmueble)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(inmueble.Nombre))
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = "El nombre del inmueble es requerido."
                };
            }

            if (inmueble.Valor <= 0)
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = "El valor del inmueble debe ser mayor a cero."
                };
            }

            if (inmueble.MonedaId <= 0)
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = "Debe especificar una moneda válida."
                };
            }

            var resultado = await _inmuebleRepo.Crear(inmueble);
            return resultado;
        }

        public async Task<DTO<Inmueble>> Actualizar(Inmueble inmueble)
        {
            // Validaciones de negocio
            if (inmueble.Id <= 0)
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = "ID de inmueble inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(inmueble.Nombre))
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = "El nombre del inmueble es requerido."
                };
            }

            if (inmueble.Valor <= 0)
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = "El valor del inmueble debe ser mayor a cero."
                };
            }

            var resultado = await _inmuebleRepo.Actualizar(inmueble);
            return resultado;
        }

        public async Task<DTO<bool>> Eliminar(Inmueble inmueble)
        {
            if (inmueble.Id <= 0)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "ID de inmueble inválido."
                };
            }

            return await _inmuebleRepo.Eliminar(inmueble);
        }

        public async Task<DTO<Inmueble>> Obtener_por_id(Inmueble inmueble)
        {
            if (inmueble.Id <= 0)
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = "ID de inmueble inválido."
                };
            }

            var resultado = await _inmuebleRepo.Obtener_por_id(inmueble);
            return resultado;
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_todos()
        {
            var resultado = await _inmuebleRepo.Obtener_todos();
            return resultado;
        }

        public async Task<DTO<Items_pagina<Inmueble>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _inmuebleRepo.Obtener_paginado(filtros);
            return resultado;
        }

        public async Task<DTO<IEnumerable<string>>> Obtener_estados()
        {
            return await _inmuebleRepo.Obtener_estados();
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_estado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                return new DTO<IEnumerable<Inmueble>>
                {
                    Correcto = false,
                    Mensaje = "El estado es requerido."
                };
            }

            return await _inmuebleRepo.Obtener_por_estado(estado);
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_administrador(int administradorId)
        {
            if (administradorId <= 0)
            {
                return new DTO<IEnumerable<Inmueble>>
                {
                    Correcto = false,
                    Mensaje = "ID de administrador inválido."
                };
            }

            return await _inmuebleRepo.Obtener_por_administrador(administradorId);
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_ubicacion(string pais, string estado, string ciudad)
        {
            return await _inmuebleRepo.Obtener_por_ubicacion(pais, estado, ciudad);
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_rango_precio(decimal precioMin, decimal precioMax)
        {
            if (precioMin < 0 || precioMax < 0)
            {
                return new DTO<IEnumerable<Inmueble>>
                {
                    Correcto = false,
                    Mensaje = "Los precios no pueden ser negativos."
                };
            }

            if (precioMin > precioMax)
            {
                return new DTO<IEnumerable<Inmueble>>
                {
                    Correcto = false,
                    Mensaje = "El precio mínimo no puede ser mayor al precio máximo."
                };
            }

            return await _inmuebleRepo.Obtener_por_rango_precio(precioMin, precioMax);
        }

        public async Task<DTO<IEnumerable<CaracteristicaInmueble>>> Obtener_caracteristicas(int inmuebleId)
        {
            if (inmuebleId <= 0)
            {
                return new DTO<IEnumerable<CaracteristicaInmueble>>
                {
                    Correcto = false,
                    Mensaje = "ID de inmueble inválido."
                };
            }

            return await _inmuebleRepo.Obtener_caracteristicas_inmueble(inmuebleId);
        }

        public async Task<DTO<CaracteristicaInmueble>> Agregar_caracteristica(int inmuebleId, CaracteristicaInmueble caracteristica)
        {
            if (inmuebleId <= 0)
            {
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = "ID de inmueble inválido."
                };
            }

            if (caracteristica.CaracteristicaID <= 0)
            {
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = "ID de característica inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(caracteristica.Valor))
            {
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = "El valor de la característica es requerido."
                };
            }

            // Asegurar que el InmuebleID esté correctamente asignado
            caracteristica.InmuebleID = inmuebleId;

            return await _inmuebleRepo.Agregar_caracteristica_inmueble(caracteristica);
        }

        public async Task<DTO<CaracteristicaInmueble>> Actualizar_caracteristica(CaracteristicaInmueble caracteristica)
        {
            if (caracteristica.Id <= 0)
            {
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = "ID de característica inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(caracteristica.Valor))
            {
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = "El valor de la característica es requerido."
                };
            }

            return await _inmuebleRepo.Actualizar_caracteristica_inmueble(caracteristica);
        }

        public async Task<DTO<bool>> Eliminar_caracteristica(int caracteristicaId)
        {
            if (caracteristicaId <= 0)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "ID de característica inválido."
                };
            }

            return await _inmuebleRepo.Eliminar_caracteristica_inmueble(caracteristicaId);
        }
    }
}