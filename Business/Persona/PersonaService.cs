using CemSys3.DTOs.Persona;
using CemSys3.Interfaces.Persona;
using CemSys3.Models;

namespace CemSys3.Business.Persona
{
    public class PersonaService : IPersona
    {
        private readonly AppDbContext _context;
        public PersonaService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Add(PersonaDTO dto)
        {
            Models.Persona persona = new Models.Persona
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Dni = dto.Dni,
                Visibilidad = true,
                FechaNacimiento = dto.FechaNacimiento,
                FechaDefuncion = dto.FechaDefuncion,
            };
            //int result = await _context.

            throw new NotImplementedException();
        }
    }
}
