using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Tramite;
using CemSys3.ViewModels.Ingreso;

namespace CemSys3.Interfaces.Tramite
{
    public interface ITramite
    {
        Task<int> Add(TramiteDTO dto);
        Task Update(TramiteDTO dto);
        Task<TramiteDTO> Get(int id);
        Task<GenericResultDTO> ActualizarInfoAdicional(int tramiteId, string informacionAdicionalTramite);
    }
}
