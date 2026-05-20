using CemSys3.Business.Concesion;
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
using CemSys3.DTOs.TramitesConcesion.Traslado;
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
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.TramiteConcesion
{
    public class TrasladoStrategy : ITramiteStrategy,
    ITramiteCreateStrategy<TrasladoDTO>, IComplementoTramite<TrasladoDTO>
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

        public TrasladoStrategy(
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
                    TipoTramiteId = (int)TipoTramiteEnum.Traslado,
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

                //3- registrar el tramite de cremacion
                Models.Traslado traslado = new Models.Traslado
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
                await _context.Traslados.AddAsync(traslado);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                //5 - crear tareas para el tramite
                await _tareaPlantilla.CrearTareasPorTramite(tramiteId, (int)TipoTramiteEnum.Traslado);

                var titulares = await _context.HistorialTitularesConcesiones
                     .Where(h => h.ConcesionId == traslado.ConcesionId && h.FechaFin == null)
                     .Select(h => new TitularesContratoDTO
                     {
                         Id = h.Persona.Id,
                     }).ToListAsync();

                //6 - crea el firmante titular
                foreach (var titular in titulares)
                {
                    await _firmantes.Add(tramiteId, titular.Id.Value, "TITULAR", true);
                }

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
            Models.Traslado traslado = await _context.Traslados
              .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de traslado no encontrado.");

            Models.Concesione concesion = await _context.Concesiones
                   .FirstOrDefaultAsync(c => c.TramiteId == traslado.ConcesionId) ?? throw new Exception("Concesion no encontrada.");

            Models.Tramite tramite = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == tramiteId) ?? throw new Exception("Trámite no encontrado.");

            if (tramite.EstadoActualId != (int)EstadosTramiteEnum.Pendiente)
            {
                throw new Exception("El trámite no se encuentra en estado pendiente, no puede ser finalizado.");
            }

            if (traslado.FechaPendiente == null)
            {
                throw new Exception("Debe asignar una fecha para el trámite antes de finalizar");

            }

            if (traslado.FechaPendiente < traslado.FechaCreacion)
            {
                throw new Exception("La fecha de realización no puede ser menor a la fecha de creación del trámite.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //1- actualizar estado del tramite a finalizado
                tramite.EstadoActualId = (int)EstadosTramiteEnum.Finalizado;

                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = traslado.FechaPendiente ?? DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Finalizado
                };
                await _historialEstadosService.Add(historial);

                traslado.FechaFinalizacion = traslado.FechaPendiente;
                tramite.FechaFinalizacion = traslado.FechaPendiente;


                //vincular concesion con parcela 
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                //consultar el difunto relacionado a la parcela para el tramite
                Models.Persona difunto = await _context.Personas.FirstOrDefaultAsync(p => p.Id == traslado.DifuntoId) ?? throw new Exception("Difunto no encontrado.");

                

                //3- vicular firmantes y difuntos al tramite
                List<FirmantesDTO> firmantes = await _firmantes.GetAllByTramite(tramite.Id);
                foreach (var firmante in firmantes)
                {
                    await _historialEstadosService.VincularTramiteAPersona(tramiteId, firmante.PersonaId);

                    //log en firmantes
                    var persona = await _personaService.Get(firmante.PersonaId);
                    persona.InformacionAdicional += $"\n● El {traslado.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de traslado (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";
                    int personaId = await _personaService.Update(persona);
                }

                //log en difunto
                await _historialEstadosService.VincularTramiteAPersona(tramiteId, difunto.Id);
                difunto.InformacionAdicional += $"\n● El {traslado.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de traslado (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";

                //4- Log en concesion, parcela.
                concesion.InformacionAdicional += $"\n● El {traslado.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de traslado (trámite {tramiteId})";

                Models.Parcela parcela = await _context.Parcelas.FirstOrDefaultAsync(p => p.Id == concesion.ParcelaId) ?? throw new Exception("Parcela no encontrada.");
                parcela.InformacionAdicional += $"\n● El {traslado.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de traslado (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";

                //5- Quitar difunto en la parcela actual
                Models.ParcelaDifunto parcelaDifunto = await _context.ParcelaDifuntos
                    .FirstOrDefaultAsync(pd => pd.ParcelaId == parcela.Id && pd.DifuntoId == difunto.Id && pd.FechaRetiro == null) ?? throw new Exception("Registro de parcela-difunto no encontrado.");

                parcelaDifunto.FechaRetiro = traslado.FechaPendiente;
                parcelaDifunto.TramiteRetiroId = tramiteId;

                parcela.CantidadDifuntos -= 1;

                string infoConcesion = "Revisar el libro de concesión";
                //5.1 pasos por si queda la parcela vacia.
                if (parcela.CantidadDifuntos == 0)
                {
                    // cancelar la concesion.
                    concesion.FechaFin = traslado.FechaFinalizacion;
                    Models.Tramite tramiteConcesion = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == concesion.TramiteId) ?? throw new Exception("Trámite no encontrado.");


                    tramiteConcesion.EstadoActualId = (int)EstadosTramiteEnum.Caducado;
                    tramiteConcesion.FechaFinalizacion = traslado.FechaFinalizacion;
                    HistorialEstadosDTO historialConcesion = new HistorialEstadosDTO
                    {
                        Fecha = traslado.FechaPendiente ?? DateTime.Now,
                        TramiteId = tramiteConcesion.Id,
                        EstadoTramiteId = (int)EstadosTramiteEnum.Caducado
                    };
                    await _historialEstadosService.Add(historialConcesion);

                    concesion.InformacionAdicional += $"\n● La concesión ({concesion.Concesion?.ToString("D5")}) ha sido cancelada/caducada automáticamente por no tener más difuntos asociados.";
                    infoConcesion = "La concesión debe ser cancelada/caducada por no tener más difuntos asociados.";
                }

                


                //si el difunto se traslado a otra parcela dentro del cementerio
                if(traslado.TipoTraslado == (int)TipoTrasladoEnum.Interno)
                {
                    //7 - generar nuevo registro de parcela-difunto con la nueva parcela destino.
                    Models.ParcelaDifunto nuevoParcelaDifunto = new Models.ParcelaDifunto
                    {
                        DifuntoId = difunto.Id,
                        ParcelaId = traslado.ParcelaDestinoId ?? 0,
                        FechaIngreso = traslado.FechaPendiente ?? DateTime.Now,
                        TramiteIngresoId = tramiteId
                    };
                    await _context.ParcelaDifuntos.AddAsync(nuevoParcelaDifunto);
                    //8 - actualizar la cantidad de difuntos en la parcela destino.
                    Models.Parcela parcelaDestino = await _context.Parcelas.Include(s=> s.Seccion).FirstOrDefaultAsync(p => p.Id == traslado.ParcelaDestinoId) ?? throw new Exception("Parcela destino no encontrada.");
                    parcelaDestino.CantidadDifuntos += 1;



                    //se inicia el contrato de concesion en estado "Sin Contrato" solo si es nicho o fosa

                    bool existeConcesion = await _context.Concesiones
                        .AnyAsync(c => c.ParcelaId == traslado.ParcelaDestinoId && c.Visibilidad == true && c.FechaFin == null);



                    if (!existeConcesion && parcelaDestino.TipoParcelaId != (int)TipoParcelaEnum.Panteon)
                    {
                        ConcesionDTO concesionNueva = new ConcesionDTO();
                        concesionNueva.ParcelaId = parcelaDestino.Id;
                        concesionNueva.TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(parcelaDestino.TipoParcelaId ?? 0);
                        concesionNueva.UsuarioId = usuarioId;
                        concesionNueva.EstadoTramiteId = (int)EstadosConcesionEnum.SinContrato;
                        concesionNueva.MensajeParcela = $"\n● El {traslado.FechaPendiente?.ToString("dd/MM/yyyy")} para difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} se genera concesión en estado '{EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>((int)EstadosConcesionEnum.SinContrato)}'.";
                        concesionNueva.InformacionAdicional = $"\n● El {traslado.FechaPendiente?.ToString("dd/MM/yyyy")} en {ParcelaFormatter.ObtenerParcela(parcelaDestino.TipoParcelaId ?? 0, parcelaDestino.NroParcela, parcelaDestino.NroFila, parcelaDestino.Seccion.Nombre.ToUpper())} se genera concesión en estado '{EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>((int)EstadosConcesionEnum.SinContrato)}'.";
                        GenericResultDTO resultadoConcesion = await _concesionService.Add(concesionNueva);
                    }

                    if (!existeConcesion && parcelaDestino.TipoParcelaId == (int)TipoParcelaEnum.Panteon)
                    {
                        //se crea la concesion para cada panteon registrado, con estado vigente
                        ConcesionDTO concesionNueva = new ConcesionDTO();
                        concesionNueva.Visibilidad = true;
                        concesionNueva.ParcelaId = parcelaDestino.Id;
                        concesionNueva.TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(parcelaDestino.TipoParcelaId ?? 0);
                        concesionNueva.UsuarioId = usuarioId;
                        concesionNueva.EstadoTramiteId = (int)EstadosConcesionEnum.Vigente;
                        GenericResultDTO resultadoConcesion = await _concesionService.Add(concesionNueva);
                    }

                    Models.Concesione concesionBD = await _context.Concesiones
                       .FirstOrDefaultAsync(c => c.ParcelaId == parcelaDestino.Id && c.FechaFin == null) ?? throw new Exception("Concesion no encontrada.");

                    concesionBD.InformacionAdicional += $"\n● El {traslado.FechaPendiente?.ToString("dd/MM/yyyy")} se registra el ingreso del difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} en estado {EnumHelper.GetDisplayNameByValue<EstadoDifuntoEnum>(difunto.EstadoDifuntoId ?? 0)}.";
                    concesionBD.InformacionAdicional += $"\n● El {traslado.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de traslado (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";

                    await _historialEstadosService.VincularTramiteAPersona(concesionBD.TramiteId, difunto.Id);
                    await _historialEstadosService.VincularTramiteAParcela(traslado.TramiteId, parcelaDestino.Id);
                }


                // 6 - generar la nota de recordatorio.
                string descripcionNota = $"\n● El {traslado.FechaPendiente:dd/MM/yyyy HH:mm} se finalizó un traslado en la concesión ({concesion.Concesion?.ToString("D5")}) (trámite {tramiteId})";
                string nombreNota = $"Para Program (concesión {concesion.Concesion?.ToString("D5") ?? "-----"})";
                string mensajeDifunto = $"{difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} marcar como trasladado";
                string nuevoDestino = $"Se traslado a {traslado.Destino?.ToUpper()}";

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
                Models.Traslado traslado = await _context.Traslados
             .FirstOrDefaultAsync(ct => ct.TramiteId == dto.TramiteId) ?? throw new Exception("Trámite de traslado no encontrado.");

                //actualizar los datos de los firmantes.
                if (dto.Firmantes != null)
                {
                    await _firmantes.ActualizarFirmantes(dto.Firmantes);
                }

                if(dto.NuevaParcelaId != 0 && dto.NuevaParcelaId == traslado.ParcelaOrigenId)
                    throw new Exception("La parcela de destino no puede ser la misma que la parcela de origen.");


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

        public async Task<TrasladoDTO> ObtenerAsync(int tramiteId)
        {
            Models.Traslado traslado = await _context.Traslados.AsNoTracking()
               .Include(t => t.Tramite)
               .Include(p => p.ParcelaDestino)
               .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de traslado no encontrado.");

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == traslado.ConcesionId) ?? throw new Exception("Concesión no encontrada");

            TrasladoDTO dto = new TrasladoDTO();
            dto.TramiteId = traslado.TramiteId;
            dto.TipoTramiteId = traslado.Tramite.TipoTramiteId;
            dto.EstadoTramiteId = traslado.Tramite.EstadoActualId;
            dto.ParcelaId = traslado.ParcelaOrigenId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;
            dto.ConcesionId = concesion.TramiteId;
            dto.CementerioId = traslado.CementerioId ?? 0;
            dto.FechaRealizacion = traslado.FechaPendiente;

            dto.SeccionId = traslado.ParcelaDestino?.SeccionId ?? 0;
            dto.TipoParcelaId = traslado.ParcelaDestino?.TipoParcelaId ?? 0;
            dto.NuevaParcelaId = traslado.ParcelaDestinoId ?? 0;
            dto.TipoTraslado = traslado.TipoTraslado;

            dto.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == traslado.ConcesionId && h.FechaFin == null)
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
                .Where(p => p.DifuntoId == traslado.DifuntoId)
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

        public async Task UpdateValores(TrasladoDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //Modificar la fecha de realizacion del tramite(estado pendiente)
                if (dto.FechaRealizacion.HasValue)
                {
                    Models.Traslado traslado = await _context.Traslados.FirstOrDefaultAsync(c => c.TramiteId == dto.TramiteId) ?? throw new Exception("Trámite de traslado no encontrado");
                    traslado.FechaPendiente = dto.FechaRealizacion.Value;
                    _context.Traslados.Update(traslado);
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
            Models.Traslado traslado = await _context.Traslados.FirstOrDefaultAsync(c => c.TramiteId == tramiteId) ?? throw new Exception("Trámite de traslado no encontrado");

            string DestinoNombre = string.Empty;

            if (cementerioId != 0)
            {
                Models.Cementerio cementerio = await _context.Cementerios.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cementerioId) ?? throw new Exception("Cementerio no encontrado");

                traslado.CementerioId = cementerio.Id;
                traslado.Destino = cementerio.Nombre.ToUpper();

                traslado.ParcelaDestinoId = null;
                traslado.TipoTraslado = TipoTraslado;
                DestinoNombre = cementerio.Nombre.ToUpper();
            }

            if (parcelaNuevaId != 0)
            {
                Models.Parcela parcela = await _context.Parcelas.AsNoTracking().Include(s=> s.Seccion).FirstOrDefaultAsync(c => c.Id == parcelaNuevaId) ?? throw new Exception("Parcela no encontrada");

                traslado.ParcelaDestinoId = parcela.Id;
                traslado.Destino = ParcelaFormatter.ObtenerParcela(parcela.TipoParcelaId ?? 0, parcela.NroParcela, parcela.NroFila, parcela.Seccion.Nombre.ToUpper());
                traslado.CementerioId = null;
                traslado.TipoTraslado = TipoTraslado;

                DestinoNombre = traslado.Destino;
            }



            _context.Traslados.Update(traslado);
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
                };

            int tramiteNotaId = await _notasService.GenerarTramiteNota(usuarioId);
            await _notasService.GenerarNotaSinTransaccion(tramiteNotaId, nota);
        }

    }
}
