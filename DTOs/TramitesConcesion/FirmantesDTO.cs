namespace CemSys3.DTOs.TramitesConcesion
{
    public class FirmantesDTO
    {
        public int Id { get; set; }

        public int TramiteId { get; set; }

        public int PersonaId { get; set; }

        public string? Parentesco { get; set; }

        public bool EsTitular { get; set; }

        public DateTime? FechaAlta { get; set; }

        public bool? Visibilidad { get; set; }
    }
}
