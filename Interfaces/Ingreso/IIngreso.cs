using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Paginacion;

namespace CemSys3.Interfaces.Ingreso
{
    public interface IIngreso
    {
        Task<GenericResultDTO> Add(IngresoDTO dto);
        Task<ResumenIngresoDTO> Get(int ingresoId);
        Task FinalizarIngreso (int ingresoId, string cobroIngreso, string cobroApertura);

        Task<PaginadoResponse<ListadoIngresosDTO>> GetAllPaginadoIngresos(
            DateOnly? fechaDesde,
            DateOnly? fechaHasta,
            int pagina = 1,
            int porPagina = 10,
            int filtro = 0);
    }
}
