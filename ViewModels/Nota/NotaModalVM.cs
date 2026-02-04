using CemSys3.DTOs.Tarea;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Nota
{
    public class NotaModalVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; } = string.Empty;
        public string Color { get; set; } = "#e3f2fd";

        [Required(ErrorMessage = "El tipo de nota es obligatorio")]
        public int TipoNotaId { get; set; }

        public bool NotaFinalizada { get; set; } = false;
        public int UsuarioId { get; set; }
        public int EstadoId { get; set; }

        public int tramiteVinculadoId { get; set; }
        public string controlador { get; set; } = string.Empty;

        public List<TareaDTO> Tareas { get; set; } = new();
    }
}
