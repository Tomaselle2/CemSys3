using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Ingreso
{
    public class ResumenIngresoVM
    {
        public int IngresoId { get; set; }
        public string? InformacionAdicionalIngreso { get; set; }

        public ResumenIngresoDTO Resumen { get; set; } = new ResumenIngresoDTO();
        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
