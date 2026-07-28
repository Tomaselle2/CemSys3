using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.Tramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.Reduccion;
using CemSys3.Enumerables;
using CemSys3.Helpers;
using CemSys3.Helpers.Enumerable;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.Tramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.TramiteConcesion
{
    public class ReduccionStrategy : ITramiteStrategy,
    ITramiteCreateStrategy<ReduccionDTO>, IComplementoTramite<ReduccionDTO>
    {
        private readonly IPlantillaTramite _plantillaService;
        private readonly IDocumentoTramiteService _documentoService;
        private readonly IPersona _personaService;
        private readonly AppDbContext _context;
        private readonly IHistorialEstados _historialEstadosService;
        private readonly ITareaPlantilla _tareaPlantilla;
        private readonly ITramite _tramiteService;
        private readonly INotas _notasService;
        private readonly IFirmantes _firmantes;
        private readonly IConcesion _concesionService;

        public ReduccionStrategy(
            IPlantillaTramite plantillaService,
            IDocumentoTramiteService documentoService,
            IPersona personaService,
            AppDbContext context,
            ITramite tramiteService,
            IHistorialEstados historialEstadosService,
            ITareaPlantilla tareaPlantilla,
            INotas notasService,
            IFirmantes firmantes,
            IConcesion concesionService)
        {
            _plantillaService = plantillaService;
            _documentoService = documentoService;
            _personaService = personaService;
            _context = context;
            _tramiteService = tramiteService;
            _historialEstadosService = historialEstadosService;
            _tareaPlantilla = tareaPlantilla;
            _notasService = notasService;
            _firmantes = firmantes;
            _concesionService = concesionService;
        }

        public async Task<int> AvanzarEstadoAsync(int tramiteId, int nuevoEstado, int usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Models.Tramite tramite = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == tramiteId) ?? throw new Exception("Trámite no encontrado");

                tramite.EstadoActualId = nuevoEstado;
                tramite.UsuarioId = usuarioId;

                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = nuevoEstado
                };
                await _historialEstadosService.Add(historial);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return tramite.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> CrearAsync(CrearTramiteDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == dto.TramiteConcesionId) ?? throw new Exception("Concesion no encontrada para inicar el trámite.");

                //1- registrar tramite
                TramiteDTO tramite = new TramiteDTO
                {
                    Visibilidad = true,
                    FechaCreacion = DateTime.Now,
                    TipoTramiteId = (int)TipoTramiteEnum.Reduccion,
                    UsuarioId = dto.UsuarioId,
                    EstadoActualId = (int)EstadosTramiteEnum.Iniciado
                };

                int tramiteId = await _tramiteService.Add(tramite);
                await _context.SaveChangesAsync(); //guardar cambios antes de continuar

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = tramite.FechaCreacion,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Iniciado
                };
                await _historialEstadosService.Add(historial);

                //3- registrar el tramite de reduccion
                Models.Reduccione reduccion = new Models.Reduccione
                {
                    TramiteId = tramiteId,
                    FechaCreacion = DateTime.Now,
                    Visibilidad = true,
                    DifuntoId = dto.DifuntoId,
                    ParcelaOrigenId = concesion.ParcelaId,
                    UsuarioId = dto.UsuarioId,
                    InfoAdicional = string.Empty,
                    ConcesionId = dto.TramiteConcesionId
                };
                await _context.Reducciones.AddAsync(reduccion);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                //5 - crear tareas para el tramite
                await _tareaPlantilla.CrearTareasPorTramite(tramiteId, (int)TipoTramiteEnum.Reduccion);

                var titulares = await _context.HistorialTitularesConcesiones
                     .Where(h => h.ConcesionId == reduccion.ConcesionId && h.FechaFin == null)
                     .Select(h => new TitularesContratoDTO
                     {
                         Id = h.Persona.Id,
                     }).ToListAsync();

                //6 - crea el firmante titular
                foreach (var titular in titulares)
                {
                    await _firmantes.Add(tramiteId, titular.Id.Value, "TITULAR", true);
                }

                reduccion.TipoTraslado = (int)TipoTrasladoEnum.Interno;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return tramiteId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task FinalizarAsync(int tramiteId, int usuarioId)
        {
            Models.Reduccione reduccion = await _context.Reducciones
                .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId)
                ?? throw new Exception("Trámite de reducción no encontrado.");

            Models.Concesione concesion = await _context.Concesiones
                .FirstOrDefaultAsync(c => c.TramiteId == reduccion.ConcesionId)
                ?? throw new Exception("Concesion no encontrada.");

            Models.Tramite tramite = await _context.Tramites
                .FirstOrDefaultAsync(t => t.Id == tramiteId)
                ?? throw new Exception("Trámite no encontrado.");

            if (tramite.EstadoActualId != (int)EstadosTramiteEnum.Pendiente)
                throw new Exception("El trámite no se encuentra en estado pendiente, no puede ser finalizado.");

            if (reduccion.FechaPendiente == null)
                throw new Exception("Debe asignar una fecha para el trámite antes de finalizar");

            if (reduccion.FechaPendiente < reduccion.FechaCreacion)
                throw new Exception("La fecha de realización no puede ser menor a la fecha de creación del trámite.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ── PRE-CALCULAR escenario ───────────────────────────────────────────────
                bool esInterno = reduccion.TipoTraslado == (int)TipoTrasladoEnum.Interno;
                bool esExterno = reduccion.TipoTraslado == (int)TipoTrasladoEnum.Externo;
                bool esNinguno = reduccion.TipoTraslado == (int)TipoTrasladoEnum.Ninguno;

                bool destinoTieneConcesion = esInterno && await _context.Concesiones
                    .AnyAsync(c => c.ParcelaId == reduccion.ParcelaDestinoId
                                && c.Visibilidad == true
                                && c.FechaFin == null);
                // ────────────────────────────────────────────────────────────────────────

                // 1 - Actualizar estado del trámite a Finalizado
                tramite.EstadoActualId = (int)EstadosTramiteEnum.Finalizado;
                await _historialEstadosService.Add(new HistorialEstadosDTO
                {
                    Fecha = reduccion.FechaPendiente ?? DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Finalizado
                });

                reduccion.FechaFinalizacion = reduccion.FechaPendiente;
                tramite.FechaFinalizacion = reduccion.FechaPendiente;

                // Vincular trámite con parcela origen
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                // 2 - Obtener difunto y cambiar estado a Reducido
                Models.Persona difunto = await _context.Personas
                    .FirstOrDefaultAsync(p => p.Id == reduccion.DifuntoId)
                    ?? throw new Exception("Difunto no encontrado.");

                difunto.EstadoDifuntoId = (int)EstadoDifuntoEnum.Reducido;

                // 3 - Vincular firmantes al trámite
                List<FirmantesDTO> firmantes = await _firmantes.GetAllByTramite(tramite.Id);
                foreach (var firmante in firmantes)
                {
                    await _historialEstadosService.VincularTramiteAPersona(tramiteId, firmante.PersonaId);
                    var persona = await _personaService.Get(firmante.PersonaId);
                    persona.InformacionAdicional += $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de reducción (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";
                    await _personaService.Update(persona);
                }

                // Log en difunto
                await _historialEstadosService.VincularTramiteAPersona(tramiteId, difunto.Id);
                difunto.InformacionAdicional += $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de reducción (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";

                // 4 - Log en concesión y parcela origen
                concesion.InformacionAdicional += $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de reducción (trámite {tramiteId})";

                Models.Parcela parcela = await _context.Parcelas
                    .Include(s => s.Seccion)
                    .FirstOrDefaultAsync(p => p.Id == concesion.ParcelaId)
                    ?? throw new Exception("Parcela no encontrada.");

                parcela.InformacionAdicional += $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de reducción (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";

                // 5 - Quitar difunto de la parcela origen (solo si se mueve, interno o externo)
                if (!esNinguno)
                {
                    Models.ParcelaDifunto parcelaDifunto = await _context.ParcelaDifuntos
                        .FirstOrDefaultAsync(pd => pd.ParcelaId == parcela.Id && pd.DifuntoId == difunto.Id && pd.FechaRetiro == null)
                        ?? throw new Exception("Registro de parcela-difunto no encontrado.");

                    parcelaDifunto.FechaRetiro = reduccion.FechaPendiente;
                    parcelaDifunto.TramiteRetiroId = tramiteId;
                    parcela.CantidadDifuntos -= 1;
                }

                string infoConcesion = "Revisar el libro de concesión";

                // 5.1 - Determinar si se debe mover o caducar la concesión
                bool parcelaOrigenQuedaVacia = parcela.CantidadDifuntos == 0;
                bool debeMoverConcesion = esInterno && parcelaOrigenQuedaVacia && !destinoTieneConcesion;

                // Caducar concesión solo si la parcela queda vacía y NO se va a mover
                if (parcelaOrigenQuedaVacia && !debeMoverConcesion)
                {
                    concesion.FechaFin = reduccion.FechaFinalizacion;
                    concesion.TramiteRetiroId = tramiteId;

                    await _historialEstadosService.CerrarHistorialParcelaConcesion(
                    concesion.TramiteId,
                    reduccion.FechaPendiente ?? DateTime.Now);

                    Models.Tramite tramiteConcesion = await _context.Tramites
                        .FirstOrDefaultAsync(t => t.Id == concesion.TramiteId)
                        ?? throw new Exception("Trámite de concesión no encontrado.");

                    tramiteConcesion.EstadoActualId = (int)EstadosTramiteEnum.Caducado;
                    concesion.Vencimiento = null;
                    tramiteConcesion.FechaFinalizacion = reduccion.FechaFinalizacion;

                    await _historialEstadosService.Add(new HistorialEstadosDTO
                    {
                        Fecha = reduccion.FechaPendiente ?? DateTime.Now,
                        TramiteId = tramiteConcesion.Id,
                        EstadoTramiteId = (int)EstadosTramiteEnum.Caducado
                    });

                    concesion.InformacionAdicional += $"\n● La concesión ({concesion.Concesion?.ToString("D5")}) ha sido cancelada/caducada automáticamente por no tener más difuntos asociados.";
                    infoConcesion = "La concesión debe ser cancelada/caducada por no tener más difuntos asociados.";

                    var titularesActuales = await _context.HistorialTitularesConcesiones
                        .Where(p => p.ConcesionId == concesion.TramiteId && p.FechaFin == null)
                        .ToListAsync();

                    foreach (var titularActual in titularesActuales)
                        titularActual.FechaFin = reduccion.FechaPendiente;
                }

                // ── CASO NINGUNO: queda en la misma parcela ─────────────────────────────
                if (esNinguno)
                {
                    // El difunto se redujo pero sigue en la misma parcela.
                    // Solo se actualiza su estado (ya hecho arriba) y se loguea.
                    parcela.InformacionAdicional += $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} el difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} fue reducido y permanece en la misma parcela.";
                    concesion.InformacionAdicional += $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} el difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} fue reducido y permanece en la parcela.";
                }

                // ── CASO INTERNO: se mueve a otra parcela dentro del cementerio ─────────
                if (esInterno)
                {
                    // Nuevo registro parcela-difunto en destino
                    await _context.ParcelaDifuntos.AddAsync(new Models.ParcelaDifunto
                    {
                        DifuntoId = difunto.Id,
                        ParcelaId = reduccion.ParcelaDestinoId ?? 0,
                        FechaIngreso = reduccion.FechaPendiente ?? DateTime.Now,
                        TramiteIngresoId = tramiteId
                    });

                    Models.Parcela parcelaDestino = await _context.Parcelas
                        .Include(s => s.Seccion)
                        .FirstOrDefaultAsync(p => p.Id == reduccion.ParcelaDestinoId)
                        ?? throw new Exception("Parcela destino no encontrada.");

                    if (debeMoverConcesion)
                    {
                        // ── CASO: origen vacía y destino vacío → mover la concesión ────

                        // 1. Cerrar registro actual en historial de parcelas
                        var historialParcelaActual = await _context.HistorialParcelasConcesions
                            .FirstOrDefaultAsync(h => h.ConcesionId == concesion.TramiteId && h.FechaFin == null);

                        if (historialParcelaActual != null)
                            historialParcelaActual.FechaFin = reduccion.FechaPendiente;

                        // 2. Registrar nueva parcela en historial
                        //_context.HistorialParcelasConcesions.Add(new HistorialParcelasConcesion
                        //{
                        //    ConcesionId = concesion.TramiteId,
                        //    ParcelaId = parcelaDestino.Id,
                        //    FechaInicio = reduccion.FechaPendiente ?? DateTime.Now,
                        //    FechaFin = null,
                        //    TramiteOrigenId = tramiteId
                        //});

                        await _historialEstadosService.CerrarHistorialParcelaConcesion(
                        concesion.TramiteId,
                        reduccion.FechaPendiente ?? DateTime.Now);

                        await _historialEstadosService.CrearHistorialParcelaConcesion(
                            concesion.TramiteId,
                            parcelaDestino.Id,
                            tramiteId,
                            reduccion.FechaPendiente ?? DateTime.Now);

                        // 3. Mover la concesión a la nueva parcela
                        concesion.ParcelaId = parcelaDestino.Id;
                        concesion.TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(parcelaDestino.TipoParcelaId ?? 0);

                        // 4. Log en concesión
                        concesion.InformacionAdicional +=
                            $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} " +
                            $"se trasladó la concesión ({concesion.Concesion?.ToString("D5")}) " +
                            $"de {ParcelaFormatter.ObtenerParcela(parcela.TipoParcelaId ?? 0, parcela.NroParcela, parcela.NroFila, parcela.Seccion.Nombre.ToUpper())} " +
                            $"a {ParcelaFormatter.ObtenerParcela(parcelaDestino.TipoParcelaId ?? 0, parcelaDestino.NroParcela, parcelaDestino.NroFila, parcelaDestino.Seccion.Nombre.ToUpper())} " +
                            $"por reducción (trámite {tramiteId}).";

                        // 5. Vincular concesión y trámite con nueva parcela
                        await _historialEstadosService.VincularTramiteAParcela(concesion.TramiteId, parcelaDestino.Id);
                        await _historialEstadosService.VincularTramiteAParcela(concesion.TramiteId, parcela.Id);
                        await _historialEstadosService.VincularTramiteAPersona(concesion.TramiteId, difunto.Id);
                        await _historialEstadosService.VincularTramiteAParcela(tramiteId, parcelaDestino.Id);
                        await _historialEstadosService.VincularTramiteAParcela(tramiteId, parcela.Id);
                        await _historialEstadosService.VincularTramiteAPersona(tramiteId, difunto.Id);


                        // 6. Log en parcela destino
                        parcelaDestino.InformacionAdicional +=
                            $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} " +
                            $"se recibió al difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} (reducido) " +
                            $"con la concesión ({concesion.Concesion?.ToString("D5")}) " +
                            $"proveniente de {ParcelaFormatter.ObtenerParcela(parcela.TipoParcelaId ?? 0, parcela.NroParcela, parcela.NroFila, parcela.Seccion.Nombre.ToUpper())}.";

                        // 7. Incrementar difuntos en destino
                        parcelaDestino.CantidadDifuntos += 1;

                        // Titulares NO se tocan
                    }
                    else if (destinoTieneConcesion)
                    {
                        // ── CASO: destino ya tiene concesión activa ──────────────────────
                        Models.Concesione concesionDestino = await _context.Concesiones
                            .FirstOrDefaultAsync(c => c.ParcelaId == reduccion.ParcelaDestinoId
                                                   && c.Visibilidad == true
                                                   && c.FechaFin == null)
                            ?? throw new Exception("Concesión destino no encontrada.");

                        concesionDestino.InformacionAdicional +=
                            $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} el difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} (reducido) " +
                            $"viene de {ParcelaFormatter.ObtenerParcela(parcela.TipoParcelaId ?? 0, parcela.NroParcela, parcela.NroFila, parcela.Seccion.Nombre.ToUpper())} " +
                            $"en estado {EnumHelper.GetDisplayNameByValue<EstadoDifuntoEnum>(difunto.EstadoDifuntoId ?? 0)}";
                        concesionDestino.InformacionAdicional +=
                            $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de reducción (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";

                        await _historialEstadosService.VincularTramiteAPersona(concesionDestino.TramiteId, difunto.Id);
                        await _historialEstadosService.VincularTramiteAParcela(tramiteId, parcelaDestino.Id);

                        parcelaDestino.CantidadDifuntos += 1;
                    }
                    else
                    {
                        // ── CASO: origen no quedó vacía, destino sin concesión ───────────
                        ConcesionDTO concesionNueva = new ConcesionDTO
                        {
                            ParcelaId = parcelaDestino.Id,
                            TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(parcelaDestino.TipoParcelaId ?? 0),
                            UsuarioId = usuarioId,
                            EstadoTramiteId = parcelaDestino.TipoParcelaId == (int)TipoParcelaEnum.Panteon
                                ? (int)EstadosConcesionEnum.Vigente
                                : (int)EstadosConcesionEnum.SinContrato,
                            FechaInicio = reduccion.FechaPendiente,
                            MensajeParcela = $"\n● El {reduccion.FechaPendiente?.ToString("dd/MM/yyyy")} para difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} (reducido) se genera concesión.",
                            InformacionAdicional =
                                $"\n● {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} (reducido) " +
                                $"viene de {ParcelaFormatter.ObtenerParcela(parcela.TipoParcelaId ?? 0, parcela.NroParcela, parcela.NroFila, parcela.Seccion.Nombre.ToUpper())} " +
                                $"en estado {EnumHelper.GetDisplayNameByValue<EstadoDifuntoEnum>(difunto.EstadoDifuntoId ?? 0)}"
                        };
                        await _concesionService.Add(concesionNueva);

                        parcelaDestino.CantidadDifuntos += 1;
                    }
                }

                // Nota de recordatorio (común a todos los casos)
                string descripcionNota = $"\n● El {reduccion.FechaPendiente:dd/MM/yyyy HH:mm} se finalizó una reducción en la concesión ({concesion.Concesion?.ToString("D5")}) (trámite {tramiteId})";
                string nombreNota = $"Para Program (concesión {concesion.Concesion?.ToString("D5") ?? "-----"})";
                string mensajeDifunto = $"{difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} marcar como reducido";
                string nuevoDestino = esNinguno
                    ? "Permanece en la misma parcela"
                    : $"Se trasladó a {reduccion.Destino?.ToUpper()}";

                await GenerarNotaRecordatorio(descripcionNota, nombreNota, mensajeDifunto, usuarioId, infoConcesion, nuevoDestino);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task GenerarDocumentosAsync(GeneraStrategyDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Models.Reduccione reduccion = await _context.Reducciones
             .FirstOrDefaultAsync(ct => ct.TramiteId == dto.TramiteId) ?? throw new Exception("Trámite de reducción no encontrado.");

                //actualizar los datos de los firmantes.
                if (dto.Firmantes != null)
                {
                    await _firmantes.ActualizarFirmantes(dto.Firmantes);
                }


                //busca el firmante que coincida con el firmanteId del dto.
                FirmantesDTO firmante = dto.Firmantes?.FirstOrDefault(f => f.Id == dto.FirmanteId) ?? new FirmantesDTO();

                //generar el documento de solicitud de cremacion con los datos del tramite, titulares y difunto.
                var plantilla = await _plantillaService.ObtenerPorTipoAutorizacionIdAsync(dto.TipoAutorizacionId); //busco la plantilla especifica

                string difuntosFormateados = DifuntoFormatter.FormatearDifuntos(dto.Difuntos);
                string NombreCementerio = await ModificarDestino(dto.CementerioId, dto.TramiteId, dto.NuevaParcelaId, dto.TipoTraslado);

                var nombreCompletoFirmante =
                string.IsNullOrWhiteSpace($"{firmante?.Apellido} {firmante?.Nombre}".Trim())
                    ? "___________________________________________"
                    : $"{firmante?.Apellido?.ToUpper()} {firmante?.Nombre?.ToUpper()}";

                var dniFirmante =
                    string.IsNullOrWhiteSpace(firmante?.Dni)
                        ? "___________________"
                        : StringExtensions.FormatearDni(firmante.Dni);

                var domicilioFirmante =
                    string.IsNullOrWhiteSpace(firmante?.Domicilio)
                        ? "_________________________________________________________________________________________________"
                        : firmante.Domicilio.ToUpper();

                var parentesco =
                   string.IsNullOrWhiteSpace(firmante?.Parentesco)
                       ? "___________________"
                       : firmante?.Parentesco?.ToUpper();

                var variables = new Dictionary<string, string>
                            {
                                { "Fecha", DateTime.Now.ToLongDateString() },
                                { "NombreCompletoFirmante", nombreCompletoFirmante },
                                { "DniFirmante", dniFirmante },
                                { "Parentesco", parentesco ?? "___________________"},
                                { "Parcela", ParcelaFormatter.ObtenerParcela(dto.TipoParcela, dto.NroParcela, dto.NroFila, dto.NombreSeccion.ToUpper()) },
                                { "Difuntos", difuntosFormateados },
                                { "NroConcesion", dto.NroConcesion.ToString("D5") },
                                { "AperturaNicho/Fosa", $"APERTURA DE {dto.TipoParcela.ToUpper()}" },
                                { "NuevaUbicacionTraslado", NombreCementerio },
                                { "DomicilioFirmante", domicilioFirmante },
                                { "crematorioDestino", NombreCementerio },
                                {"crematorio", NombreCementerio}
                            };

                await _documentoService.CrearDesdePlantillaAsync(
                    plantilla.PlantillaId,
                    dto.TramiteId,
                    dto.UsuarioId,
                    firmante?.PersonaId ?? null,
                    firmante?.Parentesco,
                    variables,
                    firmante?.Id ?? null
                );

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ReduccionDTO> ObtenerAsync(int tramiteId)
        {
            Models.Reduccione reduccion = await _context.Reducciones.AsNoTracking()
               .Include(t => t.Tramite)
               .Include(p => p.ParcelaDestino)
               .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de reducción no encontrado.");

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == reduccion.ConcesionId) ?? throw new Exception("Concesión no encontrada");

            ReduccionDTO dto = new ReduccionDTO();
            dto.TramiteId = reduccion.TramiteId;
            dto.TipoTramiteId = reduccion.Tramite.TipoTramiteId;
            dto.EstadoTramiteId = reduccion.Tramite.EstadoActualId;
            dto.ParcelaId = concesion.ParcelaId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;
            dto.ConcesionId = concesion.TramiteId;
            dto.CementerioId = reduccion.CementerioId ?? 0;
            dto.FechaRealizacion = reduccion.FechaPendiente;

            dto.SeccionId = reduccion.ParcelaDestino?.SeccionId ?? 0;
            dto.TipoParcelaId = reduccion.ParcelaDestino?.TipoParcelaId ?? 0;
            dto.NuevaParcelaId = reduccion.ParcelaDestinoId ?? 0;
            dto.TipoTraslado = reduccion.TipoTraslado;

            dto.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == reduccion.ConcesionId && h.FechaFin == null)
                    .Select(h => new TitularesContratoDTO
                    {
                        Id = h.Persona.Id,
                        Dni = h.Persona.Dni,
                        Nombre = h.Persona.Nombre,
                        Apellido = h.Persona.Apellido,
                        Sexo = h.Persona.Sexo,
                        Celular = h.Persona.Celular,
                        CorreoElectronico = h.Persona.Correo,
                        Domicilio = h.Persona.Domicilio
                    }).ToListAsync();

            dto.Cementerios = await _context.Cementerios
                .Select(c => new CementerioRequestDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre
                }).ToListAsync();

            //consultar el difuntos relacionados a la parcela para el tramite
            DifuntoContratoDTO difunto = await _context.ParcelaDifuntos
                .Where(p => p.DifuntoId == reduccion.DifuntoId)
                .OrderByDescending(p => p.FechaIngreso)
                .Take(1)
                .Select(p => new DifuntoContratoDTO
                {
                    Id = p.Difunto.Id,
                    DNI = p.Difunto.Dni,
                    Nombre = p.Difunto.Nombre,
                    Apellido = p.Difunto.Apellido,
                    FechaIngreso = p.FechaIngreso,
                    EstadoDifuntoId = p.Difunto.EstadoDifuntoId
                }).FirstOrDefaultAsync() ?? new DifuntoContratoDTO();

            dto.Difuntos.Add(difunto);

            return dto;
        }

        public async Task UpdateValores(ReduccionDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //Modificar la fecha de realizacion del tramite(estado pendiente)
                if (dto.FechaRealizacion.HasValue)
                {
                    Models.Reduccione reduccion = await _context.Reducciones.FirstOrDefaultAsync(c => c.TramiteId == dto.TramiteId) ?? throw new Exception("Trámite de reducción no encontrado");
                    reduccion.FechaPendiente = dto.FechaRealizacion.Value;
                    _context.Reducciones.Update(reduccion);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; 
            }
        }

        private async Task<string> ModificarDestino(int cementerioId, int tramiteId, int parcelaNuevaId, int TipoTraslado)
        {
            //modifica el destino del difunto en la parcela, para que quede registrado el nuevo cementerio destino.
            Models.Reduccione reduccion = await _context.Reducciones.FirstOrDefaultAsync(c => c.TramiteId == tramiteId) ?? throw new Exception("Trámite de reducción no encontrado");

            string DestinoNombre = string.Empty;

            if (cementerioId != 0)
            {
                Models.Cementerio cementerio = await _context.Cementerios.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cementerioId) ?? throw new Exception("Cementerio no encontrado");

                reduccion.CementerioId = cementerio.Id;
                reduccion.Destino = cementerio.Nombre.ToUpper();

                reduccion.ParcelaDestinoId = null;
                reduccion.TipoTraslado = TipoTraslado;
                DestinoNombre = "Y TRASLADO A " + cementerio.Nombre.ToUpper();
            }

            if (parcelaNuevaId != 0)
            {
                Models.Parcela parcela = await _context.Parcelas.AsNoTracking().Include(s => s.Seccion).FirstOrDefaultAsync(c => c.Id == parcelaNuevaId) ?? throw new Exception("Parcela no encontrada");

                reduccion.ParcelaDestinoId = parcela.Id;
                reduccion.Destino = ParcelaFormatter.ObtenerParcela(parcela.TipoParcelaId ?? 0, parcela.NroParcela, parcela.NroFila, parcela.Seccion.Nombre.ToUpper());
                reduccion.CementerioId = null;
                reduccion.TipoTraslado = TipoTraslado;

                DestinoNombre = "Y TRASLADO A " + reduccion.Destino;
            }

            if(TipoTraslado == (int)TipoTrasladoEnum.Ninguno)
            {
                reduccion.ParcelaDestinoId = null;
                reduccion.CementerioId = null;
                reduccion.TipoTraslado = TipoTraslado;
                reduccion.Destino = "MISMA UBICACIÓN";
                DestinoNombre = "(EL DIFUNTO QUEDA EN LA MISMA PARCELA)";
            }



            _context.Reducciones.Update(reduccion);
            await _context.SaveChangesAsync();

            return DestinoNombre;
        }

        private async Task GenerarNotaRecordatorio(string descripcionNota, string nombreNota, string mensajeDifunto, int usuarioId, string infoConcesion, string nuevoDestino)
        {
            NotaDTO nota = new NotaDTO();
            nota.Nombre = nombreNota;
            nota.TipoNotaId = (int)TipoNotaEnum.Recordatorio;
            nota.Descripcion = descripcionNota;
            nota.Color = "#F5DADE";
            nota.Visibilidad = true;
            nota.EstadoId = (int)EstadosNotaEnum.NotaPendiente;
            nota.FechaCreacion = DateTime.Now;
            nota.UsurioId = usuarioId;
            nota.FechaFinRecordatorio = DateTime.Now.AddDays(10);
            nota.Tareas = new List<TareaDTO>
                {
                    new() { Descripcion = mensajeDifunto, Estado = false },
                    new() { Descripcion = infoConcesion, Estado = false },
                    new() { Descripcion = nuevoDestino, Estado = false },  
                    new() { Descripcion = "Revisar / Modificar plano", Estado = false }

                };

            int tramiteNotaId = await _notasService.GenerarTramiteNota(usuarioId);
            await _notasService.GenerarNotaSinTransaccion(tramiteNotaId, nota);
        }
    }
}
