using CemSys3.Business.HistorialEstadoService;
using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.Tramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.PermisoRefaccion;
using CemSys3.Enumerables;
using CemSys3.Helpers;
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
    public class PermisoRefaccionStrategy : ITramiteStrategy,
    ITramiteCreateStrategy<PermisoRefaccionDTO>, IComplementoTramite<PermisoRefaccionDTO>
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


        public PermisoRefaccionStrategy(
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
                    TipoTramiteId = (int)TipoTramiteEnum.PermisoRefaccion,
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
                Models.PermisosRefaccione permisosRefaccione = new Models.PermisosRefaccione
                {
                    TramiteId = tramiteId,
                    FechaCreacion = DateTime.Now,
                    ParcelaId = concesion.ParcelaId,
                    Visibilidad = true,
                    UsuarioId = dto.UsuarioId,
                    ConcesionId = dto.TramiteConcesionId
                };
                await _context.PermisosRefacciones.AddAsync(permisosRefaccione);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                //5 - crear tareas para el tramite
                await _tareaPlantilla.CrearTareasPorTramite(tramiteId, (int)TipoTramiteEnum.PermisoRefaccion);

                var titulares = await _context.HistorialTitularesConcesiones
                     .Where(h => h.ConcesionId == permisosRefaccione.ConcesionId && h.FechaFin == null)
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
            Models.PermisosRefaccione permisosRefaccione = await _context.PermisosRefacciones
               .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de permiso de refacción no encontrado.");

            Models.Concesione concesion = await _context.Concesiones
                   .FirstOrDefaultAsync(c => c.TramiteId == permisosRefaccione.ConcesionId) ?? throw new Exception("Concesion no encontrada.");

            Models.Tramite tramite = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == tramiteId) ?? throw new Exception("Trámite no encontrado.");

            if (tramite.EstadoActualId != (int)EstadosTramiteEnum.Pendiente)
            {
                throw new Exception("El trámite no se encuentra en estado pendiente, no puede ser finalizado.");
            }

            if (permisosRefaccione.FechaPendiente == null)
            {
                throw new Exception("Debe asignar una fecha para el trámite antes de finalizar");

            }

            if (permisosRefaccione.FechaPendiente < permisosRefaccione.FechaCreacion)
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
                    Fecha = permisosRefaccione.FechaPendiente ?? DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Finalizado
                };
                await _historialEstadosService.Add(historial);

                permisosRefaccione.FechaFinalizacion = permisosRefaccione.FechaPendiente;
                tramite.FechaFinalizacion = permisosRefaccione.FechaPendiente;


                //vincular concesion con parcela 
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                //3- vicular firmantes y difuntos al tramite
                List<FirmantesDTO> firmantes = await _firmantes.GetAllByTramite(tramite.Id);
                foreach (var firmante in firmantes)
                {
                    await _historialEstadosService.VincularTramiteAPersona(tramiteId, firmante.PersonaId);

                    //log en firmantes
                    var persona = await _personaService.Get(firmante.PersonaId);
                    persona.InformacionAdicional += $"\n● El {permisosRefaccione.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de permiso de refacción (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";
                    int personaId = await _personaService.Update(persona);
                }

                //4- Log en concesion, parcela.
                concesion.InformacionAdicional += $"\n● El {permisosRefaccione.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de permiso de refacción (trámite {tramiteId})";

                Models.Parcela parcela = await _context.Parcelas.FirstOrDefaultAsync(p => p.Id == concesion.ParcelaId) ?? throw new Exception("Parcela no encontrada.");
                parcela.InformacionAdicional += $"\n● El {permisosRefaccione.FechaPendiente?.ToString("dd/MM/yyyy HH:mm")} se finalizó el trámite de permiso de refacción (trámite {tramiteId}) en concesión ({concesion.Concesion?.ToString("D5")})";


                //3 generar la nota de recordatorio.
                string descripcionNota = $"\n● El {DateTime.Now:dd/MM/yyyy} se finalizó un permiso de refacción (trámite {tramiteId})";
                string nombreNota = $"Para Program (concesión {concesion.Concesion?.ToString("D5") ?? "-----"})";
                string titularNota = $"\n● El {DateTime.Now:dd/MM/yyyy} se finalizó un permiso de refacción (trámite {tramiteId})";


                await GenerarNotaRecordatorio(descripcionNota, nombreNota, titularNota, usuarioId);


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
                Models.PermisosRefaccione permisosRefaccione = await _context.PermisosRefacciones
             .FirstOrDefaultAsync(ct => ct.TramiteId == dto.TramiteId) ?? throw new Exception("Trámite de permiso de refacción no encontrado.");

                //actualizar los datos de los firmantes.
                if (dto.Firmantes != null)
                {
                    await _firmantes.ActualizarFirmantes(dto.Firmantes);
                }


                //busca el firmante que coincida con el firmanteId del dto.
                FirmantesDTO firmante = dto.Firmantes?.FirstOrDefault(f => f.Id == dto.FirmanteId) ?? new FirmantesDTO();

                //generar el documento de solicitud de cremacion con los datos del tramite, titulares y difunto.
                var plantilla = await _plantillaService.ObtenerPorTipoAutorizacionIdAsync(dto.TipoAutorizacionId); //busco la plantilla especifica

                var nombreCompletoFirmante =
                string.IsNullOrWhiteSpace($"{firmante?.Apellido} {firmante?.Nombre}".Trim())
                    ? "___________________________________________"
                    : $"{firmante?.Apellido?.ToUpper()} {firmante?.Nombre?.ToUpper()}";

                var dniFirmante =
                    string.IsNullOrWhiteSpace(firmante?.Dni)
                        ? "___________________"
                        : StringExtensions.FormatearDni(firmante.Dni);

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
                                { "NroConcesion", dto.NroConcesion.ToString("D5") },
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

        public async Task<PermisoRefaccionDTO> ObtenerAsync(int tramiteId)
        {
            Models.PermisosRefaccione permisosRefaccione = await _context.PermisosRefacciones.AsNoTracking()
                .Include(t => t.Tramite)
                .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de permiso de refacción no encontrado.");

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == permisosRefaccione.ConcesionId) ?? throw new Exception("Concesión no encontrada");

            PermisoRefaccionDTO dto = new PermisoRefaccionDTO();
            dto.TramiteId = permisosRefaccione.TramiteId;
            dto.TipoTramiteId = permisosRefaccione.Tramite.TipoTramiteId;
            dto.EstadoTramiteId = permisosRefaccione.Tramite.EstadoActualId;
            dto.ParcelaId = permisosRefaccione.ParcelaId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;
            dto.ConcesionId = concesion.TramiteId;
            dto.FechaRealizacion = permisosRefaccione.FechaPendiente;

            dto.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == permisosRefaccione.ConcesionId && h.FechaFin == null)
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

            return dto;
        }

        public async Task UpdateValores(PermisoRefaccionDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //Modificar la fecha de realizacion del tramite(estado pendiente)
                if (dto.FechaRealizacion.HasValue)
                {
                    Models.PermisosRefaccione permisosRefaccione = await _context.PermisosRefacciones.FirstOrDefaultAsync(c => c.TramiteId == dto.TramiteId) ?? throw new Exception("Trámite de permiso de refacción no encontrado");
                    permisosRefaccione.FechaPendiente = dto.FechaRealizacion.Value;
                    _context.PermisosRefacciones.Update(permisosRefaccione);
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

        private async Task GenerarNotaRecordatorio(string descripcionNota, string nombreNota, string titularNota, int usuarioId)
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
                    new() { Descripcion = titularNota, Estado = false },
                };

            int tramiteNotaId = await _notasService.GenerarTramiteNota(usuarioId);
            await _notasService.GenerarNotaSinTransaccion(tramiteNotaId, nota);
        }

    }
}
