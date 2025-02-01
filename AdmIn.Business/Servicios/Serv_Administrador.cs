using AdmIn.Common;
using AdmIn.Data.Repositorios;
using AdmIn.Business.Entidades;

using AdmIn.Business.Utilidades;

namespace AdmIn.Business.Servicios
{
    public class Serv_Administrador : IServ_Administrador
    {
        private readonly Rep_ADMINISTRADOR _repAdministrador;
        private readonly IServ_Persona _servPersona;

        public Serv_Administrador()
        {
            _repAdministrador = new Rep_ADMINISTRADOR();
            _servPersona = new Serv_Persona();
        }

        public async Task<DTO<Administrador>> Crear(Administrador administrador)
        {
            var personaDto = await _servPersona.Crear(administrador);
            if (!personaDto.Correcto || personaDto.Datos == null)
                return new DTO<Administrador> { Correcto = false, Mensaje = personaDto.Mensaje };

            administrador.PersonaId = personaDto.Datos.PersonaId;
            var adminDto = await _repAdministrador.Crear(administrador.ToDataADMINISTRADOR());
            if (!adminDto.Correcto || adminDto.Datos == null)
                return new DTO<Administrador> { Correcto = false, Mensaje = adminDto.Mensaje };

            administrador.AdministradorId = adminDto.Datos.ADM_ID;
            return new DTO<Administrador> { Correcto = true, Mensaje = "Administrador creado exitosamente", Datos = administrador };
        }

        public async Task<DTO<Administrador>> Actualizar(Administrador administrador)
        {
            var personaDto = await _servPersona.Actualizar(administrador);
            if (!personaDto.Correcto)
                return new DTO<Administrador> { Correcto = false, Mensaje = personaDto.Mensaje };

            await _repAdministrador.Actualizar(administrador.ToDataADMINISTRADOR());
            return new DTO<Administrador> { Correcto = true, Mensaje = "Administrador actualizado exitosamente", Datos = administrador };
        }

        public async Task<DTO<bool>> Eliminar(Administrador administrador)
        {
            var adminDto = await _repAdministrador.Eliminar(administrador.ToDataADMINISTRADOR());
            if (!adminDto.Correcto)
                return new DTO<bool> { Correcto = false, Mensaje = adminDto.Mensaje };

            return await _servPersona.Eliminar(administrador);
        }

        public async Task<DTO<Administrador>> Obtener_por_id(Administrador administrador)
        {
            var adminDto = await _repAdministrador.Obtener_por_id(administrador.ToDataADMINISTRADOR());
            if (!adminDto.Correcto || adminDto.Datos == null)
                return new DTO<Administrador> { Correcto = false, Mensaje = adminDto.Mensaje };

            Administrador admin = adminDto.Datos.ToBusinessAdministrador();

            return new DTO<Administrador> { Correcto = true, Datos = admin };
        }

        public async Task<DTO<IEnumerable<Administrador>>> Obtener_todos()
        {
            var resultado = await _repAdministrador.Obtener_todos();
            if (!resultado.Correcto || resultado.Datos == null)
                return new DTO<IEnumerable<Administrador>> { Correcto = false, Mensaje = resultado.Mensaje };

            return new DTO<IEnumerable<Administrador>>
            {
                Correcto = true,
                Datos = resultado.Datos.Select(a => a.ToBusinessAdministrador()) // Mapeo a Administrador
            };
        }

        public async Task<DTO<Items_pagina<Administrador>>> Obtener_paginado(Filtros_paginado filtros)
        {
            var resultado = await _repAdministrador.Obtener_paginado(filtros);
            if (!resultado.Correcto || resultado.Datos == null)
                return new DTO<Items_pagina<Administrador>> { Correcto = false, Mensaje = resultado.Mensaje };

            return new DTO<Items_pagina<Administrador>>
            {
                Correcto = true,
                Datos = new Items_pagina<Administrador>
                {
                    Items = resultado.Datos.Items.Select(a => a.ToBusinessAdministrador()).ToList(), // Mapeo a Administrador
                    Total_items = resultado.Datos.Total_items
                }
            };
        }
    }
}
