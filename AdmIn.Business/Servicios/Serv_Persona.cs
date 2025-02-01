using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using AdmIn.Data.Entidades;
using AdmIn.Data.Repositorios;
using AdmIn.Data.Repositorios.AdmIn.Data.Repositorios;

namespace AdmIn.Business.Servicios
{
    public class Serv_Persona : IServ_Persona
    {
        private readonly Rep_PERSONA _repPersona;
        private readonly Rep_PERSONA_DIRECCION _repPersonaDireccion;
        private readonly Rep_PERSONA_TELEFONO _repPersonaTelefono;
        private readonly Rep_DIRECCION _repDireccion;
        private readonly Rep_TELEFONO _repTelefono;

        public Serv_Persona()
        {
            _repPersona = new Rep_PERSONA();
            _repPersonaDireccion = new Rep_PERSONA_DIRECCION();
            _repPersonaTelefono = new Rep_PERSONA_TELEFONO();
            _repDireccion = new Rep_DIRECCION();
            _repTelefono = new Rep_TELEFONO();
        }

        public async Task<DTO<PersonaBase>> Crear(PersonaBase persona)
        {
            var personaDto = await _repPersona.Crear(persona.ToDataPERSONA());
            if (!personaDto.Correcto || personaDto.Datos == null)
                return new DTO<PersonaBase> { Correcto = false, Mensaje = personaDto.Mensaje };

            persona.PersonaId = personaDto.Datos.PER_ID;

            foreach (var direccion in persona.Direcciones)
            {
                var direccionDto = await _repDireccion.Crear(direccion.ToDataDIRECCION());
                if (direccionDto.Correcto && direccionDto.Datos != null)
                {
                    await _repPersonaDireccion.Crear(new PERSONA_DIRECCION
                    {
                        PER_ID = persona.PersonaId,
                        DIR_ID = direccionDto.Datos.DIR_ID
                    });
                }
            }

            foreach (var telefono in persona.Telefonos)
            {
                var telefonoDto = await _repTelefono.Crear(telefono.ToDataTELEFONO());
                if (telefonoDto.Correcto && telefonoDto.Datos != null)
                {
                    await _repPersonaTelefono.Crear(new PERSONA_TELEFONO
                    {
                        PER_ID = persona.PersonaId,
                        TEL_ID = telefonoDto.Datos.TEL_ID
                    });
                }
            }

            return new DTO<PersonaBase> { Correcto = true, Mensaje = "Persona creada exitosamente", Datos = persona };
        }

        public async Task<DTO<PersonaBase>> Actualizar(PersonaBase entidad)
        {
            var personaDto = await _repPersona.Obtener_por_id(new PERSONA() { PER_ID = entidad.PersonaId });
            if (!personaDto.Correcto || personaDto.Datos == null)
                return new DTO<PersonaBase> { Correcto = false, Mensaje = personaDto.Mensaje };

            await _repPersona.Actualizar(entidad.ToDataPERSONA());

            var direccionesActuales = await _repPersonaDireccion.Obtener_por_persona(entidad.PersonaId);
            var telefonosActuales = await _repPersonaTelefono.Obtener_por_persona(entidad.PersonaId);

            var direccionesAEliminar = direccionesActuales.Datos.Where(pd => !entidad.Direcciones.Any(d => d.DireccionId == pd.DIR_ID));
            var telefonosAEliminar = telefonosActuales.Datos.Where(pt => !entidad.Telefonos.Any(t => t.TelefonoId == pt.TEL_ID));

            foreach (var pd in direccionesAEliminar)
                await _repPersonaDireccion.Eliminar(pd);

            foreach (var pt in telefonosAEliminar)
                await _repPersonaTelefono.Eliminar(pt);

            foreach (var direccion in entidad.Direcciones.Where(d => !direccionesActuales.Datos.Any(pd => pd.DIR_ID == d.DireccionId)))
            {
                var direccionDto = await _repDireccion.Crear(direccion.ToDataDIRECCION());
                if (direccionDto.Correcto && direccionDto.Datos != null)
                {
                    await _repPersonaDireccion.Crear(new PERSONA_DIRECCION
                    {
                        PER_ID = entidad.PersonaId,
                        DIR_ID = direccionDto.Datos.DIR_ID
                    });
                }
            }

            foreach (var telefono in entidad.Telefonos.Where(t => !telefonosActuales.Datos.Any(pt => pt.TEL_ID == t.TelefonoId)))
            {
                var telefonoDto = await _repTelefono.Crear(telefono.ToDataTELEFONO());
                if (telefonoDto.Correcto && telefonoDto.Datos != null)
                {
                    await _repPersonaTelefono.Crear(new PERSONA_TELEFONO
                    {
                        PER_ID = entidad.PersonaId,
                        TEL_ID = telefonoDto.Datos.TEL_ID
                    });
                }
            }

            return new DTO<PersonaBase> { Correcto = true, Mensaje = "Persona actualizada exitosamente", Datos = entidad };
        }

        public async Task<DTO<bool>> Eliminar(PersonaBase persona)
        {
            var direcciones = await _repPersonaDireccion.Obtener_por_persona(persona.PersonaId);
            var telefonos = await _repPersonaTelefono.Obtener_por_persona(persona.PersonaId);

            foreach (var direccion in direcciones.Datos)
                await _repPersonaDireccion.Eliminar(direccion);

            foreach (var telefono in telefonos.Datos)
                await _repPersonaTelefono.Eliminar(telefono);

            return await _repPersona.Eliminar(new PERSONA() { PER_ID = persona.PersonaId });
        }

        public async Task<DTO<PersonaBase>> Obtener_por_id(PersonaBase personaBase)
        {
            var personaDto = await _repPersona.Obtener_por_id(personaBase.ToDataPERSONA());
            if (!personaDto.Correcto || personaDto.Datos == null)
                return new DTO<PersonaBase> { Correcto = false, Mensaje = personaDto.Mensaje };

            var persona = personaDto.Datos.ToBusinessPersona();
            return new DTO<PersonaBase> { Correcto = true, Datos = persona };
        }

        public async Task<DTO<IEnumerable<PersonaBase>>> Obtener_todos()
        {
            var resultado = await _repPersona.Obtener_todos();
            if (!resultado.Correcto || resultado.Datos == null)
                return new DTO<IEnumerable<PersonaBase>> { Correcto = false, Mensaje = resultado.Mensaje };

            return new DTO<IEnumerable<PersonaBase>> { Correcto = true, Datos = resultado.Datos.Select(p => p.ToBusinessPersona()) };
        }

        public async Task<DTO<Items_pagina<PersonaBase>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _repPersona.Obtener_paginado(filtros);
            if (!resultado.Correcto || resultado.Datos == null)
                return new DTO<Items_pagina<PersonaBase>> { Correcto = false, Mensaje = resultado.Mensaje };

            return new DTO<Items_pagina<PersonaBase>>
            {
                Correcto = true,
                Datos = new Items_pagina<PersonaBase>
                {
                    Items = resultado.Datos.Items.Select(p => p.ToBusinessPersona()).ToList(),
                    Total_items = resultado.Datos.Total_items
                }
            };
        }
    }
}
