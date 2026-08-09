namespace CemSys3.DTOs.Tarifaria
{
    public class ConceptoResumenDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioSinFondo { get; set; }
        public decimal PrecioConFondo { get; set; }
    }
}
