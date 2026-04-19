using CemSys3.DTOs.Tarea;

namespace CemSys3.Interfaces.Tarea
{
    public interface ITarea
    {
        Task Add(TareaDTO dto);
        Task Update(TareaDTO dto);
        Task Delete(int id);
        Task<IEnumerable<TareaDTO>> GetAllByNota(int notaId);
        Task<List<TareaDTO>> GetAllByTramite(int tramiteId);

        Task GuardarTareas(int tramiteId, List<TareaDTO> tareas);

    }
}
