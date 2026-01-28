namespace CemSys3.DTOs.Tarea
{
    public class TareaDTO
    {
        public int Id { get; set; }

        public bool Estado { get; set; }

        public string Descripcion { get; set; } = null!;

        public int? NotaId { get; set; }

        public int? TramiteId { get; set; }

        public bool Visibilidad { get; set; }

    }
}
