using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.Tramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.Cremacion;
using CemSys3.Enumerables;
using CemSys3.Helpers;
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
    public class CremacionStrategy : ITramiteStrategy,
    ITramiteCreateStrategy<CremacionDTO>, IComplementoTramite<CremacionDTO>
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

        public CremacionStrategy(
            IPlantillaTramite plantillaService,
            IDocumentoTramiteService documentoService,
            IPersona personaService,
            AppDbContext context,
            ITramite tramiteService,
            IHistorialEstados historialEstadosService,
            ITareaPlantilla tareaPlantilla, 
            INotas notasService,
            IFirmantes firmantes)
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
            catch (Exception) {
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
                    TipoTramiteId = (int)TipoTramiteEnum.Cremacion,
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
                Models.Cremacione cremacion = new Models.Cremacione
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
                await _context.Cremaciones.AddAsync(cremacion);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                //5 - crear tareas para el tramite
                await _tareaPlantilla.CrearTareasPorTramite(tramiteId, (int)TipoTramiteEnum.Cremacion);

               var titulares = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == cremacion.ConcesionId && h.FechaFin == null)
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
            Models.Cremacione cremacion = await _context.Cremaciones
               .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de cremación no encontrado.");

            Models.Concesione concesion = await _context.Concesiones
                   .FirstOrDefaultAsync(c => c.TramiteId == cremacion.ConcesionId) ?? throw new Exception("Concesion no encontrada.");

            Models.Tramite tramite = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == tramiteId) ?? throw new Exception("Trámite no encontrado.");

            if(tramite.EstadoActualId != (int)EstadosTramiteEnum.Pendiente)
            {
                throw new Exception("El trámite no se encuentra en estado pendiente, no puede ser finalizado.");
            }

            if(cremacion.FechaPendiente == null)
            {
                throw new Exception("Debe asignar una fecha para el trámite antes de finalizar");

            }

            if(cremacion.FechaPendiente < cremacion.FechaCreacion)
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
                    Fecha = cremacion.FechaPendiente ?? DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Finalizado
                };
                await _historialEstadosService.Add(historial);

                cremacion.FechaFinalizacion = cremacion.FechaPendiente;
                tramite.FechaFinalizacion = cremacion.FechaPendiente;


                //vincular concesion con parcela 
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                //consultar el difunto relacionado a la parcela para el tramite
                Models.Persona difunto = await _context.Personas.FirstOrDefaultAsync(p => p.Id == cremacion.DifuntoId) ?? throw new Exception("Difunto no encontrado.");

                //2- actualizar el estado del difunto a cremado.
                //log en firmantes.
                difunto.EstadoDifuntoId = (int)EstadoDifuntoEnum.Cremado;

                //3- vicular firmantes y difuntos al tramite
                List<FirmantesDTO> firmantes = await _firmantes.GetAllByTramite(tramite.Id);
                foreach (var firmante in firmantes)
                {
                    await _historialEstadosService.VincularTramiteAPersona(tramiteId, firmante.PersonaId);

                    //log en firmantes
                    var persona = await _personaService.Get(firmante.PersonaId);
                    persona.InformacionAdicional += $"\n● El {cremacion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de cremación (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";
                    int personaId = await _personaService.Update(persona);
                }

                //log en difunto
                await _historialEstadosService.VincularTramiteAPersona(tramiteId, difunto.Id);
                difunto.InformacionAdicional += $"\n● El {cremacion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de cremación (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";

                //4- Log en concesion, parcela.
                concesion.InformacionAdicional += $"\n● El {cremacion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de cremación (trámite {tramiteId})";

                Models.Parcela parcela = await _context.Parcelas.FirstOrDefaultAsync(p => p.Id == concesion.ParcelaId) ?? throw new Exception("Parcela no encontrada.");
                parcela.InformacionAdicional += $"\n● El {cremacion.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de cremación (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";

                //5- Quitar difunto en la parcela
                Models.ParcelaDifunto parcelaDifunto = await _context.ParcelaDifuntos
                    .FirstOrDefaultAsync(pd => pd.ParcelaId == parcela.Id && pd.DifuntoId == difunto.Id && pd.FechaRetiro == null) ?? throw new Exception("Registro de parcela-difunto no encontrado.");

                parcelaDifunto.FechaRetiro = cremacion.FechaPendiente;
                parcelaDifunto.TramiteRetiroId = tramiteId;

                parcela.CantidadDifuntos -= 1;

                string infoConcesion = "Revisar el libro de concesión";

                //5.1 pasos por si queda la parcela vacia.
                if(parcela.CantidadDifuntos == 0)
                {
                    // cancelar la concesion.
                    concesion.FechaFin = cremacion.FechaFinalizacion;
                    concesion.TramiteRetiroId = tramiteId;
                    Models.Tramite tramiteConcesion = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == concesion.TramiteId) ?? throw new Exception("Trámite no encontrado.");


                    tramiteConcesion.EstadoActualId = (int)EstadosTramiteEnum.Caducado;
                    concesion.Vencimiento = null;
                    tramiteConcesion.FechaFinalizacion = cremacion.FechaFinalizacion;
                    HistorialEstadosDTO historialConcesion = new HistorialEstadosDTO
                    {
                        Fecha = cremacion.FechaPendiente ?? DateTime.Now,
                        TramiteId = tramiteConcesion.Id,
                        EstadoTramiteId = (int)EstadosTramiteEnum.Caducado
                    };
                    await _historialEstadosService.Add(historialConcesion);

                    concesion.InformacionAdicional += $"\n● La concesión ({concesion.Concesion?.ToString("D5")}) ha sido cancelada/caducada automáticamente por no tener más difuntos asociados.";
                    infoConcesion = "La concesión debe ser cancelada/caducada por no tener más difuntos asociados.";

                    // 1. Titulares actuales activos
                    var titularesActuales = await _context.HistorialTitularesConcesiones
                        .Where(p => p.ConcesionId == concesion.TramiteId && p.FechaFin == null)
                        .ToListAsync();

                    // 3. Cerrar titulares
                    foreach (var titularActual in titularesActuales)
                    {
                        titularActual.FechaFin = cremacion.FechaPendiente;   
                    }

                    await _historialEstadosService.CerrarHistorialParcelaConcesion(
                        concesion.TramiteId,
                        cremacion.FechaPendiente ?? DateTime.Now);
                }

                //6 - generar la nota de recordatorio.
                string descripcionNota = $"\n● El {cremacion.FechaPendiente:dd/MM/yyyy HH:mm} se finalizó una cremación en la concesión ({concesion.Concesion?.ToString("D5")}) (trámite {tramiteId})";
                string nombreNota = $"Para Program (concesión {concesion.Concesion?.ToString("D5") ?? "-----"})";
                string mensajeDifunto = $"{difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} marcar como cremado";


                await GenerarNotaRecordatorio(descripcionNota, nombreNota, mensajeDifunto, usuarioId, infoConcesion);

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
                //actualizar los datos de los firmantes.
                if(dto.Firmantes != null)
                {
                    await _firmantes.ActualizarFirmantes(dto.Firmantes);
                }


                //busca el firmante que coincida con el firmanteId del dto.
                FirmantesDTO firmante = dto.Firmantes?.FirstOrDefault(f => f.Id == dto.FirmanteId) ?? new FirmantesDTO();

                //generar el documento de solicitud de cremacion con los datos del tramite, titulares y difunto.
                var plantilla = await _plantillaService.ObtenerPorTipoAutorizacionIdAsync(dto.TipoAutorizacionId); //busco la plantilla especifica

                string difuntosFormateados = DifuntoFormatter.FormatearDifuntos(dto.Difuntos);
                string NombreCementerio = await ModificarDestino(dto.CementerioId, dto.TramiteId);

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
                                { "crematorio", NombreCementerio },
                                { "DomicilioFirmante", domicilioFirmante },
                                { "crematorioDestino", NombreCementerio }
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
            catch (Exception) {
                await transaction.RollbackAsync(); 
                throw;
            }

            
        }

        
        public async Task<CremacionDTO> ObtenerAsync(int tramiteId)
        {
            Models.Cremacione cremacion = await _context.Cremaciones.AsNoTracking()
               .Include(t => t.Tramite)
               .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de cremación no encontrado.");

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == cremacion.ConcesionId) ?? throw new Exception("Concesión no encontrada");

            CremacionDTO dto = new CremacionDTO();
            dto.TramiteId = cremacion.TramiteId;
            dto.TipoTramiteId = cremacion.Tramite.TipoTramiteId;
            dto.EstadoTramiteId = cremacion.Tramite.EstadoActualId;
            dto.ParcelaId = cremacion.ParcelaOrigenId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;
            dto.ConcesionId = concesion.TramiteId;
            dto.CementerioId = cremacion.CementerioId ?? 0;
            dto.FechaRealizacion = cremacion.FechaPendiente;
            dto.InfoAdicional = cremacion.InfoAdicional;

            dto.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == cremacion.ConcesionId && h.FechaFin == null)
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
            dto.Difuntos = await _context.ParcelaDifuntos
                .Where(p => p.DifuntoId == cremacion.DifuntoId)
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
                }).ToListAsync();

            return dto;
        }

        public async Task UpdateValores(CremacionDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //Modificar la fecha de realizacion del tramite(estado pendiente)
                if (dto.FechaRealizacion.HasValue)
                {
                    Models.Cremacione cremacion = await _context.Cremaciones.FirstOrDefaultAsync(c => c.TramiteId == dto.TramiteId) ?? throw new Exception("Trámite de cremación no encontrado");
                    cremacion.FechaPendiente = dto.FechaRealizacion.Value;
                    _context.Cremaciones.Update(cremacion);
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

        private async Task<string> ModificarDestino(int cementerioId, int tramiteId)
        {
            //modifica el destino del difunto en la parcela, para que quede registrado el nuevo cementerio destino.
            Models.Cementerio cementerio = await _context.Cementerios.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cementerioId) ?? throw new Exception("Cementerio no encontrado");

            Models.Cremacione cremacion = await _context.Cremaciones.FirstOrDefaultAsync(c => c.TramiteId == tramiteId) ?? throw new Exception("Trámite de cremación no encontrado");

            cremacion.CementerioId = cementerio.Id;
            cremacion.Destino = cementerio.Nombre.ToUpper();

            _context.Cremaciones.Update(cremacion);
            await _context.SaveChangesAsync();

            return cementerio.Nombre.ToUpper();
        }

        private async Task GenerarNotaRecordatorio(string descripcionNota, string nombreNota, string mensajeDifunto, int usuarioId, string infoConcesion)
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
                    new() { Descripcion = "Revisar / Modificar plano", Estado = false }

                };

            int tramiteNotaId = await _notasService.GenerarTramiteNota(usuarioId);
            await _notasService.GenerarNotaSinTransaccion(tramiteNotaId, nota);
        }

    }
}
