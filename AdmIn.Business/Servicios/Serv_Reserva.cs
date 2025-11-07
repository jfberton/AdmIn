using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AdmIn.Business.Servicios
{
    public class Serv_Reserva : IServ_Reserva
    {
        private readonly IReservaRepository _reservaRepo;
        private readonly IInmuebleRepository _inmuebleRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IContratoRepository? _contratoRepo; // may be null if not registered

        public Serv_Reserva(IReservaRepository reservaRepo, IInmuebleRepository inmuebleRepo, IUsuarioRepository usuarioRepo, IServiceProvider serviceProvider)
        {
            _reservaRepo = reservaRepo;
            _inmuebleRepo = inmuebleRepo;
            _usuarioRepo = usuarioRepo;
            // Resolve optional contract repo if available
            _contratoRepo = serviceProvider.GetService<IContratoRepository>();
        }

        public async Task<DTO<Reserva>> CrearReserva(int inmuebleId, int usuarioReservadorId, decimal costo, DateTime? fechaVencimiento = null, int? monedaId = null, int usuarioCreadorId = 0)
        {
            var result = new DTO<Reserva>();
            // Validaciones básicas
            if (costo < 0) { result.Correcto = false; result.Mensaje = "El costo debe ser mayor o igual a0."; return result; }

            // Obtener inmueble
            var inmRes = await _inmuebleRepo.Obtener_por_id(new Inmueble { Id = inmuebleId });
            if (!inmRes.Correcto || inmRes.Datos == null) { result.Correcto = false; result.Mensaje = "Inmueble no encontrado."; return result; }
            var inmueble = inmRes.Datos;

            // Solo puede reservarse si está disponible (CondicionId ==1)
            if (inmueble.CondicionId != 1) { result.Correcto = false; result.Mensaje = "Inmueble no está disponible para reserva."; return result; }

            // Verificar si ya existe una reserva activa para este inmueble
            var reservasAct = await _reservaRepo.Obtener_activas_por_inmueble(inmuebleId);
            if (reservasAct.Correcto && reservasAct.Datos != null && reservasAct.Datos.Any())
            { result.Correcto = false; result.Mensaje = "Ya existe una reserva activa para este inmueble."; return result; }

            // Si el usuario reservador no existe, crear uno ("agendar uno")
            var usuarioCheck = await _usuarioRepo.Obtener_por_id(new Usuario { Id = usuarioReservadorId });
            if (!usuarioCheck.Correcto || usuarioCheck.Datos == null)
            {
                var nuevoUsuario = new Usuario { Nombre = "Reservador", Email = "", Password = "", Activo = true, MonedaId = monedaId ?? 1, FechaCreacion = DateTime.Now, FechaModificacion = DateTime.Now };
                var creado = await _usuarioRepo.Crear(nuevoUsuario);
                if (!creado.Correcto || creado.Datos == null) { result.Correcto = false; result.Mensaje = "No se pudo crear usuario reservador."; return result; }
                usuarioReservadorId = creado.Datos.Id;
            }

            var reserva = new Reserva
            {
                InmuebleId = inmuebleId,
                UsuarioReservadorId = usuarioReservadorId,
                FechaCreacion = DateTime.Now,
                FechaVencimiento = fechaVencimiento ?? DateTime.Now.AddMonths(1),
                Costo = costo,
                MonedaId = monedaId,
                Estado = ReservaEstado.Activa,
                UsuarioCreadorId = usuarioCreadorId
            };

            // Crear reserva
            var creadoRes = await _reservaRepo.Crear(reserva);
            if (!creadoRes.Correcto || creadoRes.Datos == null) { return creadoRes; }

            // Actualizar condicion del inmueble a "Reservado" (condicionId =2)
            inmueble.CondicionId = 2; // Reservado
            await _inmuebleRepo.Actualizar(inmueble);

            return creadoRes;
        }

        public async Task<DTO<bool>> ExpirarReservas()
        {
            var result = new DTO<bool>();
            var vencidas = await _reservaRepo.Obtener_vencidas(DateTime.Now);
            if (!vencidas.Correcto || vencidas.Datos == null) { result.Correcto = false; result.Mensaje = "Error al obtener reservas vencidas."; return result; }

            foreach (var r in vencidas.Datos)
            {
                r.Estado = ReservaEstado.Vencida;
                await _reservaRepo.Actualizar(r);

                // Revisar si el inmueble debe volver a Disponible
                var inmRes = await _inmuebleRepo.Obtener_por_id(new Inmueble { Id = r.InmuebleId });
                if (inmRes.Correcto && inmRes.Datos != null)
                {
                    var inmueble = inmRes.Datos;
                    var otras = await _reservaRepo.Obtener_activas_por_inmueble(inmueble.Id);
                    if (!otras.Correcto || !otras.Datos.Any())
                    {
                        inmueble.CondicionId = 1; // Disponible
                        await _inmuebleRepo.Actualizar(inmueble);
                    }
                }
            }

            result.Correcto = true; result.Datos = true; result.Mensaje = "Reservas expiradas procesadas.";
            return result;
        }

        public async Task<DTO<Reserva>> ConvertirAContrato(int reservaId, bool descontarCostoAlContrato, ContratoRenta contratoParametros, int usuarioId)
        {
            var result = new DTO<Reserva>();
            var resGet = await _reservaRepo.Obtener_por_id(reservaId);
            if (!resGet.Correcto || resGet.Datos == null) { result.Correcto = false; result.Mensaje = "Reserva no encontrada."; return result; }
            var reserva = resGet.Datos;

            if (reserva.Estado != ReservaEstado.Activa) { result.Correcto = false; result.Mensaje = "Reserva no está activa."; return result; }

            if (_contratoRepo == null) { result.Correcto = false; result.Mensaje = "Repositorio de contratos no implementado."; return result; }

            // Preparar contrato
            contratoParametros.InmuebleId = reserva.InmuebleId;
            contratoParametros.InquilinoId = reserva.UsuarioReservadorId;
            contratoParametros.Deposito = contratoParametros.Deposito ?? 0;
            contratoParametros.FechaCreacion = DateTime.Now;
            contratoParametros.UsuarioCreadorId = usuarioId;

            // Aplicar monto de reserva si corresponde
            if (descontarCostoAlContrato)
            {
                contratoParametros.Deposito = (contratoParametros.Deposito ?? 0) - reserva.Costo;
                reserva.AplicadoAlContrato = true;
                reserva.MontoAplicadoAlContrato = reserva.Costo;
            }

            // Crear contrato
            var contratoCreado = await _contratoRepo.Crear(contratoParametros);
            if (!contratoCreado.Correcto || contratoCreado.Datos == null) { result.Correcto = false; result.Mensaje = "Error al crear contrato."; return result; }

            // Marcar reserva como convertida
            reserva.Estado = ReservaEstado.Convertida;
            reserva.ContratoRentaId = contratoCreado.Datos.Id;
            await _reservaRepo.Actualizar(reserva);

            // Actualizar inmueble a Alquilado (condicionId =3)
            var inmRes = await _inmuebleRepo.Obtener_por_id(new Inmueble { Id = reserva.InmuebleId });
            if (inmRes.Correcto && inmRes.Datos != null)
            {
                var inmueble = inmRes.Datos;
                inmueble.CondicionId = 3; // Alquilado
                await _inmuebleRepo.Actualizar(inmueble);
            }

            result.Correcto = true; result.Datos = reserva; result.Mensaje = "Reserva convertida a contrato.";
            return result;
        }
    }
}
