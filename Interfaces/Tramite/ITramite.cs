using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Tramite;

namespace CemSys3.Interfaces.Tramite
{
    public interface ITramite
    {
        Task<int> Add(TramiteDTO dto);
        Task Update(TramiteDTO dto);
        Task<TramiteDTO> Get(int id);
        Task<GenericResultDTO> ActualizarInfoAdicional(int tramiteId, string informacionAdicionalTramite);

        Task<ListadoTramitesDeConcesionDTO> GetListadoTramitesDeConcesion(int concesionId); //get
        Task<IEnumerable<TramiteDTO>> GetIniciadosYPendientes();
        Task<IEnumerable<TramiteDTO>> Buscar(TramiteFiltroDTO filtro);

    }
}
