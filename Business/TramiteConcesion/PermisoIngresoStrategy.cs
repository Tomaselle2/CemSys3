using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.Tramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.PermisoIngreso;
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
    public class PermisoIngresoStrategy : ITramiteStrategy,
    ITramiteCreateStrategy<PermisoIngresoDTO>
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


        public PermisoIngresoStrategy(
            IPlantillaTramite plantillaService,
            IDocumentoTramiteService documentoService,
            IPersona personaService,
            AppDbContext context,
            ITramite tramiteService, IFirmantes firmantes,
            IHistorialEstados historialEstadosService,
            ITareaPlantilla tareaPlantilla, INotas notasService)
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
            throw new NotImplementedException();
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
                    TipoTramiteId = (int)TipoTramiteEnum.PermisoIngreso,
                    UsuarioId = dto.UsuarioId,
                    EstadoActualId = (int)EstadosCambioTitularEnum.Iniciado
                };

                int tramiteId = await _tramiteService.Add(tramite);
                await _context.SaveChangesAsync(); //guardar cambios antes de continuar

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = tramite.FechaCreacion,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosCambioTitularEnum.Iniciado
                };
                await _historialEstadosService.Add(historial);

                //3- registrar el tramite de cambio de titularidad
                Models.PermisosIngreso permisoIngreso = new Models.PermisosIngreso
                {
                    TramiteId = tramiteId,
                    ParcelaId = concesion.ParcelaId,
                    UsuarioId = dto.UsuarioId,
                    FechaCreacion = DateTime.Now,
                    NombreFallecido = string.Empty,
                    Visibilidad = true,
                    ConcesionId = dto.TramiteConcesionId
                };
                await _context.PermisosIngresos.AddAsync(permisoIngreso);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);


                await _tareaPlantilla.CrearTareasPorTramite(tramiteId, (int)TipoTramiteEnum.PermisoIngreso);

                var titulares = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == permisoIngreso.ConcesionId && h.FechaFin == null)
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
            Models.PermisosIngreso permisosIngreso = await _context.PermisosIngresos
                 .Include(t => t.Tramite)
                 .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de permiso ingreso no encontrado.");

            Models.Concesione concesion = await _context.Concesiones
                   .FirstOrDefaultAsync(c => c.TramiteId == permisosIngreso.ConcesionId) ?? throw new Exception("Concesion no encontrada.");

            Models.Tramite tramite = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == tramiteId) ?? throw new Exception("Trámite no encontrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                permisosIngreso.FechaFinalizacion = DateTime.Now;
                tramite.FechaFinalizacion = DateTime.Now;

                List<FirmantesDTO> firmantes = await _firmantes.GetAllByTramite(tramite.Id);

                List<PersonaDTO> titularesNuevos = new();

                foreach (var titular in firmantes)
                {
                    titularesNuevos.Add(await _personaService.Get(titular.PersonaId));
                    await _historialEstadosService.VincularTramiteAPersona(tramiteId, titular.PersonaId);
                }


                //2- actualizar estado del tramite a finalizado
                tramite.EstadoActualId = (int)EstadosTramiteEnum.Finalizado;

                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Finalizado
                };
                await _historialEstadosService.Add(historial);

                concesion.InformacionAdicional += $"\n● El {DateTime.Now:dd/MM/yyyy} se realizó un permiso de ingreso para el difunto {permisosIngreso.NombreFallecido?.ToUpper()}";
                //3 generar la nota de recordatorio.
                string descripcionNota = $"\n● El {DateTime.Now:dd/MM/yyyy} se realizó un permiso de ingreso (trámite {tramiteId})";
                string nombreNota = $"Para Program (concesión {concesion.Concesion?.ToString("D5") ?? "-----"})";
                string titularNota = $"Agregar al difunto {permisosIngreso.NombreFallecido?.ToUpper()}";


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
            Models.PermisosIngreso permisoIngreso = await _context.PermisosIngresos
               .Include(t => t.Tramite)
               .FirstOrDefaultAsync(ct => ct.TramiteId == dto.TramiteId) ?? throw new Exception("Trámite de permiso de ingreso no encontrado.");

            try
            {
                //actualizar los datos de los firmantes.
                if (dto.Firmantes != null)
                {
                    await _firmantes.ActualizarFirmantes(dto.Firmantes);
                }

                permisoIngreso.NombreFallecido = dto.NombreNuevoDifunto.ToUpper();

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
                                { "NombreNuevoDifunto", dto.NombreNuevoDifunto.ToUpper() },
                                { "NroConcesion", dto.NroConcesion.ToString("D5") },
                                { "AperturaNicho/Fosa", $"APERTURA DE {dto.TipoParcela.ToUpper()}" },
                                { "DomicilioFirmante", domicilioFirmante },
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

        public async Task<PermisoIngresoDTO> ObtenerAsync(int tramiteId)
        {
            Models.PermisosIngreso permisoIngreso = await _context.PermisosIngresos.AsNoTracking()
               .Include(t => t.Tramite)
               .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de permiso de ingreso no encontrado.");

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == permisoIngreso.ConcesionId) ?? throw new Exception("Concesion no encontrada para inicar el trámite.");

            PermisoIngresoDTO dto = new PermisoIngresoDTO();
            dto.TramiteId = permisoIngreso.TramiteId;
            dto.TipoTramiteId = permisoIngreso.Tramite.TipoTramiteId;
            dto.EstadoTramiteId = permisoIngreso.Tramite.EstadoActualId;
            dto.ParcelaId = permisoIngreso.ParcelaId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;
            dto.ConcesionId = concesion.TramiteId;
            dto.NombreDifuntoNuevo = permisoIngreso.NombreFallecido ?? "";

            dto.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == permisoIngreso.ConcesionId && h.FechaFin == null)
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

            dto.NuevosTitulares = await _context.DocumentosTramites.Where(t => t.TramiteId == permisoIngreso.TramiteId).Select(h => new TitularesContratoDTO
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

            //consultar los difuntos relacionados a la parcela
            dto.Difuntos = await _context.ParcelaDifuntos
                .Where(p => p.ParcelaId == dto.ParcelaId && p.FechaRetiro == null)
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
