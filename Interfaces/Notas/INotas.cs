using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Paginacion;

namespace CemSys3.Interfaces.Notas
{
    public interface INotas
    {
        Task Add(NotaDTO dto);
        Task Update(NotaDTO dto);
        Task<NotaDTO> Get(int id); //devuelve la nota con id nota
        Task<NotaDTO> GetNotaIngreso(int tramiteIngresoId); //devuelve la nota vinculada al ingreso con id ingreso
        Task<PaginadoResponse<NotaDTO>> GetPaginadoByTipo(int estadoId, int filtroTipoNota = 0, int pagina = 1, int porPagina = 10);
        Task VincularNotaConIngreso(int notaId, int ingresoId);
    }
}
