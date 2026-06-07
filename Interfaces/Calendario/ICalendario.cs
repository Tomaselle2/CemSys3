using CemSys3.DTOs.Calendario;

namespace CemSys3.Interfaces.Calendario
{
    public interface ICalendario
    {
        Task<IEnumerable<CalendarDTO>> GetEventsAsync();
    }
}
