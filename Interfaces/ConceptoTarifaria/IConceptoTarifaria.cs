using CemSys3.DTOs.ConceptosTarifaria;
using CemSys3.DTOs.Paginacion;

namespace CemSys3.Interfaces.ConceptoTarifaria
{
    public interface IConceptoTarifaria
    {
        Task Add(ConceptoTarifariaDTO dto);
        Task Update(ConceptoTarifariaDTO dto);
        Task Delete(int id);
        Task<PaginadoResponse<ConceptoTarifariaDTO>> GetAllPaginado(string? filtro = null,
            int pagina = 1,
            int porPagina = 10);
        Task<ConceptoTarifariaDTO> Get(int id);


    }
}
