using CemSys3.DTOs.PlantillaTramite;

namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IPlantillaTramite
    {
        //Task<int> Add(PlantillaTramiteDTO dto);
        //Task<int> Update(PlantillaTramiteDTO dto);
        //Task<PlantillaTramiteDTO> Get(int id);

        //metodo nuevo

        Task<int> CrearAsync(PlantillaTramiteDTO dto);
        Task<int> ActualizarAsync(PlantillaTramiteDTO dto);
        Task EliminarAsync(int id);

        Task<PlantillaTramiteDTO?> ObtenerPorIdAsync(int id);
        Task<List<PlantillaTramiteDTO>> ObtenerPorTipoTramiteAsync(int tipoTramiteId);

    }
}
