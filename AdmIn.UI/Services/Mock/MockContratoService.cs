using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public class MockContratoService : IContratoService
    {
        private readonly List<Contrato> _contratos;
        private readonly List<Inquilino> _inquilinos;
        private readonly List<Pago> _pagos;
        private readonly List<Reserva> _reservas;
        private readonly List<ContratoEstado> _contratoEstados;
        private readonly List<PagoEstado> _pagoEstados;

        private readonly IInmuebleService _mockInmuebleService;
        private readonly IPersonaService _mockPersonaService; 
        private static readonly Random random = new Random();

        public MockContratoService(IInmuebleService mockInmuebleService, IPersonaService mockPersonaService)
        {
            _mockInmuebleService = mockInmuebleService;
            _mockPersonaService = mockPersonaService;

            _contratoEstados = GenerarContratoEstados().ToList();
            _pagoEstados = GenerarPagoEstados().ToList();

            _contratos = new List<Contrato>();
            _inquilinos = new List<Inquilino>();
            _pagos = new List<Pago>();
            _reservas = new List<Reserva>();

            // Fetch dependent data. Using .Result for simplicity in mocks.
            var personas = _mockPersonaService.BuscarPersonasAsync(null, null).Result;
            var inmuebles = _mockInmuebleService.ObtenerInmuebles().Result.ToList();
            var estadosInmueble = _mockInmuebleService.ObtenerEstadosInmueble().Result.ToList();

            GenerarContratosYPagos(personas, inmuebles, estadosInmueble);
        }

        private List<ContratoEstado> GenerarContratoEstados() => new List<ContratoEstado>
        {
            new() { Id = 1, Descripcion = "Vigente" },
            new() { Id = 2, Descripcion = "Finalizado normal" },
            new() { Id = 3, Descripcion = "Finalizado antes de tiempo" }
        };

        private List<PagoEstado> GenerarPagoEstados() => new List<PagoEstado>
        {
            new() { Id = 1, Estado = "Pendiente" },
            new() { Id = 2, Estado = "Informado" },
            new() { Id = 3, Estado = "Vencido" },
            new() { Id = 4, Estado = "Impago" },
            new() { Id = 5, Estado = "Pagado" }
        };

        private void GenerarContratosYPagos(List<PersonaBase> personas, List<Inmueble> inmuebles, List<EstadoInmueble> estadosInmueble)
        {
            if (!personas.Any() || !inmuebles.Any()) return;

            var personasDisponibles = personas.ToList();
            var inmueblesDisponibles = inmuebles.Where(i => i.Estado.Estado == "Disponible").ToList();
            int contratosACrear = Math.Min(Math.Min(personasDisponibles.Count, inmueblesDisponibles.Count), 5);

            for (int i = 0; i < contratosACrear; i++)
            {
                var persona = personasDisponibles[i];
                var inmueble = inmueblesDisponibles[i];

                var estadoContrato = _contratoEstados[random.Next(_contratoEstados.Count)];
                bool vigente = estadoContrato.Descripcion == "Vigente";

                var inquilino = new Inquilino
                {
                    Id = _inquilinos.Any() ? _inquilinos.Max(inq => inq.Id) + 1 : 1,
                    PersonaId = persona.PersonaId,
                    Nombre = persona.Nombre,
                    ApellidoPaterno = persona.ApellidoPaterno,
                    ApellidoMaterno = persona.ApellidoMaterno,
                    Rfc = persona.Rfc,
                    Email = persona.Email,
                    Nacionalidad = persona.Nacionalidad,
                    EsPersonaFisica = persona.EsPersonaFisica,
                    EsTitular = persona.EsTitular,
                    Direcciones = persona.Direcciones,
                    Telefonos = persona.Telefonos,
                    Inmueble = inmueble
                };
                _inquilinos.Add(inquilino);
                persona.Inquilino = inquilino;


                if (vigente)
                {
                    inmueble.Estado = estadosInmueble.First(e => e.Estado == "Ocupado");
                    if (inmueble.Inquilinos == null) inmueble.Inquilinos = new List<Inquilino>();
                    inmueble.Inquilinos.Add(inquilino);
                }
                else
                {
                     inmueble.Estado = estadosInmueble.First(e => e.Estado == "Disponible");
                }


                var contrato = new Contrato
                {
                    Id = _contratos.Any() ? _contratos.Max(c => c.Id) + 1 : 1,
                    Inquilino = inquilino,
                    Inmueble = inmueble,
                    Administrador = new Administrador { Nombre = "Admin Contrato Ejemplo" },
                    FechaInicio = DateTime.Now.AddMonths(-random.Next(1, 12)),
                    FechaFin = DateTime.Now.AddMonths(random.Next(1, 12)),
                    MontoMensual = random.Next(1000, 5000),
                    Estado = estadoContrato,
                    Pagos = new List<Pago>()
                };
                _contratos.Add(contrato);

                int pagosACrear = vigente ? 12 : random.Next(1, 12);
                for (int j = 0; j < pagosACrear; j++)
                {
                    var estadoPago = contrato.Estado.Descripcion.StartsWith("Finalizado")
                        ? _pagoEstados.First(e => e.Estado == "Pagado")
                        : (j == 0 ? _pagoEstados.First(e => e.Estado == "Pagado") : _pagoEstados.First(e => e.Estado == "Pendiente"));

                    var pago = new Pago
                    {
                        Id = _pagos.Any() ? _pagos.Max(p => p.Id) + 1 : 1,
                        Contrato = contrato,
                        FechaVencimiento = contrato.FechaInicio.AddMonths(j),
                        Estado = estadoPago,
                        Descripcion = $"Cuota {j + 1} de {pagosACrear}",
                        DetallesPago = new List<DetallePago>
                        {
                            new DetallePago { Descripcion = "Cuota mensual", Monto = contrato.MontoMensual }
                        }
                    };
                    _pagos.Add(pago);
                    contrato.Pagos.Add(pago);
                }
                if (inmueble.Contratos == null) inmueble.Contratos = new List<Contrato>();
                inmueble.Contratos.Add(contrato);

                if (vigente)
                {
                    if (inmueble.Pagos == null) inmueble.Pagos = new List<Pago>();
                    inmueble.Pagos.AddRange(contrato.Pagos);
                }
            }
        }

        public async Task<IEnumerable<Contrato>> ObtenerContratos() => await Task.FromResult(_contratos);
        public async Task<Contrato?> ObtenerContratoPorId(int id) => await Task.FromResult(_contratos.FirstOrDefault(c => c.Id == id));
        public async Task<IEnumerable<Inquilino>> ObtenerInquilinos() => await Task.FromResult(_inquilinos);
        public async Task<Inquilino?> ObtenerInquilinoPorId(int id) => await Task.FromResult(_inquilinos.FirstOrDefault(i => i.Id == id));
        public async Task<IEnumerable<Pago>> ObtenerPagos() => await Task.FromResult(_pagos);
        public async Task<Pago?> ObtenerPagoPorId(int id) => await Task.FromResult(_pagos.FirstOrDefault(p => p.Id == id));

        public async Task<Reserva> GuardarReservaAsync(Reserva reserva)
        {
            reserva.Id = _reservas.Any() ? _reservas.Max(r => r.Id) + 1 : 1;
            reserva.FechaCreacion = DateTime.Now;
            reserva.Administrador = new Administrador { Nombre = "Admin Reserva Ejemplo" };
            _reservas.Add(reserva);

            var inmueble = await _mockInmuebleService.ObtenerInmueblePorId(reserva.Inmueble.Id);
            if (inmueble != null)
            {
                inmueble.Reserva = reserva;
                var estadoReservado = (await _mockInmuebleService.ObtenerEstadosInmueble()).First(e => e.Estado == "Reservado");
                inmueble.Estado = estadoReservado;
            }
            return await Task.FromResult(reserva);
        }

        public async Task<Reserva?> ObtenerReservaPorInmuebleIdAsync(int inmuebleId)
        {
            return await Task.FromResult(_reservas.FirstOrDefault(r => r.Inmueble.Id == inmuebleId));
        }

        public async Task<Contrato> CrearContrato(Inmueble inmueble, decimal montoMensual, string observaciones, Administrador administrador, PersonaBase persona, int cantidadMeses, int mesInicio, int diaVencimiento)
        {
            var anioActual = DateTime.Today.Year;
            var fechaInicio = new DateTime(anioActual, mesInicio, 1);
            var fechaFin = fechaInicio.AddMonths(cantidadMeses).AddDays(-1);

            var contrato = new Contrato
            {
                Inmueble = inmueble,
                MontoMensual = montoMensual,
                Observacion = observaciones,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Administrador = administrador ?? new Administrador { Nombre = "Admin Nuevo Contrato" }
            };

            var inquilino = _inquilinos.FirstOrDefault(i => i.PersonaId == persona.PersonaId);
            if (inquilino == null)
            {
                 inquilino = new Inquilino
                {
                    Id = _inquilinos.Any() ? _inquilinos.Max(inq => inq.Id) + 1 : 1,
                    PersonaId = persona.PersonaId,
                    Nombre = persona.Nombre, ApellidoPaterno = persona.ApellidoPaterno, ApellidoMaterno = persona.ApellidoMaterno,
                    Rfc = persona.Rfc, Email = persona.Email, Nacionalidad = persona.Nacionalidad,
                    EsPersonaFisica = persona.EsPersonaFisica, EsTitular = persona.EsTitular,
                    Direcciones = persona.Direcciones, Telefonos = persona.Telefonos,
                    Inmueble = inmueble
                };
            }
            contrato.Inquilino = inquilino;

            var estadoContrato = _contratoEstados.First(e => e.Descripcion == "Vigente");
            contrato.Estado = estadoContrato;

            var agendaPagos = new List<Pago>();
            var fechaBase = new DateTime(DateTime.Today.Year, mesInicio, 1);

            for (int i = 0; i < cantidadMeses; i++)
            {
                var mes = fechaBase.AddMonths(i);
                var dia = Math.Min(diaVencimiento, DateTime.DaysInMonth(mes.Year, mes.Month));
                var fechaVencimiento = new DateTime(mes.Year, mes.Month, dia);

                agendaPagos.Add(new Pago
                {
                    Id = _pagos.Any() ? _pagos.Max(p => p.Id) + (i+1) : (i+1), 
                    Contrato = contrato, 
                    FechaVencimiento = fechaVencimiento,
                    Estado = _pagoEstados.First(e => e.Estado == "Pendiente"),
                    Descripcion = $"Cuota mensual {i + 1} de {cantidadMeses}",
                    DetallesPago = new List<DetallePago>
                    {
                        new DetallePago { Descripcion = "Cuota mensual", Monto = contrato.MontoMensual }
                    }
                });
            }
            contrato.Pagos = agendaPagos;
            return await Task.FromResult(contrato);
        }

        public async Task GuardarNuevoContrato(Contrato contrato)
        {
            contrato.Id = _contratos.Any() ? _contratos.Max(c => c.Id) + 1 : 1;

            foreach(var pago in contrato.Pagos)
            {
                pago.Contrato = contrato; 
                 if (!_pagos.Any(p=> p.Id == pago.Id)) _pagos.Add(pago);
            }

            if (!_inquilinos.Any(i => i.Id == contrato.Inquilino.Id))
            {
                 _inquilinos.Add(contrato.Inquilino);
            }

            var inmuebleActualizar = await _mockInmuebleService.ObtenerInmueblePorId(contrato.Inmueble.Id);
            if (inmuebleActualizar != null)
            {
                var estadoOcupado = (await _mockInmuebleService.ObtenerEstadosInmueble()).First(e => e.Estado == "Ocupado");
                inmuebleActualizar.Estado = estadoOcupado;
                if (inmuebleActualizar.Inquilinos == null) inmuebleActualizar.Inquilinos = new List<Inquilino>();
                inmuebleActualizar.Inquilinos.Add(contrato.Inquilino);
                if (inmuebleActualizar.Contratos == null) inmuebleActualizar.Contratos = new List<Contrato>();
                inmuebleActualizar.Contratos.Add(contrato);
            }
            _contratos.Add(contrato);
            await Task.CompletedTask;
        }

        public async Task<List<ContratoEstado>> ObtenerContratoEstados() => await Task.FromResult(_contratoEstados);
        public async Task<List<PagoEstado>> ObtenerPagoEstados() => await Task.FromResult(_pagoEstados);
    }
}
