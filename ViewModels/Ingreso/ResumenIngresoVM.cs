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

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
