namespace CemSys3.DTOs.Tramite
{
    public class RequisitosTramiteDTO
    {
        public int Id { get; set; }
        public int TipoTramiteId { get; set; } 
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
