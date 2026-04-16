using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;

namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IGeneradorAutorizacionesStrategy
    {
        Task GenerarAsync(GeneraStrategyDTO dto);
    }
}
