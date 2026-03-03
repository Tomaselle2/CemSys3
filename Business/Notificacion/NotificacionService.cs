using CemSys3.DTOs.Nota;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Notificaciones;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Notificacion
{
    public class NotificacionService : INotificaciones
    {
        private readonly AppDbContext _context;

        public NotificacionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificacionNotaDTO> NotificacionNotasPendientes()
        {
            NotificacionNotaDTO dto = new NotificacionNotaDTO();

            dto.CantidadNotasIngresoPendientes = await _context.Notas.Include(t=> t.Tramite)
                    .Where(n => n.Tramite.EstadoActualId == (int)EstadosNotaEnum.NotaPendiente && n.TipoNotaId == (int)TipoNotaEnum.Ingreso)
                    .CountAsync();

            dto.CantidadNotasRecordatorioPendientes = await _context.Notas.Include(t => t.Tramite)
                    .Where(n => n.Tramite.EstadoActualId == (int)EstadosNotaEnum.NotaPendiente && n.TipoNotaId == (int)TipoNotaEnum.Recordatorio)
                    .CountAsync();

            return dto;
        }
    }
}
