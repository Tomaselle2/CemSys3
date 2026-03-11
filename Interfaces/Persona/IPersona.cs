using CemSys3.DTOs.Persona;

namespace CemSys3.Interfaces.Persona
{
    public interface IPersona
    {
        Task<int> Add(PersonaDTO dto);
        Task<bool> PersonaExiste(int dni, string sexo);
        Task<PersonaDTO> GetByDNISexo(int dni, string sexo);
        Task<PersonaDTO> Get(int id);
        Task<int> Update(PersonaDTO dto);
        Task<HistorialPersonaDTO> HistorialPersona (int id);
    }
}
