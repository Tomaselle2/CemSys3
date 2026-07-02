using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Seccion;
namespace CemSys3.Interfaces.Seccion
{
    public interface ISeccion
    {
        Task<GenericResultDTO> Add(SeccionRequestDTO dto);
        Task Update(SeccionRequestDTO dto);
        Task Delete(int id);
        Task<SeccionRequestDTO> Get(int id);
        Task<PaginadoResponse<SeccionRequestDTO>> GetAllByTipoPaginado(int tipoId, string? filtro = null, int pagina = 1, int porPagina = 10);
        Task<IEnumerable<SeccionSelectDTO>> GetAllByTipo(int tipoParcelaId);

        Task<List<SeccionDTO>> GetAllSeccionesExcel();

        Task<int> ImportarSecciones(List<SeccionDTO> secciones);
    }
}
