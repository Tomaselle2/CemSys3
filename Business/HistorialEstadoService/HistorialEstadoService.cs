using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;


namespace CemSys3.Business.HistorialEstadoService
{
    public class HistorialEstadoService : IHistorialEstados
    {
        private readonly AppDbContext _context;


        public HistorialEstadoService(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(HistorialEstadosDTO dto)
        {
            Models.HistorialEstadoTramite historial = new Models.HistorialEstadoTramite()
            {
                Fecha = dto.Fecha,
                TramiteId = dto.TramiteId,
                EstadoTramiteId = dto.EstadoTramiteId
            };

            await _context.HistorialEstadoTramites.AddAsync(historial);
        }

        public async Task<IEnumerable<HistorialEstadosDTO>> GetAllById(int tramiteId)
        {
            return await _context.HistorialEstadoTramites.Where(h => h.TramiteId == tramiteId).OrderByDescending(f=>f.Fecha).Select(s => new HistorialEstadosDTO
            {
                Id = s.Id,
                TramiteId = s.TramiteId,
                Fecha = s.Fecha,
                EstadoTramiteId= s.EstadoTramiteId
            }).ToListAsync();
        }

        public async Task VincularTramiteAPersona(int tramiteId, int personaId)
        {
            bool existeEnMemoria = _context.ChangeTracker
                .Entries<TramitePersona>()
                .Any(x =>
                    x.Entity.TramiteId == tramiteId &&
                    x.Entity.PersonaId == personaId);

            if (existeEnMemoria)
                return;

            bool existeEnBd = await _context.TramitePersonas
                .AnyAsync(tp =>
                    tp.TramiteId == tramiteId &&
                    tp.PersonaId == personaId);

            if (existeEnBd)
                return;

            _context.TramitePersonas.Add(new TramitePersona
            {
                TramiteId = tramiteId,
                PersonaId = personaId,
                FechaRegistro = DateTime.Now
            });
        }


        public async Task VincularTramiteAParcela(int tramiteId, int parcelaId)
        {
            bool existeEnMemoria = _context.TramitesParcelas.Local
                .Any(x => x.TramiteId == tramiteId &&
                          x.ParcelaId == parcelaId);

            if (existeEnMemoria)
                return;

            bool existeEnBd = await _context.TramitesParcelas
                .AnyAsync(x => x.TramiteId == tramiteId &&
                               x.ParcelaId == parcelaId);

            if (existeEnBd)
                return;

            await _context.TramitesParcelas.AddAsync(new TramitesParcela
            {
                TramiteId = tramiteId,
                ParcelaId = parcelaId,
                FechaRegistro = DateTime.Now
            });
        }

        public async Task VincularTitularAConcesion(int personaId, int tramiteId)
        {
            bool existeActivo = await _context.HistorialTitularesConcesiones
                .AnyAsync(x => x.ConcesionId == tramiteId
                            && x.PersonaId == personaId
                            && x.FechaFin == null);

            if (!existeActivo)
            {
                _context.HistorialTitularesConcesiones.Add(new HistorialTitularesConcesione
                {
                    ConcesionId = tramiteId,
                    PersonaId = personaId,
                    FechaInicio = DateTime.Now,
                    FechaFin = null
                });
            }
        }

        //titulares de concesion
        public async Task<IEnumerable<HistorialTitularesDTO>> HistorialTitulares(int concesionId)
        {
            return await _context.HistorialTitularesConcesiones.AsNoTracking()
                .Where(h => h.ConcesionId == concesionId)
                .OrderByDescending(h => h.FechaInicio)
                .Select(s => new HistorialTitularesDTO
                {
                    ConcesionId = s.ConcesionId,
                    PersonaId = s.PersonaId,
                    Nombre = s.Persona.Nombre,
                    Apellido = s.Persona.Apellido,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin
                }).ToListAsync();
        }

        public async Task<IEnumerable<TramiteDTO>> HistorialTramitesConcesion(int concesionId)
        {
            var concesion = await _context.Concesiones.AsNoTracking()
                .Include(c => c.Tramite)
                .FirstOrDefaultAsync(c => c.TramiteId == concesionId)
                ?? throw new Exception("Concesión no encontrada");

            DateTime fechaInicio = concesion.FechaInicio ?? concesion.Tramite.FechaCreacion;
            DateTime fechaFin = concesion.FechaFin ?? DateTime.Now;

            // Obtener el historial de parcelas con sus fechas de entrada
            var historialParcelas = await _context.HistorialParcelasConcesions
                .Where(h => h.ConcesionId == concesionId)
                .OrderBy(h => h.FechaInicio)
                .ToListAsync();

            // IDs de todas las parcelas que tuvo la concesión
            var parcelasIds = historialParcelas.Select(h => h.ParcelaId).ToList();

            if (!parcelasIds.Any())
                parcelasIds.Add(concesion.ParcelaId);

            // ── Ingreso previo ───────────────────────────────────────────────────────
            // Solo buscarlo en la PRIMERA parcela de la concesión (la original),
            // y solo si ese ingreso ocurrió antes de que la concesión existiera.
            // Así no se mezcla con ingresos de concesiones anteriores en parcelas
            // que la concesión adquirió por traslado/reducción.
            TramiteDTO? ingresoPrevio = null;

            // Parcela original: la primera en el historial, o la actual si no hay historial
            int parcelaOriginalId = historialParcelas.Any()
                ? historialParcelas.OrderBy(h => h.FechaInicio).First().ParcelaId
                : concesion.ParcelaId;

            // Buscar el ingreso previo del difunto que dio origen a ESTA concesión.
            // Filtramos por los difuntos vinculados a la concesión para no traer
            // ingresos de difuntos de concesiones anteriores en la misma parcela.
            var difuntosEnConcesion = await _context.TramitePersonas
                .Where(tp => tp.TramiteId == concesionId)
                .Select(tp => tp.PersonaId)
                .ToListAsync();

            if (difuntosEnConcesion.Any())
            {
                ingresoPrevio = await _context.TramitesParcelas
                    .AsNoTracking()
                    .Where(tp => tp.ParcelaId == parcelaOriginalId)
                    .Join(_context.Tramites,
                        tp => tp.TramiteId,
                        t => t.Id,
                        (tp, t) => t)
                    .Where(t =>
                        t.FechaCreacion < fechaInicio &&
                        t.TipoTramiteId == (int)TipoTramiteEnum.Ingreso)
                    // Solo ingresos que involucren a algún difunto de esta concesión
                    .Where(t => _context.TramitePersonas
                        .Any(tp => tp.TramiteId == t.Id
                                && difuntosEnConcesion.Contains(tp.PersonaId)))
                    .OrderByDescending(t => t.FechaCreacion)
                    .Select(t => new TramiteDTO
                    {
                        Id = t.Id,
                        Visibilidad = t.Visibilidad,
                        FechaCreacion = t.FechaCreacion,
                        TipoTramiteId = t.TipoTramiteId,
                        UsuarioId = t.UsuarioId,
                        EstadoActualId = t.EstadoActualId
                    })
                    .FirstOrDefaultAsync();
            }

            // ── Trámites dentro del rango ────────────────────────────────────────────
            // Para parcelas adquiridas por traslado/reducción, solo traer trámites
            // desde la fecha en que la concesión llegó a esa parcela, no desde el inicio
            // de la concesión. Así no se mezclan trámites de concesiones anteriores.
            var tramitesDentro = new List<TramiteDTO>();

           

            foreach (var hp in historialParcelas)
            {
                // Para la parcela original (primera en el historial), el rango arranca
                // desde el inicio de la concesión, no desde FechaInicio del historial
                // que puede ser posterior si hubo un traslado previo.
                bool esPrimeraEntrada = hp.FechaInicio == historialParcelas.Min(x => x.FechaInicio);
                DateTime desdeParcela = esPrimeraEntrada ? fechaInicio : hp.FechaInicio;
                DateTime hastaParcela = hp.FechaFin ?? fechaFin;

                var tramitesParcela = await _context.TramitesParcelas
                    .AsNoTracking()
                    .Where(tp => tp.ParcelaId == hp.ParcelaId)
                    .Join(_context.Tramites,
                        tp => tp.TramiteId,
                        t => t.Id,
                        (tp, t) => t)
                    .Where(t =>
                        t.FechaCreacion >= desdeParcela &&
                        t.FechaCreacion <= hastaParcela &&
                        t.TipoTramiteId != (int)TipoTramiteEnum.ContratoConcesion)
                    .Select(t => new TramiteDTO
                    {
                        Id = t.Id,
                        Visibilidad = t.Visibilidad,
                        FechaCreacion = t.FechaCreacion,
                        TipoTramiteId = t.TipoTramiteId,
                        UsuarioId = t.UsuarioId,
                        EstadoActualId = t.EstadoActualId
                    })
                    .ToListAsync();

                tramitesDentro.AddRange(tramitesParcela);
            }

            // Si no hay historial de parcelas (concesión vieja), usar el rango completo
            if (!historialParcelas.Any())
            {
                tramitesDentro = await _context.TramitesParcelas
                    .AsNoTracking()
                    .Where(tp => tp.ParcelaId == concesion.ParcelaId)
                    .Join(_context.Tramites,
                        tp => tp.TramiteId,
                        t => t.Id,
                        (tp, t) => t)
                    .Where(t =>
                        t.FechaCreacion >= fechaInicio &&
                        t.FechaCreacion <= fechaFin &&
                        t.TipoTramiteId != (int)TipoTramiteEnum.ContratoConcesion)
                    .Select(t => new TramiteDTO
                    {
                        Id = t.Id,
                        Visibilidad = t.Visibilidad,
                        FechaCreacion = t.FechaCreacion,
                        TipoTramiteId = t.TipoTramiteId,
                        UsuarioId = t.UsuarioId,
                        EstadoActualId = t.EstadoActualId
                    })
                    .ToListAsync();
            }

            // ── Unir, deduplicar y ordenar ───────────────────────────────────────────
            var resultado = new List<TramiteDTO>();

            if (ingresoPrevio != null)
                resultado.Add(ingresoPrevio);

            resultado.AddRange(tramitesDentro);

            return resultado
                .DistinctBy(t => t.Id)
                .OrderByDescending(t => t.FechaCreacion)
                .ToList();
        }
        public async Task<IEnumerable<DifuntoConcesionDTO>> DifuntosEnConcesion(int concesionId)
        {
            var concesion = await _context.Concesiones
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.TramiteId == concesionId)
                ?? throw new Exception("Concesión no encontrada");

            DateTime inicio = concesion.FechaInicio ?? DateTime.MinValue;
            DateTime fin = concesion.FechaFin ?? DateTime.MaxValue;

            var parcelasHistorial = await _context.HistorialParcelasConcesions
                .AsNoTracking()
                .Where(h => h.ConcesionId == concesionId)
                .Select(h => h.ParcelaId)
                .ToListAsync();

            if (!parcelasHistorial.Any())
                parcelasHistorial.Add(concesion.ParcelaId);

            // Traer todos los registros crudos
            var registros = await _context.ParcelaDifuntos
                .AsNoTracking()
                .Where(pd =>
                    parcelasHistorial.Contains(pd.ParcelaId) &&
                    pd.FechaIngreso >= inicio &&
                    pd.FechaIngreso <= fin)
                .Select(pd => new DifuntoConcesionDTO
                {
                    ParcelaDifuntoId = pd.Id,
                    DifuntoId = pd.DifuntoId,
                    Nombre = pd.Difunto.Nombre,
                    Apellido = pd.Difunto.Apellido,
                    Dni = pd.Difunto.Dni,
                    FechaIngreso = pd.FechaIngreso,
                    FechaRetiro = pd.FechaRetiro,
                    TramiteIngresoId = pd.TramiteIngresoId,
                    TramiteRetiroId = pd.TramiteRetiroId
                })
                .OrderBy(d => d.FechaIngreso)
                .ToListAsync();

            // Agrupar por difunto: ingreso más temprano, retiro más reciente
            // Si algún registro no tiene retiro (sigue activo), el retiro del grupo es null
            var agrupado = registros
                .GroupBy(d => d.DifuntoId)
                .Select(g => new DifuntoConcesionDTO
                {
                    // Usar el id del primer registro (el más antiguo)
                    ParcelaDifuntoId = g.OrderBy(x => x.FechaIngreso).First().ParcelaDifuntoId,
                    DifuntoId = g.Key,
                    Nombre = g.First().Nombre,
                    Apellido = g.First().Apellido,
                    Dni = g.First().Dni,
                    FechaIngreso = g.Min(x => x.FechaIngreso),
                    // Si cualquier registro no tiene retiro, el difunto sigue activo
                    FechaRetiro = g.Any(x => x.FechaRetiro == null)
                        ? null
                        : g.Max(x => x.FechaRetiro),
                    TramiteIngresoId = g.OrderBy(x => x.FechaIngreso).First().TramiteIngresoId,
                    TramiteRetiroId = g.Any(x => x.FechaRetiro == null)
                        ? null
                        : g.OrderByDescending(x => x.FechaRetiro).First().TramiteRetiroId
                })
                .OrderBy(d => d.FechaIngreso)
                .ToList();

            return agrupado;
        }
    }
}
