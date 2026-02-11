using CemSys3.DTOs.HistorialEstado;

namespace CemSys3.ViewModels.Historial
{
    public class HistorialOffcanvasVM
    {
        public IEnumerable<HistorialEstadosDTO>? Historial { get; set; }
        public Type? EnumType { get; set; }   // <-- acá va el enum
        public string OffcanvasId { get; set; } = "offcanvasHistorial";
        public string Titulo { get; set; } = "Historial";
    }
}
