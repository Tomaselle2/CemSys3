using CemSys3.DTOs.PlantillaTramite;

namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IAutorizacionFactory
    {
        Task Crear(PlantillaTramiteDTO plantilla);
        Task CrearMultiples(IEnumerable<PlantillaTramiteDTO> plantilla);
    }
}
