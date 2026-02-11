using CemSys3.DTOs.HistorialEstado;

namespace CemSys3.Interfaces.HistorialEstados
{
    public interface IHistorialEstados
    {
        Task Add(HistorialEstadosDTO dto);
        Task<IEnumerable<HistorialEstadosDTO>> GetAllById(int tramiteId);
    }
}
