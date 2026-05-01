using CemSys3.DTOs.TramitesConcesion;

namespace CemSys3.Interfaces.TramitesConcesion
{
    public interface IFirmantes
    {
        Task Add(int tramiteId, int personaId, string parentesco, bool titular = false);
        Task Delete(int firmanteId);
        Task<List<FirmantesDTO>> GetAllByTramite(int tramiteId);
    }
}
