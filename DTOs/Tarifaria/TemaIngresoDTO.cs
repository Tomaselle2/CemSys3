namespace CemSys3.DTOs.Tarifaria
{
    public class TemaIngresoDTO
    {
        public string Tema { get; set; } = string.Empty;
        public List<ConceptoIngresoDTO> Conceptos { get; set; } = new List<ConceptoIngresoDTO>();
    }
}
