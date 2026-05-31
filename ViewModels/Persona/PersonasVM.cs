using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Persona
{
    public class PersonasVM
    {
        // Propiedades de búsqueda
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int? Dni { get; set; }

        public IEnumerable<PersonaDTO> Personas { get; set; } = new List<PersonaDTO>();

        //alertas
        public SweetAlertDTO? SweetAlert { get; set; }

        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();
    }
}
