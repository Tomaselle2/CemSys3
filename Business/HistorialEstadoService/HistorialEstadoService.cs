using CemSys3.DTOs.HistorialEstado;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Models;

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
    }
}
