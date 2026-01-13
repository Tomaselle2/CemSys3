using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Seccion;
using System.Collections;

namespace CemSys3.Interfaces.Seccion
{
    public interface ISeccion
    {
        Task<GenericResultDTO> Add(SeccionRequestDTO dto);
        Task Update(SeccionRequestDTO dto);
        Task Delete(int id);
        Task<SeccionRequestDTO> Get(int id);
        Task<PaginadoResponse<SeccionRequestDTO>> GetAllByTipoPaginado(int tipoId, string? filtro = null, int pagina = 1, int porPagina = 10);
    }
}
