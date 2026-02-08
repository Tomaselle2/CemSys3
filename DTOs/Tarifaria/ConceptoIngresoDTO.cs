namespace CemSys3.DTOs.Tarifaria
{
    public class ConceptoIngresoDTO
    {
        public int ConceptoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioBase { get; set; }
    }
}
