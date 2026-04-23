using CemSys3.DTOs.Tramite;

namespace CemSys3.Interfaces.TramitesConcesion
{
    public interface IRequisitos
    {
        Task<IEnumerable<RequisitosTramiteDTO>> GetAll(int concesionId);
        Task<RequisitosTramiteDTO> GetByTipoTramiteId(int tipoTramiteId);

        Task Update(int tipoTramiteId, string descripcion);
    }
}
