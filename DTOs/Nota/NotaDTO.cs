using CemSys3.DTOs.Tarea;

namespace CemSys3.DTOs.Nota
{
    public class NotaDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public int TipoNotaId { get; set; }

        public string? Descripcion { get; set; }

        public string? Color { get; set; }

        public bool Visibilidad { get; set; }

        public int EstadoId { get; set; }

        public int? TramiteIngresoId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int UsurioId { get; set; }
        public DateTime? FechaFinRecordatorio { get; set; }

        public List<TareaDTO> Tareas { get; set; } = new();
    }
}
