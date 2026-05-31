using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.HistorialEstado;
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
            bool existe = await _context.TramitePersonas
                .AnyAsync(tp => tp.TramiteId == tramiteId
                            && tp.PersonaId == personaId);

            if (!existe)
            {
                TramitePersona tramitePersona = new TramitePersona
                {
                    TramiteId = tramiteId,
                    PersonaId = personaId,
                    FechaRegistro = DateTime.Now
                };

                _context.TramitePersonas.Add(tramitePersona);
            }
        }

        public async Task VincularTramiteAParcela(int tramiteId, int parcelaId)
        {
            bool existe = await _context.TramitesParcelas
                .AnyAsync(x => x.TramiteId == tramiteId && x.ParcelaId == parcelaId);

            if (!existe)
            {
                _context.TramitesParcelas.Add(new TramitesParcela
                {
                    TramiteId = tramiteId,
                    ParcelaId = parcelaId,
                    FechaRegistro = DateTime.Now
                });
            }
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
            return await _context.HistorialTitularesConcesiones
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
            var concesion = await _context.Concesiones
                .Include(c => c.Tramite)
                .FirstOrDefaultAsync(c => c.TramiteId == concesionId)
                ?? throw new Exception("Concesión no encontrada");

            DateTime fechaInicio = concesion.FechaInicio ?? concesion.Tramite.FechaCreacion;
            DateTime fechaFin = concesion.FechaFin ?? DateTime.Now;

            // 🔹 1. Último ingreso previo a la concesión
            var ingresoPrevio = await _context.TramitesParcelas
                .AsNoTracking()
                .Where(tp => tp.ParcelaId == concesion.ParcelaId)
                .Join(_context.Tramites,
                    tp => tp.TramiteId,
                    t => t.Id,
                    (tp, t) => t)
                .Where(t =>
                    t.FechaCreacion < fechaInicio &&
                    t.TipoTramiteId == (int)TipoTramiteEnum.Ingreso
                )
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

            // 🔹 2. Trámites dentro del rango de la concesión
            var tramitesDentro = await _context.TramitesParcelas
                .AsNoTracking()
                .Where(tp => tp.ParcelaId == concesion.ParcelaId)
                .Join(_context.Tramites,
                    tp => tp.TramiteId,
                    t => t.Id,
                    (tp, t) => t)
                .Where(t =>
                    t.FechaCreacion >= fechaInicio &&
                    t.FechaCreacion <= fechaFin &&
                    t.TipoTramiteId != (int)TipoTramiteEnum.ContratoConcesion
                )
                .OrderBy(t => t.FechaCreacion)
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

            // 🔹 3. Unir resultados
            var resultado = new List<TramiteDTO>();

            if (ingresoPrevio != null)
                resultado.Add(ingresoPrevio);

            resultado.AddRange(tramitesDentro);

            // 🔹 4. Orden final
            return resultado
                .OrderByDescending(t => t.FechaCreacion)
                .ToList();
        }
    }
}
