using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.Ingreso
{
    public class CoincidenciaIngresoDTO
    {
        public bool Existe { get; set; }
        public PersonaDTO? Persona { get; set; }
        public bool EsTitular { get; set; }
        public bool EstaActivoEnCementerio { get; set; } // solo aplica si NO es titular
        public bool CoincidenciaPorDni { get; set; }      // true = matcheó por DNI, false = matcheó por nombre/apellido
    }
}
