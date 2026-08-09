namespace CemSys3.DTOs.Tarifaria
{
    public class CategoriaResumenDTO
    {
        public string Categoria { get; set; } = string.Empty;
        public List<ConceptoResumenDTO> Conceptos { get; set; } = new();
    }
}
