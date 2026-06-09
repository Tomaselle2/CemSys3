using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;

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
        Task<IEnumerable<TramiteDTO>> HistorialTramitesConcesion(int concesionId);
        Task<IEnumerable<DifuntoConcesionDTO>> DifuntosEnConcesion(int concesionId);
    }
}
