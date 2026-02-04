using CemSys3.DTOs.EmpresaSepelio;
using CemSys3.DTOs.Paginacion;

namespace CemSys3.Interfaces.EmpresaSepelio
{
    public interface IEmpresaSepelio
    {
        Task<int> Add (EmpresaSepelioRequestDTO empresa);
        Task Update (EmpresaSepelioRequestDTO empresa);
        Task Delete (int id);
        Task<PaginadoResponse<EmpresaSepelioRequestDTO>> GetAllPaginado(
            string? filtro = null,
            int pagina = 1,
            int porPagina = 10);
        Task<IEnumerable<EmpresaSepelioRequestDTO>> GetAll(); 
        Task<EmpresaSepelioRequestDTO> GetById (int id);
    }
}
