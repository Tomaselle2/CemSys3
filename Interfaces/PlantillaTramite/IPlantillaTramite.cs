using CemSys3.DTOs.PlantillaTramite;

namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IPlantillaTramite
    {
        Task<int> Add(PlantillaTramiteDTO dto);
        Task<int> Update(PlantillaTramiteDTO dto);
        Task<PlantillaTramiteDTO> Get(int id);
    }
}
