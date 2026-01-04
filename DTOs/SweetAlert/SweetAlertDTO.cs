namespace CemSys3.DTOs.SweetAlert
{
    public class SweetAlertDTO
    {
        public string? Titulo { get; set; }
        public string? Mensaje { get; set; }
        public string? Tipo { get; set; } // "success", "error", "warning", "info", "question"
    }
}
