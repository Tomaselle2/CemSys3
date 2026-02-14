using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarifaria;

namespace CemSys3.ViewModels.Ingreso
{
    public class ResumenIngresoVM
    {
        public int IngresoId { get; set; }
        public string? InformacionAdicionalIngreso { get; set; }

        public ResumenIngresoDTO Resumen { get; set; } = new ResumenIngresoDTO();

        public IEnumerable<PrecioIngresoDTO> PreciosIngresos { get; set; } = new List<PrecioIngresoDTO>();
        public IEnumerable<ConceptoIngresoDTO> PreciosAperturas { get; set; } = new List<ConceptoIngresoDTO>();
        public IEnumerable<HistorialEstadosDTO> HistorialEstados { get; set; } = new List<HistorialEstadosDTO>();
        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
