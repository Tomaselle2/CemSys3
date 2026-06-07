using CemSys3.DTOs.Calendario;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Calendario;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Calendario
{
    public class CalendarioService : ICalendario
    {
        private readonly AppDbContext _context;

        public CalendarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CalendarDTO dto)
        {
            Models.EventoCalendario evento = new EventoCalendario
            {
                Fecha = dto.start,
                Titulo = dto.title
            };

            await _context.AddAsync(evento);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            EventoCalendario? evento = await _context.EventoCalendarios.FindAsync(id);

            if (evento == null)
            {
                throw new Exception("Evento no encontrado.");
            }

            _context.EventoCalendarios.Remove(evento);
            await _context.SaveChangesAsync();
        }

        public async Task<CalendarDTO> Get(int id)
        {
            EventoCalendario? evento = await _context.EventoCalendarios.FindAsync(id);

            if (evento == null)
            {
                throw new Exception("Evento no encontrado.");
            }

            return new CalendarDTO
            {
                id = evento.Id,
                start = evento.Fecha,
                title = evento.Titulo,
                tipo = "Manual"
            };
        }

        public async Task<IEnumerable<CalendarDTO>> GetEventsAsync()
        {
            var eventos = new List<CalendarDTO>();

            var permisosRefacciones = await _context.PermisosRefacciones
                .AsNoTracking()
                .Where(p => p.FechaPendiente.HasValue && p.FechaFinalizacion == null)
                .Select(p => new CalendarDTO
                {
                    start = p.FechaPendiente!.Value,
                    title = $"Permiso de Refacción #{p.TramiteId}",
                    url = $"/PermisoRefaccion/Detalle?TramiteId={p.TramiteId}"
                })
                .ToListAsync();

            var cremaciones = await _context.Cremaciones
                .AsNoTracking()
                .Where(c => c.FechaPendiente.HasValue && c.FechaFinalizacion == null)
                .Select(c => new CalendarDTO
                {
                    start = c.FechaPendiente!.Value,
                    title = $"Cremación #{c.TramiteId}",
                    url = $"/Cremacion/Detalle?TramiteId={c.TramiteId}"
                })
                .ToListAsync();

            var traslados = await _context.Traslados
                .AsNoTracking()
                .Where(t => t.FechaPendiente.HasValue && t.FechaFinalizacion == null)
                .Select(t => new CalendarDTO
                {
                    start = t.FechaPendiente!.Value,
                    title = $"Traslado #{t.TramiteId}",
                    url = $"/Traslado/Detalle?TramiteId={t.TramiteId}"
                })
                .ToListAsync();

            var reducciones = await _context.Reducciones
                .AsNoTracking()
                .Where(r => r.FechaPendiente.HasValue && r.FechaFinalizacion == null)
                .Select(r => new CalendarDTO
                {
                    start = r.FechaPendiente!.Value,
                    title = $"Reducción #{r.TramiteId}",
                    url = $"/Reduccion/Detalle?TramiteId={r.TramiteId}"
                })
                .ToListAsync();

            var recordatorios = await _context.Notas
                .AsNoTracking()
                .Where(n => n.TipoNotaId == (int)TipoNotaEnum.Recordatorio && n.FechaFinRecordatorio.HasValue && n.Tramite.EstadoActualId == (int)EstadosNotaEnum.NotaPendiente)
                .Select(n => new CalendarDTO
                {
                    start = n.FechaFinRecordatorio!.Value,
                    title = "Nota Recordatorio",
                    tipo = "NotaRecordatorio",
                    tramiteId = n.TramiteId,
                    allDay = true
                })
                .ToListAsync();

            var eventosManuales = await _context.EventoCalendarios
                .AsNoTracking()
                .Select(e => new CalendarDTO
                {
                    id = e.Id,
                    start = e.Fecha,
                    title = e.Titulo,
                    url = null,
                    tipo = "Manual"
                })
                .ToListAsync();

            eventos.AddRange(permisosRefacciones);
            eventos.AddRange(cremaciones);
            eventos.AddRange(traslados);
            eventos.AddRange(reducciones);
            eventos.AddRange(recordatorios);
            eventos.AddRange(eventosManuales);

            return eventos;
        }

        public async Task Update(CalendarDTO dto)
        {

            EventoCalendario? evento = await _context.EventoCalendarios.FindAsync(dto.id);

            if(evento == null)
            {
                throw new Exception("Evento no encontrado.");
            }

            evento.Titulo = dto.title;
            evento.Fecha = dto.start;

            await _context.SaveChangesAsync();
        }
    }
}
