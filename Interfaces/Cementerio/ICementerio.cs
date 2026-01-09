using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.Paginacion;

namespace CemSys3.Interfaces.Cementerio
{
    public interface ICementerio
    {
        Task Add(CementerioRequestDTO cementerio);
        Task Update(CementerioRequestDTO cementerio);
        Task Delete(int id);
        Task<PaginadoResponse<CementerioRequestDTO>> GetAllPaginado(
            string? filtro = null,
            int pagina = 1,
            int porPagina = 10);
        Task<CementerioRequestDTO> GetById(int id);
    }
}
