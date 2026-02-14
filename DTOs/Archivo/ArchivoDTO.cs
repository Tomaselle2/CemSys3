namespace CemSys3.DTOs.Archivo
{
    public class ArchivoDTO
    {
        public Guid Id { get; set; }

        public string? CategoriaArchivo { get; set; }

        public int? TramiteId { get; set; }

        public string NombreArchivo { get; set; } = null!;

        public string TipoArchivo { get; set; } = null!;

        public string? Descripcion { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public bool Visibilidad { get; set; }

        public IFormFile? Archivo { get; set; }
        public string? MimeType { get; set; }
    }
}
