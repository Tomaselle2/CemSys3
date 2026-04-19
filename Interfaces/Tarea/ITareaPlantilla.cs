using CemSys3.DTOs.Tarea;

namespace CemSys3.Interfaces.Tarea
{
    public interface ITareaPlantilla
    {
        Task GuardarTareas(int tipoTramiteId, List<TareaDTO> tareas);
        Task<List<TareaDTO>> GetAllByTipoTramite(int tipoTramiteId);
        Task CrearTareasPorTramite(int tramiteId, int tipoTramiteId);
    }
}
