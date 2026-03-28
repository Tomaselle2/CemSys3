using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.HistorialEstado;

namespace CemSys3.Interfaces.HistorialEstados
{
    public interface IHistorialEstados
    {
        Task Add(HistorialEstadosDTO dto);
        Task<IEnumerable<HistorialEstadosDTO>> GetAllById(int tramiteId);

        Task VincularTramiteAPersona(int tramiteId, int personaId);
        Task VincularTramiteAParcela(int tramiteId, int parcelaId);
        Task VincularTitularAConcesion(int personaId, int tramiteId);
        Task<IEnumerable<HistorialTitularesDTO>> HistorialTitulares(int concesionId);
    }
}
