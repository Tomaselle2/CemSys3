namespace CemSys3.DTOs.Persona
{
    public class DTO_ConcesionTitular
    {
        public int? NroConcesion { get; set; }

        public int TramiteId { get; set; }

        public int EstadoId { get; set; }

        public DateOnly? Vencimiento { get; set; }

        public string TipoParcela { get; set; } = string.Empty;
    }
}
