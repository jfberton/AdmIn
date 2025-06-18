using AdmIn.Business.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public interface IContratoService // Renamed from IMockContratoService
    {
        Task<IEnumerable<Contrato>> ObtenerContratos();
        Task<Contrato?> ObtenerContratoPorId(int id);
        Task<IEnumerable<Inquilino>> ObtenerInquilinos();
        Task<Inquilino?> ObtenerInquilinoPorId(int id);
        Task<IEnumerable<Pago>> ObtenerPagos();
        Task<Pago?> ObtenerPagoPorId(int id);
        Task<Reserva> GuardarReservaAsync(Reserva reserva);
        Task<Reserva?> ObtenerReservaPorInmuebleIdAsync(int inmuebleId);
        Task<Contrato> CrearContrato(Inmueble inmueble, decimal montoMensual, string observaciones, Administrador administrador, PersonaBase inquilino, int cantidadMeses, int mesInicio, int diaVencimiento);
        Task GuardarNuevoContrato(Contrato contrato);
        Task<List<ContratoEstado>> ObtenerContratoEstados();
        Task<List<PagoEstado>> ObtenerPagoEstados();
    }
}
