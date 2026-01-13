namespace CemSys3.DTOs.Seccion
{
    public class SeccionRequestDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public bool Visibilidad { get; set; }

        public int Filas { get; set; }

        public int NroParcelas { get; set; }

        public int TipoNumeracionParcelaId { get; set; }

        public int TipoParcelaId { get; set; }
    }
}
