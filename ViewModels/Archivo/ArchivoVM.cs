using CemSys3.DTOs.Archivo;

namespace CemSys3.ViewModels.Archivo
{
    public class ArchivoVM
    {
        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();
        public IFormFile? Archivo { get; set; }
        public Guid? IdArchivo { get; set; }
        public string? CategoriaArchivo { get; set; }
        public int? TramiteId { get; set; }
        public string? NombreArchivo { get; set; }
        public string? Descripcion { get; set; }


    }
}
