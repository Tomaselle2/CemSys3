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
        public bool Visibilidad { get; set; }
        public int CantidadDifuntos { get; set; }
        public string NombrePanteon { get; set; } = string.Empty;
        public int? TipoPanteonId { get; set; }
        public string InformacionAdicional { get; set; } = string.Empty;
    }
}
