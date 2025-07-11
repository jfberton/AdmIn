using System;

namespace AdmIn.UI.Services.Mock
{
    public static class MockDataInitializer
    {
        public static void InicializarDatosMock(IServiceProvider provider)
        {
            // Forzar inicialización de datos mock en el orden correcto
            var personaService = provider.GetService(typeof(IPersonaService)) as IPersonaService;
            var empleadoService = provider.GetService(typeof(IEmpleadoService)) as IEmpleadoService;
            var inmuebleService = provider.GetService(typeof(IInmuebleService)) as IInmuebleService;
            var reparacionService = provider.GetService(typeof(IReparacionService)) as IReparacionService;
            var contratoService = provider.GetService(typeof(IContratoService)) as IContratoService;
            var usuarioService = provider.GetService(typeof(IUsuarioService)) as IUsuarioService;

            _ = personaService?.BuscarPersonasAsync(null, null).Result;
            _ = empleadoService?.ObtenerEmpleados().Result;
            _ = inmuebleService?.ObtenerInmuebles().Result;
            _ = reparacionService?.ObtenerReparaciones().Result;
            _ = inmuebleService?.ObtenerInmuebles().Result;
            _ = contratoService?.ObtenerContratos().Result;
            _ = usuarioService?.ObtenerUsuarios().Result;
        }
    }
}
