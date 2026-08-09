using CemSys3.DTOs.Tarifaria;

namespace CemSys3.Interfaces.Tarifaria
{
    public interface IPrecioIngresoService
    {
        Task<IEnumerable<PrecioIngresoDTO>> ObtenerTodasLasReglasAsync();
        Task<IEnumerable<PrecioIngresoDTO>> GetPreciosIngresoBy(int tipoParcelaId, int estadoDifuntoId);
        Task<IEnumerable<ConceptoIngresoDTO>> GetPreciosAperturas(int tipoParcelaId);
        Task<List<CategoriaResumenDTO>> ObtenerResumenGeneralAsync();
    }
}
