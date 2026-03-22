using CemSys3.DTOs.HistorialEstado;
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
            bool existe = await _context.HistorialTitularesConcesiones
                .AnyAsync(x => x.ConcesionId == tramiteId && x.PersonaId == personaId);

            if (!existe)
            {
                _context.HistorialTitularesConcesiones.Add(new HistorialTitularesConcesione
                {
                    ConcesionId = tramiteId,
                    PersonaId = personaId,
                    FechaInicio = DateTime.Now
                });
            }
        }
    }
}
