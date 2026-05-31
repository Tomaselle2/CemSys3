namespace CemSys3.DTOs.PlantillaTramite
{
    public class DocumentoDTO
    {
        public int Id { get; set; }
        public int TramiteId { get; set; }

        public int TipoAutorizacionId { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string ContenidoHtml { get; set; } = string.Empty;

        public int? PersonaId { get; set; }
        public string? Parentesco { get; set; }
        public DateTime? FechaModificacion { get; set; }

    }
}
