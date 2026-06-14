namespace CemSys3.DTOs.Parcela
{
    public class ParcelaDTO
    {
        public int Id { get; set; }
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int SeccionId { get; set; }
        public int TipoParcelaId { get; set; }
        public int? TipoNichoId { get; set; }
    }
}
