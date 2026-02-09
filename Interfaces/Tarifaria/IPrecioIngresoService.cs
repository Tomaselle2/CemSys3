using CemSys3.DTOs.Tarifaria;

namespace CemSys3.Interfaces.Tarifaria
{
    public interface IPrecioIngresoService
    {
        Task<IEnumerable<PrecioIngresoDTO>> ObtenerTodasLasReglasAsync();
    }
}
