using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdmIn.UI.Services
{
    public class Serv_Proveedor : ServicioBase<Proveedor>, IServ_Proveedor
    {
        public Serv_Proveedor(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Proveedor> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Proveedor")
        {
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_activos()
        {
            return await EjecutarPeticion<DTO<IEnumerable<Proveedor>>>(HttpMethod.Get, "obtener_activos");
        }

        public async Task<DTO<Proveedor>> Obtener_por_rfc(string rfc)
        {
            return await EjecutarPeticion<DTO<Proveedor>>(HttpMethod.Get, $"obtener_por_rfc/{rfc}");
        }

        public async Task<DTO<Proveedor>> Obtener_por_email(string email)
        {
            return await EjecutarPeticion<DTO<Proveedor>>(HttpMethod.Get, $"obtener_por_email/{email}");
        }

        public async Task<DTO<bool>> Validar_rfc_unico(string rfc, int? proveedorId = null)
        {
            var url = $"validar_rfc_unico/{rfc}";
            if (proveedorId.HasValue)
            {
                url += $"?proveedorId={proveedorId}";
            }
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Get, url);
        }

        public async Task<DTO<bool>> Validar_email_unico(string email, int? proveedorId = null)
        {
            var url = $"validar_email_unico/{email}";
            if (proveedorId.HasValue)
            {
                url += $"?proveedorId={proveedorId}";
            }
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Get, url);
        }

        public async Task<DTO<IEnumerable<TipoServicio>>> Obtener_servicios_proveedor(int proveedorId)
        {
            return await EjecutarPeticion<DTO<IEnumerable<TipoServicio>>>(HttpMethod.Get, $"obtener_servicios/{proveedorId}");
        }

        public async Task<DTO<bool>> Actualizar_servicios_proveedor(int proveedorId, List<int> serviciosIds)
        {
            var datos = new { ProveedorId = proveedorId, ServiciosIds = serviciosIds };
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "actualizar_servicios", datos);
        }
    }
}