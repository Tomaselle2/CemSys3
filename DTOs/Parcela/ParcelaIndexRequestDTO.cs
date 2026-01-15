namespace CemSys3.DTOs.Parcela
{
    //para la pagina de listado de parcelas paginado
    public class ParcelaIndexRequestDTO
    {
        public int Id { get; set; }

        public bool Visibilidad { get; set; }

        public int NroParcela { get; set; }

        public int NroFila { get; set; }

        public int CantidadDifuntos { get; set; }

        public string NombrePanteon { get; set; } = null!;

        public int SeccionId { get; set; }

        public int? TipoNichoId { get; set; }

        public int? TipoPanteonId { get; set; }

        public int? TipoParcelaId { get; set; }
    }
}
