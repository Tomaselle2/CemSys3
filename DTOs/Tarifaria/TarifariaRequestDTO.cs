namespace CemSys3.DTOs.Tarifaria
{
    public class TarifariaRequestDTO
    {
        public int Id { get; set; }

        public decimal Precio { get; set; }

        public int? NroFila { get; set; }

        public int ConceptoTarifariaId { get; set; }

        public int? AniosConcesionId { get; set; }

        public int? SeccionId { get; set; }

        public bool? Visibilidad { get; set; }

        public int TemaId { get; set; }

        public string NombreConcepto { get; set; } = string.Empty;
    }
}
