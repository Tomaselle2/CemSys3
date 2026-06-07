using CemSys3.DTOs.Calendario;

namespace CemSys3.Interfaces.Calendario
{
    public interface ICalendario
    {
        Task<IEnumerable<CalendarDTO>> GetEventsAsync();
        Task Add(CalendarDTO dto);
        Task Update(CalendarDTO dto);

        Task<CalendarDTO> Get(int id);
        Task Delete(int id);
    }
}
