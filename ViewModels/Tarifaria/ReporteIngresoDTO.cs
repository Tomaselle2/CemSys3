using CemSys3.DTOs.Tarifaria;

namespace CemSys3.ViewModels.Tarifaria
{
    public class ReporteIngresoDTO
    {
        public List<PrecioIngresoDTO> Reglas { get; set; } = new();
        public List<CategoriaResumenDTO> Resumen { get; set; } = new();
    }
}
