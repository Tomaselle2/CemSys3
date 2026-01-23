using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Tarifaria;

namespace CemSys3.Interfaces.Tarifaria
{
    public interface ITarifaria
    {
        Task<IEnumerable<TarifariaRequestDTO>> GetPrecios(); //trae los precios sin paginar de todo menos los nichos
        Task ActualizarPreciosTarifaria(List<PrecioActualizarDTO> preciosActualizar); //para actualizar todos los precios
    }
}
