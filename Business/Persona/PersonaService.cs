using CemSys3.DTOs.Persona;
using CemSys3.Interfaces.Persona;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

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
                Sexo = dto.Sexo,
                Correo = dto.Correo,
                Celular = dto.Celular,
                Domicilio = dto.Domicilio,
                DomicilioEnTirolesa = dto.DomicilioEnTirolesa,
                FallecioEnTirolesa = dto.FallecioEnTirolesa,
                NroActa = dto.NroActa,
                NroFolio = dto.NroFolio,
                NroSerie = dto.NroSerie,
                NroAge = dto.NroAge,
                NroTomo = dto.NroTomo,
                EstadoDifuntoId = dto.EstadoDifuntoId,
                CategoriaPersonaId = dto.CategoriaPersonaId
            };

            await _context.Personas.AddAsync(persona);
            await _context.SaveChangesAsync();
            return persona.Id;
        }

        public async Task<bool> PersonaExiste(int dni, string sexo)
        {
            string dniString = dni.ToString("D8");

            // DNI antiguo (7 dígitos) → empieza con 0 → se compara sexo
            if (dniString.StartsWith("0"))
            {
                return await _context.Personas.AnyAsync(p =>
                    p.Dni == dniString &&
                    p.Sexo == sexo &&
                    p.Visibilidad
                );
            }

            // DNI moderno (8 dígitos) → NO se compara sexo
            return await _context.Personas.AnyAsync(p =>
                p.Dni == dniString &&
                p.Visibilidad
            );
        }
    }
}
