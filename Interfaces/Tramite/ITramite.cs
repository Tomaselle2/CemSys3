using CemSys3.DTOs.Tramite;

namespace CemSys3.Interfaces.Tramite
{
    public interface ITramite
    {
        Task<int> Add(TramiteDTO dto);
        Task Update(TramiteDTO dto);
        Task<TramiteDTO> Get(int id);
    }
}
