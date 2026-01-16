namespace CemSys3.DTOs.ConceptosTarifaria
{
    public class ConceptoTarifariaDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public bool Visibilidad { get; set; }

        public int TemaId { get; set; }
    }
}
