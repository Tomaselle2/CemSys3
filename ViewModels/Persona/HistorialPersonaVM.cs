using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Persona
{
    public class HistorialPersonaVM
    {
        public PersonaDTO persona { get; set; } = new PersonaDTO();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
