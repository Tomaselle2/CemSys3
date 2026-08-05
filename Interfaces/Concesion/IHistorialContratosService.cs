using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;

namespace CemSys3.Interfaces.Concesion
{
    public interface IHistorialContratosService
    {
        Task<GenericResultDTO> Add(HistorialContratoDTO dto);

        Task<PaginadoResponse<HistorialContratoTablaDTO>> GetAllPaginado(
        int pagina = 1,
        int porPagina = 10,
        DateOnly? fechaDesde = null,
        DateOnly? fechaHasta = null);

        Task<List<HistorialContratoTablaDTO>> GetAllParaExportar(
            DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null);
    }
}
