using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.Tramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.Traslado;
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

        public TrasladoStrategy(
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

        public Task FinalizarAsync(int tramiteId, int usuarioId)
        {
            throw new NotImplementedException();
        }

        public async Task GenerarDocumentosAsync(GeneraStrategyDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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
            dto.Difuntos = await _context.ParcelaDifuntos
                .Where(p => p.DifuntoId == traslado.DifuntoId)
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
    }
}
