namespace CemSys3.DTOs.Parcela
{
    public class ModificarParcelaDTO
    {
        public int Id { get; set; }
        public string? NombrePanteon { get; set; }
        public string? infoAdicional { get; set; }
        public int? TipoNichoId { get; set; }
        public int? TipoPanteonId { get; set; }
    }
}
