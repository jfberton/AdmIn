using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;

namespace AdmIn.Business.Servicios
{
    public class Serv_InmuebleCondicion : IServ_InmuebleCondicion
    {
        private readonly IInmuebleCondicionRepository _condicionRepo;

        public Serv_InmuebleCondicion(IInmuebleCondicionRepository condicionRepository)
        {
            _condicionRepo = condicionRepository;
        }

        public async Task<DTO<InmuebleCondicion>> Crear(InmuebleCondicion condicion)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(condicion.Nombre))
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "El nombre de la condición es requerido."
                };
            }

            if (condicion.Nombre.Length > 50)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "El nombre de la condición no puede exceder 50 caracteres."
                };
            }

            // Verificar si ya existe una condición con el mismo nombre
            var existente = await _condicionRepo.Obtener_por_nombre(condicion.Nombre);
            if (existente.Correcto && existente.Datos != null)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "Ya existe una condición con ese nombre."
                };
            }

            // Validar color hexadecimal
            if (!string.IsNullOrEmpty(condicion.Color) && !condicion.Color.StartsWith("#"))
            {
                condicion.Color = "#" + condicion.Color;
            }

            if (string.IsNullOrEmpty(condicion.Color))
            {
                condicion.Color = "#6b7280"; // Color por defecto
            }

            condicion.FechaCreacion = DateTime.Now;
            condicion.FechaModificacion = DateTime.Now;

            return await _condicionRepo.Crear(condicion);
        }

        public async Task<DTO<InmuebleCondicion>> Actualizar(InmuebleCondicion condicion)
        {
            // Validaciones de negocio
            if (condicion.Id <= 0)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "ID de condición inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(condicion.Nombre))
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "El nombre de la condición es requerido."
                };
            }

            if (condicion.Nombre.Length > 50)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "El nombre de la condición no puede exceder 50 caracteres."
                };
            }

            // Verificar si ya existe otra condición con el mismo nombre
            var existente = await _condicionRepo.Obtener_por_nombre(condicion.Nombre);
            if (existente.Correcto && existente.Datos != null && existente.Datos.Id != condicion.Id)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "Ya existe una condición con ese nombre."
                };
            }

            // Validar color hexadecimal
            if (!string.IsNullOrEmpty(condicion.Color) && !condicion.Color.StartsWith("#"))
            {
                condicion.Color = "#" + condicion.Color;
            }

            if (string.IsNullOrEmpty(condicion.Color))
            {
                condicion.Color = "#6b7280"; // Color por defecto
            }

            condicion.FechaModificacion = DateTime.Now;

            return await _condicionRepo.Actualizar(condicion);
        }

        public async Task<DTO<bool>> Eliminar(InmuebleCondicion condicion)
        {
            if (condicion.Id <= 0)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "ID de condición inválido."
                };
            }

            // No permitir eliminar la condición por defecto (ID = 1, "Disponible")
            if (condicion.Id == 1)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = "No se puede eliminar la condición por defecto 'Disponible'."
                };
            }

            return await _condicionRepo.Eliminar(condicion);
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_id(InmuebleCondicion condicion)
        {
            if (condicion.Id <= 0)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "ID de condición inválido."
                };
            }

            return await _condicionRepo.Obtener_por_id(condicion);
        }

        public async Task<DTO<IEnumerable<InmuebleCondicion>>> Obtener_todos()
        {
            return await _condicionRepo.Obtener_todos();
        }

        public async Task<DTO<Items_pagina<InmuebleCondicion>>> Obtener_paginado(Filtros_paginado filtros)
        {
            if (filtros.Top <= 0)
                filtros.Top = 10;

            if (filtros.Skip < 0)
                filtros.Skip = 0;

            return await _condicionRepo.Obtener_paginado(filtros);
        }

        public async Task<DTO<IEnumerable<InmuebleCondicion>>> Obtener_activos()
        {
            return await _condicionRepo.Obtener_activos();
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_nombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = "El nombre de la condición es requerido."
                };
            }

            return await _condicionRepo.Obtener_por_nombre(nombre);
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_defecto()
        {
            return await _condicionRepo.Obtener_por_defecto();
        }
    }
}