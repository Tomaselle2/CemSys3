using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.Seccion;

namespace CemSys3.Interfaces.Parcela
{
    public interface IParcela
    {
        Task Add (SeccionRequestDTO dto);

        //para agregar una parcela a una seccion de fosa o panteon
        Task<GenericResultDTO> AddOne(int secccionId);

        Task<PaginadoResponse<ParcelaIndexRequestDTO>> GetAllPaginadoBySeccion(
            int seccionId,
            int filtro = 0,
            int pagina = 1,
            int porPagina = 10);

        Task<IEnumerable<ParcelaIndexRequestDTO>> GetAllBySeccionId(int seccionId, int estadoDifunto);

        Task AumentarDifunto(int parcelaId);
        Task<ParcelaHistorialDTO> HistorialParcela(int parcelaId);

        Task UpdateParcela(ModificarParcelaDTO dto);

        Task<IEnumerable<ParcelaDTO>> GetAllNichosDisponibles();
    }
}
